using HarmonyLib;
using System;
using System.Collections.Generic;
using System.IO;

namespace KeelPlugins
{
    internal static class CustomDataSetupLoaderPatch
    {
        public static void Patch(Harmony harmony)
        {
            var typeList = new List<Type>
            {
                typeof(PrefabData),
                typeof(HairData),
                typeof(BackHairData),
                typeof(CombineTextureData),
                typeof(UnderhairData),
                typeof(WearData),
                typeof(AccessoryData),
            };

            foreach(var type in typeList)
            {
                var patchType = type.Assembly.GetType($"CustomDataSetupLoader`1[{type.Name}]");
                var dicType = typeof(Dictionary<,>).MakeGenericType(new Type[] { typeof(int), type });

                var setupMethod = AccessTools.Method(patchType, "Setup", new Type[] { dicType, typeof(AssetBundleController) });
                var setupPrefix = AccessTools.Method(typeof(CustomDataSetupLoaderPatch), nameof(SetupPrefix));
                harmony.Patch(setupMethod, new HarmonyMethod(setupPrefix), null);

                var setupSearchMethod = AccessTools.Method(patchType, "Setup_Search", new Type[] { dicType, typeof(string) });
                var setupSearchPrefix = AccessTools.Method(typeof(CustomDataSetupLoaderPatch), nameof(SetupSearchPrefix));
                harmony.Patch(setupSearchMethod, new HarmonyMethod(setupSearchPrefix), null);
            }
        }

        private static bool SetupPrefix(object __instance, ref object datas, ref AssetBundleController abc)
        {
            string text = BepInEx.Utility.CombinePaths(MoreSlotID.ModFolder, "list", abc.assetBundleName, "_list.txt");
            if(File.Exists(text))
            {
                Console.WriteLine("Load list:" + text);
                CustomDataListLoader customDataListLoader = new CustomDataListLoader();
                customDataListLoader.Load(text);
                var args = new Type[] { datas.GetType(), typeof(AssetBundleController), typeof(CustomDataListLoader) };
                Traverse.Create(__instance).Field("action").Method("Invoke", args).GetValue(datas, abc, customDataListLoader);
                return false;
            }

            return true;
        }

        private static bool SetupSearchPrefix(object __instance, ref object datas, ref string search)
        {
            string text = string.Empty;
            int num = search.LastIndexOf("/");
            if(num != -1)
            {
                text = search.Substring(0, num);
                search = search.Remove(0, num + 1);
            }

            string[] files = Directory.GetFiles(GlobalData.assetBundlePath + "/" + text, search, SearchOption.TopDirectoryOnly);
            Array.Sort(files);
            foreach(string path in files)
            {
                if(Path.GetExtension(path).Length == 0)
                {
                    string text2 = Path.GetFileNameWithoutExtension(path);
                    if(text.Length > 0)
                    {
                        text2 = text + "/" + text2;
                    }
                    AssetBundleController assetBundleController = new AssetBundleController();
                    assetBundleController.OpenFromFile(GlobalData.assetBundlePath, text2);
                    var args = new Type[] { datas.GetType(), typeof(AssetBundleController) };
                    Traverse.Create(__instance).Method("Setup", args).GetValue(datas, assetBundleController);
                    assetBundleController.Close(false);
                }
            }

            if(Directory.Exists(MoreSlotID.ModFolder))
            {
                foreach(var modDir in Directory.GetDirectories(MoreSlotID.ModFolder, "*", SearchOption.TopDirectoryOnly))
                {
                    var abdataDir = Path.Combine(modDir, "abdata");
                    var listDir = BepInEx.Utility.CombinePaths(abdataDir, "list", text);

                    if(Directory.Exists(listDir))
                    {
                        foreach(string list in Directory.GetFiles(listDir, search + "_Mlist.txt"))
                        {
                            Console.WriteLine("Load Mlist: " + list);

                            StreamReader streamReader = new StreamReader(new FileStream(list, FileMode.Open));
                            string assetBundleName = streamReader.ReadLine();

                            MoreSlotID.AssetList[assetBundleName] = abdataDir;

                            string contents = streamReader.ReadToEnd();
                            string tempFileName = Path.GetTempFileName();

                            File.WriteAllText(tempFileName, contents);
                            CustomDataListLoader customDataListLoader = new CustomDataListLoader();
                            customDataListLoader.Load(tempFileName);
                            File.Delete(tempFileName);

                            foreach(var data in Traverse.Create(customDataListLoader).Field("datas").GetValue<List<string[]>>())
                                MoreSlotID.ThumbList[data[data.Length - 1]] = abdataDir;

                            AssetBundleController assetBundleController2 = new AssetBundleController();
                            assetBundleController2.OpenFromFile(abdataDir, assetBundleName);
                            var args = new Type[] { datas.GetType(), typeof(AssetBundleController), typeof(CustomDataListLoader) };
                            Traverse.Create(__instance).Field("action").Method("Invoke", args).GetValue(datas, assetBundleController2, customDataListLoader);
                            assetBundleController2.Close(false);
                        }
                    }
                }
            }

            return false;
        }
    }
}
