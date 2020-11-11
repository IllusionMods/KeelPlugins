using HarmonyLib;
using System;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace KeelPlugins.Core
{
    public class EventFactory
    {
        private static Harmony harmony = new Harmony("KeelPlugins.EventFactory");
        private static MultiKeyDictionary<Type, string, MethodInfo> methods = new MultiKeyDictionary<Type, string, MethodInfo>();

        [MethodImpl(MethodImplOptions.Synchronized)]
        public static void AddAccessor(Action @event, Action value, Type type, string name, MethodInfo patch)
        {
            LazyPatch(type, name, patch);
            @event += value;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public static void RemoveAccessor(Action @event, Action value)
        {
            @event -= value;
        }

        private static void LazyPatch(Type type, string name, MethodInfo patch)
        {
            if(!methods.TryGetValue(type, name, out _))
            {
                var methodInfo = AccessTools.Method(type, name);
                harmony.Patch(methodInfo, new HarmonyMethod(patch));
                methods.Add(type, name, methodInfo);
            }
        }

        public static void InvokeEvent(Action e)
        {
            if(e != null)
            {
                foreach(var @delegate in e.GetInvocationList())
                    ((Action)@delegate).Invoke();
            }
        }
    }
}
