using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using KeelPlugins;
using KeelPlugins.Utils;
using System.Linq;
using System.IO;
using UILib;

[assembly: System.Reflection.AssemblyFileVersion(BetterSceneLoader.BetterSceneLoader.Version)]

namespace BetterSceneLoader
{
    [BepInProcess(Constants.StudioProcessName)]
    [BepInPlugin(GUID, PluginName, Version)]
    public class BetterSceneLoader : BaseUnityPlugin
    {
        public const string Version = "1.0.2." + BuildNumber.Version;
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

            Harmony.CreateAndPatchAll(typeof(Hooks));
            UIUtility.InitKOI(typeof(BetterSceneLoader).Assembly);
        }

        private void DeleteScene(string path)
        {
            File.Delete(path);
        }

        private void LoadScene(string path)
        {
            Studio.Studio.Instance.StartCoroutine(Studio.Studio.Instance.LoadSceneCoroutine(path));
        }

        private void SaveScene(string path)
        {
            Studio.Studio.Instance.dicObjectCtrl.Values.ToList().ForEach(x => x.OnSavePreprocessing());
            Studio.Studio.Instance.sceneInfo.cameraSaveData = Studio.Studio.Instance.cameraCtrl.Export();
            Studio.Studio.Instance.sceneInfo.Save(path);
        }

        private void ImportScene(string path)
        {
            Studio.Studio.Instance.ImportScene(path);
        }

        private class Hooks
        {
            [HarmonyPrefix, HarmonyPatch(typeof(StudioScene), "Start")]
            public static void StudioEntrypoint()
            {
                SceneLoaderUI.CreateUI();
            }
        }
    }
}
