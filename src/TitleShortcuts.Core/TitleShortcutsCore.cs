using BepInEx;

namespace TitleShortcuts.Core
{
    public abstract class TitleShortcutsCore : BaseUnityPlugin
    {
        public const string GUID = "keelhauled.titleshortcuts";
        public const string PluginName = "Title Shortcuts";

        protected const string SECTION_HOTKEYS = "Keyboard shortcuts";

        protected static TitleShortcutsCore Plugin;

        protected virtual void Awake()
        {
            Plugin = this;
            Log.SetLogSource(Logger);
        }
    }
}
