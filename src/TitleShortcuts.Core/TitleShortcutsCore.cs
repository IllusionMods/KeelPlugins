using BepInEx;

namespace KeelPlugins
{
    public abstract class TitleShortcutsCore : BaseUnityPlugin
    {
        public const string GUID = "keelhauled.titleshortcuts";
        public const string PluginName = "Title shortcuts";

        protected const string SECTION_HOTKEYS = "Keyboard shortcuts";
        protected const string SECTION_GENERAL = "General";

        protected const string DESCRIPTION_AUTOSTART = "Choose which mode to start automatically when launching the game.\n" +
                                                       "Hold esc or F1 during startup to cancel automatic behaviour or hold another shortcut to use that instead.";
    }
}
