using System;
using System.Linq;
//using System.Text;
//using HarmonyLib;
//using Mono.Cecil;
//using System.Reflection;
//using System.Diagnostics;
//using BepInEx;
//using BepInEx.Logging;

using System.Collections.Generic;
using BepInEx.Logging;
using Mono.Cecil;
using Mono.Cecil.Cil;
using HarmonyLib;
using BepInEx.Bootstrap;

namespace BepinexUnityLogFilter
{
    public class UnityLogFilter
    {
        private static Harmony harmony;
        private static ManualLogSource logger;

        public static IEnumerable<string> TargetDLLs { get; } = new string[0];
        public static void Patch(AssemblyDefinition assembly) { }

        //public static void Patch(AssemblyDefinition assembly)
        //{
        //    //logger = new ManualLogSource(nameof(UnityLogFilter));
        //    //logger.LogInfo("HELLO");
        //    //var types = ass.MainModule.Types;
        //}

        public static void Finish()
        {
            harmony = new Harmony(nameof(UnityLogFilter));
            logger = Logger.CreateLogSource(nameof(UnityLogFilter));

            logger.LogInfo("HELLO1");

            harmony.Patch(typeof(Chainloader).GetMethod(nameof(Chainloader.Initialize)),
                          postfix: new HarmonyMethod(typeof(UnityLogFilter).GetMethod(nameof(BreakingPatch))));
        }

        //public static IEnumerable<string> TargetDLLs { get; } = new[] { "UnityEngine.dll" };

        //public static void Patch(AssemblyDefinition ass)
        //{

        //    logger = Logger.CreateLogSource(nameof(UnityLogFilter));

        //    var method = ass.MainModule.GetType("UnityEngine", "Application").Methods.First(x => x.Name == "CallLogCallback");
        //    //method.Body.Instructions.Insert(0, Instruction.Create(OpCodes.Call, )
        //}

        //static object test;

        //public static void Finish()
        //{

        //}

        public static void BreakingPatch()
        {
            logger.LogInfo("HELLO2");
        }
    }
}
