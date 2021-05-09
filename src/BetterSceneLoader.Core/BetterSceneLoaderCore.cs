using BepInEx;
using BepInEx.Configuration;
using KeelPlugins.Utils;
using System.IO;

namespace BetterSceneLoader.Core
{
    public abstract class BetterSceneLoaderCore : BaseUnityPlugin
    {
        public const string GUID = "keelhauled.bettersceneloader";
        public const string PluginName = "BetterSceneLoader";

        private const string CATEGORY_GENERAL = "General";

        public static ConfigEntry<int> ColumnAmount { get; set; }
        public static ConfigEntry<float> ScrollSensitivity { get; set; }
        public static ConfigEntry<bool> AutoClose { get; set; }
        public static ConfigEntry<bool> SmallWindow { get; set; }

        internal static readonly SceneLoaderUI SceneLoaderUI = new SceneLoaderUI();

        protected virtual void Awake()
        {
            SmallWindow = Config.Bind(CATEGORY_GENERAL, nameof(SmallWindow), true, new ConfigDescription("", null, new ConfigurationManagerAttributes { Order = 1 }));
            AutoClose = Config.Bind(CATEGORY_GENERAL, nameof(AutoClose), true, new ConfigDescription("", null, new ConfigurationManagerAttributes { Order = 2 }));
            ColumnAmount = Config.Bind(CATEGORY_GENERAL, nameof(ColumnAmount), 3, new ConfigDescription("", new AcceptableValueRange<int>(1, 10)));
            ScrollSensitivity = Config.Bind(CATEGORY_GENERAL, nameof(ScrollSensitivity), 3f, new ConfigDescription("", new AcceptableValueRange<float>(1f, 10f)));

            SmallWindow.SettingChanged += (x, y) => SceneLoaderUI.UpdateWindow();
            ColumnAmount.SettingChanged += (x, y) => SceneLoaderUI.UpdateWindow();
            ScrollSensitivity.SettingChanged += (x, y) => SceneLoaderUI.UpdateWindow();

            SceneLoaderUI.OnLoadButtonClick += LoadScene;
            SceneLoaderUI.OnSaveButtonClick += SaveScene;
            SceneLoaderUI.OnDeleteButtonClick += DeleteScene;
            SceneLoaderUI.OnImportButtonClick += ImportScene;
        }

        protected abstract void LoadScene(string path);
        protected abstract void SaveScene(string path);
        protected abstract void ImportScene(string path);

        protected void DeleteScene(string path)
        {
            File.Delete(path);
        }
    }
}
