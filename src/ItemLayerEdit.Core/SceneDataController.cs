using ExtensibleSaveFormat;
using KKAPI.Studio.SaveLoad;
using KKAPI.Utilities;
using MessagePack;
using Studio;
using System.Collections.Generic;
using System.Linq;

namespace ItemLayerEdit.Koikatu
{
    internal class SceneDataController : SceneCustomFunctionController
    {
        private const string SaveId = "SavedLayers";

        protected override void OnSceneLoad(SceneOperationKind operation, ReadOnlyDictionary<int, ObjectCtrlInfo> loadedItems)
        {
            var data = GetExtendedData();
            if(data == null)
                return;

            if((operation == SceneOperationKind.Load || operation == SceneOperationKind.Import) && data.data.TryGetValue(SaveId, out var saveDataBytes))
            {
                var saveData = MessagePackSerializer.Deserialize<List<LayerSaveData>>((byte[])saveDataBytes);
                foreach(var layerData in saveData)
                {
                    if(loadedItems.TryGetValue(layerData.ObjectId, out var itemInfo) && itemInfo is OCIItem item)
                    {
                        item.objectItem.AddComponent<LayerDataContainer>().DefaultLayer = layerData.DefaultLayer;
                        item.objectItem.SetAllLayers(layerData.NewLayer);
                    }
                }
            }
        }

        protected override void OnSceneSave()
        {
            var saveData = new List<LayerSaveData>();
            foreach(var item in Studio.Studio.Instance.dicObjectCtrl.Values.OfType<OCIItem>())
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

            if(saveData.Count > 0)
            {
                var data = new PluginData();
                data.data.Add(SaveId, MessagePackSerializer.Serialize(saveData));
                SetExtendedData(data);
            }
        }
    }
}
