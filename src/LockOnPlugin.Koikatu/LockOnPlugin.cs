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

        private const string SECTION_HOTKEYS = "Keyboard shortcuts";
        private const string SECTION_GENERAL = "General";

        private const string DESCRIPTION_TRACKSPEED = "The speed at which the target is followed.";
        private const string DESCRIPTION_SCROLLMALES = "Choose whether to include males in the rotation when switching between characters using the hotkeys from the plugin.";
        private const string DESCRIPTION_LEASHLENGTH = "The amount of slack allowed when tracking.";
        private const string DESCRIPTION_AUTOLOCK = "Lock on automatically after switching characters.";
        private const string DESCRIPTION_SHOWINFOMSG = "Show various messages about the plugin on screen.";

        internal static ConfigEntry<float> TrackingSpeedNormal { get; set; }
        internal static ConfigEntry<bool> ScrollThroughMalesToo { get; set; }
        internal static ConfigEntry<bool> ShowInfoMsg { get; set; }
        internal static ConfigEntry<float> LockLeashLength { get; set; }
        internal static ConfigEntry<bool> AutoSwitchLock { get; set; }
        internal static ConfigEntry<KeyboardShortcut> LockOnKey { get; set; }
        internal static ConfigEntry<KeyboardShortcut> PrevCharaKey { get; set; }
        internal static ConfigEntry<KeyboardShortcut> NextCharaKey { get; set; }

        private static Harmony harmony;
        private static GameObject bepinex;

        private void Awake()
        {
            bepinex = gameObject;
            Logger = base.Logger;

            TargetData.LoadData();

            TrackingSpeedNormal = Config.AddSetting(SECTION_GENERAL, "Tracking speed", 0.1f, new ConfigDescription(DESCRIPTION_TRACKSPEED, new AcceptableValueRange<float>(0.01f, 0.3f), new ConfigurationManagerAttributes { Order = 10 }));
            ScrollThroughMalesToo = Config.AddSetting(SECTION_GENERAL, "Scroll through males too", true, new ConfigDescription(DESCRIPTION_SCROLLMALES, null, new ConfigurationManagerAttributes { Order = 8 }));
            ShowInfoMsg = Config.AddSetting(SECTION_GENERAL, "Show info messages", false, new ConfigDescription(DESCRIPTION_SHOWINFOMSG, null, new ConfigurationManagerAttributes { Order = 6 }));
            LockLeashLength = Config.AddSetting(SECTION_GENERAL, "Leash length", 0f, new ConfigDescription(DESCRIPTION_LEASHLENGTH, new AcceptableValueRange<float>(0f, 0.5f), new ConfigurationManagerAttributes { Order = 9 }));
            AutoSwitchLock = Config.AddSetting(SECTION_GENERAL, "Auto switch lock", false, new ConfigDescription(DESCRIPTION_AUTOLOCK, null, new ConfigurationManagerAttributes { Order = 7 }));

            LockOnKey = Config.AddSetting(SECTION_HOTKEYS, "Lock on", new KeyboardShortcut(KeyCode.Mouse4), new ConfigDescription("", null, new ConfigurationManagerAttributes { Order = 10 }));
            PrevCharaKey = Config.AddSetting(SECTION_HOTKEYS, "Select previous character", new KeyboardShortcut(KeyCode.None), new ConfigDescription("", null, new ConfigurationManagerAttributes { Order = 8 }));
            NextCharaKey = Config.AddSetting(SECTION_HOTKEYS, "Select next character", new KeyboardShortcut(KeyCode.None), new ConfigDescription("", null, new ConfigurationManagerAttributes { Order = 9 }));

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
