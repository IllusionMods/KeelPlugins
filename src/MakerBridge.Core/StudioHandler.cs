using HarmonyLib;
using Studio;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace MakerBridge
{
    internal class StudioHandler : HandlerCore
    {
        protected override string WatchedFilePath => MakerBridge.MakerCardPath;
        protected override string OutputFilePath => MakerBridge.OtherCardPath;

        protected override void SaveCharacter(string path)
        {
            var characters = GetSelectedCharacters();
            if(characters.Count > 0)
            {
                var empty = new Texture2D(1, 1, TextureFormat.ARGB32, false);
                empty.SetPixel(0, 0, Color.black);
                empty.Apply();

                var charFile = characters[0].charInfo.chaFile;
                charFile.pngData = empty.EncodeToPNG();
                charFile.facePngData = empty.EncodeToPNG();

                using(var fileStream = new FileStream(path, FileMode.Create, FileAccess.Write))
                    charFile.SaveCharaFile(fileStream, true);
            }
            else
            {
                MakerBridge.LogMsg("Select a character to send to maker");
            }
        }

        protected override void LoadCharacter(string path)
        {
            var characters = GetSelectedCharacters();
            if(characters.Count > 0)
            {
                MakerBridge.LogMsg("Character received");

                foreach(var chara in characters)
                    chara.ChangeChara(path);

                UpdateStateInfo();
            }
            else
            {
                MakerBridge.LogMsg("Select a character before replacing it");
            }
        }

        private List<OCIChar> GetSelectedCharacters()
        {
            return GuideObjectManager.Instance.selectObjectKey.Select(x => Studio.Studio.GetCtrlInfo(x) as OCIChar).Where(x => x != null).ToList();
        }

        private void UpdateStateInfo()
        {
            var mpCharCtrl = FindObjectOfType<MPCharCtrl>();
            if(mpCharCtrl && (mpCharCtrl.select == 0 || mpCharCtrl.select == 1))
                mpCharCtrl.OnClickRoot(mpCharCtrl.select);
        }
    }
}
