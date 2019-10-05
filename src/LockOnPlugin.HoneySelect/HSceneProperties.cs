using UnityEngine;

namespace LockOnPlugin
{
    internal partial class HSceneMono : LockOnBase
    {
        protected override float CameraMoveSpeed
        {
            get { return camera.moveSpeed; }
            set { camera.moveSpeed = value; }
        }

        protected override Vector3 CameraTargetPos
        {
            get { return camera.TargetPos; }
            set { camera.TargetPos = value; }
        }

        protected override Vector3 LockOnTargetPos
        {
            get { return camera.transBase.InverseTransformPoint(lockOnTarget.transform.position); }
        }

        protected override Vector3 CameraAngle
        {
            get { return camera.CameraAngle; }
            set { camera.CameraAngle = value; }
        }

        protected override float CameraFov
        {
            get { return camera.CameraFov; }
            set { camera.CameraFov = value; }
        }

        protected override Vector3 CameraDir
        {
            get { return camera.CameraDir; }
            set { camera.CameraDir = value; }
        }

        protected override bool CameraTargetTex
        {
            set { camera.isOutsideTargetTex = value; }
        }

        protected override float CameraZoomSpeed
        {
            get { return defaultCameraSpeed; }
        }

        protected override Transform CameraTransform
        {
            get { return camera.transform; }
        }

        protected override Vector3 CameraForward
        {
            get { return camera.transBase.InverseTransformDirection(camera.transform.forward); }
        }

        protected override Vector3 CameraRight
        {
            get { return camera.transBase.InverseTransformDirection(camera.transform.right); }
        }

        protected override Vector3 CameraUp
        {
            get { return camera.transBase.InverseTransformDirection(camera.transform.up); }
        }
    }
}
