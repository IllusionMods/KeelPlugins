using BepInEx;
using System;
using System.IO;
using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ModLoader.PlayHome
{
    [BepInPlugin(GUID, "ModLoader", Version)]
    public class Loader : BaseUnityPlugin
    {
        public const string GUID = "keelhauled.modloader";
        public const string Version = "1.0.0." + BuildNumber.Version;

        internal static string ModFolder = Path.Combine(Paths.GameRootPath, "mods");
        private static Dictionary<string, string> AssetAbdata = new Dictionary<string, string>();

        private void Awake()
        {
            Log.SetLogSource(Logger);
            var harmony = Harmony.CreateAndPatchAll(typeof(Loader));

            {
                var type = typeof(PrefabData);
                var targetType = type.Assembly.GetType($"CustomDataSetupLoader`1[{type.Name}]");
                var dicType = typeof(Dictionary<,>).MakeGenericType(new[] { typeof(int), type });
                var targetMethod = AccessTools.Method(targetType, "Setup_Search", new[] { dicType, typeof(string) });
                var patchMethod = AccessTools.Method(typeof(Loader), nameof(LoadMods));
                harmony.Patch(targetMethod, new HarmonyMethod(patchMethod));
            }

            {
                var targetMethod = AccessTools.Method(typeof(AssetBundleController), nameof(AssetBundleController.LoadAsset)).MakeGenericMethod(typeof(UnityEngine.Object));
                var patchMethod = AccessTools.Method(typeof(Loader), nameof(LoadAsset));
                harmony.Patch(targetMethod, new HarmonyMethod(patchMethod));
            }
        }

        public static bool LoadAsset(ref object __result, ref string assetName, AssetBundleController __instance)
        {
            if(__instance.assetBundleName.Contains("thumnbnail/thumbnail_") || __instance.assetBundleName.Contains("thumnbnail/thumnbs_"))
            {
                if(AssetAbdata.TryGetValue(assetName, out var modAbdataDir))
                {
                    var path = BepInEx.Utility.CombinePaths(modAbdataDir, "thumnbnail_R", assetName + ".png");
                    if(File.Exists(path))
                    {
                        __result = LoadPNG(path);
                        return false;
                    } 
                }
            }

            return true;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(ItemDataBase), MethodType.Constructor, new[] { typeof(int), typeof(string), typeof(string), typeof(int), typeof(bool) })]
        private static void ExtendIds(ref int ___id, int id)
        {
            ___id = id > 999999 && id < 1000000000 ? id : id % 1000;
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

            foreach(var folder in Directory.GetDirectories(ModFolder))
            {
                var modAbDir = Path.Combine(folder, "abdata");
                var modListDir = BepInEx.Utility.CombinePaths(folder, "abdata", "list", dir);

                if(Directory.Exists(modListDir))
                {
                    foreach(var list in Directory.GetFiles(modListDir, filter + "_Mlist.txt"))
                    {
                        using(var fileStream = new FileStream(list, FileMode.Open))
                        using(var streamReader = new StreamReader(fileStream))
                        {
                            var assetBundleName = streamReader.ReadLine();
                            var contents = streamReader.ReadToEnd();

                            var tempFileName = Path.GetTempFileName();
                            File.WriteAllText(tempFileName, contents);
                            var customDataListLoader = new CustomDataListLoader();
                            customDataListLoader.Load(tempFileName);
                            File.Delete(tempFileName);

                            var assetBundleController2 = new AssetBundleController();
                            assetBundleController2.OpenFromFile(Path.Combine(folder, "abdata"), assetBundleName);

                            var count = datas.Count;
                            ___action(datas, assetBundleController2, customDataListLoader);
                            for(int i = count; i < datas.Count; i++)
                            {
                                var prefabData = (PrefabData)datas.Values.ElementAt(i);
                                prefabData.assetbundleDir = modAbDir;
                            }

                            assetBundleController2.Close(false);
                        }
                    }

                    foreach(var item in Directory.GetFiles(Path.Combine(modAbDir, "thumnbnail_R"), "*.png"))
                        AssetAbdata[Path.GetFileNameWithoutExtension(item)] = modAbDir;
                }
            }
        }

        //[HarmonyPrefix]
        //[HarmonyPatch(typeof(EditMode), "CreateData", new[] { typeof(PrefabData), typeof(Rect), typeof(AssetBundleController) })]
        //[HarmonyPatch(typeof(EditMode), "CreateData", new[] { typeof(CombineTextureData), typeof(Rect), typeof(AssetBundleController) })]
        //[HarmonyPatch(typeof(EditMode), "CreateData", new[] { typeof(HeadData), typeof(Rect), typeof(AssetBundleController) })]
        //[HarmonyPatch(typeof(EditMode), "CreateData", new[] { typeof(AccessoryData), typeof(Rect), typeof(AssetBundleController) })]
        //private static bool LoadThumbs(ref CustomSelectSet __result, Rect smallRect, AssetBundleController abc, ItemDataBase item)
        //{
        //    if(item.id == 721523105)
        //    {
        //        Log.Info("TEMP");
        //    }

        //    var traverse = Traverse.Create(item);
        //    var prefab = traverse.Field("prefab").GetValue<string>();
        //    var textureName = traverse.Field("textureName").GetValue<string>();
        //    var path = traverse.Field("path").GetValue<string>();
        //    var prefab_F = traverse.Field("prefab_F").GetValue<string>();
        //    var assetName = prefab ?? textureName ?? path ?? prefab_F;

        //    Texture2D texture = null;
        //    if(item.assetbundleDir.Contains(ModFolder))
        //    {
        //        if(abc.assetBundleName.Contains("thumnbnail/thumbnail_") || abc.assetBundleName.Contains("thumnbnail/thumnbs_"))
        //        {
        //            string thumbPath = BepInEx.Utility.CombinePaths(item.assetbundleDir, "thumnbnail_R", assetName + ".png");
        //            if(File.Exists(thumbPath))
        //            {
        //                Log.Info("Load thumb: " + thumbPath);
        //                texture = LoadPNG(thumbPath);
        //            }
        //            else
        //            {
        //                Log.Info($"Thumbnail '{thumbPath}' does not exist");
        //            }
        //        }
        //    }
        //    else
        //        texture = abc.LoadAsset<Texture2D>(assetName);

        //    var vector = new Vector2(256f, 256f);
        //    var thumbnail_L = Sprite.Create(texture, new Rect(Vector2.zero, vector), vector * 0.5f, 100f, 0U, SpriteMeshType.FullRect);
        //    var thumbnail_S = Sprite.Create(texture, smallRect, smallRect.size * 0.5f, 100f, 0U, SpriteMeshType.FullRect);
        //    __result = new CustomSelectSet(item.id, item.name_LineFeed, thumbnail_S, thumbnail_L, item.isNew);

        //    return false;
        //}
    }
}
