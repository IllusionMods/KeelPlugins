using BepInEx;
using System;
using System.Linq;

namespace TitleShortcuts.Core
{
    public abstract class TitleShortcutsCore : BaseUnityPlugin
    {
        public const string GUID = "keelhauled.titleshortcuts";
        public const string PluginName = "Title Shortcuts";

        protected const string SECTION_HOTKEYS = "Keyboard shortcuts";
        protected const string SECTION_GENERAL = "General";

        protected const string DESCRIPTION_AUTOSTART = "Choose which mode to start automatically when launching the game.\n" +
                                                       "Hold esc or F1 during startup to cancel automatic behaviour or hold another shortcut to use that instead.";

        protected virtual string[] PossibleArguments { get; }

        protected static TitleShortcutsCore Plugin;

        protected virtual void Awake()
        {
            Plugin = this;
            Log.SetLogSource(Logger);
        }

        private static string startupArgument;
        protected static string StartupArgument
        {
            get
            {
                if(startupArgument == null)
                {
                    if(Plugin.PossibleArguments != null)
                    {
                        var args = Environment.GetCommandLineArgs();
                        if(args != null && args.Length > 0)
                            startupArgument = args.Select(x => x.Trim().ToLower()).FirstOrDefault(Plugin.PossibleArguments.Contains);
                    }

                    if(startupArgument == null)
                        startupArgument = "";
                }

                return startupArgument;
            }
        }
    }
}
