using BepInEx.Bootstrap;
using BepInEx.Logging;
using HarmonyLib;
using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

[assembly: System.Reflection.AssemblyFileVersion(UnityLogFilter.UnityLogFilter.Version)]

namespace UnityLogFilter
{
    public class UnityLogFilter
    {
        public const string Version = "1.0.2." + BuildNumber.Version;

        public static IEnumerable<string> TargetDLLs { get; } = new string[0];
        public static void Patch(AssemblyDefinition ass) { }

        private static Harmony Harmony;
        private static readonly List<Regex> filters = new List<Regex>();
        private static readonly string filterFilePath = Path.Combine(BepInEx.Paths.ConfigPath, "UnityLogFilter.txt");

        public static void Finish()
        {
            Harmony = new Harmony(nameof(UnityLogFilter));
            Log.SetLogSource(Logger.CreateLogSource(nameof(UnityLogFilter)));

            if(File.Exists(filterFilePath))
            {
                foreach(var line in File.ReadAllLines(filterFilePath))
                {
                    if(VerifyRegex(line))
                        filters.Add(new Regex(line));
                    else
                        Log.Warning($"'{line}' is not a valid regex pattern");
                }

                Log.Info($"Loaded {filters.Count} filter{(filters.Count == 1 ? "" : "s")}");
            }
            else
            {
                File.Create(filterFilePath);
                Log.Info($"{filterFilePath} created, add regular expressions to it.");
            }

            Harmony.Patch(AccessTools.Method(typeof(Chainloader), nameof(Chainloader.Initialize)),
                          postfix: new HarmonyMethod(AccessTools.Method(typeof(UnityLogFilter), nameof(ChainloaderHook))));
        }

        public static void ChainloaderHook()
        {
            Harmony.Patch(AccessTools.Method(typeof(UnityLogSource), "UnityLogMessageHandler"),
                          prefix: new HarmonyMethod(AccessTools.Method(typeof(UnityLogFilter), nameof(LogPatch))));
        }

        public static bool LogPatch(LogEventArgs eventArgs)
        {
            return !(eventArgs.Data is string msg && filters.Any(x => x.IsMatch(msg)));
        }

        public static bool VerifyRegex(string testPattern)
        {
            bool isValid = true;

            if(testPattern != null && testPattern.Trim().Length > 0)
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
