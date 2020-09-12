using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using System.Collections;
using System.ComponentModel;
using HarmonyLib;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace KeelPlugins
{
    [BepInProcess(KoikatuConstants.MainGameProcessName)]
    [BepInProcess(KoikatuConstants.MainGameProcessNameSteam)]
    [BepInProcess(KoikatuConstants.VRProcessName)]
    [BepInProcess(KoikatuConstants.VRProcessNameSteam)]
    [BepInPlugin(GUID, PluginName, Version)]
    public class TitleShortcuts : TitleShortcutsCore
    {
        private static ConfigEntry<AutoStartOption> AutoStart { get; set; }
        private ConfigEntry<KeyboardShortcut> StartFemaleMaker { get; set; }
        private ConfigEntry<KeyboardShortcut> StartMaleMaker { get; set; }
        private ConfigEntry<KeyboardShortcut> StartUploader { get; set; }
        private ConfigEntry<KeyboardShortcut> StartDownloader { get; set; }
        private ConfigEntry<KeyboardShortcut> StartFreeH { get; set; }
        private ConfigEntry<KeyboardShortcut> StartLiveShow { get; set; }

        private static bool autostartFinished = false;
        private bool checkInput = false;
        private bool cancelAuto = false;
        private TitleScene titleScene;

        protected override string[] PossibleArguments => new[] { "-femalemaker", "-malemaker", "-freeh", "-live" };

        private void Awake()
        {
            AutoStart = Config.Bind(SECTION_GENERAL, "Automatic start mode", AutoStartOption.Disabled, new ConfigDescription(DESCRIPTION_AUTOSTART));
            StartFemaleMaker = Config.Bind(SECTION_HOTKEYS, "Open female maker", new KeyboardShortcut(KeyCode.F));
            StartMaleMaker = Config.Bind(SECTION_HOTKEYS, "Open male maker", new KeyboardShortcut(KeyCode.M));
            StartUploader = Config.Bind(SECTION_HOTKEYS, "Open uploader", new KeyboardShortcut(KeyCode.U));
            StartDownloader = Config.Bind(SECTION_HOTKEYS, "Open downloader", new KeyboardShortcut(KeyCode.D));
            StartFreeH = Config.Bind(SECTION_HOTKEYS, "Start free H", new KeyboardShortcut(KeyCode.H));
            StartLiveShow = Config.Bind(SECTION_HOTKEYS, "Start live show", new KeyboardShortcut(KeyCode.L));

            if (AutoStart.Value != AutoStartOption.Disabled || !string.IsNullOrEmpty(StartupArgument))
            {
                SceneManager.sceneLoaded += StartInput;
                Harmony.CreateAndPatchAll(typeof(TitleShortcuts));
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(Manager.Scene.Data), nameof(Manager.Scene.Data.isFade), MethodType.Setter)]
        private static bool DisableFadeout()
        {
            return autostartFinished;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(LogoScene), "Start")]
        private static bool DisableIntro(ref IEnumerator __result)
        {
            IEnumerator LoadTitle()
            {
                Singleton<Manager.Scene>.Instance.LoadReserve(new Manager.Scene.Data
                {
                    levelName = "Title",
                    isFade = false
                }, false);
                yield break;
            }
            __result = LoadTitle();
            return false;
        }

        private void StartInput(Scene scene, LoadSceneMode mode)
        {
            var title = FindObjectOfType<TitleScene>();

            if (title)
            {
                if (!checkInput)
                {
                    titleScene = title;
                    checkInput = true;
                    StartCoroutine(InputCheck());
                }
            }
            else
            {
                checkInput = false;
            }
        }

        private IEnumerator InputCheck()
        {
            while (checkInput)
            {
                if (!cancelAuto && AutoStart.Value != AutoStartOption.Disabled && (Input.GetKey(KeyCode.Escape) || Input.GetKey(KeyCode.F1)))
                {
                    Logger.Log(LogLevel.Message, "Automatic start cancelled");
                    cancelAuto = true;
                    autostartFinished = true;
                }

                if (!Manager.Scene.Instance.IsNowLoadingFade)
                {
                    if (StartFemaleMaker.Value.IsPressed() || firstLaunch && StartupArgument == "-femalemaker")
                    {
                        yield return StartMode(titleScene.OnCustomFemale, "Starting female maker");
                    }
                    else if (StartMaleMaker.Value.IsPressed() || firstLaunch && StartupArgument == "-malemaker")
                    {
                        yield return StartMode(titleScene.OnCustomMale, "Starting male maker");
                    }

                    else if (StartUploader.Value.IsPressed())
                    {
                        yield return StartMode(titleScene.OnUploader, "Starting uploader");
                    }
                    else if (StartDownloader.Value.IsPressed())
                    {
                        yield return StartMode(titleScene.OnDownloader, "Starting downloader");
                    }

                    else if (StartFreeH.Value.IsPressed() || firstLaunch && StartupArgument == "-freeh")
                    {
                        yield return StartMode(titleScene.OnOtherFreeH, "Starting free H");
                    }
                    else if (StartLiveShow.Value.IsPressed() || firstLaunch && StartupArgument == "-live")
                    {
                        yield return StartMode(titleScene.OnOtherIdolLive, "Starting live show");
                    }

                    else if (!cancelAuto && AutoStart.Value != AutoStartOption.Disabled)
                    {
                        switch (AutoStart.Value)
                        {
                            case AutoStartOption.FemaleMaker:
                                yield return StartMode(titleScene.OnCustomFemale, "Automatically starting female maker");
                                break;

                            case AutoStartOption.MaleMaker:
                                yield return StartMode(titleScene.OnCustomMale, "Automatically starting male maker");
                                break;

                            case AutoStartOption.FreeH:
                                yield return StartMode(titleScene.OnOtherFreeH, "Automatically starting free H");
                                break;

                            case AutoStartOption.LiveStage:
                                yield return StartMode(titleScene.OnOtherIdolLive, "Automatically starting live show");
                                break;
                        }
                    }

                    cancelAuto = true;
                }

                yield return null;
            }
        }

        private IEnumerator StartMode(UnityAction action, string msg)
        {
            firstLaunch = false;
            if (!FindObjectOfType<ConfigScene>())
            {
                Logger.LogMessage(msg);
                checkInput = false;
                //yield return null;
                action();
                autostartFinished = true;
            }
            yield break;
        }

        private enum AutoStartOption
        {
            Disabled,
            [Description("Female maker")]
            FemaleMaker,
            [Description("Male maker")]
            MaleMaker,
            [Description("Free H")]
            FreeH,
            [Description("Live stage")]
            LiveStage
        }
    }
}
