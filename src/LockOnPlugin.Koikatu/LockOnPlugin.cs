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
        public const string GUID = "keelhauled.lockonpluginkk";
        public const string Version = "1.0.0";
        internal static new ManualLogSource Logger;

        private const string SECTION_HOTKEYS = "Keyboard Shortcuts";
        private const string SECTION_GENERAL = "";

        private const string DESCRIPTION_TRACKSPEED = "The speed at which the target is followed.";
        private const string DESCRIPTION_SCROLLMALES = "Choose whether to include males in the rotation when switching between characters using the hotkeys from the plugin.";
        private const string DESCRIPTION_LEASHLENGTH = "The amount of slack allowed when tracking.";
        private const string DESCRIPTION_AUTOLOCK = "Lock on automatically after switching characters.";

        internal static ConfigWrapper<float> TrackingSpeedNormal { get; set; }
        internal static ConfigWrapper<bool> ScrollThroughMalesToo { get; set; }
        internal static ConfigWrapper<bool> ShowInfoMsg { get; set; }
        internal static ConfigWrapper<float> LockLeashLength { get; set; }
        internal static ConfigWrapper<bool> AutoSwitchLock { get; set; }
        internal static ConfigWrapper<bool> ShowDebugTargets { get; set; }
        internal static ConfigWrapper<KeyboardShortcut> LockOnKey { get; set; }
        internal static ConfigWrapper<KeyboardShortcut> LockOnGuiKey { get; set; }
        internal static ConfigWrapper<KeyboardShortcut> PrevCharaKey { get; set; }
        internal static ConfigWrapper<KeyboardShortcut> NextCharaKey { get; set; }

        private static Harmony harmony;
        private static GameObject bepinex;

        private LockOnPlugin()
        {
            Logger = base.Logger;

            TrackingSpeedNormal = Config.GetSetting(SECTION_GENERAL, "TrackingSpeed", 0.1f, new ConfigDescription(DESCRIPTION_TRACKSPEED, new AcceptableValueRange<float>(0.01f, 0.3f)));
            ScrollThroughMalesToo = Config.GetSetting(SECTION_GENERAL, "ScrollThroughMalesToo", true, new ConfigDescription(DESCRIPTION_SCROLLMALES));
            ShowInfoMsg = Config.GetSetting(SECTION_GENERAL, "ShowInfoMsg", false);
            LockLeashLength = Config.GetSetting(SECTION_GENERAL, "LeashLength", 0f, new ConfigDescription(DESCRIPTION_LEASHLENGTH, new AcceptableValueRange<float>(0f, 0.5f)));
            AutoSwitchLock = Config.GetSetting(SECTION_GENERAL, "AutoSwitchLock", false, new ConfigDescription(DESCRIPTION_AUTOLOCK));
            ShowDebugTargets = Config.GetSetting(SECTION_GENERAL, "ShowDebugTargets", false, new ConfigDescription("", null, "Advanced"));

            LockOnKey = Config.GetSetting(SECTION_HOTKEYS, "LockOn", new KeyboardShortcut(KeyCode.Mouse4));
            LockOnGuiKey = Config.GetSetting(SECTION_HOTKEYS, "ShowTargetGUI", new KeyboardShortcut(KeyCode.None));
            PrevCharaKey = Config.GetSetting(SECTION_HOTKEYS, "SelectPrevChara", new KeyboardShortcut(KeyCode.None));
            NextCharaKey = Config.GetSetting(SECTION_HOTKEYS, "SelectNextChara", new KeyboardShortcut(KeyCode.None));
        }

        private void Awake()
        {
            TargetData.LoadData();

            bepinex = gameObject;
            harmony = new Harmony($"{GUID}.harmony");
            HarmonyWrapper.PatchAll(typeof(Hooks), harmony);
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
            public static void CustomSceneInit()
            {
                bepinex.GetOrAddComponent<MakerMono>();
            }

            [HarmonyPrefix, HarmonyPatch(typeof(CustomScene), "OnDestroy")]
            public static void CustomSceneStop()
            {
                Destroy(bepinex.GetComponent<MakerMono>());
            }

            [HarmonyPrefix, HarmonyPatch(typeof(StudioScene), "Start")]
            public static void StudioSceneInit()
            {
                bepinex.GetOrAddComponent<StudioMono>();
            }

            [HarmonyPrefix, HarmonyPatch(typeof(HSceneProc), "SetShortcutKey")]
            public static void HSceneStart()
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
