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
        public const string Version = "1.0.3";

        private const string CATEGORY_RENDER = "Rendering";
        private const string CATEGORY_SHADOW = "Shadows";
        private const string CATEGORY_MISC = "Misc";

        private const string DESCRIPTION_RESOLUTION = "Dummy setting for the custom drawer.";
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
        
        private static ConfigEntry<string> Resolution { get; set; }
        private static ConfigEntry<VSyncType> VSyncCount { get; set; }
        private static ConfigEntry<bool> LimitFrameRate { get; set; }
        private static ConfigEntry<int> TargetFrameRate { get; set; }
        private static ConfigEntry<int> AntiAliasing { get; set; }
        private static ConfigEntry<AnisotropicFiltering> AnisotropicTextures { get; set; }
        private static ConfigEntry<ShadowQuality2> ShadowType { get; set; }
        private static ConfigEntry<ShadowResolution> ShadowRes { get; set; }
        private static ConfigEntry<ShadowProjection> ShadowProject { get; set; }
        private static ConfigEntry<int> ShadowCascades { get; set; }
        private static ConfigEntry<float> ShadowDistance { get; set; }
        private static ConfigEntry<float> ShadowNearPlaneOffset { get; set; }
        private static ConfigEntry<float> CameraNearClipPlane { get; set; }
        private static ConfigEntry<BackgroundRun> RunInBackground { get; set; }
        private static ConfigEntry<bool> OptimizeInBackground { get; set; }

        private bool fullscreen = Screen.fullScreen;
        private string resolutionX = Screen.width.ToString();
        private string resolutionY = Screen.height.ToString();
        private int _focusFrameCounter;

        private void ResolutionDrawer(SettingEntryBase entry)
        {
            fullscreen = GUILayout.Toggle(fullscreen, " Fullscreen", GUILayout.Width(90));
            string resX = GUILayout.TextField(resolutionX, GUILayout.Width(60));
            string resY = GUILayout.TextField(resolutionY, GUILayout.Width(60));

            if(resX != resolutionX && int.TryParse(resX, out _)) resolutionX = resX;
            if(resY != resolutionY && int.TryParse(resY, out _)) resolutionY = resY;

            if(GUILayout.Button("Apply", GUILayout.ExpandWidth(true)))
            {
                int x = int.Parse(resolutionX);
                int y = int.Parse(resolutionY);

                if(Screen.width != x || Screen.height != y || Screen.fullScreen != fullscreen)
                    Screen.SetResolution(x, y, fullscreen);
            }
        }

        private void Awake()
        {
            Resolution = Config.AddSetting(CATEGORY_RENDER, "Resolution", "", new ConfigDescription(DESCRIPTION_RESOLUTION, null, new Action<SettingEntryBase>(ResolutionDrawer)));
            VSyncCount = Config.AddSetting(CATEGORY_RENDER, "VSync level", VSyncType.Enabled, new ConfigDescription(DESCRIPTION_VSYNC));
            LimitFrameRate = Config.AddSetting(CATEGORY_RENDER, "Limit framerate", false, new ConfigDescription(DESCRIPTION_LIMITFRAMERATE));
            TargetFrameRate = Config.AddSetting(CATEGORY_RENDER, "Target framefate", 60);
            AntiAliasing = Config.AddSetting(CATEGORY_RENDER, "Anti aliasing multiplier", 8, new ConfigDescription(DESCRIPTION_ANTIALIASING));
            AnisotropicTextures = Config.AddSetting(CATEGORY_RENDER, "Anisotropic filtering", AnisotropicFiltering.ForceEnable, new ConfigDescription(DESCRIPTION_ANISOFILTER));
            ShadowType = Config.AddSetting(CATEGORY_SHADOW, "Shadow type", ShadowQuality2.SoftHard);
            ShadowRes = Config.AddSetting(CATEGORY_SHADOW, "Shadow resolution", ShadowResolution.VeryHigh);
            ShadowProject = Config.AddSetting(CATEGORY_SHADOW, "Shadow projection", ShadowProjection.CloseFit);
            ShadowCascades = Config.AddSetting(CATEGORY_SHADOW, "Shadow cascades", 4, new ConfigDescription(DESCRIPTION_SHADOWCASCADES, new AcceptableValueList<int>(0, 2, 4)));
            ShadowDistance = Config.AddSetting(CATEGORY_SHADOW, "Shadow distance", 50f, new ConfigDescription(DESCRIPTION_SHADOWDISTANCE, new AcceptableValueRange<float>(0f, 100f)));
            ShadowNearPlaneOffset = Config.AddSetting(CATEGORY_SHADOW, "Shadow near plane offset", 2f, new ConfigDescription(DESCRIPTION_SHADOWNEARPLANEOFFSET, new AcceptableValueRange<float>(0f, 4f)));
            CameraNearClipPlane = Config.AddSetting(CATEGORY_MISC, "Camera near clip plane", 0.06f, new ConfigDescription(DESCRIPTION_CAMERANEARCLIPPLANE, new AcceptableValueRange<float>(0.01f, 0.06f)));
            RunInBackground = Config.AddSetting(CATEGORY_MISC, "Run in background", BackgroundRun.Yes, new ConfigDescription(DESCRIPTION_RUNINBACKGROUND));
            OptimizeInBackground = Config.AddSetting(CATEGORY_MISC, "Optimize in background", true, new ConfigDescription(DESCRIPTION_OPTIMIZEINBACKGROUND));

            QualitySettings.vSyncCount = (int)VSyncCount.Value;
            VSyncCount.SettingChanged += (sender, args) => QualitySettings.vSyncCount = (int)VSyncCount.Value;

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

            if(RunInBackground.Value == BackgroundRun.No)
                Application.runInBackground = false;

            RunInBackground.SettingChanged += (sender, args) =>
            {
                switch(RunInBackground.Value)
                {
                    case BackgroundRun.No:
                        Application.runInBackground = false;
                        break;

                    case BackgroundRun.Limited:
                    case BackgroundRun.Yes:
                        Application.runInBackground = true;
                        break;
                }
            };
        }

        private void Update()
        {
            if(RunInBackground.Value != BackgroundRun.Limited) return;

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
            {
                QualitySettings.antiAliasing = hasFocus ? AntiAliasing.Value : 0;
            }

            if(RunInBackground.Value != BackgroundRun.Limited) return;

            Application.runInBackground = true;
            _focusFrameCounter = 0;
        }

        private enum VSyncType
        {
            Disabled,
            Enabled,
            Half
        }

        private enum ShadowQuality2
        {
            Disabled = ShadowQuality.Disable,
            [Description("Hard only")]
            HardOnly = ShadowQuality.HardOnly,
            [Description("Soft and hard")]
            SoftHard = ShadowQuality.All
        }

        private enum BackgroundRun
        {
            No,
            Yes,
            Limited
        }
    }
}
