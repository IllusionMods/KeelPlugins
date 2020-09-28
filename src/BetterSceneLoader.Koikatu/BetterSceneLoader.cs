using BepInEx;
using HarmonyLib;
using System.IO;
using System.Linq;
using UILib;

namespace KeelPlugins
{
    [BepInProcess(KoikatuConstants.StudioProcessName)]
    [BepInPlugin(GUID, PluginName, Version)]
    public class BetterSceneLoader : BetterSceneLoaderCore
    {
        public const string Version = "1.0.0";

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
            Studio.Studio.Instance.StartCoroutine(Studio.Studio.Instance.LoadSceneCoroutine(path));
        }

        protected override void SaveScene(string path)
        {
            Studio.Studio.Instance.dicObjectCtrl.Values.ToList().ForEach(x => x.OnSavePreprocessing());
            Studio.Studio.Instance.sceneInfo.cameraSaveData = Studio.Studio.Instance.cameraCtrl.Export();
            Studio.Studio.Instance.sceneInfo.Save(path);
        }

        protected override void DeleteScene(string path)
        {
            File.Delete(path);
        }

        protected override void ImportScene(string path)
        {
            Studio.Studio.Instance.ImportScene(path);
        }
    }
}
