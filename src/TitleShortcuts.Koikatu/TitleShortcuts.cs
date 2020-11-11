using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using Manager;
using KeelPlugins.Koikatu;
using TitleShortcuts.Core;

namespace TitleShortcuts.Koikatu
{
    [BepInProcess(Constants.MainGameProcessName)]
    [BepInProcess(Constants.MainGameProcessNameSteam)]
    [BepInPlugin(GUID, PluginName, Version)]
    public class TitleShortcuts : TitleShortcutsCore
    {
        public const string Version = "1.2.1." + BuildNumber.Version;

        private static ConfigEntry<KeyboardShortcut> StartFemaleMaker { get; set; }
        private static ConfigEntry<KeyboardShortcut> StartMaleMaker { get; set; }
        private static ConfigEntry<KeyboardShortcut> StartUploader { get; set; }
        private static ConfigEntry<KeyboardShortcut> StartDownloader { get; set; }
        private static ConfigEntry<KeyboardShortcut> StartFreeH { get; set; }
        private static ConfigEntry<KeyboardShortcut> StartLiveShow { get; set; }

        private const string ArgumentFemaleMaker = "-femalemaker";
        private const string ArgumentMaleMaker = "-malemaker";
        private const string ArgumentFreeH = "-freeh";
        private const string ArgumentLive = "-live";

        private static bool autostartFinished = false;
        private static TitleScene titleScene;

        protected override string[] PossibleArguments => new[] { ArgumentFemaleMaker, ArgumentMaleMaker, ArgumentFreeH, ArgumentLive };

        protected override void Awake()
        {
            base.Awake();

            StartFemaleMaker = Config.Bind(SECTION_HOTKEYS, "Open female maker", new KeyboardShortcut(KeyCode.F));
            StartMaleMaker = Config.Bind(SECTION_HOTKEYS, "Open male maker", new KeyboardShortcut(KeyCode.M));
            StartUploader = Config.Bind(SECTION_HOTKEYS, "Open uploader", new KeyboardShortcut(KeyCode.U));
            StartDownloader = Config.Bind(SECTION_HOTKEYS, "Open downloader", new KeyboardShortcut(KeyCode.D));
            StartFreeH = Config.Bind(SECTION_HOTKEYS, "Start free H", new KeyboardShortcut(KeyCode.H));
            StartLiveShow = Config.Bind(SECTION_HOTKEYS, "Start live show", new KeyboardShortcut(KeyCode.L));

            Harmony.CreateAndPatchAll(GetType());
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(TitleScene), "Start")]
        private static void TitleStart(TitleScene __instance)
        {
            titleScene = __instance;
            Plugin.StartCoroutine(TitleInput());
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(LogoScene), "Start")]
        private static bool DisableLogo(ref IEnumerator __result)
        {
            IEnumerator LoadTitle()
            {
                Singleton<Scene>.Instance.LoadReserve(new Scene.Data
                {
                    levelName = "Title",
                    isFade = false
                }, false);
                yield break;
            }
            __result = LoadTitle();
            return false;
        }

        private static IEnumerator TitleInput()
        {
            yield return null;

            while(titleScene != null)
            {
                if(StartFemaleMaker.Value.IsDown() || !autostartFinished && StartupArgument == ArgumentFemaleMaker)
                {
                    StartMode(titleScene.OnCustomFemale, "Starting female maker");
                }
                else if(StartMaleMaker.Value.IsDown() || !autostartFinished && StartupArgument == ArgumentMaleMaker)
                {
                    StartMode(titleScene.OnCustomMale, "Starting male maker");
                }

                else if(StartUploader.Value.IsDown())
                {
                    StartMode(titleScene.OnUploader, "Starting uploader");
                }
                else if(StartDownloader.Value.IsDown())
                {
                    StartMode(titleScene.OnDownloader, "Starting downloader");
                }

                else if(StartFreeH.Value.IsDown() || !autostartFinished && StartupArgument == ArgumentFreeH)
                {
                    StartMode(titleScene.OnOtherFreeH, "Starting free H");
                }
                else if(StartLiveShow.Value.IsDown() || !autostartFinished && StartupArgument == ArgumentLive)
                {
                    StartMode(titleScene.OnOtherIdolLive, "Starting live show");
                }

                yield return null;
            }
        }

        private static void StartMode(UnityAction action, string msg)
        {
            if(!FindObjectOfType<ConfigScene>())
            {
                Logger.LogMessage(msg);
                titleScene = null;
                action();
                autostartFinished = true;
            }
        }
    }
}
