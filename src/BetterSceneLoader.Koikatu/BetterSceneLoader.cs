using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using KeelPlugins;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using UILib;
using UnityEngine;
using UnityEngine.UI;
using KKAPI.Studio.UI;
using BetterSceneLoader.Core;

namespace BetterSceneLoader.Koikatu
{
    [BepInProcess(KoikatuConstants.StudioProcessName)]
    [BepInPlugin("keelhauled.bettersceneloader", "BetterSceneLoader", Version)]
    public class BetterSceneLoader : BetterSceneLoaderCore
    {
        public const string Version = "1.0.0";

        protected override void Awake()
        {
            base.Awake();
            Harmony.CreateAndPatchAll(GetType());
            UIUtility.InitKOI(typeof(BetterSceneLoader).Assembly);
        }

        [HarmonyPrefix, HarmonyPatch(typeof(StudioScene), "Start")]
        public static void StudioEntrypoint()
        {
            sceneLoaderUI.CreateUI();
            sceneLoaderUI.toolbarToggle = CustomToolbarButtons.AddLeftToolbarToggle(PngAssist.ChangeTextureFromByte(Properties.Resources.pluginicon), false, x => sceneLoaderUI.Show(x));
        }

        protected override void LoadScene(string path)
        {
            Plugin.StartCoroutine(Studio.Studio.Instance.LoadSceneCoroutine(path));
        }

        protected override void SaveScene(string path)
        {
            Studio.Studio.Instance.dicObjectCtrl.Values.ToList().ForEach(x => x.OnSavePreprocessing());
            Studio.Studio.Instance.sceneInfo.cameraSaveData = Studio.Studio.Instance.cameraCtrl.Export();
            Studio.Studio.Instance.sceneInfo.Save(path);
        }

        protected override void DeleteScene(string path)
        {
            File.Delete(path);
        }

        protected override void ImportScene(string path)
        {
            Studio.Studio.Instance.ImportScene(path);
        }
    }
}
