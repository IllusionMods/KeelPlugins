using BepInEx;
using BepInEx.Configuration;
using ConfigurationManager;
using System;
using System.ComponentModel;
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
        private ConfigEntry<DisplayModes> DisplayMode { get; set; }
        private ConfigEntry<VSyncTypes> VSync { get; set; }
        private ConfigEntry<bool> LimitFrameRate { get; set; }
        private ConfigEntry<int> TargetFrameRate { get; set; }
        private ConfigEntry<int> AntiAliasing { get; set; }
        private ConfigEntry<AnisotropicFiltering> AnisotropicTextures { get; set; }
        private ConfigEntry<ShadowTypes> ShadowType { get; set; }
        private ConfigEntry<ShadowResolution> ShadowRes { get; set; }
        private ConfigEntry<ShadowProjection> ShadowProject { get; set; }
        private ConfigEntry<int> ShadowCascades { get; set; }
        private ConfigEntry<float> ShadowDistance { get; set; }
        private ConfigEntry<float> ShadowNearPlaneOffset { get; set; }
        private ConfigEntry<float> CameraNearClipPlane { get; set; }
        private ConfigEntry<BackgroundRunModes> RunInBackground { get; set; }
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
                        if(DisplayMode.Value == DisplayModes.BorderlessFullscreen)
                            StartCoroutine(DisplayInterop.MakeBorderless());
                    });
                }
            }
        }

        private void Awake()
        {
            Resolution = Config.AddSetting(CATEGORY_RENDER, "Resolution", "", new ConfigDescription(DESCRIPTION_RESOLUTION, null, new Action<SettingEntryBase>(ResolutionDrawer)));
            DisplayMode = Config.AddSetting(CATEGORY_RENDER, "Display mode", DisplayModes.Windowed);
            VSync = Config.AddSetting(CATEGORY_RENDER, "VSync level", VSyncTypes.Enabled, new ConfigDescription(DESCRIPTION_VSYNC));
            LimitFrameRate = Config.AddSetting(CATEGORY_RENDER, "Limit framerate", false, new ConfigDescription(DESCRIPTION_LIMITFRAMERATE));
            TargetFrameRate = Config.AddSetting(CATEGORY_RENDER, "Target framefate", 60);
            AntiAliasing = Config.AddSetting(CATEGORY_RENDER, "Anti aliasing multiplier", 4, new ConfigDescription(DESCRIPTION_ANTIALIASING));
            AnisotropicTextures = Config.AddSetting(CATEGORY_RENDER, "Anisotropic filtering", AnisotropicFiltering.ForceEnable, new ConfigDescription(DESCRIPTION_ANISOFILTER));
            ShadowType = Config.AddSetting(CATEGORY_SHADOW, "Shadow type", ShadowTypes.SoftHard);
            ShadowRes = Config.AddSetting(CATEGORY_SHADOW, "Shadow resolution", ShadowResolution.VeryHigh);
            ShadowProject = Config.AddSetting(CATEGORY_SHADOW, "Shadow projection", ShadowProjection.CloseFit);
            ShadowCascades = Config.AddSetting(CATEGORY_SHADOW, "Shadow cascades", 4, new ConfigDescription(DESCRIPTION_SHADOWCASCADES, new AcceptableValueList<int>(0, 2, 4)));
            ShadowDistance = Config.AddSetting(CATEGORY_SHADOW, "Shadow distance", 50f, new ConfigDescription(DESCRIPTION_SHADOWDISTANCE, new AcceptableValueRange<float>(0f, 100f)));
            ShadowNearPlaneOffset = Config.AddSetting(CATEGORY_SHADOW, "Shadow near plane offset", 2f, new ConfigDescription(DESCRIPTION_SHADOWNEARPLANEOFFSET, new AcceptableValueRange<float>(0f, 4f)));
            CameraNearClipPlane = Config.AddSetting(CATEGORY_MISC, "Camera near clip plane", 0.06f, new ConfigDescription(DESCRIPTION_CAMERANEARCLIPPLANE, new AcceptableValueRange<float>(0.01f, 0.06f)));
            RunInBackground = Config.AddSetting(CATEGORY_MISC, "Run in background", BackgroundRunModes.Yes, new ConfigDescription(DESCRIPTION_RUNINBACKGROUND));
            OptimizeInBackground = Config.AddSetting(CATEGORY_MISC, "Optimize in background", true, new ConfigDescription(DESCRIPTION_OPTIMIZEINBACKGROUND));

            SetDisplayMode();
            DisplayMode.SettingChanged += (sender, args) => SetDisplayMode();

            QualitySettings.vSyncCount = (int)VSync.Value;
            VSync.SettingChanged += (sender, args) => QualitySettings.vSyncCount = (int)VSync.Value;

            if(LimitFrameRate.Value) Application.targetFrameRate = TargetFrameRate.Value;
            LimitFrameRate.SettingChanged += (sender, args) => Application.targetFrameRate = LimitFrameRate.Value ? TargetFrameRate.Value : -1;
            TargetFrameRate.SettingChanged += (sender, args) => { if(LimitFrameRate.Value) Application.targetFrameRate = TargetFrameRate.Value; };

            QualitySettings.antiAliasing = AntiAliasing.Value;
            AntiAliasing.SettingChanged += (sender, args) => QualitySettings.antiAliasing = AntiAliasing.Value;

            QualitySettings.anisotropicFiltering = AnisotropicTextures.Value;
            AnisotropicTextures.SettingChanged += (sender, args) => QualitySettings.anisotropicFiltering = AnisotropicTextures.Value;

            QualitySettings.shadows = (ShadowQuality)ShadowType.Value;
            ShadowType.SettingChanged += (sender, args) => QualitySettings.shadows = (ShadowQuality)ShadowType.Value;

            QualitySettings.shadowResolution = ShadowRes.Value;
            ShadowRes.SettingChanged += (sender, args) => QualitySettings.shadowResolution = ShadowRes.Value;

            QualitySettings.shadowProjection = ShadowProject.Value;
            ShadowProject.SettingChanged += (sender, args) => QualitySettings.shadowProjection = ShadowProject.Value;

            QualitySettings.shadowCascades = ShadowCascades.Value;
            ShadowCascades.SettingChanged += (sender, args) => QualitySettings.shadowCascades = ShadowCascades.Value;

            QualitySettings.shadowDistance = ShadowDistance.Value;
            ShadowDistance.SettingChanged += (sender, args) => QualitySettings.shadowDistance = ShadowDistance.Value;

            QualitySettings.shadowNearPlaneOffset = ShadowNearPlaneOffset.Value;
            ShadowNearPlaneOffset.SettingChanged += (sender, args) => QualitySettings.shadowNearPlaneOffset = ShadowNearPlaneOffset.Value;

            //SceneManager.sceneLoaded += (scene, mode) => { if(Camera.main) Camera.main.nearClipPlane = CameraNearClipPlane.Value; };
            CameraNearClipPlane.SettingChanged += (sender, args) => { if(Camera.main) Camera.main.nearClipPlane = CameraNearClipPlane.Value; };

            if(RunInBackground.Value == BackgroundRunModes.No)
                Application.runInBackground = false;

            RunInBackground.SettingChanged += (sender, args) =>
            {
                switch(RunInBackground.Value)
                {
                    case BackgroundRunModes.No:
                        Application.runInBackground = false;
                        break;

                    case BackgroundRunModes.Limited:
                    case BackgroundRunModes.Yes:
                        Application.runInBackground = true;
                        break;
                }
            };
        }

        private void Update()
        {
            if(RunInBackground.Value != BackgroundRunModes.Limited)
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

            if(RunInBackground.Value != BackgroundRunModes.Limited)
                return;

            Application.runInBackground = true;
            _focusFrameCounter = 0;
        }

        private void SetDisplayMode()
        {
            switch(DisplayMode.Value)
            {
                case DisplayModes.Windowed:
                    DisplayInterop.MakeWindowed();
                    break;
                case DisplayModes.Fullscreen:
                    DisplayInterop.MakeFullscreen();
                    break;
                case DisplayModes.BorderlessFullscreen:
                    StartCoroutine(DisplayInterop.MakeBorderless());
                    break;
            }
        }

        private enum VSyncTypes
        {
            Disabled,
            Enabled,
            Half
        }

        private enum ShadowTypes
        {
            Disabled = ShadowQuality.Disable,
            [Description("Hard only")]
            HardOnly = ShadowQuality.HardOnly,
            [Description("Soft and hard")]
            SoftHard = ShadowQuality.All
        }

        private enum BackgroundRunModes
        {
            No,
            Yes,
            Limited
        }

        private enum DisplayModes
        {
            Fullscreen,
            [Description("Borderless fullscreen")]
            BorderlessFullscreen,
            Windowed
        }
    }
}
