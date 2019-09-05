using BepInEx;
using BepInEx.Harmony;
using HarmonyLib;
using KKAPI.Studio.SaveLoad;
using Studio;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace KeelPlugins
{
    [BepInDependency(KKAPI.KoikatuAPI.GUID)]
    [BepInProcess(KoikatuConstants.StudioProcessName)]
    [BepInPlugin(GUID, "Item Layer Edit", Version)]
    public class ItemLayerEdit : BaseUnityPlugin
    {
        public const string GUID = "keelhauled.itemlayeredit";
        public const string Version = "1.0.0";

        private static Harmony harmony;
        private static List<GameObject> targetObjects = new List<GameObject>();
        private static GameObject panel;
        private static Slider layerSliderComponent;
        private static TMP_InputField layerInputComponent;
        private static bool pluginSetup = false;

        private void Awake()
        {
            harmony = HarmonyWrapper.PatchAll(typeof(Hooks));
            StudioSaveLoadApi.RegisterExtraBehaviour<SceneDataController>(GUID);
        }

#if DEBUG
        private void OnDestroy()
        {
            harmony.UnpatchAll(GetType());
            DestroyImmediate(panel);

            var studio = Studio.Studio.Instance;
            studio.treeNodeCtrl.onSelect -= OnSelect;
            studio.treeNodeCtrl.onSelectMultiple -= OnSelectMultiple;
            studio.treeNodeCtrl.onDeselect -= OnDeselect;
            studio.treeNodeCtrl.onDelete -= OnDelete;
        }
#endif

        private static void OnSelect(TreeNodeObject node) => UpdateTargetObjects();
        private static void OnSelectMultiple() => UpdateTargetObjects();
        private static void OnDeselect(TreeNodeObject node) => UpdateTargetObjects();
        private static void OnDelete(TreeNodeObject node) => UpdateTargetObjects();

        private static void SetupStudio()
        {
            if(pluginSetup)
                return;

            var studio = Studio.Studio.Instance;
            studio.treeNodeCtrl.onSelect += OnSelect;
            studio.treeNodeCtrl.onSelectMultiple += OnSelectMultiple;
            studio.treeNodeCtrl.onDeselect += OnDeselect;
            studio.treeNodeCtrl.onDelete += OnDelete;

            panel = LayerUIBackend.CreatePanel();
            LayerUIBackend.CreateText(panel.transform);
            layerSliderComponent = LayerUIBackend.CreateSlider(panel.transform);
            layerInputComponent = LayerUIBackend.CreateInputfield(panel.transform);
            var layerDefButtonComponent = LayerUIBackend.CreateButton(panel.transform);

            layerDefButtonComponent.onClick.AddListener(() =>
            {
                SetTargetObjectLayersDefault();

                var defaultLayer = targetObjects.First().layer;
                layerInputComponent.text = defaultLayer.ToString();
                layerSliderComponent.value = defaultLayer;
            });

            layerSliderComponent.onValueChanged.AddListener((x) =>
            {
                SetTargetObjectLayers((int)x);
                layerInputComponent.text = x.ToString();
            });

            layerInputComponent.onValueChanged.AddListener((x) =>
            {
                if(int.TryParse(x, out int result))
                {
                    SetTargetObjectLayers(result);
                    layerSliderComponent.value = result;
                }
            });

            UpdateTargetObjects();
            pluginSetup = true;
        }

        private static void UpdateTargetObjects()
        {
            targetObjects.Clear();

            foreach(var objectCtrl in Studio.Studio.GetSelectObjectCtrl())
            {
                if(objectCtrl is OCIItem item)
                    targetObjects.Add(item.objectItem);
            }

            if(targetObjects.Count > 0)
            {
                layerSliderComponent.value = targetObjects[0].layer;
                layerInputComponent.text = targetObjects[0].layer.ToString();
            }
        }

        private static void SetTargetObjectLayers(int layer)
        {
            foreach(var targetObject in targetObjects)
            {
                if(targetObject.AddComponentIfNotExist<LayerDataContainer>(out var data))
                    data.DefaultLayer = targetObject.layer;

                targetObject.SetAllLayers(layer);
            }
        }

        private static void SetTargetObjectLayersDefault()
        {
            foreach(var targetObject in targetObjects)
            {
                var data = targetObject.GetComponent<LayerDataContainer>();
                if(data && targetObject.layer != data.DefaultLayer)
                    targetObject.SetAllLayers(data.DefaultLayer);
            }
        }

        private class Hooks
        {
            [HarmonyPostfix, HarmonyPatch(typeof(ManipulatePanelCtrl), "SetActive")]
            public static void ActivatePanel(ManipulatePanelCtrl __instance)
            {
                var traverse = Traverse.Create(__instance);
                if(traverse.Field("kinds").GetValue<int[]>().Contains(1))
                {
                    var rootPanel = traverse.Property("rootPanel").GetValue<IList>();
                    var rootObject = Traverse.Create(rootPanel[1]).Field("root").GetValue<GameObject>();
                    rootObject.SetActive(true);
                    SetupStudio();
                }
            }
        }
    }
}
