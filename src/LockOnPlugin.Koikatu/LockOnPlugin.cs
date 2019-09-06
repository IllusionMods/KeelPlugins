using BepInEx;
using BepInEx.Configuration;
using BepInEx.Harmony;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;

namespace KeelPlugins
{
    [BepInPlugin(GUID, "LockOnPlugin", Version)]
    public class LockOnPlugin : BaseUnityPlugin
    {
        public const string GUID = "keelhauled.lockonplugin";
        public const string Version = "1.0.0";
        internal static new ManualLogSource Logger;

        private const string SECTION_HOTKEYS = "Keyboard Shortcuts";
        private const string SECTION_GENERAL = "General";

        private const string DESCRIPTION_TRACKSPEED = "The speed at which the target is followed.";
        private const string DESCRIPTION_SCROLLMALES = "Choose whether to include males in the rotation when switching between characters using the hotkeys from the plugin.";
        private const string DESCRIPTION_LEASHLENGTH = "The amount of slack allowed when tracking.";
        private const string DESCRIPTION_AUTOLOCK = "Lock on automatically after switching characters.";
        private const string DESCRIPTION_SHOWINFOMSG = "Show various messages about the plugin on screen.";

        internal static ConfigWrapper<float> TrackingSpeedNormal { get; set; }
        internal static ConfigWrapper<bool> ScrollThroughMalesToo { get; set; }
        internal static ConfigWrapper<bool> ShowInfoMsg { get; set; }
        internal static ConfigWrapper<float> LockLeashLength { get; set; }
        internal static ConfigWrapper<bool> AutoSwitchLock { get; set; }
        internal static ConfigWrapper<KeyboardShortcut> LockOnKey { get; set; }
        internal static ConfigWrapper<KeyboardShortcut> PrevCharaKey { get; set; }
        internal static ConfigWrapper<KeyboardShortcut> NextCharaKey { get; set; }

        private static Harmony harmony;
        private static GameObject bepinex;

        private void Awake()
        {
            bepinex = gameObject;
            Logger = base.Logger;

            TargetData.LoadData();

            TrackingSpeedNormal = Config.GetSetting(SECTION_GENERAL, "Tracking peed", 0.1f, new ConfigDescription(DESCRIPTION_TRACKSPEED, new AcceptableValueRange<float>(0.01f, 0.3f)));
            ScrollThroughMalesToo = Config.GetSetting(SECTION_GENERAL, "Scroll through males too", true, new ConfigDescription(DESCRIPTION_SCROLLMALES));
            ShowInfoMsg = Config.GetSetting(SECTION_GENERAL, "Show info messages", false, new ConfigDescription(DESCRIPTION_SHOWINFOMSG));
            LockLeashLength = Config.GetSetting(SECTION_GENERAL, "Leash length", 0f, new ConfigDescription(DESCRIPTION_LEASHLENGTH, new AcceptableValueRange<float>(0f, 0.5f)));
            AutoSwitchLock = Config.GetSetting(SECTION_GENERAL, "Auto switch lock", false, new ConfigDescription(DESCRIPTION_AUTOLOCK));

            LockOnKey = Config.GetSetting(SECTION_HOTKEYS, "Lock on", new KeyboardShortcut(KeyCode.Mouse4));
            PrevCharaKey = Config.GetSetting(SECTION_HOTKEYS, "Select previous character", new KeyboardShortcut(KeyCode.None));
            NextCharaKey = Config.GetSetting(SECTION_HOTKEYS, "Select next character", new KeyboardShortcut(KeyCode.None));

            harmony = HarmonyWrapper.PatchAll(typeof(Hooks));
        }

#if DEBUG
        private void OnDestroy()
        {
            harmony.UnpatchAll();
        }
#endif

        private class Hooks
        {
            [HarmonyPrefix, HarmonyPatch(typeof(CustomScene), "Start")]
            public static void MakerEntrypoint()
            {
                bepinex.GetOrAddComponent<MakerMono>();
            }

            [HarmonyPrefix, HarmonyPatch(typeof(CustomScene), "OnDestroy")]
            public static void MakerEnd()
            {
                Destroy(bepinex.GetComponent<MakerMono>());
            }

            [HarmonyPrefix, HarmonyPatch(typeof(StudioScene), "Start")]
            public static void StudioEntrypoint()
            {
                bepinex.GetOrAddComponent<StudioMono>();
            }

            [HarmonyPrefix, HarmonyPatch(typeof(HSceneProc), "SetShortcutKey")]
            public static void HSceneEntrypoint()
            {
                bepinex.GetOrAddComponent<HSceneMono>();
            }

            [HarmonyPrefix, HarmonyPatch(typeof(HSceneProc), "OnDestroy")]
            public static void HSceneEnd()
            {
                Destroy(bepinex.GetComponent<HSceneMono>());
            }
        }
    }
}
