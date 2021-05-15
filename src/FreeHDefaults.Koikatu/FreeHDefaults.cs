﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using BepInEx;
using ChaCustom;
using FreeH;
using HarmonyLib;
using Illusion.Component;
using Manager;
using UniRx;
using UnityEngine;

namespace FreeHDefaults.Koikatu
{
    [BepInPlugin(GUID, "FreeH Defaults", Version)]
    public class FreeHDefaults : BaseUnityPlugin
    {
        public const string GUID = "keelhauled.freehdefaults";
        public const string Version = "1.0.0." + BuildNumber.Version;

        private static List<CustomFileInfo> lastFileList;
        private static Savedata saveData = new Savedata();
        private static readonly string saveFilePath = Path.Combine(Paths.ConfigPath, "FreeHDefaults.xml");
        private static readonly XmlSerializer xmlSerializer = new XmlSerializer(typeof(Savedata));
        private static readonly int[] modeMagic = { 0, 1012, 1100, 3000, 4000 };

        private void Start()
        {
            Log.SetLogSource(Logger);
            Harmony.CreateAndPatchAll(typeof(FreeHDefaults));

            if(File.Exists(saveFilePath))
            {
                using(var reader = new StreamReader(saveFilePath))
                    saveData = (Savedata)xmlSerializer.Deserialize(reader);
            }
        }

        [HarmonyPostfix, HarmonyPatch(typeof(TitleScene), nameof(TitleScene.Start))]
        private static void PrepareSavedata()
        {
            // vanilla code will load values from FreeHBackData when freeh chara select is started
            var backData = Singleton<Scene>.Instance.commonSpace.GetOrAddComponent<FreeHBackData>();
            backData.heroine = LoadChara(saveData.HeroinePath, x => backData.heroine = new SaveData.Heroine(x, false));
            backData.partner = LoadChara(saveData.PartnerPath, x => backData.partner = new SaveData.Heroine(x, false));
            backData.player = LoadChara(saveData.PlayerPath, x => backData.player = new SaveData.Player(x, false));
            backData.map = saveData.map;
            backData.timeZone = saveData.timeZone;
            backData.stageH1 = saveData.stageH1;
            backData.stageH2 = saveData.stageH2;
            backData.statusH = saveData.statusH;
            backData.discovery = saveData.discovery;
            backData.categorys = new List<int>{ saveData.category };
        }

        [HarmonyPrefix, HarmonyPatch(typeof(FreeHScene), nameof(FreeHScene.NormalSetup))]
        private static void HookFreeHProps(FreeHScene __instance)
        {
            var member = __instance.member;
            member.resultHeroine.Where(x => x != null).Subscribe(x => SaveChara(x.charFile, y => saveData.HeroinePath = y));
            member.resultPartner.Where(x => x != null).Subscribe(x => SaveChara(x.charFile, y => saveData.PartnerPath = y));
            member.resultPlayer.Where(x => x != null).Subscribe(x => SaveChara(x.charFile, y => saveData.PlayerPath = y));
            member.resultMapInfo.Subscribe(x  => { if(x != null) saveData.map = x.No; SaveXml(); });
            member.resultTimeZone.Subscribe(x => { saveData.timeZone = x; SaveXml(); });
            member.resultStage1.Subscribe(x => { saveData.stageH1 = x; SaveXml(); });
            member.resultStage2.Subscribe(x => { saveData.stageH2 = x; SaveXml(); });
            member.resultStatus.Subscribe(x => { saveData.statusH = x; SaveXml(); });
            member.resultDiscovery.Subscribe(x => { saveData.discovery = x; SaveXml(); });
        }

        [HarmonyPrefix, HarmonyPatch(typeof(FreeHScene), nameof(FreeHScene.SetMainCanvasObject))]
        private static void SaveMode(int _mode)
        {
            saveData.category = modeMagic[_mode];
            SaveXml();
        }

        [HarmonyPostfix, HarmonyPatch(typeof(FreeHClassRoomCharaFile), nameof(FreeHClassRoomCharaFile.Start))]
        private static void UpdateFileList(FreeHClassRoomCharaFile __instance)
        {
            lastFileList = __instance.listCtrl.lstFileInfo;
        }
        
        [HarmonyPostfix, HarmonyPatch(typeof(FreeHPreviewCharaList), nameof(FreeHPreviewCharaList.Start))]
        private static void HookCancelButton(FreeHPreviewCharaList __instance)
        {
            __instance.onCancel += () => lastFileList = null;
        }
        
        [HarmonyPostfix, HarmonyPatch(typeof(FreeHScene), nameof(FreeHScene.MasturbationSetup))]
        private static void FixDiscoveryLogic(FreeHScene __instance)
        {
            __instance.tglDiscoverySafeMasturbation.isOn = !__instance.discovery;
            __instance.tglDiscoveryOutMasturbation.isOn = __instance.discovery;
        }
        
        [HarmonyPostfix, HarmonyPatch(typeof(HSceneProc), nameof(HSceneProc.SetShortcutKey))]
        private static void HookHSceneValues(HSceneProc __instance)
        {
            __instance.flags.ctrlCamera.CameraFov = saveData.Fov;
            __instance.flags.isAibuSelect = saveData.isAibuSelect;
            
            __instance.gameObject.GetComponent<ShortcutKey>().procList
                .First(x => x.keyCode == KeyCode.A).call.AddListener(() =>
            {
                saveData.isAibuSelect = __instance.flags.isAibuSelect;
                SaveXml();
            });

            __instance.StartCoroutine(DetectFovChange(__instance.flags.ctrlCamera));
        }

        private static IEnumerator DetectFovChange(CameraControl_Ver2 camera)
        {
            yield return new WaitForSeconds(5f);
            float tempFov = 0;
            
            while(true)
            {
                if(Math.Abs(tempFov - camera.CameraFov) > 0.001f)
                {
                    tempFov = saveData.Fov = camera.CameraFov;
                    SaveXml();
                }
                
                yield return new WaitForSeconds(5f);
            }
        }

        private static T LoadChara<T>(string path, Func<ChaFileControl, T> action) where T : SaveData.CharaData
        {
            if(!File.Exists(path))
                return null;
            
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

        private static void SaveXml()
        {
            using(var writer = new StreamWriter(saveFilePath))
                xmlSerializer.Serialize(writer, saveData); 
        }
    }
}