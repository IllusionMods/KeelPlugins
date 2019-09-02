using UnityEngine;

namespace KeelPlugins
{
    internal partial class StudioMono : LockOnBase
    {
        protected override float CameraMoveSpeed
        {
            get { return camera.moveSpeed; }
            set { camera.moveSpeed = value; }
        }

        protected override Vector3 CameraTargetPos
        {
            get { return camera.targetPos; }
            set { camera.targetPos = value; }
        }

        protected override Vector3 CameraAngle
        {
            get { return camera.cameraAngle; }
            set { camera.cameraAngle = value; }
        }

        protected override float CameraFov
        {
            get { return camera.fieldOfView; }
            set { camera.fieldOfView = value; }
        }

        protected override Vector3 CameraDir
        {
            get { return cameraData.distance; }
            set { cameraData.distance = value; }
        }

        protected override bool CameraTargetTex
        {
            set { camera.isConfigTargetTex = value; }
        }

        protected override float CameraZoomSpeed
        {
            get { return defaultCameraSpeed * Studio.Studio.optionSystem.cameraSpeed; }
        }

        protected override Transform CameraTransform
        {
            get { return camera.transform; }
        }

        protected override bool AllowTracking
        {
            get { return !(guideObjectManager.isOperationTarget && guideObjectManager.mode == 1); }
        }

        protected override bool InputFieldSelected
        {
            get { return base.InputFieldSelected || studio.isInputNow || guideObjectManager.isOperationTarget; }
        }

        protected override bool CameraEnabled
        {
            get { return camera.enabled; }
        }
    }
}
