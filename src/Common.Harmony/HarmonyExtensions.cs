using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using MonoMod.Utils;

namespace KeelPlugins.Harmony
{
    public static class HarmonyExtensions
	{
		public static void Hook(this MethodBase method, Action action, Harmony harmony = null)
        {
            if(harmony == null)
                harmony = new Harmony(Guid.NewGuid().ToString());

            PatchFactory.dynamicMethod = Transpilers.EmitDelegate(action).operand as DynamicMethod;
            var factoryMethod = AccessTools.Method(typeof(PatchFactory), nameof(PatchFactory.Get));
            harmony.Patch(method, new HarmonyMethod(factoryMethod));
        }
    }

    public class PatchFactory
    {
        public static DynamicMethod dynamicMethod;

        public static MethodInfo Get()
        {
            return dynamicMethod;
        }
    }
}
