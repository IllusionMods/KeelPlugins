using ExtensibleSaveFormat;
using KKAPI.Studio.SaveLoad;
using KKAPI.Utilities;
using MessagePack;
using Studio;
using UnityEngine;

namespace RealPOV.Koikatu
{
    public class SceneDataController : SceneCustomFunctionController
    {
        private const string DictID = "PovData";
        
        protected override void OnSceneLoad(SceneOperationKind operation, ReadOnlyDictionary<int, ObjectCtrlInfo> loadedItems)
        {
            var extData = GetExtendedData();
            if(extData == null)
                return;
            
            if(operation == SceneOperationKind.Load && extData.data.TryGetValue(DictID, out var povRawData))
            {
                var povData = MessagePackSerializer.Deserialize<ScenePovData>((byte[])povRawData);
                if(povData.FormatVersion < 1)
                    povData.CharaPrevVisibleHeadAlways = true;
                RealPOV.EnablePov(povData);
            }
        }

        protected override void OnSceneSave()
        {
            var povData = RealPOV.GetPovData();
            if(povData != null)
            {
                var pluginData = new PluginData();
                pluginData.data.Add(DictID, MessagePackSerializer.Serialize(povData));
                SetExtendedData(pluginData);
            }
        }
    }
    
    [MessagePackObject(true)]
    public class ScenePovData
    {
        public int FormatVersion = 1;
        public Vector3 Rotation;
        public int CharaId;
        public bool CharaPrevVisibleHeadAlways;
        public float Fov;
    }
}
