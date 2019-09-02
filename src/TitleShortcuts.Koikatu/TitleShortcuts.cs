using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using System.Collections;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace KeelPlugins
{
    [BepInProcess(KoikatuConstants.KoikatuMainProcessName)]
    [BepInProcess(KoikatuConstants.KoikatuSteamProcessName)]
    [BepInProcess(KoikatuConstants.KoikatuVRProcessName)]
    [BepInProcess(KoikatuConstants.KoikatuSteamVRProcessName)]
    [BepInPlugin(GUID, "TitleShortcuts", Version)]
    public class TitleShortcuts : BaseUnityPlugin
    {
        public const string GUID = "keelhauled.titleshortcuts";
        public const string Version = "1.1.1";

        private const string DESCRIPTION_AUTOSTART = "Choose which mode to start automatically when launching.\nDuring startup, " +
                                                     "hold esc or F1 to cancel automatic behaviour or hold another shortcut to use that instead.";

        private ConfigWrapper<AutoStartOption> AutoStart { get; }
        private ConfigWrapper<KeyboardShortcut> StartFemaleMaker { get; }
        private ConfigWrapper<KeyboardShortcut> StartMaleMaker { get; }
        private ConfigWrapper<KeyboardShortcut> StartUploader { get; }
        private ConfigWrapper<KeyboardShortcut> StartDownloader { get; }
        private ConfigWrapper<KeyboardShortcut> StartFreeH { get; }
        private ConfigWrapper<KeyboardShortcut> StartLiveShow { get; }

        private bool check = false;
        private bool cancelAuto = false;
        private TitleScene titleScene;

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

        private TitleShortcuts()
        {
            AutoStart = Config.GetSetting("", "AutoStartMode", AutoStartOption.Disabled, new ConfigDescription(DESCRIPTION_AUTOSTART));
            StartFemaleMaker = Config.GetSetting("", "StartFemaleMaker", new KeyboardShortcut(KeyCode.F));
            StartMaleMaker = Config.GetSetting("", "StartMaleMaker", new KeyboardShortcut(KeyCode.M));
            StartUploader = Config.GetSetting("", "StartUploader", new KeyboardShortcut(KeyCode.U));
            StartDownloader = Config.GetSetting("", "StartDownloader", new KeyboardShortcut(KeyCode.D));
            StartFreeH = Config.GetSetting("", "StartFreeH", new KeyboardShortcut(KeyCode.H));
            StartLiveShow = Config.GetSetting("", "StartLiveShow", new KeyboardShortcut(KeyCode.L));
        }

        private void Awake()
        {
            SceneManager.sceneLoaded += StartInput;
        }

        private void StartInput(Scene scene, LoadSceneMode mode)
        {
            var title = FindObjectOfType<TitleScene>();

            if(title)
            {
                if(!check)
                {
                    titleScene = title;
                    check = true;
                    StartCoroutine(InputCheck());
                }
            }
            else
            {
                check = false;
            }
        }

        private IEnumerator InputCheck()
        {
            while(check)
            {
                if(!cancelAuto && AutoStart.Value != AutoStartOption.Disabled && (Input.GetKey(KeyCode.Escape) || Input.GetKey(KeyCode.F1)))
                {
                    Logger.Log(LogLevel.Message, "Automatic start cancelled");
                    cancelAuto = true;
                }

                if(!Manager.Scene.Instance.IsNowLoadingFade)
                {
                    if(StartFemaleMaker.Value.IsPressed())
                    {
                        StartMode(titleScene.OnCustomFemale, "Starting female maker");
                    }
                    else if(StartMaleMaker.Value.IsPressed())
                    {
                        StartMode(titleScene.OnCustomMale, "Starting male maker");
                    }

                    else if(StartUploader.Value.IsPressed())
                    {
                        StartMode(titleScene.OnUploader, "Starting uploader");
                    }
                    else if(StartDownloader.Value.IsPressed())
                    {
                        StartMode(titleScene.OnDownloader, "Starting downloader");
                    }

                    else if(StartFreeH.Value.IsPressed())
                    {
                        StartMode(titleScene.OnOtherFreeH, "Starting free H");
                    }
                    else if(StartLiveShow.Value.IsPressed())
                    {
                        StartMode(titleScene.OnOtherIdolLive, "Starting live show");
                    }

                    else if(!cancelAuto && AutoStart.Value != AutoStartOption.Disabled)
                    {
                        switch(AutoStart.Value)
                        {
                            case AutoStartOption.FemaleMaker:
                                StartMode(titleScene.OnCustomFemale, "Automatically starting female maker");
                                break;

                            case AutoStartOption.MaleMaker:
                                StartMode(titleScene.OnCustomMale, "Automatically starting male maker");
                                break;

                            case AutoStartOption.FreeH:
                                StartMode(titleScene.OnOtherFreeH, "Automatically starting free H");
                                break;

                            case AutoStartOption.LiveStage:
                                StartMode(titleScene.OnOtherIdolLive, "Automatically starting live show");
                                break;
                        }
                    }

                    cancelAuto = true;
                }

                yield return null;
            }
        }

        private void StartMode(UnityAction action, string msg)
        {
            if(FindObjectOfType<ConfigScene>()) return;
            Logger.LogMessage(msg);
            check = false;
            action();
        }
    }
}
