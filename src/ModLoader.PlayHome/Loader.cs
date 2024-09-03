using BepInEx;
using HarmonyLib;
using KeelPlugins.Harmony;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using XUnity.ResourceRedirector;

namespace ModLoader.PlayHome
{
    [BepInPlugin(GUID, "ModLoader", Version)]
    public class Loader : BaseUnityPlugin
    {
        public const string GUID = "keelhauled.modloader";
        public const string Version = "1.0.1." + BuildNumber.Version;

        private static string modFolder;
        private static string[] modFolders;
        private static readonly Dictionary<string, string> bundleAbdata = new Dictionary<string, string>();
        private static readonly Dictionary<string, string> thumbAbdata = new Dictionary<string, string>();

        private void Awake()
        {
            Log.SetLogSource(Logger);
            GatherData();

            HarmonyExtensions.CreateAndPatchAll(typeof(Loader));
            ResourceRedirection.RegisterAsyncAndSyncAssetBundleLoadingHook(0, AssetBundleLoadingHook);
            //ResourceRedirection.RegisterResourceLoadedHook(HookBehaviour.OneCallbackPerResourceLoaded, ResourceLoadedHook);
        }

        private void GatherData()
        {
            modFolder = Path.Combine(Paths.GameRootPath, "mods");
            modFolders = Directory.GetDirectories(modFolder).Where(x => Directory.Exists(Path.Combine(x, "abdata"))).ToArray();

            foreach(var folder in modFolders)
            {
                var abdataDir = Path.Combine(folder, "abdata");
                var thumbDir = Path.Combine(abdataDir, "thumnbnail_R");
                var bundles = Directory.GetFiles(abdataDir, "*", SearchOption.AllDirectories).Where(x => !x.Contains("abdata\\list") && !x.Contains("abdata\\thumnbnail")).ToList();

                foreach(var bundle in bundles)
                {
                    var key = bundle.Substring(@"abdata\");

                    if(bundleAbdata.TryGetValue(key, out _))
                        Log.Warning($"Duplicate assetbundle {key}");
                    else
                        bundleAbdata.Add(key, abdataDir);
                }

                if(Directory.Exists(thumbDir))
                {
                    foreach(var thumbPath in Directory.GetFiles(thumbDir, "*.png"))
                        thumbAbdata[Path.GetFileNameWithoutExtension(thumbPath)] = abdataDir; 
                }
            }
        }

        private void AssetBundleLoadingHook(IAssetBundleLoadingContext context)
        {
            var normalized = context.GetNormalizedPath();
            if(normalized.Contains(@"abdata\"))
            {
                if(!context.Parameters.Path.Contains(modFolder) && bundleAbdata.TryGetValue(normalized.Substring(@"abdata\"), out var newPath))
                    context.Parameters.Path = newPath;
            }
        }

        //private static void ResourceLoadedHook(ResourceLoadedContext context)
        //{
        //    if(context.Parameters.Path == "FemaleBody")
        //    {
        //        var path = Path.Combine(modFolder, "uncensor/resources.unity3d");
        //        var ab = AssetBundle.LoadFromFile(path);
        //        var asset = ab.LoadAsset<Female>("FemaleBody");
        //        context.Asset = asset;
        //    }
        //}

        [HarmonyPatchExt(typeof(AssetBundleController), nameof(AssetBundleController.LoadAsset), null, new[] { typeof(UnityEngine.Object) })]
        private static bool LoadThumbs(ref object __result, ref string assetName, AssetBundleController __instance)
        {
            if(__instance.assetBundleName.Contains("thumnbnail/thumbnail_") || __instance.assetBundleName.Contains("thumnbnail/thumnbs_"))
            {
                if(thumbAbdata.TryGetValue(assetName, out var modAbdataDir))
                {
                    var path = BepInEx.Utility.CombinePaths(modAbdataDir, "thumnbnail_R", assetName + ".png");
                    if(File.Exists(path))
                    {
                        Log.Debug($"Loading thumbnail ({path})");
                        __result = LoadPNG(path);
                        return false;
                    }
                }
            }

            return true;
        }

        private static Texture2D LoadPNG(string file)
        {
            try
            {
                using(var fileStream = new FileStream(file, FileMode.Open, FileAccess.Read))
                using(var binaryReader = new BinaryReader(fileStream))
                {
                    var pngBytes = binaryReader.ReadBytes((int)binaryReader.BaseStream.Length);
                    var texture2D = new Texture2D(1, 1, TextureFormat.ARGB32, false);
                    texture2D.LoadImage(pngBytes);
                    return texture2D;
                }
            }
            catch(Exception ex)
            {
                Log.Warning(ex);
                return null;
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(ItemDataBase), MethodType.Constructor, typeof(int), typeof(string), typeof(string), typeof(int), typeof(bool))]
        private static void ExtendIds(ref int ___id, int id)
        {
            ___id = id > 999999 && id < 1000000000 ? id : id % 1000;
        }

        [HarmonyPatchExt("CustomDataSetupLoader`1[ItemDataBase], Assembly-CSharp", "Setup_Search", new[] { typeof(Dictionary<int, ItemDataBase>), typeof(string) })]
        private static void LoadMods(Dictionary<int, object> datas, ref string search, Action<Dictionary<int, object>, AssetBundleController, CustomDataListLoader> ___action)
        {
            var dir = "";
            var filter = "";
            var lastSlash = search.LastIndexOf("/", StringComparison.Ordinal);

            if(lastSlash != -1)
            {
                dir = search.Substring(0, lastSlash);
                filter = search.Remove(0, lastSlash + 1);
            }

            foreach(var folder in modFolders)
            {
                var abdataDir = Path.Combine(folder, "abdata");
                var listDir = BepInEx.Utility.CombinePaths(abdataDir, "list", dir);

                if(Directory.Exists(listDir))
                {
                    foreach(var listPath in Directory.GetFiles(listDir, filter + "_Mlist.txt"))
                    {
                        using(var fileStream = new FileStream(listPath, FileMode.Open))
                        using(var streamReader = new StreamReader(fileStream))
                        {
                            var assetBundleName = streamReader.ReadLine();

                            var listLoader = new CustomDataListLoader();
                            listLoader.Load(streamReader);

                            var abc = new AssetBundleController();
                            abc.OpenFromFile(Path.Combine(folder, "abdata"), assetBundleName);

                            var count = datas.Count;
                            ___action(datas, abc, listLoader);
                            for(int i = count; i < datas.Count; i++)
                            {
                                var prefabData = (ItemDataBase)datas.Values.ElementAt(i);
                                prefabData.assetbundleDir = abdataDir;
                            }

                            abc.Close();
                        }
                    }
                }
            }
        }
    }
}
