using BepInEx;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace KeelPlugins
{
    [BepInPlugin(GUID, PluginName, Version)]
    public class DragAndDrop : DragAndDropCore
    {
        public const string Version = "1.0.0";

        private static readonly byte[] CharaToken = Encoding.UTF8.GetBytes("【AIS_Chara】");
        private static readonly byte[] SexToken = Encoding.UTF8.GetBytes("sex");

        internal override void OnFiles(List<string> aFiles, POINT aPos)
        {
            var goodFiles = aFiles.Where(x =>
            {
                var ext = Path.GetExtension(x).ToLower();
                return ext == ".png";
            });

            if(goodFiles.Count() == 0)
            {
                Logger.LogMessage("No files to handle");
                return;
            }

            var cardHandler = CardHandlerMethods.GetActiveCardHandler();
            if(cardHandler != null)
            {
                foreach(var file in goodFiles)
                {
                    var bytes = File.ReadAllBytes(file);

                    if(BoyerMoore.ContainsSequence(bytes, CharaToken))
                    {
                        var index = new BoyerMoore(SexToken).Search(bytes).First();
                        var sex = bytes[index + SexToken.Length];
                        cardHandler.Character_Load(file, aPos, sex);
                    }
                    else
                    {
                        Logger.LogMessage("This file does not contain any AIS related data");
                    }
                }
            }
            else
            {
                Logger.LogMessage("No handler found for this scene");
            }
        }
    }
}
