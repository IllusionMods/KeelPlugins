using System;
using System.Linq;
using System.Collections.Generic;
using BepInEx.Logging;
using HarmonyLib;
using BepInEx.Bootstrap;
using Mono.Cecil;
using System.IO;
using System.Text.RegularExpressions;

namespace KeelPlugins
{
    public class UnityLogFilter
    {
        public const string Version = "1.0.0." + BuildNumber.Version;

        public static IEnumerable<string> TargetDLLs { get; } = new string[0];
        public static void Patch(AssemblyDefinition ass) { }

        private static Harmony harmony;
        private static ManualLogSource logger;
        private static IEnumerable<Regex> filters = new Regex[0];
        private static string FilterFile = Path.Combine(BepInEx.Paths.ConfigPath, "UnityLogFilter.txt");

        public static void Finish()
        {
            harmony = new Harmony(nameof(UnityLogFilter));
            logger = Logger.CreateLogSource(nameof(UnityLogFilter));

            if(File.Exists(FilterFile))
                filters = File.ReadAllLines(FilterFile).Where(VerifyRegex).Select(x => new Regex(x));

            harmony.Patch(AccessTools.Method(typeof(Chainloader), nameof(Chainloader.Initialize)),
                          postfix: new HarmonyMethod(AccessTools.Method(typeof(UnityLogFilter), nameof(ChainloaderHook))));
        }

        public static void ChainloaderHook()
        {
            harmony.Patch(AccessTools.Method(typeof(UnityLogSource), "UnityLogMessageHandler"),
                          prefix: new HarmonyMethod(AccessTools.Method(typeof(UnityLogFilter), nameof(LogPatch))));
        }

        public static bool LogPatch(LogEventArgs eventArgs)
        {
            return !(eventArgs.Data is string msg && filters.Any(x => x.IsMatch(msg)));
        }

        public static bool VerifyRegex(string testPattern)
        {
            bool isValid = true;

            if((testPattern != null) && (testPattern.Trim().Length > 0))
            {
                try
                {
                    Regex.Match("", testPattern);
                }
                catch(ArgumentException)
                {
                    // BAD PATTERN: Syntax error
                    isValid = false;
                }
            }
            else
            {
                //BAD PATTERN: Pattern is null or blank
                isValid = false;
            }

            return isValid;
        }
    }
}
