using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace ModLoader.PlayHome
{
    [BepInPlugin(GUID, PluginName, Version)]
    public class ModLoader : BaseUnityPlugin
    {
        public const string GUID = "keelhauled.modloader";
        public const string PluginName = "ModLoader";
        public const string Version = "1.0.0." + BuildNumber.Version;

        private static new ManualLogSource Logger;
        private Harmony Harmony;

        private Type[] types = new[]
        {
            typeof(PrefabData),
            typeof(HairData),
            typeof(BackHairData),
            typeof(CombineTextureData),
            typeof(UnderhairData),
            typeof(WearData),
            typeof(AccessoryData),
        };

        private void Awake()
        {
            Logger = base.Logger;
            Harmony = new Harmony($"{GUID}.harmony");

            Harmony.Patch(typeof(AssetBundleController).GetMethod("LoadAsset", AccessTools.all).MakeGenericMethod(typeof(Texture2D)),
                          prefix: new HarmonyMethod(GetType().GetMethod(nameof(LoadAssetPatch), AccessTools.all)));

            Harmony.Patch(typeof(ItemDataBase).GetConstructors(AccessTools.all).First(),
                          prefix: new HarmonyMethod(GetType().GetMethod(nameof(IDPatch), AccessTools.all)));

            foreach(var type in types)
            {
                var targetType = type.Assembly.GetType("CustomDataSetupLoader`1").MakeGenericType(type);
                var dicType = typeof(Dictionary<,>).MakeGenericType(typeof(int), type);

                Harmony.Patch(targetType.GetMethod("Setup", AccessTools.all),
                              prefix: new HarmonyMethod(GetType().GetMethod(nameof(SetupPatch), AccessTools.all)));

                Harmony.Patch(targetType.GetMethod("Setup_Search", AccessTools.all),
                              prefix: new HarmonyMethod(GetType().GetMethod(nameof(SetupSearchPrefix), AccessTools.all)),
                              postfix: new HarmonyMethod(GetType().GetMethod(nameof(SetupSearchPatch), AccessTools.all)));
            }
        }

        private static bool LoadAssetPatch(AssetBundleController __instance, ref string assetName, ref object __result)
        {
            if(__instance.assetBundleName.Contains("thumnbnail/thumbnail_") || __instance.assetBundleName.Contains("thumnbnail/thumnbs_"))
            {
                string text = __instance.directory + "/thumnbnail_R/" + assetName + ".png";
                if(File.Exists(text))
                {
                    Logger.LogInfo("Load thumb:" + text);
                    __result = LoadPNG(text);
                    return false;
                }
            }

            return true;
        }

        private static Texture2D LoadPNG(string file)
        {
            byte[] array;

            using(var fileStream = new FileStream(file, FileMode.Open, FileAccess.Read))
            using(var binaryReader = new BinaryReader(fileStream))
            {
                try
                {
                    array = binaryReader.ReadBytes((int)binaryReader.BaseStream.Length);
                }
                catch(Exception ex)
                {
                    Logger.LogWarning(ex);
                    array = null;
                }

                binaryReader.Close();
            }

            if(array == null)
                return null;

            var texture2D = new Texture2D(1, 1, TextureFormat.ARGB32, false);
            texture2D.LoadImage(array);
            return texture2D;
        }

        private static bool IDPatch(ItemDataBase __instance, ref int id, ref string name, ref string assetbundleName, ref int order, ref bool isNew)
        {
            name = name.Replace("\\n", "\n");
            __instance.id = ((id > 999999 && id < 1000000000) ? id : (id % 1000));
            __instance.name_LineFeed = name;
            __instance.name = name.Replace("\n", string.Empty);
            __instance.assetbundleDir = GlobalData.assetBundlePath;
            __instance.assetbundleName = assetbundleName;
            __instance.order = order;
            __instance.isNew = isNew;

            return false;
        }

        private static bool SetupPatch(ref AssetBundleController abc, ref Action<object, AssetBundleController, CustomDataListLoader> ___action, ref object datas)
        {
            string text = abc.directory + "/list/" + abc.assetBundleName + "_list.txt";
            if(File.Exists(text))
            {
                Logger.LogInfo("Load list:" + text);
                CustomDataListLoader customDataListLoader = new CustomDataListLoader();
                customDataListLoader.Load(text);
                ___action(datas, abc, customDataListLoader);
                return false;
            }

            return true;
        }

        private static string searchValue;

        private static void SetupSearchPrefix(ref string search)
        {
            searchValue = search;
        }

        private static void SetupSearchPatch(ref object datas, ref string search, ref Action<object, AssetBundleController, CustomDataListLoader> ___action)
        {
            int num = searchValue.LastIndexOf("/");
            var text = searchValue.Substring(0, num);

            if(!Directory.Exists(GlobalData.assetBundlePath + "/list/" + text))
                return;

            foreach(var text3 in Directory.GetFiles(GlobalData.assetBundlePath + "/list/" + text, search + "_Mlist.txt"))
            {
                Logger.LogInfo("Load Mlist:" + text3);
                StreamReader streamReader = new StreamReader(new FileStream(text3, FileMode.Open));
                string assetBundleName = streamReader.ReadLine();
                string contents = streamReader.ReadToEnd();
                string tempFileName = Path.GetTempFileName();
                File.WriteAllText(tempFileName, contents);
                CustomDataListLoader customDataListLoader = new CustomDataListLoader();
                customDataListLoader.Load(tempFileName);
                File.Delete(tempFileName);
                AssetBundleController assetBundleController2 = new AssetBundleController();
                assetBundleController2.OpenFromFile(GlobalData.assetBundlePath, assetBundleName);
                ___action(datas, assetBundleController2, customDataListLoader);
                assetBundleController2.Close(false);
            }
        }
    }
}
