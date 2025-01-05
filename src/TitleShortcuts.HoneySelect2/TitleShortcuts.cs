using BepInEx;
using BepInEx.Configuration;
using KeelPlugins;
using System.Collections;
using HS2;
using TitleShortcuts.Core;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

[assembly: System.Reflection.AssemblyVersion(TitleShortcuts.HoneySelect2.TitleShortcuts.Version)]

namespace TitleShortcuts.HoneySelect2
{
    [BepInProcess(Constants.MainGameProcessName)]
    [BepInPlugin(GUID, PluginName, Version)]
    public class TitleShortcuts : TitleShortcutsCore
    {
        public const string Version = "1.3.1." + BuildNumber.Version;

        private ConfigEntry<KeyboardShortcut> StartFemaleMaker { get; set; }
        private ConfigEntry<KeyboardShortcut> StartMaleMaker { get; set; }
        private ConfigEntry<KeyboardShortcut> StartUploader { get; set; }
        private ConfigEntry<KeyboardShortcut> StartDownloader { get; set; }
        private ConfigEntry<KeyboardShortcut> StartGame { get; set; }

        private TitleScene titleScene;

        protected override void Awake()
        {
            base.Awake();

            StartFemaleMaker = Config.Bind(SECTION_HOTKEYS, "Open female maker", new KeyboardShortcut(KeyCode.F));
            StartMaleMaker = Config.Bind(SECTION_HOTKEYS, "Open male maker", new KeyboardShortcut(KeyCode.M));
            StartUploader = Config.Bind(SECTION_HOTKEYS, "Open uploader", new KeyboardShortcut(KeyCode.U));
            StartDownloader = Config.Bind(SECTION_HOTKEYS, "Open downloader", new KeyboardShortcut(KeyCode.D));
            StartGame = Config.Bind(SECTION_HOTKEYS, "Open main game", new KeyboardShortcut(KeyCode.G));

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
                if (!Manager.Scene.IsNowLoadingFade)
                {
                    if (StartFemaleMaker.Value.IsPressed()) StartMode(titleScene.OnMakeFemale, "Starting female maker");
                    else if (StartMaleMaker.Value.IsPressed()) StartMode(titleScene.OnMakeMale, "Starting male maker");
                    else if (StartUploader.Value.IsPressed()) StartMode(titleScene.OnUpload, "Starting uploader");
                    else if (StartDownloader.Value.IsPressed()) StartMode(titleScene.OnDownload, "Starting downloader");
                    else if (StartGame.Value.IsPressed()) StartMode(titleScene.OnPlay, "Starting main game");
                }

                yield return null;
            }

            void StartMode(UnityAction action, string msg)
            {
                var configWindow = FindObjectOfType<Config.ConfigWindow>();
                if (!configWindow || !configWindow.active)
                {
                    Log.Message(msg);
                    action();
                    titleScene = null;
                }
            }
        }
    }
}
