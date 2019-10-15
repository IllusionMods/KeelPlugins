using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using System;
using UnityEngine;

namespace KeelPlugins
{
    [BepInPlugin(GUID, "Graphics Settings", Version)]
    public class GraphicsSettings : BaseUnityPlugin
    {
        public const string GUID = "keelhauled.graphicssettings";
        public const string Version = "1.1.0";

        private const string CATEGORY_GENERAL = "General";
        private const string CATEGORY_RENDER = "Rendering";
        private const string CATEGORY_SHADOW = "Shadows";

        private const string DESCRIPTION_RESOLUTION = "Dummy setting for the custom drawer. Resolution is saved automatically by the game after clicking apply.";
        private const string DESCRIPTION_ANISOFILTER = "Improves distant textures when they are being viewer from indirect angles.";
        private const string DESCRIPTION_VSYNC = "VSync synchronizes the output video of the graphics card to the refresh rate of the monitor. " +
                                                 "This prevents tearing and produces a smoother video output.\n" +
                                                 "Half vsync synchronizes the output to half the refresh rate of your monitor.";
        private const string DESCRIPTION_FRAMERATELIMIT = "Limits your framerate to whatever value is set. -1 equals unlocked framerate.\n" +
                                                          "VSync has to be disabled for this setting to take effect.";
        private const string DESCRIPTION_ANTIALIASING = "Smooths out jagged edges on objects.";
        private const string DESCRIPTION_SHADOWPROJECTION = "Close Fit renders higher resolution shadows but they can sometimes wobble slightly if the camera moves." +
                                                            "Stable Fit is lower resolution but no wobble.";
        private const string DESCRIPTION_SHADOWCASCADES = "Increasing the number of cascades lessens the effects of perspective aliasing on shadows.";
        private const string DESCRIPTION_SHADOWDISTANCE = "Increasing the distance lowers the shadow resolution slighly.";
        private const string DESCRIPTION_SHADOWNEARPLANEOFFSET = "A low shadow near plane offset value can create the appearance of holes in shadows.";
        private const string DESCRIPTION_CAMERANEARCLIPPLANE = "Determines how close the camera can be to objects without clipping into them. Lower equals closer.\n" +
                                                               "Note: The saved value is not loaded at the start currently.";
        private const string DESCRIPTION_RUNINBACKGROUND = "Should the game be running when it is in the background (when the window is not focused)?\n" +
                                                           "On \"no\", the game will stop completely when it is in the background.\n" +
                                                           "On \"limited\", the game will stop if it has been unfocused and not loading anything for a couple seconds.";
        private const string DESCRIPTION_OPTIMIZEINBACKGROUND = "Optimize the game when it is the background and unfocused. " +
                                                                "Settings such as anti-aliasing will be turned off or reduced in this state.";

        private ConfigEntry<string> Resolution { get; set; }
        private ConfigEntry<SettingEnum.DisplayMode> DisplayMode { get; set; }
        private ConfigEntry<SettingEnum.VSyncType> VSync { get; set; }
        private ConfigEntry<int> FramerateLimit { get; set; }
        private ConfigEntry<int> AntiAliasing { get; set; }
        private ConfigEntry<AnisotropicFiltering> AnisotropicFiltering { get; set; }
        private ConfigEntry<SettingEnum.ShadowQuality> ShadowQuality { get; set; }
        private ConfigEntry<ShadowResolution> ShadowResolution { get; set; }
        private ConfigEntry<ShadowProjection> ShadowProjection { get; set; }
        private ConfigEntry<int> ShadowCascades { get; set; }
        private ConfigEntry<float> ShadowDistance { get; set; }
        private ConfigEntry<float> ShadowNearPlaneOffset { get; set; }
        private ConfigEntry<SettingEnum.BackgroundRunMode> RunInBackground { get; set; }
        private ConfigEntry<bool> OptimizeInBackground { get; set; }

        private string resolutionX = Screen.width.ToString();
        private string resolutionY = Screen.height.ToString();
        private int focusFrameCounter = 0;
        private bool framerateToggle = false;

        private void Awake()
        {
            Resolution = Config.AddSetting(CATEGORY_RENDER, "Resolution", "", new ConfigDescription(DESCRIPTION_RESOLUTION, null, new ConfigurationManagerAttributes { Order = 9, HideDefaultButton = true, CustomDrawer = new Action<ConfigEntryBase>(ResolutionDrawer) }));
            DisplayMode = Config.AddSetting(CATEGORY_RENDER, "Display mode", SettingEnum.DisplayMode.Windowed, new ConfigDescription("", null, new ConfigurationManagerAttributes { Order = 10 }));
            VSync = Config.AddSetting(CATEGORY_RENDER, "VSync", SettingEnum.VSyncType.Enabled, new ConfigDescription(DESCRIPTION_VSYNC, null, new ConfigurationManagerAttributes { Order = 8 }));
            FramerateLimit = Config.AddSetting(CATEGORY_RENDER, "Framerate limit", -1, new ConfigDescription(DESCRIPTION_FRAMERATELIMIT, null, new ConfigurationManagerAttributes { Order = 7, HideDefaultButton = true, CustomDrawer = new Action<ConfigEntryBase>(FramerateLimitDrawer) }));
            AntiAliasing = Config.AddSetting(CATEGORY_RENDER, "Anti-aliasing multiplier", 4, new ConfigDescription(DESCRIPTION_ANTIALIASING, new AcceptableValueRange<int>(0, 8)));
            AnisotropicFiltering = Config.AddSetting(CATEGORY_RENDER, "Anisotropic filtering", UnityEngine.AnisotropicFiltering.ForceEnable, new ConfigDescription(DESCRIPTION_ANISOFILTER));
            ShadowQuality = Config.AddSetting(CATEGORY_SHADOW, "Shadow quality", SettingEnum.ShadowQuality.SoftHard);
            ShadowResolution = Config.AddSetting(CATEGORY_SHADOW, "Shadow resolution", UnityEngine.ShadowResolution.VeryHigh);
            ShadowProjection = Config.AddSetting(CATEGORY_SHADOW, "Shadow projection", UnityEngine.ShadowProjection.CloseFit);
            ShadowCascades = Config.AddSetting(CATEGORY_SHADOW, "Shadow cascades", 4, new ConfigDescription(DESCRIPTION_SHADOWCASCADES, new AcceptableValueList<int>(0, 2, 4)));
            ShadowDistance = Config.AddSetting(CATEGORY_SHADOW, "Shadow distance", 50f, new ConfigDescription(DESCRIPTION_SHADOWDISTANCE, new AcceptableValueRange<float>(0f, 100f)));
            ShadowNearPlaneOffset = Config.AddSetting(CATEGORY_SHADOW, "Shadow near plane offset", 2f, new ConfigDescription(DESCRIPTION_SHADOWNEARPLANEOFFSET, new AcceptableValueRange<float>(0f, 4f)));
            RunInBackground = Config.AddSetting(CATEGORY_GENERAL, "Run in background", SettingEnum.BackgroundRunMode.Yes, new ConfigDescription(DESCRIPTION_RUNINBACKGROUND));
            OptimizeInBackground = Config.AddSetting(CATEGORY_GENERAL, "Optimize in background", true, new ConfigDescription(DESCRIPTION_OPTIMIZEINBACKGROUND));

            InitSetting(DisplayMode, SetDisplayMode);
            InitSetting(VSync, () => QualitySettings.vSyncCount = (int)VSync.Value);
            InitSetting(AntiAliasing, () => QualitySettings.antiAliasing = AntiAliasing.Value);
            InitSetting(AnisotropicFiltering, () => QualitySettings.anisotropicFiltering = AnisotropicFiltering.Value);
            InitSetting(ShadowQuality, () => QualitySettings.shadows = (ShadowQuality)ShadowQuality.Value);
            InitSetting(ShadowResolution, () => QualitySettings.shadowResolution = ShadowResolution.Value);
            InitSetting(ShadowProjection, () => QualitySettings.shadowProjection = ShadowProjection.Value);
            InitSetting(ShadowCascades, () => QualitySettings.shadowCascades = ShadowCascades.Value);
            InitSetting(ShadowDistance, () => QualitySettings.shadowDistance = ShadowDistance.Value);
            InitSetting(ShadowNearPlaneOffset, () => QualitySettings.shadowNearPlaneOffset = ShadowNearPlaneOffset.Value);
            InitSetting(RunInBackground, SetBackgroundRunMode);

            if(FramerateLimit.Value != -1)
            {
                Application.targetFrameRate = FramerateLimit.Value;
                framerateToggle = true;
            }
        }

        private void ResolutionDrawer(ConfigEntryBase configEntry)
        {
            string resX = GUILayout.TextField(resolutionX, GUILayout.Width(70));
            string resY = GUILayout.TextField(resolutionY, GUILayout.Width(70));

            if(resX != resolutionX && int.TryParse(resX, out _)) resolutionX = resX;
            if(resY != resolutionY && int.TryParse(resY, out _)) resolutionY = resY;

            if(GUILayout.Button("Apply", GUILayout.ExpandWidth(true)))
            {
                int x = int.Parse(resolutionX);
                int y = int.Parse(resolutionY);

                if(Screen.width != x || Screen.height != y)
                {
                    WindowInterop.SetResolutionCallback(this, x, y, Screen.fullScreen, () =>
                    {
                        var type = Type.GetType("ConfigurationManager.ConfigurationManager, ConfigurationManager");
                        Traverse.Create(FindObjectOfType(type)).Method("CalculateWindowRect").GetValue(); // update configmanager window size
                        if(DisplayMode.Value == SettingEnum.DisplayMode.BorderlessFullscreen)
                            WindowInterop.MakeBorderless(this);
                    });
                }
            }
        }

        private void FramerateLimitDrawer(ConfigEntryBase configEntry)
        {
            var toggle = GUILayout.Toggle(framerateToggle, "Enabled", GUILayout.Width(70));
            if(toggle != framerateToggle)
            {
                if(framerateToggle = toggle)
                {
                    var refreshRate = Screen.currentResolution.refreshRate;
                    FramerateLimit.Value = refreshRate;
                    Application.targetFrameRate = refreshRate;
                }
                else
                {
                    FramerateLimit.Value = -1;
                    Application.targetFrameRate = -1;
                }
            }

            var slider = (int)GUILayout.HorizontalSlider(FramerateLimit.Value, 30, 200, GUILayout.ExpandWidth(true));
            if(slider != FramerateLimit.Value && framerateToggle)
            {
                FramerateLimit.Value = Application.targetFrameRate = slider;
                if(!framerateToggle)
                    framerateToggle = true;
            }

            GUILayout.TextField(FramerateLimit.Value.ToString(), GUILayout.Width(50));
        }

        private void InitSetting<T>(ConfigEntry<T> configEntry, Action setter)
        {
            setter();
            configEntry.SettingChanged += (sender, args) => setter();
        }

        private void Update()
        {
            if(RunInBackground.Value != SettingEnum.BackgroundRunMode.Limited)
                return;

            if(!Manager.Scene.Instance.IsNowLoadingFade)
            {
                // Run for a bunch of frames to let the game load anything it's currently loading (scenes, cards, etc)
                // When loading it sometimes advances a frame at which point it would stop without this
                if(focusFrameCounter < 100)
                    focusFrameCounter++;
                else if(focusFrameCounter == 100)
                    Application.runInBackground = false;
            }
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            if(OptimizeInBackground.Value)
                QualitySettings.antiAliasing = hasFocus ? AntiAliasing.Value : 0;

            if(RunInBackground.Value != SettingEnum.BackgroundRunMode.Limited)
                return;

            Application.runInBackground = true;
            focusFrameCounter = 0;
        }

        private void SetDisplayMode()
        {
            switch(DisplayMode.Value)
            {
                case SettingEnum.DisplayMode.Windowed:
                    WindowInterop.MakeWindowed();
                    break;
                case SettingEnum.DisplayMode.Fullscreen:
                    WindowInterop.MakeFullscreen();
                    break;
                case SettingEnum.DisplayMode.BorderlessFullscreen:
                    WindowInterop.MakeBorderless(this);
                    break;
            }
        }

        private void SetBackgroundRunMode()
        {
            switch(RunInBackground.Value)
            {
                case SettingEnum.BackgroundRunMode.No:
                    Application.runInBackground = false;
                    break;

                case SettingEnum.BackgroundRunMode.Limited:
                case SettingEnum.BackgroundRunMode.Yes:
                    Application.runInBackground = true;
                    break;
            }
        }
    }
}
