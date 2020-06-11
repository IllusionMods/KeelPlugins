using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BepInEx;
using BepInEx.Harmony;
using HarmonyLib;
using UnityEngine;

namespace KeelPlugins
{
    [BepInProcess(KoikatuConstants.MainGameProcessName)]
    [BepInPlugin(GUID, PluginName, Version)]
    public class CameraFrameMask : BaseUnityPlugin
    {
        public const string GUID = "keelhauled.cameraframemask";
        public const string PluginName = "CameraFrameMask";
        public const string Version = "1.0.0" + BuildNumber.Version;

        private static Harmony harmony;
        private static MaskComponent maskComponent;

        public void Awake()
        {
            harmony = HarmonyWrapper.PatchAll();
        }

        [HarmonyPrefix, HarmonyPatch(typeof(CustomScene), "Start")]
        public static void MakerEntrypoint()
        {
            maskComponent = Camera.main.GetOrAddComponent<MaskComponent>();
        }

        [HarmonyPrefix, HarmonyPatch(typeof(CustomScene), "OnDestroy")]
        public static void MakerEnd()
        {
            maskComponent = null;
            Destroy(Camera.main.GetComponent<MaskComponent>());
        }

        public void Update()
        {
            if(Input.GetKeyDown(KeyCode.B))
                maskComponent?.MaskFrames(60);
        }

        private void OnDestroy()
        {
            harmony.UnpatchAll();
        }
    }
}
