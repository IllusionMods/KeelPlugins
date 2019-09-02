using BepInEx;
using BepInEx.Configuration;
using System.ComponentModel;
using UnityEngine;

namespace KeelPlugins
{
    [BepInPlugin(GUID, "Graphics Settings", Version)]
    public class GraphicsSettings : BaseUnityPlugin
    {
        public const string GUID = "keelhauled.graphicssettings";
        public const string Version = "1.0.3";

        const string CATEGORY_RENDER = "Rendering";
        const string CATEGORY_SHADOW = "Shadows";
        const string CATEGORY_MISC = "Misc";

        const string DESCRIPTION_ANISOFILTER = "Improves distant textures when they are being viewer from indirect angles.";
        const string DESCRIPTION_VSYNC = "VSync synchronizes the output video of the graphics card to the refresh rate of the monitor. " +
                                         "This prevents tearing and produces a smoother video output.\n" +
                                         "Half vsync synchronizes the output to half the refresh rate of your monitor.";
        const string DESCRIPTION_LIMITFRAMERATE = "VSync has to be disabled for this to take effect.";
        const string DESCRIPTION_ANTIALIASING = "Smooths out jagged edges on objects.";
        const string DESCRIPTION_SHADOWPROJECTION = "Close Fit renders higher resolution shadows but they can sometimes wobble slightly if the camera moves." +
                                                    "Stable Fit is lower resolution but no wobble.";
        const string DESCRIPTION_SHADOWCASCADES = "Increasing the number of cascades lessens the effects of perspective aliasing on shadows.";
        const string DESCRIPTION_SHADOWDISTANCE = "Increasing the distance lowers the shadow resolution slighly.";
        const string DESCRIPTION_SHADOWNEARPLANEOFFSET = "A low shadow near plane offset value can create the appearance of holes in shadows.";
        const string DESCRIPTION_CAMERANEARCLIPPLANE = "Determines how close the camera can be to objects without clipping into them. Lower equals closer.\n" +
                                                       "Note: The saved value is not loaded at the start currently.";
        const string DESCRIPTION_RUNINBACKGROUND = "Should the game be running when it is in the background (when the window is not focused)?\n" +
                                                   "On \"no\", the game will stop completely when it is in the background.\n" +
                                                   "On \"limited\", the game will stop if it has been unfocused and not loading anything for a couple seconds.";
        const string DESCRIPTION_OPTIMIZEINBACKGROUND = "Optimize the game when it is the background and unfocused. " +
                                                        "Settings such as anti-aliasing will be turned off or reduced in this state.";

        [Browsable(true)]
        [Category(CATEGORY_RENDER)]
        [DisplayName("!Resolution")]
        [CustomSettingDraw(nameof(ResolutionDrawer))]
        ConfigWrapper<string> ApplyResolution { get; set; }
        
        ConfigWrapper<VSyncType> VSyncCount { get; }
        ConfigWrapper<bool> LimitFrameRate { get; }
        ConfigWrapper<int> TargetFrameRate { get; }
        ConfigWrapper<int> AntiAliasing { get; }
        ConfigWrapper<AnisotropicFiltering> AnisotropicTextures { get; }
        ConfigWrapper<ShadowQuality2> ShadowType { get; }
        ConfigWrapper<ShadowResolution> ShadowRes { get; }
        ConfigWrapper<ShadowProjection> ShadowProject { get; }
        ConfigWrapper<int> ShadowCascades { get; }
        ConfigWrapper<float> ShadowDistance { get; }
        ConfigWrapper<float> ShadowNearPlaneOffset { get; }
        ConfigWrapper<float> CameraNearClipPlane { get; }
        ConfigWrapper<BackgroundRun> RunInBackground { get; }
        ConfigWrapper<bool> OptimizeInBackground { get; }

        GraphicsSettings()
        {
            VSyncCount = Config.GetSetting(CATEGORY_RENDER, "VSyncLevel", VSyncType.Enabled, new ConfigDescription(DESCRIPTION_VSYNC));
            LimitFrameRate = Config.GetSetting(CATEGORY_RENDER, "LimitFramerate", false, new ConfigDescription(DESCRIPTION_LIMITFRAMERATE));
            TargetFrameRate = Config.GetSetting(CATEGORY_RENDER, "TargetFrameRate", 60);
            AntiAliasing = Config.GetSetting(CATEGORY_RENDER, "AntiAliasingMultiplier", 8, new ConfigDescription(DESCRIPTION_ANTIALIASING));
            AnisotropicTextures = Config.GetSetting(CATEGORY_RENDER, "AnisotropicFiltering", AnisotropicFiltering.ForceEnable, new ConfigDescription(DESCRIPTION_ANISOFILTER));
            ShadowType = Config.GetSetting(CATEGORY_SHADOW, "ShadowType", ShadowQuality2.SoftHard);
            ShadowRes = Config.GetSetting(CATEGORY_SHADOW, "ShadowRes", ShadowResolution.VeryHigh);
            ShadowProject = Config.GetSetting(CATEGORY_SHADOW, "ShadowProjection", ShadowProjection.CloseFit);
            ShadowCascades = Config.GetSetting(CATEGORY_SHADOW, "ShadowCascades", 4, new ConfigDescription(DESCRIPTION_SHADOWCASCADES, new AcceptableValueList<int>(0, 2, 4)));
            ShadowDistance = Config.GetSetting(CATEGORY_SHADOW, "ShadowDistance", 50f, new ConfigDescription(DESCRIPTION_SHADOWDISTANCE, new AcceptableValueRange<float>(0f, 100f)));
            ShadowNearPlaneOffset = Config.GetSetting(CATEGORY_SHADOW, "ShadowNearPlaneOffset", 2f, new ConfigDescription(DESCRIPTION_SHADOWNEARPLANEOFFSET, new AcceptableValueRange<float>(0f, 4f)));
            CameraNearClipPlane = Config.GetSetting(CATEGORY_MISC, "CameraNearClipPlane", 0.06f, new ConfigDescription(DESCRIPTION_CAMERANEARCLIPPLANE, new AcceptableValueRange<float>(0.01f, 0.06f)));
            RunInBackground = Config.GetSetting(CATEGORY_MISC, "RunInBackground", BackgroundRun.Yes, new ConfigDescription(DESCRIPTION_RUNINBACKGROUND));
            OptimizeInBackground = Config.GetSetting(CATEGORY_MISC, "OptimizeInBackground", true, new ConfigDescription(DESCRIPTION_OPTIMIZEINBACKGROUND));
        }

        bool fullscreen = Screen.fullScreen;
        string resolutionX = Screen.width.ToString();
        string resolutionY = Screen.height.ToString();

        void ResolutionDrawer()
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

        void Awake()
        {
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

        int _focusFrameCounter;

        void Update()
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

        void OnApplicationFocus(bool hasFocus)
        {
            if(OptimizeInBackground.Value)
            {
                QualitySettings.antiAliasing = hasFocus ? AntiAliasing.Value : 0;
            }

            if(RunInBackground.Value != BackgroundRun.Limited) return;

            Application.runInBackground = true;
            _focusFrameCounter = 0;
        }

        enum VSyncType
        {
            Disabled,
            Enabled,
            Half
        }

        enum ShadowQuality2
        {
            Disabled = ShadowQuality.Disable,
            [Description("Hard only")]
            HardOnly = ShadowQuality.HardOnly,
            [Description("Soft and hard")]
            SoftHard = ShadowQuality.All
        }

        enum BackgroundRun
        {
            No,
            Yes,
            Limited
        }
    }
}
