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
        private static FreeHDefaults plugin;
        private static List<CustomFileInfo> lastFileList;
        private static Savedata saveData = new Savedata();
        private static readonly string saveFilePath = Path.Combine(Paths.ConfigPath, "FreeHDefaults.xml");
        private static readonly XmlSerializer xmlSerializer = new XmlSerializer(typeof(Savedata));
        private static readonly int[] modeMagic = { 0, 1012, 1100, 3000, 4000 };

        private void Start()
        {
            Log.SetLogSource(Logger);
            plugin = this;
            harmony = Harmony.CreateAndPatchAll(typeof(FreeHDefaults));

            if(File.Exists(saveFilePath))
            {
                using(var reader = new StreamReader(saveFilePath))
                    saveData = (Savedata)xmlSerializer.Deserialize(reader);
            }
            
            //TitleScene_Start();
        }
        private void OnDestroy() => harmony.UnpatchSelf();

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

        private static T LoadChara<T>(string path, Func<ChaFileControl, T> action)
        {
            var chaFileControl = new ChaFileControl();
            chaFileControl.LoadCharaFile(path, 1);
            return action(chaFileControl);
        }

        [HarmonyPostfix, HarmonyPatch(typeof(FreeHScene), "Start")]
        private static void FreeHScene_Start_Postfix(FreeHScene __instance)
        {
            plugin.StartCoroutine(Delay());
            
            IEnumerator Delay()
            {
                yield return null;
                yield return null;
                
                var member = Traverse.Create(__instance).Field("member").GetValue<FreeHScene.Member>();

                member.resultHeroine.Where(x => x != null).Subscribe(x =>
                {
                    var fullPath = GetFullPath(x.charFile.charaFileName);
                    if(!string.IsNullOrEmpty(fullPath))
                    {
                        saveData.HeroinePath = fullPath;
                        SaveXml();
                    }
                });
                
                member.resultPartner.Where(x => x != null).Subscribe(x =>
                {
                    var fullPath = GetFullPath(x.charFile.charaFileName);
                    if(!string.IsNullOrEmpty(fullPath))
                    {
                        saveData.PartnerPath = fullPath;
                        SaveXml();
                    }
                });
                
                member.resultPlayer.Where(x => x != null).Subscribe(x =>
                {
                    var fullPath = GetFullPath(x.charFile.charaFileName);
                    if(!string.IsNullOrEmpty(fullPath))
                    {
                        saveData.PlayerPath = fullPath;
                        SaveXml();
                    }
                });

                member.resultTimeZone.Subscribe(x =>
                {
                    saveData.timeZone = x;
                    SaveXml();
                });

                member.resultStage1.Subscribe(x =>
                {
                    saveData.stageH1 = x;
                    SaveXml();
                });
                
                member.resultStage2.Subscribe(x =>
                {
                    saveData.stageH2 = x;
                    SaveXml();
                });

                member.resultStatus.Subscribe(x =>
                {
                    saveData.statusH = x;
                    SaveXml();
                });
                
                member.resultDiscovery.Subscribe(x =>
                {
                    saveData.discovery = x;
                    SaveXml();
                });
            }
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
            var listCtrl = Traverse.Create(__instance).Field("listCtrl").GetValue<ClassRoomFileListCtrl>();
            lastFileList = Traverse.Create(listCtrl).Field("lstFileInfo").GetValue<List<CustomFileInfo>>();
        }
        
        [HarmonyPostfix, HarmonyPatch(typeof(FreeHPreviewCharaList), "Start")]
        private static void HookCancelButton(FreeHPreviewCharaList __instance)
        {
            __instance.onCancel += () => lastFileList = null;
        }

        private static string GetFullPath(string fileName)
        {
            var path = lastFileList?.First(x => x.FileName == fileName.Remove(fileName.Length - 4)).FullPath;
            lastFileList = null;
            return path;
        }

        private static void SaveXml()
        {
            using(var writer = new StreamWriter(saveFilePath))
                xmlSerializer.Serialize(writer, saveData); 
        }
    }
}