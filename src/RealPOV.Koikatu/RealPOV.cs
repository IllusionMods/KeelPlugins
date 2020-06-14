using BepInEx;
using HarmonyLib;
using Studio;
using System.Linq;
using UnityEngine;

namespace KeelPlugins
{
    [BepInPlugin(GUID, PluginName, Version)]
    public class RealPOV : RealPOVCore
    {
        private static int backupLayer;
        private static ChaControl currentChara;
        private bool isStudio = Paths.ProcessName == "CharaStudio";

        internal override void EnablePOV()
        {
            if(isStudio)
            {
                var selectedCharas = GuideObjectManager.Instance.selectObjectKey.Select(x => Studio.Studio.GetCtrlInfo(x) as OCIChar).Where(x => x != null).ToList();
                if(selectedCharas.Count > 0)
                    currentChara = selectedCharas.First().charInfo;
            }
            else
            {
                var cameraTarget = GameObject.Find("HScene/CameraBase/Camera/CameraTarget");
                if(cameraTarget)
                    currentChara = FindObjectsOfType<ChaControl>().OrderBy(x => Vector3.Distance(cameraTarget.transform.position, x.neckLookCtrl.transform.position)).First();
            }

            if(currentChara)
            {
                //foreach(var bone in currentChara.neckLookCtrl.neckLookScript.aBones)
                //    bone.neckBone.rotation = new Quaternion();

                GameCamera = Camera.main;

                base.EnablePOV();

                backupLayer = GameCamera.gameObject.layer;
                GameCamera.gameObject.layer = 0;
            }
        }

        internal override void DisablePOV()
        {
            base.DisablePOV();
            GameCamera.gameObject.layer = backupLayer;
        }

        [HarmonyPrefix, HarmonyPatch(typeof(NeckLookControllerVer2), "LateUpdate")]
        private static bool ApplyRotation(NeckLookControllerVer2 __instance)
        {
            if(POVEnabled)
            {
                if(__instance.neckLookScript && currentChara.neckLookCtrl == __instance)
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
