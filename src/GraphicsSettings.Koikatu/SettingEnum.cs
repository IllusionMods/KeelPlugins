using System.ComponentModel;

namespace KeelPlugins
{
    internal class SettingEnum
    {
        public enum VSyncType
        {
            Disabled,
            Enabled,
            Half
        }

        public enum ShadowQuality
        {
            Disabled = UnityEngine.ShadowQuality.Disable,
            [Description("Hard only")]
            HardOnly = UnityEngine.ShadowQuality.HardOnly,
            [Description("Soft and hard")]
            SoftHard = UnityEngine.ShadowQuality.All
        }

        public enum BackgroundRunMode
        {
            No,
            Yes,
            Limited
        }

        public enum DisplayMode
        {
            Fullscreen,
            [Description("Borderless fullscreen")]
            BorderlessFullscreen,
            Windowed
        }
    }
}
