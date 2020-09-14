using BepInEx;
using BepInEx.Logging;
using System;
using System.Linq;

namespace KeelPlugins
{
    public abstract class TitleShortcutsCore : BaseUnityPlugin
    {
        public const string GUID = "keelhauled.titleshortcuts";
        public const string PluginName = "Title shortcuts";
        public const string Version = "1.1.2." + BuildNumber.Version;

        protected const string SECTION_HOTKEYS = "Keyboard shortcuts";

        protected virtual string[] PossibleArguments { get; }

        protected static TitleShortcutsCore plugin;
        protected static new ManualLogSource Logger;

        protected virtual void Awake()
        {
            plugin = this;
            Logger = base.Logger;
        }

        private static string startupArgument;
        protected static string StartupArgument
        {
            get
            {
                if(startupArgument == null)
                {
                    if(plugin.PossibleArguments != null)
                    {
                        var args = Environment.GetCommandLineArgs();
                        if(args != null && args.Length > 0)
                            startupArgument = args.Select(x => x.Trim().ToLower()).FirstOrDefault(x => plugin.PossibleArguments.Contains(x));
                    }

                    if(startupArgument == null)
                        startupArgument = "";
                }

                return startupArgument;
            }
        }
    }
}
