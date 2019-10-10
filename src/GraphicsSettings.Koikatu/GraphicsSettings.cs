using BepInEx;
using BepInEx.Configuration;
using ConfigurationManager;
using HarmonyLib;
using System;
using UnityEngine;

namespace KeelPlugins
{
    [BepInDependency(ConfigurationManager.ConfigurationManager.GUID)]
    [BepInPlugin(GUID, "Graphics Settings", Version)]
    public class GraphicsSettings : BaseUnityPlugin
    {
        public const string GUID = "keelhauled.graphicssettings";
        public const string Version = "1.0.4";

        private const string CATEGORY_RENDER = "Rendering";
        private const string CATEGORY_SHADOW = "Shadows";
        private const string CATEGORY_MISC = "Misc";

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
        private ConfigEntry<float> CameraNearClipPlane { get; set; }
        private ConfigEntry<SettingEnum.BackgroundRunMode> RunInBackground { get; set; }
        private ConfigEntry<bool> OptimizeInBackground { get; set; }

        private string resolutionX = Screen.width.ToString();
        private string resolutionY = Screen.height.ToString();
        private int _focusFrameCounter;

        private void ResolutionDrawer(SettingEntryBase entry)
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
                    DisplayInterop.SetResolutionCallback(this, x, y, Screen.fullScreen, () =>
                    {
                        Traverse.Create(FindObjectOfType<ConfigurationManager.ConfigurationManager>()).Method("CalculateWindowRect").GetValue();
                        if(DisplayMode.Value == SettingEnum.DisplayMode.BorderlessFullscreen)
                            DisplayInterop.MakeBorderless(this);
                    });
                }
            }
        }

        private void Awake()
        {
            Resolution = Config.AddSetting(CATEGORY_RENDER, "Resolution", "", new ConfigDescription(DESCRIPTION_RESOLUTION, null, new Action<SettingEntryBase>(ResolutionDrawer)));
            DisplayMode = Config.AddSetting(CATEGORY_RENDER, "Display mode", SettingEnum.DisplayMode.Windowed);
            VSync = Config.AddSetting(CATEGORY_RENDER, "VSync level", SettingEnum.VSyncType.Enabled, DESCRIPTION_VSYNC);
            LimitFramerate = Config.AddSetting(CATEGORY_RENDER, "Limit framerate", false, DESCRIPTION_LIMITFRAMERATE);
            TargetFramerate = Config.AddSetting(CATEGORY_RENDER, "Target framerate", 60);
            AntiAliasing = Config.AddSetting(CATEGORY_RENDER, "Anti-aliasing multiplier", 4, DESCRIPTION_ANTIALIASING);
            AnisotropicFiltering = Config.AddSetting(CATEGORY_RENDER, "Anisotropic filtering", UnityEngine.AnisotropicFiltering.ForceEnable, DESCRIPTION_ANISOFILTER);
            ShadowQuality = Config.AddSetting(CATEGORY_SHADOW, "Shadow type", SettingEnum.ShadowQuality.SoftHard);
            ShadowResolution = Config.AddSetting(CATEGORY_SHADOW, "Shadow resolution", UnityEngine.ShadowResolution.VeryHigh);
            ShadowProjection = Config.AddSetting(CATEGORY_SHADOW, "Shadow projection", UnityEngine.ShadowProjection.CloseFit);
            ShadowCascades = Config.AddSetting(CATEGORY_SHADOW, "Shadow cascades", 4, new ConfigDescription(DESCRIPTION_SHADOWCASCADES, new AcceptableValueList<int>(0, 2, 4)));
            ShadowDistance = Config.AddSetting(CATEGORY_SHADOW, "Shadow distance", 50f, new ConfigDescription(DESCRIPTION_SHADOWDISTANCE, new AcceptableValueRange<float>(0f, 100f)));
            ShadowNearPlaneOffset = Config.AddSetting(CATEGORY_SHADOW, "Shadow near plane offset", 2f, new ConfigDescription(DESCRIPTION_SHADOWNEARPLANEOFFSET, new AcceptableValueRange<float>(0f, 4f)));
            CameraNearClipPlane = Config.AddSetting(CATEGORY_MISC, "Camera near clip plane", 0.06f, new ConfigDescription(DESCRIPTION_CAMERANEARCLIPPLANE, new AcceptableValueRange<float>(0.01f, 0.06f)));
            RunInBackground = Config.AddSetting(CATEGORY_MISC, "Run in background", SettingEnum.BackgroundRunMode.Yes, DESCRIPTION_RUNINBACKGROUND);
            OptimizeInBackground = Config.AddSetting(CATEGORY_MISC, "Optimize in background", true, DESCRIPTION_OPTIMIZEINBACKGROUND);

            if(LimitFramerate.Value) Application.targetFrameRate = TargetFramerate.Value;
            LimitFramerate.SettingChanged += (sender, args) => Application.targetFrameRate = LimitFramerate.Value ? TargetFramerate.Value : -1;
            TargetFramerate.SettingChanged += (sender, args) => { if(LimitFramerate.Value) Application.targetFrameRate = TargetFramerate.Value; };

            //SceneManager.sceneLoaded += (scene, mode) => { if(Camera.main) Camera.main.nearClipPlane = CameraNearClipPlane.Value; };
            CameraNearClipPlane.SettingChanged += (sender, args) => { if(Camera.main) Camera.main.nearClipPlane = CameraNearClipPlane.Value; };

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
                    DisplayInterop.MakeWindowed();
                    break;
                case SettingEnum.DisplayMode.Fullscreen:
                    DisplayInterop.MakeFullscreen();
                    break;
                case SettingEnum.DisplayMode.BorderlessFullscreen:
                    DisplayInterop.MakeBorderless(this);
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
