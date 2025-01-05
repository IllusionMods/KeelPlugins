using System;
using BepInEx;
using HarmonyLib;
using RealPOV.Core;
using Studio;
using System.Collections.Generic;
using System.Linq;
using BepInEx.Configuration;
using KKAPI.Studio.SaveLoad;
using UnityEngine;
using UnityEngine.SceneManagement;

[assembly: System.Reflection.AssemblyVersion(RealPOV.Koikatu.RealPOV.Version)]

namespace RealPOV.Koikatu
{
    [BepInPlugin(GUID, PluginName, Version)]
    [BepInDependency(KKAPI.KoikatuAPI.GUID)]
    public class RealPOV : RealPOVCore
    {
        public const string Version = "1.4.0." + BuildNumber.Version;

        private ConfigEntry<bool> HideHead { get; set; }
        private ConfigEntry<PovSex> SelectedPOV { get; set; }

        private static int backupLayer;
        private static ChaControl currentChara;
        private static Queue<ChaControl> charaQueue;
        private readonly bool isStudio = Paths.ProcessName == "CharaStudio";
        private bool prevVisibleHeadAlways;
        private HFlag hFlag;
        private static int currentCharaId = -1;
        private static RealPOV plugin;

        private float dofOrigSize;
        private float dofOrigAperature;

        protected override void Awake()
        {
            plugin = this;
            defaultFov = 45f;
            defaultViewOffset = 0.05f;
            base.Awake();

            HideHead = Config.Bind(SECTION_GENERAL, "Hide character head", false, "When entering POV, hide the character's head. Prevents accessories and hair from obstructing the view.");
            SelectedPOV = Config.Bind(SECTION_GENERAL, "Selected POV", PovSex.Male, "Choose which sex to use as your point of view.");

            Harmony.CreateAndPatchAll(typeof(Hooks));
            StudioSaveLoadApi.RegisterExtraBehaviour<SceneDataController>(GUID);

            SceneManager.sceneLoaded += (arg0, scene) =>
            {
                hFlag = FindObjectOfType<HFlag>();
                charaQueue = null;
            };
            SceneManager.sceneUnloaded += arg0 => charaQueue = null;
        }

        public static void EnablePov(ScenePovData povData)
        {
            if(Studio.Studio.Instance.dicObjectCtrl.TryGetValue(povData.CharaId, out var chara))
            {
                currentChara = ((OCIChar)chara).charInfo;
                currentCharaId = chara.objectInfo.dicKey;
                currentCharaGo = currentChara.gameObject;
                LookRotation[currentCharaGo] = povData.Rotation;
                CurrentFOV = povData.Fov;
                plugin.EnablePov();
                // order matters here, as we want to override the value after it's been set by the call above
                plugin.prevVisibleHeadAlways = povData.CharaPrevVisibleHeadAlways;
            }
        }

        public static ScenePovData GetPovData()
        {
            return new ScenePovData
            {
                CharaId = currentCharaId,
                CharaPrevVisibleHeadAlways = plugin.prevVisibleHeadAlways,
                Fov = CurrentFOV != null ? CurrentFOV.Value : defaultFov,
                Rotation = currentCharaGo != null ? LookRotation[currentCharaGo] : new Vector3(0, 0, 0)
            };
        }

        protected override void EnablePov()
        {
            if(!currentChara)
            {
                if(isStudio)
                {
                    var selectedCharas = GuideObjectManager.Instance.selectObjectKey.Select(x => Studio.Studio.GetCtrlInfo(x) as OCIChar).Where(x => x != null).ToList();
                    if(selectedCharas.Count > 0)
                    {
                        var ociChar = selectedCharas.First();
                        currentChara = ociChar.charInfo;
                        currentCharaId = ociChar.objectInfo.dicKey;
                        currentCharaGo = currentChara.gameObject;
                    }
                    else
                    {
                        Logger.LogMessage("Select a character in workspace to enter its POV");
                    }
                }
                else
                {
                    if(charaQueue == null)
                        charaQueue = new Queue<ChaControl>(FindObjectsOfType<ChaControl>());

                    currentChara = GetCurrentChara();
                    if(!currentChara)
                    {
                        charaQueue = new Queue<ChaControl>(FindObjectsOfType<ChaControl>());
                        currentChara = GetCurrentChara();
                    }

                    currentCharaGo = null;
                    if(currentChara)
                        currentCharaGo = currentChara.gameObject;
                    else
                        Log.Message("Can't enter POV: Could not find any valid characters");
                }
            }

            if(currentChara)
            {
                prevVisibleHeadAlways = currentChara.fileStatus.visibleHeadAlways;
                if(HideHead.Value) currentChara.fileStatus.visibleHeadAlways = false;

                GameCamera = Camera.main;
                var cc = (MonoBehaviour)GameCamera.GetComponent<CameraControl_Ver2>();
                if(!cc) cc = GameCamera.GetComponent<Studio.CameraControl>();
                if(cc) cc.enabled = false;

                // Fix depth of field being completely out of focus
                var depthOfField = GameCamera.GetComponent<UnityStandardAssets.ImageEffects.DepthOfField>();
                dofOrigSize = depthOfField.focalSize;
                dofOrigAperature = depthOfField.aperture;
                if(depthOfField.enabled)
                {
                    depthOfField.focalTransform.localPosition = new Vector3(0, 0, 0.25f);
                    depthOfField.focalSize = 0.9f;
                    depthOfField.aperture = 0.6f;
                }

                // only use head rotation if there is no existing rotation
                if(!LookRotation.TryGetValue(currentCharaGo, out _))
                    LookRotation[currentCharaGo] = currentChara.objHeadBone.transform.rotation.eulerAngles;

                base.EnablePov();

                backupLayer = GameCamera.gameObject.layer;
                GameCamera.gameObject.layer = 0;
            }
        }

        private ChaControl GetCurrentChara()
        {
            for(int i = 0; i < charaQueue.Count; i++)
            {
                var chaControl = charaQueue.Dequeue();

                // Remove destroyed
                if(!chaControl)
                    continue;

                // Rotate the queue
                charaQueue.Enqueue(chaControl);

                if(chaControl.sex == 0 && hFlag && (hFlag.mode == HFlag.EMode.aibu || hFlag.mode == HFlag.EMode.lesbian || hFlag.mode == HFlag.EMode.masturbation)) 
                    continue;
                if(SelectedPOV.Value != PovSex.Either && chaControl.sex != (int)SelectedPOV.Value)
                    continue;
                // Found a valid character, otherwise skip (needed for story mode H because roam mode characters are in the queue too, just disabled)
                if(chaControl.objTop.activeInHierarchy)
                    return chaControl;
            }
            return null;
        }

        protected override void DisablePov()
        {
            currentChara.fileStatus.visibleHeadAlways = prevVisibleHeadAlways;
            currentChara = null;
            currentCharaId = -1;

            if(GameCamera != null)
            {
                var cc = (MonoBehaviour)GameCamera.GetComponent<CameraControl_Ver2>();
                if(!cc) cc = GameCamera.GetComponent<Studio.CameraControl>();
                if(cc) cc.enabled = true;

                var depthOfField = GameCamera.GetComponent<UnityStandardAssets.ImageEffects.DepthOfField>();
                depthOfField.focalSize = dofOrigSize;
                depthOfField.aperture = dofOrigAperature;
            }

            base.DisablePov();

            if(GameCamera != null)
                GameCamera.gameObject.layer = backupLayer;
        }

        private class Hooks
        {
            [HarmonyPrefix, HarmonyPatch(typeof(NeckLookControllerVer2), nameof(NeckLookControllerVer2.LateUpdate)), HarmonyWrapSafe]
            private static bool ApplyRotation(NeckLookControllerVer2 __instance)
            {
                if(POVEnabled)
                {
                    if(!currentChara)
                    {
                        plugin.DisablePov();
                        return true;
                    }

                    Vector3 rot;
                    if(LookRotation.TryGetValue(currentCharaGo, out var val))
                        rot = val;
                    else
                        LookRotation[currentCharaGo] = rot = currentChara.objHeadBone.transform.rotation.eulerAngles;

                    if(__instance.neckLookScript && currentChara.neckLookCtrl == __instance)
                    {
                        __instance.neckLookScript.aBones[0].neckBone.rotation = Quaternion.identity;
                        __instance.neckLookScript.aBones[1].neckBone.rotation = Quaternion.identity;
                        __instance.neckLookScript.aBones[1].neckBone.Rotate(rot);

                        var eyeObjs = currentChara.eyeLookCtrl.eyeLookScript.eyeObjs;
                        var pos = Vector3.Lerp(eyeObjs[0].eyeTransform.position, eyeObjs[1].eyeTransform.position, 0.5f);
                        GameCamera.transform.SetPositionAndRotation(pos, currentChara.objHeadBone.transform.rotation);
                        GameCamera.transform.Translate(Vector3.forward * ViewOffset.Value);
                        if (CurrentFOV == null) throw new InvalidOperationException("CurrentFOV == null");
                        GameCamera.fieldOfView = CurrentFOV.Value;

                        return false;
                    }
                }

                return true;
            }

            [HarmonyPostfix]
            [HarmonyPatch(typeof(HSceneProc), "ChangeAnimator")]
            [HarmonyPatch(typeof(HFlag), nameof(HFlag.selectAnimationListInfo), MethodType.Setter)]
            private static void ResetAllRotations()
            {
                LookRotation.Clear();
            }
        }

        private enum PovSex
        {
            Male,
            Female,
            Either
        }
    }
}
