using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using ActionGame;
using BepInEx;
using ChaCustom;
using FreeH;
using HarmonyLib;
using Manager;
using UniRx;

namespace FreeHDefaults.Koikatu
{
    [BepInPlugin(GUID, "FreeH Defaults", Version)]
    public class FreeHDefaults : BaseUnityPlugin
    {
        public const string GUID = "keelhauled.freehdefaults";
        public const string Version = "1.0.0." + BuildNumber.Version;

        private static Harmony harmony;
        private static List<CustomFileInfo> lastFileList;
        private static Savedata saveData = new Savedata();
        private static readonly string saveFilePath = Path.Combine(Paths.ConfigPath, "FreeHDefaults.xml");
        private static readonly XmlSerializer xmlSerializer = new XmlSerializer(typeof(Savedata));
        private static readonly int[] modeMagic = { 0, 1012, 1100, 3000, 4000 };

        private void Start()
        {
            Log.SetLogSource(Logger);
            harmony = Harmony.CreateAndPatchAll(typeof(FreeHDefaults));

            if(File.Exists(saveFilePath))
            {
                using(var reader = new StreamReader(saveFilePath))
                    saveData = (Savedata)xmlSerializer.Deserialize(reader);
            }

#if DEBUG
            TitleScene_Start();
#endif
        }
#if DEBUG
        private void OnDestroy() => harmony.UnpatchSelf();
#endif

        [HarmonyPostfix, HarmonyPatch(typeof(TitleScene), "Start")]
        private static void TitleScene_Start()
        {
            var backData = Singleton<Scene>.Instance.commonSpace.GetOrAddComponent<FreeHBackData>();
            backData.heroine = LoadChara(saveData.HeroinePath, x => backData.heroine = new SaveData.Heroine(x, false));
            backData.partner = LoadChara(saveData.PartnerPath, x => backData.partner = new SaveData.Heroine(x, false));
            backData.player = LoadChara(saveData.PlayerPath, x => backData.player = new SaveData.Player(x, false));
            backData.timeZone = saveData.timeZone;
            backData.stageH1 = saveData.stageH1;
            backData.stageH2 = saveData.stageH2;
            backData.statusH = saveData.statusH;
            backData.discovery = saveData.discovery;
            backData.categorys = new List<int>{ saveData.category };
        }

        [HarmonyPrefix, HarmonyPatch(typeof(FreeHScene), "NormalSetup")]
        private static void HookProps(FreeHScene __instance)
        {
            var member = Traverse.Create(__instance).Field("member").GetValue<FreeHScene.Member>();

            member.resultHeroine.Where(x => x != null).Subscribe(x => SaveChara(x.charFile, y => saveData.HeroinePath = y));
            member.resultPartner.Where(x => x != null).Subscribe(x => SaveChara(x.charFile, y => saveData.PartnerPath = y));
            member.resultPlayer.Where(x => x != null).Subscribe(x => SaveChara(x.charFile, y => saveData.PlayerPath = y));

            member.resultTimeZone.Subscribe(x => SaveValue(() => saveData.timeZone = x));
            member.resultStage1.Subscribe(x => SaveValue(() => saveData.stageH1 = x));
            member.resultStage2.Subscribe(x => SaveValue(() => saveData.stageH2 = x));
            member.resultStatus.Subscribe(x => SaveValue(() => saveData.statusH = x));
            member.resultDiscovery.Subscribe(x => SaveValue(() => saveData.discovery = x));
        }

        [HarmonyPrefix, HarmonyPatch(typeof(FreeHScene), "SetMainCanvasObject")]
        private static void SaveMode(int _mode)
        {
            saveData.category = modeMagic[_mode];
            SaveXml();
        }

        [HarmonyPostfix, HarmonyPatch(typeof(FreeHClassRoomCharaFile), "Start")]
        private static void UpdateFileList(FreeHClassRoomCharaFile __instance)
        {
            lastFileList = Traverse.Create(__instance).Field("listCtrl").Field("lstFileInfo").GetValue<List<CustomFileInfo>>();
        }
        
        [HarmonyPostfix, HarmonyPatch(typeof(FreeHPreviewCharaList), "Start")]
        private static void HookCancelButton(FreeHPreviewCharaList __instance)
        {
            __instance.onCancel += () => lastFileList = null;
        }

        private static T LoadChara<T>(string path, Func<ChaFileControl, T> action)
        {
            var chaFileControl = new ChaFileControl();
            chaFileControl.LoadCharaFile(path, 1);
            return action(chaFileControl);
        }

        private static void SaveChara(ChaFile chaFile, Action<string> action)
        {
            var fullPath = lastFileList?.First(x => x.FileName == chaFile.charaFileName.Remove(chaFile.charaFileName.Length - 4)).FullPath;
            lastFileList = null;
            
            if(!string.IsNullOrEmpty(fullPath))
            {
                action(fullPath);
                SaveXml();
            }
        }

        private static void SaveValue(Action action)
        {
            action();
            SaveXml();
        }

        private static void SaveXml()
        {
            using(var writer = new StreamWriter(saveFilePath))
                xmlSerializer.Serialize(writer, saveData); 
        }
    }
}