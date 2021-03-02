using BepInEx;
using HarmonyLib;
using RealPOV.Core;
using Studio;
using System.Collections.Generic;
using System.Linq;
using BepInEx.Configuration;
using UnityEngine;
using UnityEngine.SceneManagement;

[assembly: System.Reflection.AssemblyFileVersion(RealPOV.Koikatu.RealPOV.Version)]

namespace RealPOV.Koikatu
{
    [BepInPlugin(GUID, PluginName, Version)]
    public class RealPOV : RealPOVCore
    {
        public const string Version = "1.0.4." + BuildNumber.Version;

        internal ConfigEntry<bool> HideHead { get; set; }

        private static int backupLayer;
        private static ChaControl currentChara;
        private static Queue<ChaControl> charaQueue;
        private readonly bool isStudio = Paths.ProcessName == "CharaStudio";
        private bool prevVisibleHeadAlways;
        private HFlag hFlag;

        protected override void Awake()
        {
            defaultFov = 90;
            defaultViewOffset = 0.001f;
            base.Awake();

            HideHead = Config.Bind(SECTION_GENERAL, "Hide character head", true, "Whene entering POV, hide the character's head. Prevents accessories and hair from obstructing the view.");

            Harmony.CreateAndPatchAll(GetType());

            SceneManager.sceneLoaded += (arg0, scene) =>
            {
                hFlag = FindObjectOfType<HFlag>();
                charaQueue = null;
            };
            SceneManager.sceneUnloaded += arg0 => charaQueue = null;
        }

        internal override void EnablePOV()
        {
            if (isStudio)
            {
                var selectedCharas = GuideObjectManager.Instance.selectObjectKey.Select(x => Studio.Studio.GetCtrlInfo(x) as OCIChar).Where(x => x != null).ToList();
                if (selectedCharas.Count > 0)
                    currentChara = selectedCharas.First().charInfo;
                else
                    Logger.LogMessage("Select a character in workspace to enter its POV");
            }
            else
            {
                Queue<ChaControl> CreateQueue()
                {
                    return new Queue<ChaControl>(FindObjectsOfType<ChaControl>());
                }
                ChaControl GetCurrentChara()
                {
                    for (int i = 0; i < charaQueue.Count; i++)
                    {
                        var chaControl = charaQueue.Dequeue();

                        // Remove destroyed
                        if (chaControl == null)
                            continue;

                        // Rotate the queue
                        charaQueue.Enqueue(chaControl);
                        if (chaControl.sex == 0 && hFlag != null && (hFlag.mode == HFlag.EMode.aibu || hFlag.mode == HFlag.EMode.lesbian || hFlag.mode == HFlag.EMode.masturbation)) continue;
                        // Found a valid character, otherwise skip (needed for story mode H because roam mode characters are in the queue too, just disabled)
                        if (chaControl.objTop.activeInHierarchy) return chaControl;
                    }
                    return null;
                }

                if (charaQueue == null) charaQueue = CreateQueue();

                currentChara = GetCurrentChara();
                if (currentChara == null)
                {
                    charaQueue = CreateQueue();
                    currentChara = GetCurrentChara();
                }
            }

            if (currentChara)
            {
                //foreach(var bone in currentChara.neckLookCtrl.neckLookScript.aBones)
                //    bone.neckBone.rotation = new Quaternion();

                prevVisibleHeadAlways = currentChara.fileStatus.visibleHeadAlways;
                if (HideHead.Value) currentChara.fileStatus.visibleHeadAlways = false;

                GameCamera = Camera.main;
                var cc = (MonoBehaviour)GameCamera.GetComponent<CameraControl_Ver2>() ?? GameCamera.GetComponent<Studio.CameraControl>();
                if (cc) cc.enabled = false;

                LookRotation = currentChara.objHeadBone.transform.rotation.eulerAngles;

                base.EnablePOV();

                backupLayer = GameCamera.gameObject.layer;
                GameCamera.gameObject.layer = 0;
            }
        }

        internal override void DisablePOV()
        {
            currentChara.fileStatus.visibleHeadAlways = prevVisibleHeadAlways;

            var cc = (MonoBehaviour)GameCamera.GetComponent<CameraControl_Ver2>() ?? GameCamera.GetComponent<Studio.CameraControl>();
            if (cc) cc.enabled = true;

            base.DisablePOV();

            GameCamera.gameObject.layer = backupLayer;
        }

        [HarmonyPrefix, HarmonyPatch(typeof(NeckLookControllerVer2), "LateUpdate")]
        private static bool ApplyRotation(NeckLookControllerVer2 __instance)
        {
            if (POVEnabled)
            {
                if (__instance.neckLookScript && currentChara.neckLookCtrl == __instance)
                {
                    __instance.neckLookScript.aBones[0].neckBone.rotation = Quaternion.identity;
                    __instance.neckLookScript.aBones[1].neckBone.rotation = Quaternion.identity;
                    __instance.neckLookScript.aBones[1].neckBone.Rotate(LookRotation);

                    var eyeObjs = currentChara.eyeLookCtrl.eyeLookScript.eyeObjs;
                    GameCamera.transform.position = Vector3.Lerp(eyeObjs[0].eyeTransform.position, eyeObjs[1].eyeTransform.position, 0.5f);
                    GameCamera.transform.rotation = currentChara.objHeadBone.transform.rotation;
                    GameCamera.transform.Translate(Vector3.forward * ViewOffset.Value);
                    GameCamera.fieldOfView = CurrentFOV;

                    return false;
                }
                else
                {
                    __instance.target = currentChara.eyeLookCtrl.transform;
                }
            }

            return true;
        }
    }
}
