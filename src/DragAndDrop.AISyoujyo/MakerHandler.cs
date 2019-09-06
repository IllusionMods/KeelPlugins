using CharaCustom;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;

namespace KeelPlugins
{
    internal class MakerHandler : CardHandlerMethods
    {
        public override bool Condition => GameObject.FindObjectOfType<CharaCustom.CharaCustom>();

        public override void Character_Load(string path, POINT pos, byte sex)
        {
            var customCharaWindow = GameObject.FindObjectOfType<CustomCharaWindow>();
            var traverse = Traverse.Create(customCharaWindow);

            if(customCharaWindow)
            {
                int num = 0;
                var tglLoadOption = traverse.Field("tglLoadOption").GetValue<Toggle[]>();

                if(tglLoadOption != null)
                {
                    if(tglLoadOption[0].isOn) num |= 1;
                    if(tglLoadOption[1].isOn) num |= 2;
                    if(tglLoadOption[2].isOn) num |= 4;
                    if(tglLoadOption[3].isOn) num |= 8;
                    if(tglLoadOption[4].isOn) num |= 16;
                }
                
                customCharaWindow.onClick03?.Invoke(new CustomCharaFileInfo { FullPath = path }, num);
            }
        }
    }
}
