using BepInEx;
using BepInEx.Configuration;
using KKAPI.Studio.SaveLoad;
using Studio;
using System.Linq;
using UnityEngine;

namespace KeelPlugins
{
    [BepInDependency(KKAPI.KoikatuAPI.GUID)]
    [BepInProcess(KoikatuConstants.StudioProcessName)]
    [BepInPlugin(GUID, "Item Layer Edit", Version)]
    public class ItemLayerEdit : BaseUnityPlugin
    {
        public const string GUID = "keelhauled.itemlayeredit";
        public const string Version = "1.0.0." + BuildNumber.Version;

        private ConfigEntry<KeyboardShortcut> ChangeLayer { get; set; }

        private void Start()
        {
            StudioSaveLoadApi.RegisterExtraBehaviour<SceneDataController>(GUID);

            ChangeLayer = Config.Bind("General", "Change layer", new KeyboardShortcut(KeyCode.V), "Toggle the selected objects between character and map layers");
        }

        private void Update()
        {
            if(ChangeLayer.Value.IsDown())
            {
                var layer = GetSelectedObjectLayer();
                if(layer == 11)
                    SetSelectedObjectLayer(10);
                else if(layer == 10)
                    SetSelectedObjectLayer(11);
            }
        }

        private static void SetSelectedObjectLayer(int layer)
        {
            var targetObjects = Studio.Studio.GetSelectObjectCtrl().OfType<OCIItem>().Select(x => x.objectItem);

            foreach(var targetObject in targetObjects)
            {
                if(targetObject.AddComponentIfNotExist<LayerDataContainer>(out var data))
                    data.DefaultLayer = targetObject.layer;

                targetObject.SetAllLayers(layer);
            }
        }

        private static int GetSelectedObjectLayer()
        {
            var targetObject = Studio.Studio.GetSelectObjectCtrl().OfType<OCIItem>().Select(x => x.objectItem).FirstOrDefault();
            return targetObject != null ? targetObject.layer : -1;
        }
    }
}
