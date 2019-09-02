using BepInEx;
using BepInEx.Configuration;
using BepInEx.Harmony;
using BepInEx.Logging;
using HarmonyLib;
using System.IO;
using UnityEngine;

namespace KeelPlugins
{
    [BepInPlugin(GUID, "MakerBridge", Version)]
    public class MakerBridge : BaseUnityPlugin
    {
        public const string GUID = "keelhauled.makerbridge";
        public const string Version = "1.0.1";
        internal static new ManualLogSource Logger;

        internal static string MakerCardPath;
        internal static string OtherCardPath;
        private static GameObject bepinex;

        internal static ConfigWrapper<KeyboardShortcut> SendChara { get; set; }

        private MakerBridge()
        {
            Logger = base.Logger;
            SendChara = Config.GetSetting("", "SendCharacter", new KeyboardShortcut(KeyCode.B));
        }

        private void Awake()
        {
            var tempPath = Path.GetTempPath();
            MakerCardPath = Path.Combine(tempPath, "makerbridge1.png");
            OtherCardPath = Path.Combine(tempPath, "makerbridge2.png");

            bepinex = gameObject;
            var harmony = new Harmony("keelhauled.makerbridge.harmony");
            HarmonyWrapper.PatchAll(typeof(Hooks), harmony);
        }

        private class Hooks
        {
            [HarmonyPrefix, HarmonyPatch(typeof(CustomScene), "Start")]
            public static void CustomSceneInit()
            {
                bepinex.GetOrAddComponent<MakerHandler>();
            }

            [HarmonyPrefix, HarmonyPatch(typeof(CustomScene), "OnDestroy")]
            public static void CustomSceneStop()
            {
                Destroy(bepinex.GetComponent<MakerHandler>());
            }

            [HarmonyPrefix, HarmonyPatch(typeof(StudioScene), "Start")]
            public static void StudioSceneInit()
            {
                bepinex.GetOrAddComponent<StudioHandler>();
            }
        }
    }
}
