using BepInEx;
using HarmonyLib;
using System.Linq;
using UILib;

namespace KeelPlugins
{
    [BepInProcess(HoneySelectConstants.StudioNeoProcessName32bit)]
    [BepInProcess(HoneySelectConstants.StudioNeoProcessName64bit)]
    [BepInPlugin(GUID, PluginName, Version)]
    public class BetterSceneLoader : BetterSceneLoaderCore
    {
        public const string Version = "1.2.0." + BuildNumber.Version;

        protected override void Awake()
        {
            base.Awake();
            Harmony.CreateAndPatchAll(typeof(BetterSceneLoader));
            UIUtility.InitKOI(typeof(BetterSceneLoader).Assembly);
        }

        [HarmonyPrefix, HarmonyPatch(typeof(StudioScene), "Start")]
        public static void StudioEntrypoint()
        {
            SceneLoaderUI.CreateUI();
        }

        protected override void LoadScene(string path)
        {
            Studio.Studio.Instance.LoadScene(path);
        }

        protected override void SaveScene(string path)
        {
            Studio.Studio.Instance.dicObjectCtrl.Values.ToList().ForEach(x => x.OnSavePreprocessing());
            Studio.Studio.Instance.sceneInfo.cameraSaveData = Studio.Studio.Instance.cameraCtrl.Export();
            Studio.Studio.Instance.sceneInfo.Save(path);
        }

        protected override void ImportScene(string path)
        {
            Studio.Studio.Instance.ImportScene(path);
        }
    }
}
