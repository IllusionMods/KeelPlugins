using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;

namespace LockOnPlugin
{
    [BepInPlugin(GUID, "LockOnPlugin", Version)]
    public class LockOnPlugin : BaseUnityPlugin
    {
        public const string GUID = "keelhauled.lockonplugin";
        public const string Version = "2.6.0";

        private const string SECTION_HOTKEYS = "Keyboard Shortcuts";
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
        internal static ConfigEntry<KeyboardShortcut> GuiHotkey { get; set; }

        private void Awake()
        {
            TrackingSpeedNormal = Config.AddSetting(SECTION_GENERAL, "Tracking speed", 0.1f, new ConfigDescription(DESCRIPTION_TRACKSPEED, new AcceptableValueRange<float>(0.01f, 0.3f)));
            ScrollThroughMalesToo = Config.AddSetting(SECTION_GENERAL, "Scroll through males too", true, new ConfigDescription(DESCRIPTION_SCROLLMALES));
            ShowInfoMsg = Config.AddSetting(SECTION_GENERAL, "Show info messages", false, new ConfigDescription(DESCRIPTION_SHOWINFOMSG));
            LockLeashLength = Config.AddSetting(SECTION_GENERAL, "Leash length", 0f, new ConfigDescription(DESCRIPTION_LEASHLENGTH, new AcceptableValueRange<float>(0f, 0.5f)));
            AutoSwitchLock = Config.AddSetting(SECTION_GENERAL, "Auto switch lock", false, new ConfigDescription(DESCRIPTION_AUTOLOCK));

            LockOnKey = Config.AddSetting(SECTION_HOTKEYS, "Lock on", new KeyboardShortcut(KeyCode.Mouse4));
            PrevCharaKey = Config.AddSetting(SECTION_HOTKEYS, "Select previous character", new KeyboardShortcut(KeyCode.None));
            NextCharaKey = Config.AddSetting(SECTION_HOTKEYS, "Select next character", new KeyboardShortcut(KeyCode.None));
            GuiHotkey = Config.AddSetting(SECTION_HOTKEYS, "Show gui", new KeyboardShortcut(KeyCode.None));
        }

        private void Start()
        {
            try
            {
                HoneySelectPatches.Init();
                StudioNeoPatches.Init();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        public void OnLevelWasLoaded(int level)
        {
            switch(SceneManager.GetActiveScene().name)
            {
                case "Studio":
                {
                    new GameObject(LockOnBase.NAME_HSCENEMAKER).AddComponent<NeoMono>();
                    break;
                }

                case "HScene":
                {
                    new GameObject(LockOnBase.NAME_HSCENEMAKER).AddComponent<HSceneMono>();
                    break;
                }

                case "CustomScene":
                {
                    new GameObject(LockOnBase.NAME_HSCENEMAKER).AddComponent<MakerMono>();
                    break;
                }
            }
        }
    }
}
