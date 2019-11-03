using BepInEx;
using BepInEx.Configuration;
using System.Collections;
using System.ComponentModel;
using AIProject;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace KeelPlugins
{
    [BepInProcess(AISyoujyoConstants.MainGameProcessName)]
    [BepInPlugin(GUID, PluginName, Version)]
    public class TitleShortcuts : TitleShortcutsCore
    {
        public const string Version = "1.1.1";

        private ConfigEntry<AutoStartOption> AutoStart { get; set; }
        private ConfigEntry<KeyboardShortcut> StartFemaleMaker { get; set; }
        private ConfigEntry<KeyboardShortcut> StartMaleMaker { get; set; }
        private ConfigEntry<KeyboardShortcut> StartUploader { get; set; }
        private ConfigEntry<KeyboardShortcut> StartDownloader { get; set; }

        private bool checkInput = false;
        private bool cancelAuto = false;
        private TitleScene titleScene;

        private void Awake()
        {
            AutoStart = Config.AddSetting(SECTION_GENERAL, "Automatic start mode", AutoStartOption.Disabled, new ConfigDescription(DESCRIPTION_AUTOSTART));
            StartFemaleMaker = Config.AddSetting(SECTION_HOTKEYS, "Open female maker", new KeyboardShortcut(KeyCode.F));
            StartMaleMaker = Config.AddSetting(SECTION_HOTKEYS, "Open male maker", new KeyboardShortcut(KeyCode.M));
            StartUploader = Config.AddSetting(SECTION_HOTKEYS, "Open uploader", new KeyboardShortcut(KeyCode.U));
            StartDownloader = Config.AddSetting(SECTION_HOTKEYS, "Open downloader", new KeyboardShortcut(KeyCode.D));

            SceneManager.sceneLoaded += StartInput;
        }

        private void StartInput(Scene scene, LoadSceneMode mode)
        {
            var title = FindObjectOfType<TitleScene>();

            if(title)
            {
                if(!checkInput)
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
            while(checkInput)
            {
                if(!cancelAuto && AutoStart.Value != AutoStartOption.Disabled && (Input.GetKey(KeyCode.Escape) || Input.GetKey(KeyCode.F1)))
                {
                    Logger.Log(BepInEx.Logging.LogLevel.Message, "Automatic start cancelled");
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
                        }
                    }

                    cancelAuto = true;
                }

                yield return null;
            }
        }

        private void StartMode(UnityAction action, string msg)
        {
            if(!FindObjectOfType<ConfigScene.ConfigWindow>())
            {
                Logger.LogMessage(msg);
                checkInput = false;
                action();
            }
        }

        private enum AutoStartOption
        {
            Disabled,
            [Description("Female maker")]
            FemaleMaker,
            [Description("Male maker")]
            MaleMaker
        }
    }
}
