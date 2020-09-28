using BepInEx;
using BepInEx.Configuration;
using KeelPlugins;
using UnityEngine;

namespace BetterSceneLoader.Core
{
    public abstract class BetterSceneLoaderCore : BaseUnityPlugin
    {
        private const string CATEGORY_GENERAL = "General";

        public static ConfigEntry<int> ColumnAmount { get; set; }
        public static ConfigEntry<float> ScrollSensitivity { get; set; }
        public static ConfigEntry<bool> AutoClose { get; set; }
        public static ConfigEntry<bool> SmallWindow { get; set; }

        internal static BetterSceneLoaderCore Plugin;
        internal static SceneLoaderUI sceneLoaderUI = new SceneLoaderUI();

        protected virtual void Awake()
        {
            Plugin = this;

            SmallWindow = Config.Bind(CATEGORY_GENERAL, nameof(SmallWindow), true, new ConfigDescription("", null, new ConfigurationManagerAttributes { Order = 1 }));
            AutoClose = Config.Bind(CATEGORY_GENERAL, nameof(AutoClose), true, new ConfigDescription("", null, new ConfigurationManagerAttributes { Order = 2 }));
            ColumnAmount = Config.Bind(CATEGORY_GENERAL, nameof(ColumnAmount), 3, new ConfigDescription("", new AcceptableValueRange<int>(1, 10)));
            ScrollSensitivity = Config.Bind(CATEGORY_GENERAL, nameof(ScrollSensitivity), 3f, new ConfigDescription("", new AcceptableValueRange<float>(1f, 10f)));

            SmallWindow.SettingChanged += (x, y) => sceneLoaderUI.UpdateWindow();
            ColumnAmount.SettingChanged += (x, y) => sceneLoaderUI.UpdateWindow();
            ScrollSensitivity.SettingChanged += (x, y) => sceneLoaderUI.UpdateWindow();

            sceneLoaderUI.OnLoadButtonClick += LoadScene;
            sceneLoaderUI.OnSaveButtonClick += SaveScene;
            sceneLoaderUI.OnDeleteButtonClick += DeleteScene;
            sceneLoaderUI.OnImportButtonClick += ImportScene;
        }

        protected abstract void LoadScene(string path);
        protected abstract void SaveScene(string path);
        protected abstract void DeleteScene(string path);
        protected abstract void ImportScene(string path);
    }
}
