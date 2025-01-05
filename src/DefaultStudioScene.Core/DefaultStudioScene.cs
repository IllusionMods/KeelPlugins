using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using System.Collections;
using System.IO;
using UnityEngine;
using KKAPI.Utilities;
using KeelPlugins;

[assembly: System.Reflection.AssemblyVersion(DefaultStudioScene.Koikatu.DefaultStudioScene.Version)]

namespace DefaultStudioScene.Koikatu
{
    [BepInProcess(Constants.StudioProcessName)]
    [BepInPlugin(GUID, "DefaultStudioScene", Version)]
    [BepInDependency(KKAPI.KoikatuAPI.GUID)]
    public class DefaultStudioScene : BaseUnityPlugin
    {
        public const string GUID = "keelhauled.defaultstudioscene";
        public const string Version = "1.0.4." + BuildNumber.Version;

        private static ConfigEntry<string> DefaultScenePath { get; set; }
        private static DefaultStudioScene plugin;

        private const string fileExtension = ".png";
        private const string filter = "Scenes (*.png)|*.png|All files|*.*";
        private string defaultDir = Path.Combine(Paths.GameRootPath, @"userdata\studio\scene");

        public void Awake()
        {
            plugin = this;
            Config.Bind("General", $"Default Scene Replacement", "", new ConfigDescription("", null, new ConfigurationManagerAttributes { Order = 2, HideDefaultButton = true, CustomDrawer = SceneButtonDrawer}));
            DefaultScenePath = Config.Bind("General", "Default Scene Path", "", new ConfigDescription("", null, new ConfigurationManagerAttributes { Order = 1 }));
            Harmony.CreateAndPatchAll(GetType());
        }

        [HarmonyPostfix, HarmonyPatch(typeof(Studio.Studio), nameof(Studio.Studio.Init))]
        private static void LoadScene()
        {
            plugin.StartCoroutine(Coroutine());

            IEnumerator Coroutine()
            {
                yield return new WaitUntil(() => Studio.Studio.Instance.rootButtonCtrl != null);

                if(!string.IsNullOrEmpty(DefaultScenePath.Value) && File.Exists(DefaultScenePath.Value))
                    plugin.StartCoroutine(Studio.Studio.Instance.LoadSceneCoroutine(DefaultScenePath.Value));
            }
        }

        private void SceneButtonDrawer(ConfigEntryBase configEntry)
        {
            if(GUILayout.Button($"Select a scene", GUILayout.ExpandWidth(true)))
                OpenFileDialog.Show(OnAccept, "Select a scene", defaultDir, filter, fileExtension, OpenFileDialog.OpenSaveFileDialgueFlags.OFN_FILEMUSTEXIST);
        }

        private void OnAccept(string[] paths)
        {
            if(paths.IsNullOrEmpty()) return;
            DefaultScenePath.Value = paths[0];
        }
    }
}
