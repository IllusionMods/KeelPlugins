using BepInEx;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KeelPlugins
{
    [BepInPlugin(GUID, "MainThreadDispatcher_KeelPlugins", Version)]
    public class MainThreadDispatcher : BaseUnityPlugin
    {
        public const string GUID = "keelhauled.mainthreaddispatcher";
        public const string Version = "1.0.0";

        private static MainThreadDispatcher instance;
        private static Queue<Action> executionQueue = new Queue<Action>();

        public static void EnsureInstance(GameObject parent)
        {
            if(!instance)
                instance = parent.AddComponent<MainThreadDispatcher>();
        }

        public static void Enqueue(IEnumerator action)
        {
            lock(executionQueue)
                executionQueue.Enqueue(() => instance.StartCoroutine(action));
        }

        public static void Enqueue(Action action)
        {
            Enqueue(ActionWrapper(action));
        }

        private static IEnumerator ActionWrapper(Action action)
        {
            action();
            yield return null;
        }

        private void Update()
        {
            lock(executionQueue)
            {
                while(executionQueue.Count > 0)
                    executionQueue.Dequeue().Invoke();
            }
        }

        private void OnDestroy()
        {
            instance = null;
        }
    }
}
