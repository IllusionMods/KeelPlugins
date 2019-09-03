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

        private ConfigWrapper<AutoStartOption> AutoStart { get; }
        private ConfigWrapper<KeyboardShortcut> StartFemaleMaker { get; }
        private ConfigWrapper<KeyboardShortcut> StartMaleMaker { get; }

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

        private TitleShortcuts()
        {
            AutoStart = Config.GetSetting("", "AutoStartMode", AutoStartOption.Disabled, new ConfigDescription(TitleShortcutsConstants.DESCRIPTION_AUTOSTART));
            StartFemaleMaker = Config.GetSetting("", "StartFemaleMaker", new KeyboardShortcut(KeyCode.F));
            StartMaleMaker = Config.GetSetting("", "StartMaleMaker", new KeyboardShortcut(KeyCode.M));
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
