using ChaCustom;
using Manager;
using MessagePack;
using System;
using System.Linq;
using UnityEngine;

namespace KeelPlugins
{
    internal class MakerHandler : CardHandlerMethods
    {
        public override bool Condition => Scene.Instance && Scene.Instance.NowSceneNames.Any(x => x == "CustomScene");

        public override void Character_Load(string path, POINT pos, byte sex)
        {
            var cfw = GameObject.FindObjectsOfType<CustomFileWindow>().FirstOrDefault(x => x.fwType == CustomFileWindow.FileWindowType.CharaLoad);
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
                DragAndDrop.Logger.LogMessage("Warning: The character's sex has been changed to match the editor mode.");
            }

            chaCtrl.ChangeCoordinateType(true);
            chaCtrl.Reload(!loadCoord, !loadFace && !loadCoord, !loadHair, !loadBody);
            CustomBase.Instance.updateCustomUI = true;
        }

        public override void Coordinate_Load(string path, POINT pos)
        {
            var cfw = GameObject.FindObjectsOfType<CustomFileWindow>().FirstOrDefault(x => x.fwType == CustomFileWindow.FileWindowType.CoordinateLoad);
            var loadClothes = true;
            var loadAcs = true;

            if(cfw)
            {
                loadClothes = cfw.tglCoordeLoadClothes && cfw.tglCoordeLoadClothes.isOn;
                loadAcs = cfw.tglCoordeLoadAcs && cfw.tglCoordeLoadAcs.isOn;
            }

            var chaCtrl = CustomBase.Instance.chaCtrl;
            var bytes = MessagePackSerializer.Serialize(chaCtrl.nowCoordinate.clothes);
            var bytes2 = MessagePackSerializer.Serialize(chaCtrl.nowCoordinate.accessory);
            chaCtrl.nowCoordinate.LoadFile(path);

            if(!loadClothes)
                chaCtrl.nowCoordinate.clothes = MessagePackSerializer.Deserialize<ChaFileClothes>(bytes);
            if(!loadAcs)
                chaCtrl.nowCoordinate.accessory = MessagePackSerializer.Deserialize<ChaFileAccessory>(bytes2);

            chaCtrl.Reload(false, true, true, true);
            chaCtrl.AssignCoordinate((ChaFileDefine.CoordinateType)chaCtrl.chaFile.status.coordinateType);
            CustomBase.Instance.updateCustomUI = true;
        }
    }
}
