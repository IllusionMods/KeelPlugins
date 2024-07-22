using HarmonyLib;
using RealPOV.Core;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using Studio;
using System.Collections.Generic;

namespace RealPOV.PlayHome
{
    internal class PointOfView : MonoBehaviour
    {
        private static bool lockNormalCamera;
        private static Human currentTarget;
        private static Female female;
        private static List<Human> targets;
        private static int currentTargetIndex = 0;

        private bool OnUI => EventSystem.current && EventSystem.current.IsPointerOverGameObject();
        private bool dragging;

        private Vector3 FemaleOffset => new Vector3(RealPOV.FemaleOffsetX.Value, RealPOV.FemaleOffsetY.Value, RealPOV.FemaleOffsetZ.Value);
        private Vector3 MaleOffset => new Vector3(RealPOV.MaleOffsetX.Value, RealPOV.MaleOffsetY.Value, RealPOV.MaleOffsetZ.Value);

        private float fovBackup;
        private float nearClipBackup;
        private LookAtRotator.TYPE neckBackup;
        private LookAtRotator.TYPE femEyeBackup;

        private static Vector3 rotation;

        private Harmony harmony;

        private void Awake()
        {
            targets = new List<Human>();
            harmony = Harmony.CreateAndPatchAll(GetType());
        }

        private void OnDestroy()
        {
            harmony.UnpatchSelf();

            if(lockNormalCamera)
                Restore();
        }

        private void Update()
        {
            if(RealPOVCore.POVHotkey.Value.IsDown())
            {
                if(lockNormalCamera)
                    Restore();
                else
                    SetPOV();
            }

            if(lockNormalCamera)
            {
                if(RealPOV.CycleNextHotkey.Value.IsDown())
                {
                    Restore();
                    currentTargetIndex++;
                    if(currentTargetIndex >= targets.Count) {
                        currentTargetIndex = 0;
                    }
                    SetPOV();
                }
                else if(RealPOV.CyclePrevHotkey.Value.IsDown())
                {
                    Restore();
                    currentTargetIndex--;
                    if(currentTargetIndex < 0) {
                        currentTargetIndex = targets.Count - 1;
                    }
                    SetPOV();
                }

                if(currentTarget)
                    UpdateCamera();
                else
                    Restore();
            }
        }

        private void SetPOV()
        {
            if(!lockNormalCamera)
            {
                female = FindObjectOfType<Female>();
                
                targets.Clear();
                Male[] males = FindObjectsOfType<Male>();
                if(RealPOV.IncludeFemalePOV.Value)
                {
                    Female[] females = FindObjectsOfType<Female>();
                    if(females != null && females.Length > 0)
                    {
                        targets.AddRange(females);
                        targets.AddRange(males);
                    }
                }
                else
                {
                    targets.AddRange(males);
                }

                if(targets.Count != 0)
                {
                    if(currentTargetIndex > targets.Count - 1)
                    {
                        currentTargetIndex = 0;
                    }
                    currentTarget = targets[currentTargetIndex];
                }
                else
                {
                    Log.Info("Unable to enter POV, no targets available.");
                }

                if(currentTarget)
                {
                    neckBackup = currentTarget.NeckLook.CalcType;
                    currentTarget.NeckLook.Change(LookAtRotator.TYPE.NO, Camera.main.transform, true);
                    fovBackup = Camera.main.fieldOfView;
                    nearClipBackup = Camera.main.nearClipPlane;
                    lockNormalCamera = true;

                    if(female)
                    {
                        femEyeBackup = female.EyeLook.CalcType;
                        female.EyeLook.Change(LookAtRotator.TYPE.TARGET, Camera.main.transform, true);
                    }
                }
            }
        }

        private void Restore()
        {
            if(currentTarget)
            {
                rotation.Set(0, 0, 0);
                currentTarget.NeckLook.ChangePtn(neckBackup);
                currentTarget = null;

                if(female)
                    female.EyeLook.ChangePtn(femEyeBackup);
            }

            Camera.main.nearClipPlane = nearClipBackup;
            Camera.main.fieldOfView = fovBackup;
            lockNormalCamera = false;
        }

        private void UpdateCamera()
        {
            var leftEye = currentTarget.head.Rend_eye_L.transform.position;
            var rightEye = currentTarget.head.Rend_eye_R.transform.position;
            Camera.main.transform.position = Vector3.Lerp(leftEye, rightEye, 0.5f);
            var offset = currentTarget.sex == Character.SEX.MALE ? MaleOffset : FemaleOffset;
            Camera.main.transform.Translate(offset);
            Camera.main.transform.rotation = currentTarget.head.Rend_eye_L.transform.rotation;
            Camera.main.fieldOfView = RealPOVCore.DefaultFOV.Value;
            Camera.main.nearClipPlane = RealPOV.DefaultNearClip.Value;

            if(!OnUI && (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(2)))
                dragging = true;
            else if(!Input.GetMouseButton(0) && !Input.GetMouseButton(1) && !Input.GetMouseButton(2))
                dragging = false;

            if(dragging)
            {
                if(ConfigData.dragLock)
                    GameCursor.Lock();

                float x = Input.GetAxis("Mouse X") * RealPOVCore.MouseSens.Value;
                float y = Input.GetAxis("Mouse Y") * RealPOVCore.MouseSens.Value;

                if(Input.GetMouseButton(0))
                {
                    rotation += new Vector3(-y, x, 0f);
                }
                else if(Input.GetMouseButton(1))
                {
                    if(Input.GetKey(KeyCode.LeftShift))
                    {
                        rotation += new Vector3(0f, 0f, -x);
                    }
                    else
                    {
                        Camera.main.fieldOfView += x;
                        RealPOVCore.DefaultFOV.Value = Camera.main.fieldOfView;
                    }
                }
            }
        }

        [HarmonyPrefix, HarmonyPatch(typeof(IllusionCamera), "LateUpdate")]
        public static bool IllusionCameraHook()
        {
            return !lockNormalCamera;
        }

        [HarmonyPrefix, HarmonyPatch(typeof(CameraControl), "LateUpdate")]
        public static bool StudioCameraHook()
        {
            return !lockNormalCamera;
        }

        [HarmonyPrefix, HarmonyPatch(typeof(LookAtRotator), "Calc")]
        public static void LookAtRotatorHook(LookAtRotator __instance)
        {
            if(lockNormalCamera && currentTarget)
            {
                if(currentTarget.NeckLook == __instance && __instance.CalcType == LookAtRotator.TYPE.NO)
                {
                    var rotateBones = Traverse.Create(__instance).Field("rotateBones").GetValue<IList>();
                    Traverse.Create(rotateBones[1]).Field("bone").GetValue<Transform>().Rotate(rotation);
                }
            }
        }
    }
}
