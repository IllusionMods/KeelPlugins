using ChaCustom;
using HarmonyLib;
using System;
using System.IO;
using System.Threading;
using UniRx;
using UnityEngine;

namespace KeelPlugins
{
    internal class MakerHandler : MonoBehaviour
    {
        private void Start()
        {
            var watcher = new FileSystemWatcher
            {
                Path = Path.GetDirectoryName(MakerBridge.OtherCardPath),
                Filter = Path.GetFileName(MakerBridge.OtherCardPath)
            };

            watcher.Created += FileChanged;
            watcher.Changed += FileChanged;
            watcher.EnableRaisingEvents = true;
        }

        private void FileChanged(object sender, FileSystemEventArgs e)
        {
            bool fileIsBusy = true;
            while(fileIsBusy)
            {
                try
                {
                    using(var file = File.Open(e.FullPath, FileMode.Open, FileAccess.Read, FileShare.Read)) { }
                    fileIsBusy = false;
                }
                catch(IOException)
                {
                    MakerBridge.Logger.LogDebug("File is still being written to, retrying.");
                    Thread.Sleep(100);
                }
            }

            MainThreadDispatcher.Post(LoadChara, null);
        }

        private void Update()
        {
            if(MakerBridge.SendChara.Value.IsDown())
                SaveCharacter();
        }

        private void LoadChara(object x)
        {
            var customCharaFile = FindObjectOfType<CustomCharaFile>();
            var traverse = Traverse.Create(customCharaFile);
            var fileWindow = traverse.Field("fileWindow").GetValue<CustomFileWindow>();
            var listCtrl = traverse.Field("listCtrl").GetValue<CustomFileListCtrl>();

            var index = listCtrl.GetInclusiveCount() + 1;
            listCtrl.AddList(index, "", "", "", MakerBridge.OtherCardPath, "", new DateTime());
            listCtrl.Create(customCharaFile.OnChangeSelect);
            listCtrl.SelectItem(index);
            fileWindow.btnChaLoadLoad.onClick.Invoke();
            listCtrl.RemoveList(index);
            listCtrl.Create(customCharaFile.OnChangeSelect);
        }

        private void SaveCharacter()
        {
            var customBase = CustomBase.Instance;
            if(customBase)
            {
                var empty = new Texture2D(1, 1, TextureFormat.ARGB32, false);
                empty.SetPixel(0, 0, Color.black);
                empty.Apply();

                var charFile = customBase.chaCtrl.chaFile;
                charFile.pngData = empty.EncodeToPNG();
                charFile.facePngData = empty.EncodeToPNG();

                customBase.chaCtrl.chaFile.SaveCharaFile(MakerBridge.MakerCardPath, byte.MaxValue, false);
            }
        }
    }
}
