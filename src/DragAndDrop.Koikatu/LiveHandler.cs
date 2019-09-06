using Manager;
using System.Linq;

namespace KeelPlugins
{
    internal class LiveHandler : CardHandlerMethods
    {
        public override bool Condition => Scene.Instance && Scene.Instance.NowSceneNames.Any(x => x == "LiveStage");

        public override void Character_Load(string path, POINT pos, byte sex)
        {
            var chara = Singleton<ChaControl>.Instance;
            chara.chaFile.LoadCharaFile(path, byte.MaxValue, true, true);
            chara.ChangeCoordinateType((ChaFileDefine.CoordinateType)chara.fileStatus.coordinateType, true);
            chara.Reload(false, false, false, false);
        }

        public override void Coordinate_Load(string path, POINT pos)
        {
            var chara = Singleton<ChaControl>.Instance;
            chara.nowCoordinate.LoadFile(path);
            chara.AssignCoordinate((ChaFileDefine.CoordinateType)chara.fileStatus.coordinateType);
            chara.Reload(false, true, true, true);
        }
    }
}
