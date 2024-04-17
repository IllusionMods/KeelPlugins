using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using KeelPlugins;
using KeelPlugins.Utils;
using System.IO;
using UILib;
using UnityEngine;

[assembly: System.Reflection.AssemblyFileVersion(BetterSceneLoader.BetterSceneLoader.Version)]

namespace BetterSceneLoader
{
    [BepInProcess(Constants.StudioProcessName)]
    [BepInPlugin(GUID, PluginName, Version)]
    [BepInDependency(KKAPI.KoikatuAPI.GUID)]
    public class BetterSceneLoader : BaseUnityPlugin
    {
        public const string Version = "1.0.3." + BuildNumber.Version;
        public const string GUID = "keelhauled.bettersceneloader";
        public const string PluginName = "BetterSceneLoader";

        private const string CATEGORY_GENERAL = "General";
        private const string CATEGORY_UISIZE = "UI Size";

        public static ConfigEntry<int> ColumnAmount { get; set; }
        public static ConfigEntry<float> ScrollSensitivity { get; set; }
        public static ConfigEntry<bool> AutoClose { get; set; }
        public static ConfigEntry<float> AnchorLeft { get; set; }
        public static ConfigEntry<float> AnchorBottom { get; set; }
        public static ConfigEntry<float> AnchorRight { get; set; }
        public static ConfigEntry<float> AnchorTop { get; set; }
        public static ConfigEntry<float> UIMargin { get; set; }

        private static ImageGrid sceneLoaderUI;

        protected virtual void Awake()
        {
            sceneLoaderUI = new ImageGrid(
                defaultPath: BepInEx.Utility.CombinePaths(Paths.GameRootPath, "UserData", "Studio", "scene"),
                onSaveButtonClick: SaveScene,
                onLoadButtonClick: LoadScene,
                onImportButtonClick: ImportScene
            );

            AutoClose = Config.Bind(CATEGORY_GENERAL, "Auto Close", true, new ConfigDescription("Automatically close scene window after loading"));
            ColumnAmount = Config.Bind(CATEGORY_GENERAL, "Column Amount", 7, new ConfigDescription("", new AcceptableValueRange<int>(1, 12)));
            ScrollSensitivity = Config.Bind(CATEGORY_GENERAL, "Scroll Sensitivity", 3f, new ConfigDescription("", new AcceptableValueRange<float>(1f, 10f)));
            AnchorLeft = Config.Bind(CATEGORY_UISIZE, "Left Anchor", 0f, new ConfigDescription("", new AcceptableValueRange<float>(0f, 1f)));
            AnchorBottom = Config.Bind(CATEGORY_UISIZE, "Bottom Anchor", 0f, new ConfigDescription("", new AcceptableValueRange<float>(0f, 1f)));
            AnchorRight = Config.Bind(CATEGORY_UISIZE, "Right Anchor", 1f, new ConfigDescription("", new AcceptableValueRange<float>(0f, 1f)));
            AnchorTop = Config.Bind(CATEGORY_UISIZE, "Top Anchor", 1f, new ConfigDescription("", new AcceptableValueRange<float>(0f, 1f)));
            UIMargin = Config.Bind(CATEGORY_UISIZE, "Margin", 60f, new ConfigDescription("", new AcceptableValueRange<float>(0f, 200f), new ConfigurationManagerAttributes { Order = 1 }));

            ColumnAmount.SettingChanged += (x, y) => sceneLoaderUI.UpdateWindow();
            ScrollSensitivity.SettingChanged += (x, y) => sceneLoaderUI.UpdateWindow();
            AnchorLeft.SettingChanged += (x, y) => sceneLoaderUI.UpdateWindow();
            AnchorBottom.SettingChanged += (x, y) => sceneLoaderUI.UpdateWindow();
            AnchorRight.SettingChanged += (x, y) => sceneLoaderUI.UpdateWindow();
            AnchorTop.SettingChanged += (x, y) => sceneLoaderUI.UpdateWindow();
            UIMargin.SettingChanged += (x, y) => sceneLoaderUI.UpdateWindow();

            Harmony.CreateAndPatchAll(typeof(Hooks));
            UIUtility.InitKOI(typeof(BetterSceneLoader).Assembly);
            Log.SetLogSource(Logger);
        }

        private void LoadScene(string path)
        {
            Studio.Studio.Instance.StartCoroutine(Studio.Studio.Instance.LoadSceneCoroutine(path));
        }

        private void SaveScene()
        {
            Studio.Studio.Instance.systemButtonCtrl.OnClickSave();
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
                sceneLoaderUI.CreateUI("BetterSceneLoaderCanvas", 10, "Scenes");
            }
        }
    }
}
