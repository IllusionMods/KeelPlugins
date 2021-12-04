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
                RealPOV.EnablePov(MessagePackSerializer.Deserialize<ScenePovData>((byte[])povRawData));
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
        public Vector3 Rotation;
        public int CharaId;
        public float Fov;
    }
}