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
    [BepInPlugin(GUID, PluginName, Version)]
    public class TitleShortcuts : TitleShortcutsCore
    {
        private ConfigEntry<AutoStartOption> AutoStart { get; set; }
        private ConfigEntry<KeyboardShortcut> StartFemaleMaker { get; set; }
        private ConfigEntry<KeyboardShortcut> StartMaleMaker { get; set; }

        private bool checkInput = false;
        private bool cancelAuto = false;
        private TitleScene titleScene;

        protected override string[] PossibleArguments => new[] { "-femalemaker", "-malemaker" };

        private void Awake()
        {
            AutoStart = Config.Bind(SECTION_GENERAL, "Automatic start mode", AutoStartOption.Disabled, new ConfigDescription(DESCRIPTION_AUTOSTART));
            StartFemaleMaker = Config.Bind(SECTION_HOTKEYS, "Female maker", new KeyboardShortcut(KeyCode.F));
            StartMaleMaker = Config.Bind(SECTION_HOTKEYS, "Male maker", new KeyboardShortcut(KeyCode.M));
        }

        private void OnLevelWasLoaded(int level)
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
                    Logger.Log(LogLevel.Message, "Automatic start cancelled");
                    cancelAuto = true;
                }

                if(!Manager.Scene.Instance.IsNowLoadingFade)
                {
                    if(StartFemaleMaker.Value.IsPressed() || firstLaunch && StartupArgument == "-femalemaker")
                    {
                        StartMode(titleScene.OnCustomFemale, "Starting female maker");
                    }
                    else if(StartMaleMaker.Value.IsPressed() || firstLaunch && StartupArgument == "-malemaker")
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
            firstLaunch = false;
            if (!FindObjectOfType<ConfigScene>())
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
