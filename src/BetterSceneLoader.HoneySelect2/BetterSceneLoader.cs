using BepInEx;
using BetterSceneLoader.Core;
using HarmonyLib;
using KeelPlugins.HoneySelect2;
using UILib;

[assembly: System.Reflection.AssemblyFileVersion(BetterSceneLoader.HoneySelect2.BetterSceneLoader.Version)]

namespace BetterSceneLoader.HoneySelect2
{
    [BepInProcess(Constants.StudioProcessName)]
    [BepInPlugin(GUID, PluginName, Version)]
    public class BetterSceneLoader : BetterSceneLoaderCore
    {
        public const string Version = "1.0.1." + BuildNumber.Version;

        protected override void Awake()
        {
            base.Awake();
            Harmony.CreateAndPatchAll(typeof(BetterSceneLoader));
            UIUtility.InitKOI(typeof(BetterSceneLoader).Assembly);
        }

        public static void StudioEntrypoint()
        {
            SceneLoaderUI.CreateUI();
        }

        protected override void LoadScene(string path)
        {

        }

        protected override void SaveScene(string path)
        {

        }

        protected override void ImportScene(string path)
        {

        }
    }
}
