using ChaCustom;
using System;
using System.Linq;
using UnityEngine;

namespace MakerBridge
{
    internal class MakerHandler : HandlerCore
    {
        private void Start()
        {
            watcher = CharaCardWatcher.Watch(MakerBridge.OtherCardPath, LoadChara);
            watcher.EnableRaisingEvents = true;
        }

        private void Update()
        {
            if(MakerBridge.SendChara.Value.IsDown())
                SaveCharacter(MakerBridge.MakerCardPath);
        }

        private void SaveCharacter(string path)
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

                customBase.chaCtrl.chaFile.SaveCharaFile(path);
            }
        }

        private void LoadChara(string path)
        {
            var cfw = FindObjectsOfType<CustomFileWindow>().FirstOrDefault(x => x.fwType == CustomFileWindow.FileWindowType.CharaLoad);
            var loadFace = true;
            var loadBody = true;
            var loadHair = true;
            var parameter = true;
            var loadCoord = true;

            if(cfw)
            {
                loadFace = cfw.tglChaLoadFace && cfw.tglChaLoadFace.isOn;
                loadBody = cfw.tglChaLoadBody && cfw.tglChaLoadBody.isOn;
                loadHair = cfw.tglChaLoadHair && cfw.tglChaLoadHair.isOn;
                parameter = cfw.tglChaLoadParam && cfw.tglChaLoadParam.isOn;
                loadCoord = cfw.tglChaLoadCoorde && cfw.tglChaLoadCoorde.isOn;
            }

            var chaCtrl = CustomBase.Instance.chaCtrl;
            var originalSex = chaCtrl.sex;

            chaCtrl.chaFile.LoadFileLimited(path, chaCtrl.sex, loadFace, loadBody, loadHair, parameter, loadCoord);
            if(chaCtrl.chaFile.GetLastErrorCode() != 0)
                throw new Exception("LoadFileLimited failed");

            if(chaCtrl.chaFile.parameter.sex != originalSex)
            {
                chaCtrl.chaFile.parameter.sex = originalSex;
                MakerBridge.LogMsg("Warning: The character's sex has been changed to match the editor mode.");
            }

            chaCtrl.ChangeCoordinateType();
            chaCtrl.Reload(!loadCoord, !loadFace && !loadCoord, !loadHair, !loadBody);
            CustomBase.Instance.updateCustomUI = true;

            MakerBridge.LogMsg("Character received");
        }
    }
}
