using BepInEx;
using BepInEx.Configuration;
using BepInEx.Harmony;
using HarmonyLib;
using System.Collections.Generic;
using UnityEngine;

namespace KeelPlugins
{
    [BepInProcess(KoikatuConstants.MainGameProcessName)]
    [BepInProcess(KoikatuConstants.MainGameProcessNameSteam)]
    [BepInPlugin(GUID, "RealPov", Version)]
    public class RealPOV : RealPOVCore
    {
        private static ConfigEntry<float> ViewOffset { get; set; }
        private static ConfigEntry<float> DefaultFov { get; set; }
        private static ConfigEntry<float> MouseSens { get; set; }
        private static ConfigEntry<KeyboardShortcut> PovHotkey { get; set; }

        private Harmony harmony;
        private static List<Vector3> rotation = new List<Vector3> { new Vector3(), new Vector3() };
        private static ChaControl currentChara;
        private static bool povEnabled = false;
        private static float currentFov;
        private static float backupFov;
        private static Vector3 backupPos;
        private static int backupLayer;

        protected override void Awake()
        {
            base.Awake();

            ViewOffset = Config.Bind(SECTION_GENERAL, "View offset", 0.03f);
            DefaultFov = Config.Bind(SECTION_GENERAL, "Default FOV", 70f);
            MouseSens = Config.Bind(SECTION_GENERAL, "Mouse sensitivity", 1f);
            PovHotkey = Config.Bind(SECTION_HOTKEYS, "Toggle POV", new KeyboardShortcut(KeyCode.Backspace));

            harmony = HarmonyWrapper.PatchAll(GetType());
        }

        private void Update()
        {
            if(PovHotkey.Value.IsDown())
                TogglePov();

            if(povEnabled)
            {
                if(Input.GetMouseButton(0))
                {
                    var x = Input.GetAxis("Mouse X") * MouseSens.Value;
                    var y = -Input.GetAxis("Mouse Y") * MouseSens.Value;

                    rotation[0] += new Vector3(y, x, 0f);
                    rotation[1] += new Vector3(y, x, 0f);
                }
                else if(Input.GetMouseButton(1))
                {
                    currentFov += Input.GetAxis("Mouse X");
                }
            }
        }

#if DEBUG
        private void OnDestroy()
        {
            harmony.UnpatchAll();
        }
#endif

        private void TogglePov()
        {
            if(currentChara != null)
            {
                if(povEnabled)
                {
                    Camera.main.fieldOfView = backupFov;
                    Camera.main.transform.position = backupPos;
                    Camera.main.gameObject.layer = backupLayer;
                    povEnabled = false;
                }
                else
                {
                    backupFov = Camera.main.fieldOfView;
                    backupPos = Camera.main.transform.position;
                    backupLayer = Camera.main.gameObject.layer;
                    Camera.main.gameObject.layer = 0;
                    povEnabled = true;
                }
            }
        }

        [HarmonyPrefix, HarmonyPatch(typeof(NeckLookControllerVer2), "LateUpdate")]
        private static bool ApplyRotation(NeckLookControllerVer2 __instance)
        {
            if(povEnabled)
            {
                if(__instance.neckLookScript && currentChara.neckLookCtrl == __instance)
                {
                    __instance.neckLookScript.aBones[0].neckBone.rotation = Quaternion.identity;
                    __instance.neckLookScript.aBones[1].neckBone.rotation = Quaternion.identity;

                    //__instance.neckLookScript.aBones[0].neckBone.Rotate(rotation[0]);
                    __instance.neckLookScript.aBones[1].neckBone.Rotate(rotation[1]);
                }

                var eyeObjs = currentChara.eyeLookCtrl.eyeLookScript.eyeObjs;
                Camera.main.transform.position = Vector3.Lerp(eyeObjs[0].eyeTransform.position, eyeObjs[1].eyeTransform.position, 0.5f);
                Camera.main.transform.rotation = currentChara.objHeadBone.transform.rotation;
                Camera.main.transform.Translate(Vector3.forward * ViewOffset.Value);
                Camera.main.fieldOfView = currentFov;

                return false;
            }

            return true;
        }

        [HarmonyPrefix, HarmonyPatch(typeof(HSceneProc), "SetShortcutKey")]
        private static void HSceneStart()
        {
            currentChara = FindObjectOfType<ChaControl>();
            currentFov = DefaultFov.Value;

            foreach(var item in currentChara.neckLookCtrl.neckLookScript.aBones)
                item.neckBone.rotation = new Quaternion();
        }

        [HarmonyPrefix, HarmonyPatch(typeof(HSceneProc), "OnDestroy")]
        private static void HSceneEnd()
        {
            povEnabled = false;
            currentChara = null;
        }
    }
}
