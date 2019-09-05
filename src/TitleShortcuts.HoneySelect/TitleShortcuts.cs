using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using System;
using System.Collections;
using System.ComponentModel;
using UnityEngine;

namespace KeelPlugins
{
    [BepInProcess(HoneySelectConstants.MainGameProcessName64bit)]
    [BepInProcess(HoneySelectConstants.MainGameProcessName32bit)]
    [BepInPlugin(GUID, "TitleShortcuts", Version)]
    public class TitleShortcuts : BaseUnityPlugin
    {
        public const string GUID = "keelhauled.titleshortcuts";
        public const string Version = "1.0.0";

        private const string SECTION_HOTKEYS = "Keyboard Shortcuts";
        private const string SECTION_GENERAL = "General";

        private ConfigWrapper<AutoStartOption> AutoStart { get; set; }
        private ConfigWrapper<KeyboardShortcut> StartFemaleMaker { get; set; }
        private ConfigWrapper<KeyboardShortcut> StartMaleMaker { get; set; }

        private bool check = false;
        private bool cancelAuto = false;
        private TitleScene titleScene;

        private enum AutoStartOption
        {
            Disabled,
            [Description("Female maker")]
            FemaleMaker,
            [Description("Male maker")]
            MaleMaker
        }

        private void Awake()
        {
            AutoStart = Config.GetSetting(SECTION_GENERAL, "Automatic start mode", AutoStartOption.Disabled, new ConfigDescription(TitleShortcutsConstants.DESCRIPTION_AUTOSTART));
            StartFemaleMaker = Config.GetSetting(SECTION_HOTKEYS, "Female maker", new KeyboardShortcut(KeyCode.F));
            StartMaleMaker = Config.GetSetting(SECTION_HOTKEYS, "Male maker", new KeyboardShortcut(KeyCode.M));
        }

        private void OnLevelWasLoaded(int level)
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

        private void StartMode(Action action, string msg)
        {
            if(!FindObjectOfType<ConfigScene>())
            {
                Logger.LogMessage(msg);
                check = false;
                action();
            }
        }
    }
}
