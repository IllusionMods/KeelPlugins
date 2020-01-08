using BepInEx;
using System;
using System.Linq;

namespace KeelPlugins
{
    public abstract class TitleShortcutsCore : BaseUnityPlugin
    {
        public const string GUID = "keelhauled.titleshortcuts";
        public const string PluginName = "Title shortcuts";
        public const string Version = "1.1.1." + BuildNumber.Version;

        protected const string SECTION_HOTKEYS = "Keyboard shortcuts";
        protected const string SECTION_GENERAL = "General";

        protected const string DESCRIPTION_AUTOSTART = "Choose which mode to start automatically when launching the game.\n" +
                                                       "Hold esc or F1 during startup to cancel automatic behaviour or hold another shortcut to use that instead.";

        protected string Argument = "none";
        protected virtual string[] PossibleArguments { get; } = new string[] { };

        protected void CheckArgument()
        {
            string[] args = Environment.GetCommandLineArgs();
            if (args == null || args.Length == 0) return;
            Argument = args.Select(x => x.Trim().ToLower()).FirstOrDefault(x => PossibleArguments.Contains(x));
        }
    }
}
