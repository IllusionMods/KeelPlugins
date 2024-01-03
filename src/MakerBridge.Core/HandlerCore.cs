using System.IO;
using UnityEngine;

namespace MakerBridge
{
    internal abstract class HandlerCore : MonoBehaviour
    {
        protected FileSystemWatcher watcher;

        protected abstract string WatchedFilePath { get; }
        protected abstract string OutputFilePath { get; }

        protected abstract void SaveCharacter(string path);
        protected abstract void LoadCharacter(string path);

        private void Start()
        {
            watcher = CharaCardWatcher.Watch(WatchedFilePath, LoadCharacter);
            watcher.EnableRaisingEvents = true;
        }

        private void Update()
        {
            if(MakerBridge.SendChara.Value.IsDown())
                SaveCharacter(OutputFilePath);
        }

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
