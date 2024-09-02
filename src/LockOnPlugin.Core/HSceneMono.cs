using IllusionUtility.GetUtility;
using UnityEngine;

namespace LockOnPlugin
{
    internal class HSceneMono : LockOnBase
    {
        private CameraControl_Ver2 camera = Singleton<CameraControl_Ver2>.Instance;

        protected override float CameraMoveSpeed
        {
            get => camera.moveSpeed;
            set => camera.moveSpeed = value;
        }

        protected override Vector3 CameraTargetPos
        {
            get => camera.TargetPos;
            set => camera.TargetPos = value;
        }

        protected override Vector3 CameraAngle
        {
            get => camera.CameraAngle;
            set => camera.CameraAngle = value;
        }

        protected override float CameraFov
        {
            get => camera.CameraFov;
            set => camera.CameraFov = value;
        }

        protected override Vector3 CameraDir
        {
            get => camera.CameraDir;
            set => camera.CameraDir = value;
        }

        protected override bool CameraTargetTex
        {
            set => camera.isConfigTargetTex = value;
        }

        protected override float CameraZoomSpeed => defaultCameraSpeed;
        protected override Transform CameraTransform => camera.transform;
        protected override bool AllowTracking => true;
        protected override bool InputFieldSelected => base.InputFieldSelected;
        protected override bool CameraEnabled => camera.enabled;
        protected override Vector3 CameraForward => camera.transBase.InverseTransformDirection(camera.transform.forward);
        protected override Vector3 CameraRight => camera.transBase.InverseTransformDirection(camera.transform.right);
        protected override Vector3 CameraUp => camera.transBase.InverseTransformDirection(camera.transform.up);
        protected override Vector3 LockOnTargetPos => camera.transBase.InverseTransformPoint(lockOnTarget.transform.position);

        private ChaInfo GetClosestChara()
        {
            ChaInfo closestChara = null;
            float smallestMagnitude = 0f;

            foreach(var chara in FindObjectsOfType<ChaControl>())
            {
                float magnitude = 0f;
                foreach(var targetname in TargetData.data.presenceTargets)
                {
                    var target = chara.objBodyBone.transform.FindLoop(targetname);
                    float distance = Vector3.Distance(camera.TargetPos, camera.transBase.InverseTransformPoint(target.transform.position));
                    magnitude += distance;
                }

                if(!closestChara)
                {
                    closestChara = chara;
                    smallestMagnitude = magnitude;
                }
                else
                {
                    if(magnitude < smallestMagnitude)
                    {
                        closestChara = chara;
                        smallestMagnitude = magnitude;
                    }
                }
            }

            return closestChara;
        }

        protected override bool LockOn()
        {
            if(!lockedOn) currentCharaInfo = GetClosestChara();
            return base.LockOn();
        }
    }
}
