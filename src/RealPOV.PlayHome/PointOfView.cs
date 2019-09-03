using BepInEx.Logging;
using HarmonyLib;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace KeelPlugins
{
    internal class PointOfView : MonoBehaviour
    {
        private static NeckRotator neckRotator;
        private static Dictionary<NeckMode, NeckRotator> neckRotators = new Dictionary<NeckMode, NeckRotator>
        {
            { NeckMode.Both, new NeckRotator(NeckMode.Both) },
            { NeckMode.First, new NeckRotator(NeckMode.First) },
            { NeckMode.Second, new NeckRotator(NeckMode.Second) }
        };

        private static bool lockNormalCamera;
        private static Human currentTarget;
        private static Female female;
        private static Male male;

        private bool OnUI => EventSystem.current && EventSystem.current.IsPointerOverGameObject();
        private bool dragging = false;

        private Vector3 FemaleOffset => new Vector3(RealPOV.FemaleOffsetX.Value, RealPOV.FemaleOffsetY.Value, RealPOV.FemaleOffsetZ.Value);
        private Vector3 MaleOffset => new Vector3(RealPOV.MaleOffsetX.Value, RealPOV.MaleOffsetY.Value, RealPOV.MaleOffsetZ.Value);

        private float fovBackup;
        private LookAtRotator.TYPE neckBackup;
        private LookAtRotator.TYPE femEyeBackup;

        private void Awake()
        {
            neckRotator = neckRotators[RealPOV.DefaultNeckMode.Value];
        }

        private void OnDestroy()
        {
            if(lockNormalCamera)
                Restore();
        }

        private void Update()
        {
            if(lockNormalCamera) RealPOV.POVHotkey.KeyHoldAction(Restore);
            RealPOV.POVHotkey.KeyUpAction(SetPOV);

            if(lockNormalCamera)
            {
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
                male = FindObjectOfType<Male>();
                currentTarget = male;

                if(currentTarget)
                {
                    neckBackup = currentTarget.NeckLook.CalcType;
                    currentTarget.NeckLook.Change(LookAtRotator.TYPE.NO, Camera.main.transform, true);
                    fovBackup = Camera.main.fieldOfView;
                    lockNormalCamera = true;

                    if(female)
                    {
                        femEyeBackup = female.EyeLook.CalcType;
                        female.EyeLook.Change(LookAtRotator.TYPE.TARGET, Camera.main.transform, true);
                    }
                }
            }
            else
            {
                switch(neckRotator.mode)
                {
                    case NeckMode.Both:
                        neckRotator = neckRotators[NeckMode.First];
                        break;
                    case NeckMode.First:
                        neckRotator = neckRotators[NeckMode.Second];
                        break;
                    case NeckMode.Second:
                        neckRotator = neckRotators[NeckMode.Both];
                        break;
                }

                RealPOV.Logger.Log(LogLevel.Message, neckRotator.mode);
            }
        }

        private void Restore()
        {
            if(currentTarget)
            {
                currentTarget.NeckLook.ChangePtn(neckBackup);
                currentTarget = null;

                if(female)
                    female.EyeLook.ChangePtn(femEyeBackup);
            }

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
            Camera.main.fieldOfView = RealPOV.DefaultFOV.Value;

            if(!OnUI && (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(2)))
                dragging = true;
            else if(!Input.GetMouseButton(0) && !Input.GetMouseButton(1) && !Input.GetMouseButton(2))
                dragging = false;

            if(dragging)
            {
                if(ConfigData.dragLock)
                    GameCursor.Lock();

                float x = Input.GetAxis("Mouse X") * RealPOV.MouseSensitivity.Value;
                float y = Input.GetAxis("Mouse Y") * RealPOV.MouseSensitivity.Value;

                if(Input.GetMouseButton(0))
                {
                    switch(neckRotator.mode)
                    {
                        case NeckMode.Both:
                            var change = new Vector3(-y, x, 0f) / 2f;
                            neckRotator.first += change;
                            neckRotator.second += change;
                            break;
                        case NeckMode.First:
                            neckRotator.first += new Vector3(-y, x, 0f);
                            break;
                        case NeckMode.Second:
                            neckRotator.second += new Vector3(-y, x, 0f);
                            break;
                    }
                }
                else if(Input.GetMouseButton(1))
                {
                    if(Input.GetKey(KeyCode.LeftShift))
                    {
                        var change = new Vector3(0f, 0f, -x);
                        neckRotator.first += change;
                        neckRotator.second += change;
                    }
                    else
                    {
                        Camera.main.fieldOfView += x;
                        RealPOV.DefaultFOV.Value = Camera.main.fieldOfView;
                    }
                }
            }
        }

        [HarmonyPrefix, HarmonyPatch(typeof(IllusionCamera), "LateUpdate")]
        public static bool IllusionCameraHook()
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
                    Traverse.Create(rotateBones[0]).Field("bone").GetValue<Transform>().Rotate(neckRotator.first);
                    Traverse.Create(rotateBones[1]).Field("bone").GetValue<Transform>().Rotate(neckRotator.second);
                }
            }
        }
    }
}
