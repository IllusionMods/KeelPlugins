using System.IO;
using UnityEngine;

namespace MakerBridge.Core
{
    internal class HandlerCore : MonoBehaviour
    {
        protected FileSystemWatcher watcher;

        private void OnEnable()
        {
            if(watcher != null)
                watcher.EnableRaisingEvents = true;
        }

        private void OnDisable()
        {
            if(watcher != null)
                watcher.EnableRaisingEvents = false;
        }
    }
}
