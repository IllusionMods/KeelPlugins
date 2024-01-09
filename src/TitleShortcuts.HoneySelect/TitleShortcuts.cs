using BepInEx;
using BepInEx.Configuration;
using KeelPlugins;
using System;
using System.Collections;
using TitleShortcuts.Core;
using UnityEngine;

[assembly: System.Reflection.AssemblyFileVersion(TitleShortcuts.HoneySelect.TitleShortcuts.Version)]

namespace TitleShortcuts.HoneySelect
{
    [BepInProcess(Constants.MainGameProcessName64bit)]
    [BepInProcess(Constants.MainGameProcessName32bit)]
    [BepInPlugin(GUID, PluginName, Version)]
    public class TitleShortcuts : TitleShortcutsCore
    {
        public const string Version = "1.2.2." + BuildNumber.Version;

        private ConfigEntry<KeyboardShortcut> StartFemaleMaker { get; set; }
        private ConfigEntry<KeyboardShortcut> StartMaleMaker { get; set; }

        private TitleScene titleScene;

        protected override void Awake()
        {
            base.Awake();

            StartFemaleMaker = Config.Bind(SECTION_HOTKEYS, "Female maker", new KeyboardShortcut(KeyCode.F));
            StartMaleMaker = Config.Bind(SECTION_HOTKEYS, "Male maker", new KeyboardShortcut(KeyCode.M));
        }

        private void OnLevelWasLoaded(int level)
        {
            StopAllCoroutines();

            titleScene = FindObjectOfType<TitleScene>();

            if (titleScene) StartCoroutine(InputCheck());
        }

        private IEnumerator InputCheck()
        {
            while (titleScene)
            {
                if (!Manager.Scene.Instance.IsNowLoadingFade)
                {
                    if (StartFemaleMaker.Value.IsPressed()) StartMode(titleScene.OnCustomFemale, "Starting female maker");
                    else if (StartMaleMaker.Value.IsPressed()) StartMode(titleScene.OnCustomMale, "Starting male maker");
                }

                yield return null;
            }

            void StartMode(Action action, string msg)
            {
                if (!FindObjectOfType<ConfigScene>())
                {
                    Log.Message(msg);
                    action();
                    titleScene = null;
                }
            }
        }
    }
}
