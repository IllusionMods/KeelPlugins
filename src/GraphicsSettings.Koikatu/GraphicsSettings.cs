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

        private const string CATEGORY_RENDER = "Rendering";
        private const string CATEGORY_SHADOW = "Shadows";
        private const string CATEGORY_GENERAL = "General";

        private const string DESCRIPTION_RESOLUTION = "Dummy setting for the custom drawer. Resolution is saved automatically by the game after clicking apply.";
        private const string DESCRIPTION_ANISOFILTER = "Improves distant textures when they are being viewer from indirect angles.";
        private const string DESCRIPTION_VSYNC = "VSync synchronizes the output video of the graphics card to the refresh rate of the monitor. " +
                                                 "This prevents tearing and produces a smoother video output.\n" +
                                                 "Half vsync synchronizes the output to half the refresh rate of your monitor.";
        private const string DESCRIPTION_LIMITFRAMERATE = "VSync has to be disabled for this to take effect.";
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
        private ConfigEntry<bool> LimitFramerate { get; set; }
        private ConfigEntry<int> TargetFramerate { get; set; }
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
        private int _focusFrameCounter;

        private void Awake()
        {
            Resolution = Config.AddSetting(CATEGORY_RENDER, "Resolution", "", new ConfigDescription(DESCRIPTION_RESOLUTION, null, new ConfigurationManagerAttributes { CustomDrawer = new Action<ConfigEntryBase>(ResolutionDrawer), Order = 9, HideDefaultButton = true }));
            DisplayMode = Config.AddSetting(CATEGORY_RENDER, "DisplayMode", SettingEnum.DisplayMode.Windowed, new ConfigDescription("", null, new ConfigurationManagerAttributes { Order = 10, DispName = "Display mode" }));
            VSync = Config.AddSetting(CATEGORY_RENDER, "VSync", SettingEnum.VSyncType.Enabled, new ConfigDescription(DESCRIPTION_VSYNC, null, new ConfigurationManagerAttributes { Order = 8 }));
            LimitFramerate = Config.AddSetting(CATEGORY_RENDER, "LimitFramerate", false, new ConfigDescription(DESCRIPTION_LIMITFRAMERATE, null, new ConfigurationManagerAttributes { Order = 7, DispName = "Limit framerate" }));
            TargetFramerate = Config.AddSetting(CATEGORY_RENDER, "TargetFramerate", 60, new ConfigDescription("", null, new ConfigurationManagerAttributes { Order = 6, DispName = "Target framerate" }));
            AntiAliasing = Config.AddSetting(CATEGORY_RENDER, "AntialiasingMultiplier", 4, new ConfigDescription(DESCRIPTION_ANTIALIASING, null, new ConfigurationManagerAttributes { DispName = "Anti-aliasing multiplier" }));
            AnisotropicFiltering = Config.AddSetting(CATEGORY_RENDER, "AnisotropicFiltering", UnityEngine.AnisotropicFiltering.ForceEnable, new ConfigDescription(DESCRIPTION_ANISOFILTER, null, new ConfigurationManagerAttributes { DispName = "Anisotropic filtering" }));
            ShadowQuality = Config.AddSetting(CATEGORY_SHADOW, "ShadowQuality", SettingEnum.ShadowQuality.SoftHard, new ConfigDescription("", null, new ConfigurationManagerAttributes { DispName = "Shadow quality" }));
            ShadowResolution = Config.AddSetting(CATEGORY_SHADOW, "ShadowResolution", UnityEngine.ShadowResolution.VeryHigh, new ConfigDescription("", null, new ConfigurationManagerAttributes { DispName = "Shadow resolution" }));
            ShadowProjection = Config.AddSetting(CATEGORY_SHADOW, "ShadowProjection", UnityEngine.ShadowProjection.CloseFit, new ConfigDescription("", null, new ConfigurationManagerAttributes { DispName = "Shadow projection" }));
            ShadowCascades = Config.AddSetting(CATEGORY_SHADOW, "ShadowCascades", 4, new ConfigDescription(DESCRIPTION_SHADOWCASCADES, new AcceptableValueList<int>(0, 2, 4), new ConfigurationManagerAttributes { DispName = "Shadow cascades" }));
            ShadowDistance = Config.AddSetting(CATEGORY_SHADOW, "ShadowFistance", 50f, new ConfigDescription(DESCRIPTION_SHADOWDISTANCE, new AcceptableValueRange<float>(0f, 100f), new ConfigurationManagerAttributes { DispName = "Shadow distance" }));
            ShadowNearPlaneOffset = Config.AddSetting(CATEGORY_SHADOW, "ShadowNearPlaneOffset", 2f, new ConfigDescription(DESCRIPTION_SHADOWNEARPLANEOFFSET, new AcceptableValueRange<float>(0f, 4f), new ConfigurationManagerAttributes { DispName = "Shadow near plane offset" }));
            RunInBackground = Config.AddSetting(CATEGORY_GENERAL, "RunInBackground", SettingEnum.BackgroundRunMode.Yes, new ConfigDescription(DESCRIPTION_RUNINBACKGROUND, null, new ConfigurationManagerAttributes { DispName = "Run in background" }));
            OptimizeInBackground = Config.AddSetting(CATEGORY_GENERAL, "OptimizeInBackground", true, new ConfigDescription(DESCRIPTION_OPTIMIZEINBACKGROUND, null, new ConfigurationManagerAttributes { DispName = "Optimize in background" }));

            if(LimitFramerate.Value) Application.targetFrameRate = TargetFramerate.Value;
            LimitFramerate.SettingChanged += (sender, args) => Application.targetFrameRate = LimitFramerate.Value ? TargetFramerate.Value : -1;
            TargetFramerate.SettingChanged += (sender, args) => { if(LimitFramerate.Value) Application.targetFrameRate = TargetFramerate.Value; };

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
        }

        private void ResolutionDrawer(ConfigEntryBase entry)
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
                if(_focusFrameCounter < 100)
                    _focusFrameCounter++;
                else if(_focusFrameCounter == 100)
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
            _focusFrameCounter = 0;
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
