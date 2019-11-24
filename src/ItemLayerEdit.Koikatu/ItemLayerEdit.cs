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
        
        private void Awake()
        {
            StudioSaveLoadApi.RegisterExtraBehaviour<SceneDataController>(GUID);
        }

        string guiLayer = "10";

        private void OnGUI()
        {
            var rect = new Rect(300f, 500f, 400f, 400f);

            GUI.Box(rect, "");
            GUILayout.BeginArea(rect);

            GUILayout.BeginHorizontal();
            if(GUILayout.Button("Change layer") && int.TryParse(guiLayer, out int layer))
                SetSelectedObjectLayer(layer);

            guiLayer = GUILayout.TextField(guiLayer);
            GUILayout.EndHorizontal();

            GUILayout.EndArea();
        }

        private static void SetSelectedObjectLayer(int layer)
        {
            var targetObjects = Studio.Studio.GetSelectObjectCtrl().Select(x => (x as OCIItem)?.objectItem);

            foreach(var targetObject in targetObjects)
            {
                if(targetObject.AddComponentIfNotExist<LayerDataContainer>(out var data))
                    data.DefaultLayer = targetObject.layer;

                targetObject.SetAllLayers(layer);
            }
        }

        private static int GetSelectedObjectLayer()
        {
            return Studio.Studio.GetSelectObjectCtrl().Select(x => (x as OCIItem)?.objectItem).First().layer;
        }
    }
}
