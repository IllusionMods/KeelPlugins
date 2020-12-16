using HarmonyLib;
using System;
using System.Reflection;

namespace KeelPlugins.Harmony
{
    public static class HarmonyExtensions
    {
        public static HarmonyLib.Harmony CreateAndPatchAll(Type type)
        {
            var harmony = new HarmonyLib.Harmony(Guid.NewGuid().ToString());
            harmony.PatchAll(type);

            foreach(var method in type.GetMethods(AccessTools.all))
            {
                foreach(var attr in method.GetCustomAttributes<HarmonyPatchExtAttribute>(false))
                    attr.Patch(harmony, method);
            }

            return harmony;
        }
    }
}
