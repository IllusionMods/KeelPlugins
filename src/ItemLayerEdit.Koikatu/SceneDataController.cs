using ExtensibleSaveFormat;
using KKAPI.Studio.SaveLoad;
using KKAPI.Utilities;
using MessagePack;
using Studio;
using System.Collections.Generic;
using UnityEngine;

namespace KeelPlugins
{
    internal class SceneDataController : SceneCustomFunctionController
    {
        private const string SaveId = "SavedLayers";

        protected override void OnSceneLoad(SceneOperationKind operation, ReadOnlyDictionary<int, ObjectCtrlInfo> loadedItems)
        {
            var data = GetExtendedData();
            if(data == null)
                return;

            if(operation == SceneOperationKind.Load && data.data.TryGetValue(SaveId, out var saveDataBytes))
            {
                var saveData = MessagePackSerializer.Deserialize<List<LayerSaveData>>((byte[])saveDataBytes);
                foreach(var layerData in saveData)
                {
                    if(loadedItems.TryGetValue(layerData.ObjectId, out var itemInfo) && itemInfo is OCIItem item)
                    {
                        item.objectItem.AddComponent<LayerDataContainer>().DefaultLayer = layerData.DefaultLayer;
                        item.objectItem.layer = layerData.NewLayer;
                        foreach(Transform child in item.objectItem.transform)
                            child.gameObject.layer = layerData.NewLayer;
                    }
                }
            }
        }

        protected override void OnSceneSave()
        {
            var saveData = new List<LayerSaveData>();
            foreach(var objectCtrlInfo in Studio.Studio.Instance.dicObjectCtrl.Values)
            {
                if(objectCtrlInfo is OCIItem item)
                {
                    var data = item.objectItem.GetComponent<LayerDataContainer>();
                    if(data && data.DefaultLayer != item.objectItem.layer)
                    {
                        saveData.Add(new LayerSaveData
                        {
                            DefaultLayer = data.DefaultLayer,
                            NewLayer = item.objectItem.layer,
                            ObjectId = item.objectInfo.dicKey
                        });
                    }
                }
            }

            if(saveData.Count > 0)
            {
                var data = new PluginData();
                data.data.Add(SaveId, MessagePackSerializer.Serialize(saveData));
                SetExtendedData(data);
            }
        }
    }
}
