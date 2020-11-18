using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using System.Collections;
using System.IO;
using UnityEngine;

[assembly: System.Reflection.AssemblyFileVersion(DefaultStudioScene.Koikatu.DefaultStudioScene.Version)]

namespace DefaultStudioScene.Koikatu
{
    [BepInPlugin(GUID, "DefaultStudioScene", Version)]
    public class DefaultStudioScene : BaseUnityPlugin
    {
        public const string GUID = "keelhauled.defaultstudioscene";
        public const string Version = "1.0.1." + BuildNumber.Version;

        private static ConfigEntry<string> DefaultScenePath { get; set; }
        private static DefaultStudioScene plugin;

        public void Awake()
        {
            plugin = this;
            DefaultScenePath = Config.Bind("General", "DefaultScenePath", "");
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
    }
}
