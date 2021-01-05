using AIProject;
using BepInEx;
using BepInEx.Configuration;
using KeelPlugins.AISyoujyo;
using System.Collections;
using TitleShortcuts.Core;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

[assembly: System.Reflection.AssemblyFileVersion(TitleShortcuts.AISyoujyo.TitleShortcuts.Version)]

namespace TitleShortcuts.AISyoujyo
{
    [BepInProcess(Constants.MainGameProcessName)]
    [BepInProcess(Constants.SteamGameProcessName)]
    [BepInPlugin(GUID, PluginName, Version)]
    public class TitleShortcuts : TitleShortcutsCore
    {
        public const string Version = "1.2.2." + BuildNumber.Version;

        private ConfigEntry<KeyboardShortcut> StartFemaleMaker { get; set; }
        private ConfigEntry<KeyboardShortcut> StartMaleMaker { get; set; }
        private ConfigEntry<KeyboardShortcut> StartUploader { get; set; }
        private ConfigEntry<KeyboardShortcut> StartDownloader { get; set; }

        private TitleScene titleScene;

        protected override void Awake()
        {
            base.Awake();

            StartFemaleMaker = Config.Bind(SECTION_HOTKEYS, "Open female maker", new KeyboardShortcut(KeyCode.F));
            StartMaleMaker = Config.Bind(SECTION_HOTKEYS, "Open male maker", new KeyboardShortcut(KeyCode.M));
            StartUploader = Config.Bind(SECTION_HOTKEYS, "Open uploader", new KeyboardShortcut(KeyCode.U));
            StartDownloader = Config.Bind(SECTION_HOTKEYS, "Open downloader", new KeyboardShortcut(KeyCode.D));

            SceneManager.sceneLoaded += StartInput;
        }

        private void StartInput(Scene scene, LoadSceneMode mode)
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
                    else if (StartUploader.Value.IsPressed()) StartMode(titleScene.OnUploader, "Starting uploader");
                    else if (StartDownloader.Value.IsPressed()) StartMode(titleScene.OnDownloader, "Starting downloader");
                }

                yield return null;
            }

            void StartMode(UnityAction action, string msg)
            {
                if (!FindObjectOfType<ConfigScene.ConfigWindow>())
                {
                    Log.Message(msg);
                    action();
                    titleScene = null;
                }
            }
        }
    }
}
