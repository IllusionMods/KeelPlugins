using UnityEngine;

namespace LockOnPlugin.Koikatu
{
    internal class MakerMono : LockOnBase
    {
        private CameraControl_Ver2 camera = Singleton<CameraControl_Ver2>.Instance;

        protected override void Start()
        {
            base.Start();
            currentCharaInfo = FindObjectOfType<ChaInfo>();
            Guitime.pos = new Vector2(1f, 1f);
        }

        protected override void ResetModState()
        {
            base.ResetModState();
            currentCharaInfo = FindObjectOfType<ChaInfo>();
        }

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
    }
}
