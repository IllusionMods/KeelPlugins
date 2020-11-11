using ExtensibleSaveFormat;
using KKAPI;
using KKAPI.Chara;

namespace EyeLookEditor.Koikatu
{
    public class EyeLookCharaController : CharaCustomFunctionController
    {
        private EyeTypeState eyeTypeState;

        private float _thresholdAngleDifference;
        private float _bendingMultiplier;
        private float _maxAngleDifference;
        private float _upBendingAngle;
        private float _downBendingAngle;
        private float _minBendingAngle;
        private float _maxBendingAngle;
        private float _leapSpeed;
        private float _forntTagDis;
        private float _nearDis;
        private float _hAngleLimit;
        private float _vAngleLimit;

        public float ThresholdAngleDifference
        {
            get => _thresholdAngleDifference;
            set { eyeTypeState.thresholdAngleDifference = value; _thresholdAngleDifference = value; }
        }

        public float BendingMultiplier
        {
            get => _bendingMultiplier;
            set { eyeTypeState.bendingMultiplier = value; _bendingMultiplier = value; }
        }

        public float MaxAngleDifference
        {
            get => _maxAngleDifference;
            set { eyeTypeState.maxAngleDifference = value; _maxAngleDifference = value; }
        }

        public float UpBendingAngle
        {
            get => _upBendingAngle;
            set { eyeTypeState.upBendingAngle = value; _upBendingAngle = value; }
        }

        public float DownBendingAngle
        {
            get => _downBendingAngle;
            set { eyeTypeState.downBendingAngle = value; _downBendingAngle = value; }
        }

        public float MinBendingAngle
        {
            get => _minBendingAngle;
            set { eyeTypeState.minBendingAngle = value; _minBendingAngle = value; }
        }

        public float MaxBendingAngle
        {
            get => _maxBendingAngle;
            set { eyeTypeState.maxBendingAngle = value; _maxBendingAngle = value; }
        }

        public float LeapSpeed
        {
            get => _leapSpeed;
            set { eyeTypeState.leapSpeed = value; _leapSpeed = value; }
        }

        public float ForntTagDis
        {
            get => _forntTagDis;
            set { eyeTypeState.forntTagDis = value; _forntTagDis = value; }
        }

        public float NearDis
        {
            get => _nearDis;
            set { eyeTypeState.nearDis = value; _nearDis = value; }
        }

        public float HAngleLimit
        {
            get => _hAngleLimit;
            set { eyeTypeState.hAngleLimit = value; _hAngleLimit = value; }
        }

        public float VAngleLimit
        {
            get => _vAngleLimit;
            set { eyeTypeState.vAngleLimit = value; _vAngleLimit = value; }
        }

        protected override void OnCardBeingSaved(GameMode currentGameMode)
        {
            var pData = new PluginData();
            pData.data.Add(nameof(ThresholdAngleDifference), ThresholdAngleDifference);
            pData.data.Add(nameof(BendingMultiplier), BendingMultiplier);
            pData.data.Add(nameof(MaxAngleDifference), MaxAngleDifference);
            pData.data.Add(nameof(UpBendingAngle), UpBendingAngle);
            pData.data.Add(nameof(DownBendingAngle), DownBendingAngle);
            pData.data.Add(nameof(MinBendingAngle), MinBendingAngle);
            pData.data.Add(nameof(MaxBendingAngle), MaxBendingAngle);
            pData.data.Add(nameof(LeapSpeed), LeapSpeed);
            pData.data.Add(nameof(ForntTagDis), ForntTagDis);
            pData.data.Add(nameof(NearDis), NearDis);
            pData.data.Add(nameof(HAngleLimit), HAngleLimit);
            pData.data.Add(nameof(VAngleLimit), VAngleLimit);
            pData.version = 1;

            SetExtendedData(pData);
        }

        protected override void OnReload(GameMode currentGameMode)
        {
            eyeTypeState = ChaControl.eyeLookCtrl.eyeLookScript.eyeTypeStates[1];

            ThresholdAngleDifference = DefaultValue.ThresholdAngleDifference;
            BendingMultiplier = DefaultValue.MaxBendingAngle;
            MaxAngleDifference = DefaultValue.MaxAngleDifference;
            UpBendingAngle = DefaultValue.UpBendingAngle;
            DownBendingAngle = DefaultValue.DownBendingAngle;
            MinBendingAngle = DefaultValue.MinBendingAngle;
            MaxBendingAngle = DefaultValue.MaxBendingAngle;
            LeapSpeed = DefaultValue.LeapSpeed;
            ForntTagDis = DefaultValue.ForntTagDis;
            NearDis = DefaultValue.NearDis;
            HAngleLimit = DefaultValue.HAngleLimit;
            VAngleLimit = DefaultValue.VAngleLimit;

            var pData = GetExtendedData();
            if(pData != null)
            {
                { ThresholdAngleDifference = pData.data.TryGetValue(nameof(ThresholdAngleDifference), out var val) && val is float f ? f : DefaultValue.ThresholdAngleDifference; }
                { BendingMultiplier = pData.data.TryGetValue(nameof(BendingMultiplier), out var val) && val is float f ? f : DefaultValue.MaxBendingAngle; }
                { MaxAngleDifference = pData.data.TryGetValue(nameof(MaxAngleDifference), out var val) && val is float f ? f : DefaultValue.MaxAngleDifference; }
                { UpBendingAngle = pData.data.TryGetValue(nameof(UpBendingAngle), out var val) && val is float f ? f : DefaultValue.UpBendingAngle; }
                { DownBendingAngle = pData.data.TryGetValue(nameof(DownBendingAngle), out var val) && val is float f ? f : DefaultValue.DownBendingAngle; }
                { MinBendingAngle = pData.data.TryGetValue(nameof(MinBendingAngle), out var val) && val is float f ? f : DefaultValue.MinBendingAngle; }
                { MaxBendingAngle = pData.data.TryGetValue(nameof(MaxBendingAngle), out var val) && val is float f ? f : DefaultValue.MaxBendingAngle; }
                { LeapSpeed = pData.data.TryGetValue(nameof(LeapSpeed), out var val) && val is float f ? f : DefaultValue.LeapSpeed; }
                { ForntTagDis = pData.data.TryGetValue(nameof(ForntTagDis), out var val) && val is float f ? f : DefaultValue.ForntTagDis; }
                { NearDis = pData.data.TryGetValue(nameof(NearDis), out var val) && val is float f ? f : DefaultValue.NearDis; }
                { HAngleLimit = pData.data.TryGetValue(nameof(HAngleLimit), out var val) && val is float f ? f : DefaultValue.HAngleLimit; }
                { VAngleLimit = pData.data.TryGetValue(nameof(VAngleLimit), out var val) && val is float f ? f : DefaultValue.VAngleLimit; }
            }
        }
    }
}
