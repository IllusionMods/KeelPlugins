using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using BepInEx;
using BepInEx.Configuration;
using ChaCustom;
using FreeH;
using HarmonyLib;
using Illusion.Component;
using Manager;
using UniRx;
using UnityEngine;

[assembly: System.Reflection.AssemblyFileVersion(FreeHDefaults.Koikatu.FreeHDefaults.Version)]

namespace FreeHDefaults.Koikatu
{
    [BepInPlugin(GUID, "FreeH Defaults", Version)]
#if KK
    [BepInProcess("Koikatu")]
    [BepInProcess("Koikatsu Party")]
    //todo VR doesn't work because different FreeHScene class are used. Could take code from FreeHRandom but it'll be a mess
    //[BepInProcess("KoikatuVR")]
    //[BepInProcess("Koikatsu Party VR")]
#elif KKS
    [BepInProcess("KoikatsuSunshine")]
    //[BepInProcess("KoikatsuSunshine_VR")]
#endif
    public class FreeHDefaults : BaseUnityPlugin
    {
        public const string GUID = "keelhauled.freehdefaults";
        public const string Version = "2.0.0." + BuildNumber.Version;

        private Harmony harmony;
        private static readonly string saveFilePath = Path.Combine(Paths.ConfigPath, "FreeHDefaults.xml");
        private static readonly XmlSerializer xmlSerializer = new XmlSerializer(typeof(Savedata));
        /// <summary>
        /// This is used to tell the game what tab free h menu should open on (normal, mast, lesb, 3p, darkness).
        /// For darkness this should be 4000 instead of 0 but that triggers an issue where non-darkness card can be
        /// loaded in darkness slot, so instead just load into normal to be safe.
        /// </summary>
        private static readonly int[] modeMagic = { 0, 1012, 1100, 3000, 0 };
        private static readonly Dictionary<ChaFile, string> chaFileFullPathLookup = new Dictionary<ChaFile, string>();
        private static Savedata saveData = new Savedata();

        private void Awake()
        {
            Log.SetLogSource(Logger);
            harmony = Harmony.CreateAndPatchAll(typeof(FreeHDefaults));

            harmony.Patch(original: AccessTools.FirstMethod(typeof(ChaFile), info => info.Name == nameof(ChaFile.LoadFile) && info.GetParameters().FirstOrDefault()?.ParameterType == typeof(BinaryReader)),
                          prefix: new HarmonyMethod(typeof(FreeHDefaults), nameof(FreeHDefaults.ChaFileLoadHook)) { wrapTryCatch = true });

            if (File.Exists(saveFilePath))
            {
                using (var reader = new StreamReader(saveFilePath))
                    saveData = (Savedata)xmlSerializer.Deserialize(reader);
            }
        }

        private static void ChaFileLoadHook(ChaFile __instance, BinaryReader br)
        {
            // Keep track of what filenames cards get loaded from
            // Doesn't handle studio scenes and files loaded from memory but it doesn't matter here
            if (br.BaseStream is FileStream fs)
            {
                // .Name should already be the full path, but it usually has a bunch of ../ in it, GetFullPath will clean it up
                var fullPath = Path.GetFullPath(fs.Name);
                chaFileFullPathLookup[__instance] = fullPath;
#if DEBUG
                Log.Debug($"FullName for {__instance} is {fullPath}");
#endif
            }
#if DEBUG
            else
                Log.Warning($"Failed to get FullName for {__instance}, BaseStream is {br.BaseStream} in {new StackTrace()}");
#endif
        }

        [HarmonyPostfix, HarmonyPatch(typeof(TitleScene), nameof(TitleScene.Start)), HarmonyWrapSafe]
        private static void PrepareSavedata()
        {
            // vanilla code will load values from FreeHBackData when freeh chara select is started
#if KK
            var backData = Singleton<Scene>.Instance.commonSpace.AddComponent<FreeHBackData>();
#elif KKS
            var backData = Scene.commonSpace.AddComponent<FreeHBackData>();
#endif
            backData.heroine = LoadChara(saveData.HeroinePath, x => backData.heroine = new SaveData.Heroine(x, false));
            backData.partner = LoadChara(saveData.PartnerPath, x => backData.partner = new SaveData.Heroine(x, false));
            backData.player = LoadChara(saveData.PlayerPath, x => backData.player = new SaveData.Player(x, false));
            backData.map = saveData.map;
            backData.timeZone = saveData.timeZone;
            backData.stageH1 = saveData.stageH1;
            backData.stageH2 = saveData.stageH2;
            backData.statusH = saveData.statusH;
            backData.discovery = saveData.discovery;
            backData.categorys = new List<int> { saveData.category };
        }

        [HarmonyPrefix, HarmonyPatch(typeof(FreeHScene), nameof(FreeHScene.NormalSetup)), HarmonyWrapSafe]
        private static void HookFreeHProps(FreeHScene __instance)
        {
            // Handle removed maps
            if (!__instance.member.map.infoDic.ContainsKey(__instance.mapNo))
            {
                __instance.mapNo = __instance.member.map.infoDic.Keys.Min();
                saveData.map = __instance.mapNo;
                SaveXml();
            }

            var member = __instance.member;
            member.resultHeroine.Where(x => x != null).Subscribe(x => SaveChara(x.charFile, y => saveData.HeroinePath = y));
            member.resultPartner.Where(x => x != null).Subscribe(x => SaveChara(x.charFile, y => saveData.PartnerPath = y));
            member.resultPlayer.Where(x => x != null).Subscribe(x => SaveChara(x.charFile, y => saveData.PlayerPath = y));
            member.resultMapInfo.Subscribe(x => { if (x != null) saveData.map = x.No; SaveXml(); });
            member.resultTimeZone.Subscribe(x => { saveData.timeZone = x; SaveXml(); });
            member.resultStage1.Subscribe(x => { saveData.stageH1 = x; SaveXml(); });
            member.resultStage2.Subscribe(x => { saveData.stageH2 = x; SaveXml(); });
            member.resultStatus.Subscribe(x => { saveData.statusH = x; SaveXml(); });
            member.resultDiscovery.Subscribe(x => { saveData.discovery = x; SaveXml(); });
        }

        [HarmonyPrefix, HarmonyPatch(typeof(FreeHScene), nameof(FreeHScene.SetMainCanvasObject)), HarmonyWrapSafe]
        private static void SaveMode(int _mode)
        {
            saveData.category = modeMagic.SafeGet(_mode);
            SaveXml();
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

            while (true)
            {
                if (Math.Abs(tempFov - camera.CameraFov) > 0.001f)
                {
                    tempFov = saveData.Fov = camera.CameraFov;
                    SaveXml();
                }

                yield return new WaitForSeconds(5f);
            }
        }

        private static T LoadChara<T>(string path, Func<ChaFileControl, T> action) where T : SaveData.CharaData
        {
            if (!File.Exists(path))
                return null;

            var chaFileControl = new ChaFileControl();
            chaFileControl.LoadCharaFile(path, 1);
            return action(chaFileControl);
        }

        private static void SaveChara(ChaFile chaFile, Action<string> action)
        {
            if (chaFile == null) throw new ArgumentNullException(nameof(chaFile));
            if (action == null) throw new ArgumentNullException(nameof(action));
            // Need to do this because of differences in KK and KKP
            chaFileFullPathLookup.TryGetValue(chaFile, out var fullPath);
            if (!string.IsNullOrEmpty(fullPath))
            {
                action(fullPath);
                SaveXml();
            }
        }

        private static void SaveXml()
        {
            using (var writer = new StreamWriter(saveFilePath))
                xmlSerializer.Serialize(writer, saveData);
        }
    }
}