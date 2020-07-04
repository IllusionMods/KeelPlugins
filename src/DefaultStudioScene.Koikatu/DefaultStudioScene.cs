using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using System.IO;

namespace KeelPlugins
{
    [BepInPlugin(GUID, "DefaultStudioScene", Version)]
    public class DefaultStudioScene : BaseUnityPlugin
    {
        public const string GUID = "keelhauled.defaultstudioscene";
        public const string Version = "1.0.0." + BuildNumber.Version;

        private static ConfigEntry<string> DefaultScenePath { get; set; }

        public void Awake()
        {
            DefaultScenePath = Config.Bind("General", "DefaultScenePath", "");
            Harmony.CreateAndPatchAll(GetType());
        }

        [HarmonyPostfix, HarmonyPatch(typeof(Studio.Studio), nameof(Studio.Studio.Init))]
        private static void LoadScene()
        {
            if(!string.IsNullOrEmpty(DefaultScenePath.Value) && File.Exists(DefaultScenePath.Value))
                Studio.Studio.Instance.StartCoroutine(Studio.Studio.Instance.LoadSceneCoroutine(DefaultScenePath.Value));
        }
    }
}
