using HarmonyLib;
using KeelPlugins.Core;
using System;
using System.Reflection;

namespace KeelPlugins.Koikatu
{
    internal class Maker
    {
        private static void Start() => EventFactory.InvokeEvent(_OnStart);
        private static MethodInfo StartMethod = AccessTools.Method(typeof(Maker), nameof(Start));

        private static Action _OnStart;
        public static event Action OnStart
        {
            add => EventFactory.AddAccessor(_OnStart, value, typeof(CustomScene), "Start", StartMethod);
            remove => EventFactory.RemoveAccessor(_OnStart, value);
        }

        private static void Destroy() => EventFactory.InvokeEvent(_OnExit);
        private static MethodInfo DestroyMethod = AccessTools.Method(typeof(Maker), nameof(Destroy));

        private static Action _OnExit;
        public static event Action OnExit
        {
            add => EventFactory.AddAccessor(_OnExit, value, typeof(CustomScene), "OnDestroy", StartMethod);
            remove => EventFactory.RemoveAccessor(_OnExit, value);
        }
    }
}
