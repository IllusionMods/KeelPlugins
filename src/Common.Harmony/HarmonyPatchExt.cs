using HarmonyLib;
using System;
using System.Reflection;

namespace KeelPlugins.Harmony
{
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
    public sealed class HarmonyPatchExtAttribute : Attribute
    {
        private readonly MethodInfo targetMethod;
        private readonly bool firstRun = true;

        public HarmonyPatchExtAttribute(Type targetType, string targetMethodName, Type[] paramTypes = null, Type[] genericTypes = null)
        {
            if(firstRun)
            {
                firstRun = false;
                targetMethod = AccessTools.Method(targetType, targetMethodName, paramTypes, genericTypes);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="targetTypeName">Fully qualified type name with assembly name</param>
        /// <param name="targetMethodName"></param>
        /// <param name="paramTypes"></param>
        /// <param name="genericTypes"></param>
        public HarmonyPatchExtAttribute(string targetTypeName, string targetMethodName, Type[] paramTypes = null, Type[] genericTypes = null)
        {
            if(firstRun)
            {
                firstRun = false;
                var targetType = Type.GetType(targetTypeName);
                targetMethod = AccessTools.Method(targetType, targetMethodName, paramTypes, genericTypes); 
            }
        }

        public void Patch(HarmonyLib.Harmony harmony, MethodInfo patchmethod)
        {
            harmony.Patch(targetMethod, new HarmonyMethod(patchmethod));
        }
    }
}
