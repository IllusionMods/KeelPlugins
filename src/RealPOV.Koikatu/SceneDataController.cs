using ExtensibleSaveFormat;
using KKAPI.Studio.SaveLoad;
using KKAPI.Utilities;
using MessagePack;
using RealPOV.Core;
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
                var povData = MessagePackSerializer.Deserialize<PovData>((byte[])povRawData);
                RealPOV.EnablePov(povData);
            }
        }

        protected override void OnSceneSave()
        {
            if(RealPOV.currentCharaID != -1)
            {
                var povData = new PovData
                {
                    CharaId = RealPOV.currentCharaID,
                    Fov = RealPOVCore.CurrentFOV,
                    Rotation = RealPOVCore.LookRotation
                };
                
                var pluginData = new PluginData();
                pluginData.data.Add(DictID, MessagePackSerializer.Serialize(povData));
                SetExtendedData(pluginData);
            }
        }

        [MessagePackObject(true)]
        public class PovData
        {
            public Vector3 Rotation;
            public int CharaId;
            public float Fov;
        }
    }
}