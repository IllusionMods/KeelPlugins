using System;
using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using KeelPlugins;
using System.Collections;
using TitleShortcuts.Core;
using UnityEngine;

[assembly: System.Reflection.AssemblyVersion(TitleShortcuts.KoikatuSunshine.TitleShortcuts.Version)]

namespace TitleShortcuts.KoikatuSunshine
{
    [BepInProcess(Constants.MainGameProcessName)]
    //[BepInProcess(Constants.MainGameProcessNameSteam)]
    [BepInPlugin(GUID, PluginName, Version)]
    public class TitleShortcuts : TitleShortcutsCore
    {
        public const string Version = "1.3.1." + BuildNumber.Version;

        private static ConfigEntry<KeyboardShortcut> StartFemaleMaker { get; set; }
        private static ConfigEntry<KeyboardShortcut> StartMaleMaker { get; set; }
        private static ConfigEntry<KeyboardShortcut> StartUploader { get; set; }
        private static ConfigEntry<KeyboardShortcut> StartDownloader { get; set; }
        private static ConfigEntry<KeyboardShortcut> StartFreeH { get; set; }
        private static ConfigEntry<KeyboardShortcut> StartGameLoad { get; set; }

        protected override void Awake()
        {
            base.Awake();

            StartFemaleMaker = Config.Bind(SECTION_HOTKEYS, "Open female maker", new KeyboardShortcut(KeyCode.F));
            StartMaleMaker = Config.Bind(SECTION_HOTKEYS, "Open male maker", new KeyboardShortcut(KeyCode.M));
            StartUploader = Config.Bind(SECTION_HOTKEYS, "Open uploader", new KeyboardShortcut(KeyCode.U));
            StartDownloader = Config.Bind(SECTION_HOTKEYS, "Open downloader", new KeyboardShortcut(KeyCode.D));
            StartFreeH = Config.Bind(SECTION_HOTKEYS, "Start free H", new KeyboardShortcut(KeyCode.H));
            StartGameLoad = Config.Bind(SECTION_HOTKEYS, "Open Load Game screen", new KeyboardShortcut(KeyCode.G));

            Harmony.CreateAndPatchAll(GetType());
        }

        [HarmonyPrefix, HarmonyPatch(typeof(TitleScene), nameof(TitleScene.Start))]
        private static void TitleStart(TitleScene __instance)
        {
            Plugin.StartCoroutine(TitleInput(__instance));
        }

        private static IEnumerator TitleInput(TitleScene titleScene)
        {
            do
            {
                yield return null;

                if(StartFemaleMaker.Value.IsDown()) StartMode(titleScene.OnCustomFemale, "Starting female maker");
                else if(StartMaleMaker.Value.IsDown()) StartMode(titleScene.OnCustomMale, "Starting male maker");
                else if(StartUploader.Value.IsDown()) StartMode(titleScene.OnUploader, "Starting uploader");
                else if(StartDownloader.Value.IsDown()) StartMode(titleScene.OnDownloader, "Starting downloader");
                else if(StartFreeH.Value.IsDown()) StartMode(titleScene.OnOtherFreeH, "Starting free H");
                else if(StartGameLoad.Value.IsDown()) StartMode(titleScene.OnLoad, "Opening Load Game screen");
            }
            while(titleScene);

            void StartMode(Action action, string msg)
            {
                if(titleScene && (!FindObjectOfType<ConfigScene>() || !ConfigScene.active))
                {
                    Log.Message(msg);
                    action();
                    titleScene = null;
                }
            }
        }
        
        [HarmonyPostfix, HarmonyPatch(typeof(FreeHScene), nameof(FreeHScene.Start))]
        private static void TitleStart(FreeHScene __instance, ref IEnumerator __result)
        {
            var origResult = __result;
            __result = new object[]{origResult, HSceneInput(__instance)}.GetEnumerator();
        }

        private static IEnumerator HSceneInput(FreeHScene freeHScene)
        {
            for(int i = 0; i < 4; i++)
                yield return null;
            
            while(freeHScene != null)
            {
                if(StartFreeH.Value.IsDown())
                {
                    freeHScene.enterButton.onClick.Invoke();
                    freeHScene = null;
                }
                yield return null;
            }
        }
    }
}
