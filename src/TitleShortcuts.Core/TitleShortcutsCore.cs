using BepInEx;

namespace KeelPlugins
{
    public abstract class TitleShortcutsCore : BaseUnityPlugin
    {
        public const string GUID = "keelhauled.titleshortcuts";
        public const string PluginName = "Title shortcuts";

        protected const string SECTION_HOTKEYS = "Keyboard Shortcuts";
        protected const string SECTION_GENERAL = "General";

        protected const string DESCRIPTION_AUTOSTART = "Choose which mode to start automatically when launching.\nDuring startup, " +
                                                     "hold esc or F1 to cancel automatic behaviour or hold another shortcut to use that instead.";
    }
}
