using System;
using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using KeelPlugins;
using System.Collections;
using KeelPlugins.Harmony;
using TitleShortcuts.Core;
using UnityEngine;

[assembly: System.Reflection.AssemblyVersion(TitleShortcuts.Koikatu.TitleShortcuts.Version)]

namespace TitleShortcuts.Koikatu
{
    [BepInProcess(Constants.MainGameProcessName)]
    [BepInProcess(Constants.MainGameProcessNameSteam)]
    [BepInPlugin(GUID, PluginName, Version)]
    public class TitleShortcuts : TitleShortcutsCore
    {
        public const string Version = "1.3.1." + BuildNumber.Version;

        private static ConfigEntry<KeyboardShortcut> StartFemaleMaker { get; set; }
        private static ConfigEntry<KeyboardShortcut> StartMaleMaker { get; set; }
        private static ConfigEntry<KeyboardShortcut> StartUploader { get; set; }
        private static ConfigEntry<KeyboardShortcut> StartDownloader { get; set; }
        private static ConfigEntry<KeyboardShortcut> StartFreeH { get; set; }
        private static ConfigEntry<KeyboardShortcut> StartLiveShow { get; set; }
        private static ConfigEntry<KeyboardShortcut> StartGameLoad { get; set; }

        protected override void Awake()
        {
            base.Awake();

            StartFemaleMaker = Config.Bind(SECTION_HOTKEYS, "Open female maker", new KeyboardShortcut(KeyCode.F));
            StartMaleMaker = Config.Bind(SECTION_HOTKEYS, "Open male maker", new KeyboardShortcut(KeyCode.M));
            StartUploader = Config.Bind(SECTION_HOTKEYS, "Open uploader", new KeyboardShortcut(KeyCode.U));
            StartDownloader = Config.Bind(SECTION_HOTKEYS, "Open downloader", new KeyboardShortcut(KeyCode.D));
            StartFreeH = Config.Bind(SECTION_HOTKEYS, "Start free H", new KeyboardShortcut(KeyCode.H));
            StartLiveShow = Config.Bind(SECTION_HOTKEYS, "Start live show", new KeyboardShortcut(KeyCode.L));
            StartGameLoad = Config.Bind(SECTION_HOTKEYS, "Open Load Game screen", new KeyboardShortcut(KeyCode.G));

            Harmony.CreateAndPatchAll(GetType());
        }

        [HarmonyPrefix, HarmonyPatch(typeof(TitleScene), "Start")]
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
                else if(StartLiveShow.Value.IsDown()) StartMode(titleScene.OnOtherIdolLive, "Starting live show");
                else if(StartGameLoad.Value.IsDown()) StartMode(titleScene.OnLoad, "Opening Load Game screen");
            }
            while(titleScene);

            void StartMode(Action action, string msg)
            {
                if(titleScene && !FindObjectOfType<ConfigScene>())
                {
                    Log.Message(msg);
                    action();
                    titleScene = null;
                }
            }
        }
        
        [HarmonyPostfix, HarmonyPatch(typeof(FreeHScene), "Start")]
        private static void TitleStart(FreeHScene __instance, ref IEnumerator __result)
        {
            __result = __result.AppendCo(HSceneInput(__instance));
        }

        private static IEnumerator HSceneInput(FreeHScene freeHScene)
        {
            for(int i = 0; i < 3; i++)
                yield return null;
            
            do
            {
                yield return null;

                if(StartFreeH.Value.IsDown())
                {
                    freeHScene.enterButton.onClick.Invoke();
                    freeHScene = null;
                }
            }
            while(freeHScene);
        }
    }
}
