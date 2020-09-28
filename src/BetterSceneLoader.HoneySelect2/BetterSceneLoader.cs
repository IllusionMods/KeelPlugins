using BepInEx;
using HarmonyLib;
using UILib;

namespace KeelPlugins
{
    [BepInProcess(HoneySelect2Constants.StudioProcessName)]
    [BepInPlugin(GUID, PluginName, Version)]
    public class BetterSceneLoader : BetterSceneLoaderCore
    {
        public const string Version = "1.0.0." + BuildNumber.Version;

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
