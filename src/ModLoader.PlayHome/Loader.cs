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
        public const string Version = "1.0.0." + BuildNumber.Version;

        private static string modFolder = Path.Combine(Paths.GameRootPath, "mods");
        private static string[] modFolders;
        private static Dictionary<string, string> modFiles = new Dictionary<string, string>();
        private static Dictionary<string, string> assetAbdata = new Dictionary<string, string>();

        private void Awake()
        {
            Log.SetLogSource(Logger);
            HarmonyExtensions.CreateAndPatchAll(typeof(Loader));

            modFolders = Directory.GetDirectories(modFolder, "*", SearchOption.TopDirectoryOnly);
            modFiles = modFolders.SelectMany(x => Directory.GetFiles(Path.Combine(x, "abdata"), "*", SearchOption.AllDirectories)).ToDictionary(x => x.Substring(@"abdata\"), x => x);
            ResourceRedirection.RegisterAsyncAndSyncAssetBundleLoadingHook(0, Hook);
        }

        private void Hook(IAssetBundleLoadingContext context)
        {
            var normalized = context.GetNormalizedPath();

            if(normalized.Contains(@"abdata\"))
            {
                var path = normalized.Substring(@"abdata\");

                if(!context.Parameters.Path.Contains(modFolder) && modFiles.TryGetValue(path, out var newPath))
                {
                    context.Parameters.Path = newPath;
                } 
            }
        }

        [HarmonyPatchExt(typeof(AssetBundleController), nameof(AssetBundleController.LoadAsset), null, new[] { typeof(UnityEngine.Object) })]
        private static bool LoadThumbs(ref object __result, ref string assetName, AssetBundleController __instance)
        {
            if(__instance.assetBundleName.Contains("thumnbnail/thumbnail_") || __instance.assetBundleName.Contains("thumnbnail/thumnbs_"))
            {
                if(assetAbdata.TryGetValue(assetName, out var modAbdataDir))
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
        [HarmonyPatch(typeof(ItemDataBase), MethodType.Constructor, new[] { typeof(int), typeof(string), typeof(string), typeof(int), typeof(bool) })]
        private static void ExtendIds(ref int ___id, int id)
        {
            ___id = id > 999999 && id < 1000000000 ? id : id % 1000;
        }

        [HarmonyPatchExt("CustomDataSetupLoader`1[PrefabData], Assembly-CSharp", "Setup_Search", new[] { typeof(Dictionary<int, PrefabData>), typeof(string) })]
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
                            Traverse.Create(listLoader).Method("Load", new[] { typeof(TextReader) }).GetValue(streamReader);

                            var abc = new AssetBundleController();
                            abc.OpenFromFile(Path.Combine(folder, "abdata"), assetBundleName);

                            var count = datas.Count;
                            ___action(datas, abc, listLoader);
                            for(int i = count; i < datas.Count; i++)
                            {
                                var prefabData = (PrefabData)datas.Values.ElementAt(i);
                                prefabData.assetbundleDir = abdataDir;
                            }

                            abc.Close(false);
                        }
                    }

                    foreach(var thumbPath in Directory.GetFiles(Path.Combine(abdataDir, "thumnbnail_R"), "*.png"))
                        assetAbdata[Path.GetFileNameWithoutExtension(thumbPath)] = abdataDir;
                }
            }
        }
    }
}
