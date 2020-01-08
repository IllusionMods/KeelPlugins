using BepInEx;
using BepInEx.Logging;

namespace KeelPlugins
{
    public abstract class RealPOVCore : BaseUnityPlugin
    {
        public const string GUID = "keelhauled.realpov";
        public const string PluginName = "RealPOV";
        public const string Version = "1.0.2." + BuildNumber.Version;

        internal const string SECTION_GENERAL = "General";
        internal const string SECTION_HOTKEYS = "Keyboard shortcuts";

        internal static new ManualLogSource Logger;

        protected virtual void Awake()
        {
            Logger = base.Logger;
        }
    }
}
