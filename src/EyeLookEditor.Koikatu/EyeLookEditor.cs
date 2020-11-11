using BepInEx;
using KKAPI.Chara;
using KKAPI.Maker;
using KKAPI.Maker.UI;
using System;

namespace EyeLookEditor.Koikatu
{
    [BepInPlugin(GUID, "EyeLookEditor", Version)]
    public class EyeLookEditor : BaseUnityPlugin
    {
        public const string GUID = "keelhauled.eyelookeditor";
        public const string Version = "1.0.0." + BuildNumber.Version;

        private void Start()
        {
            MakerAPI.RegisterCustomSubCategories += RegisterCustomSubCategories;
            CharacterApi.RegisterExtraBehaviour<EyeLookCharaController>(GUID);
        }

        private void RegisterCustomSubCategories(object sender, RegisterSubCategoriesEvent e)
        {
            var irisCategory = MakerConstants.Face.Iris;

            var category = new MakerCategory(irisCategory.CategoryName, "eyelookeditor", irisCategory.Position, "EyeLookEditor");
            e.AddSubCategory(category);

            var stringToValue = new Func<string, float>(txt => float.Parse(txt));
            var valueToString = new Func<float, string>(f => f.ToString("0.#"));

            var slider_thresholdAngleDifference = e.AddControl(new MakerSlider(category, "thresholdAngleDifference", -10f, 10f, DefaultValue.ThresholdAngleDifference, this) { StringToValue = stringToValue, ValueToString = valueToString });
            var slider_bendingMultiplier = e.AddControl(new MakerSlider(category, "bendingMultiplier", 0f, 10f, DefaultValue.BendingMultiplier, this) { StringToValue = stringToValue, ValueToString = valueToString });
            var slider_maxAngleDifference = e.AddControl(new MakerSlider(category, "maxAngleDifference", 0f, 100f, DefaultValue.MaxAngleDifference, this) { StringToValue = stringToValue, ValueToString = valueToString });
            var slider_upBendingAngle = e.AddControl(new MakerSlider(category, "upBendingAngle", -100f, 0f, DefaultValue.UpBendingAngle, this) { StringToValue = stringToValue, ValueToString = valueToString });
            var slider_downBendingAngle = e.AddControl(new MakerSlider(category, "downBendingAngle", 0f, 100f, DefaultValue.DownBendingAngle, this) { StringToValue = stringToValue, ValueToString = valueToString });
            var slider_minBendingAngle = e.AddControl(new MakerSlider(category, "minBendingAngle", -100f, 0f, DefaultValue.MinBendingAngle, this) { StringToValue = stringToValue, ValueToString = valueToString });
            var slider_maxBendingAngle = e.AddControl(new MakerSlider(category, "maxBendingAngle", 0f, 100f, DefaultValue.MaxBendingAngle, this) { StringToValue = stringToValue, ValueToString = valueToString });
            var slider_leapSpeed = e.AddControl(new MakerSlider(category, "leapSpeed", 0f, 100f, DefaultValue.LeapSpeed, this) { StringToValue = stringToValue, ValueToString = valueToString });
            var slider_forntTagDis = e.AddControl(new MakerSlider(category, "forntTagDis", 0f, 100f, DefaultValue.ForntTagDis, this) { StringToValue = stringToValue, ValueToString = valueToString });
            var slider_nearDis = e.AddControl(new MakerSlider(category, "nearDis", 0f, 100f, DefaultValue.NearDis, this) { StringToValue = stringToValue, ValueToString = valueToString });
            var slider_hAngleLimit = e.AddControl(new MakerSlider(category, "hAngleLimit", 0f, 180f, DefaultValue.HAngleLimit, this) { StringToValue = stringToValue, ValueToString = valueToString });
            var slider_vAngleLimit = e.AddControl(new MakerSlider(category, "vAngleLimit", 0f, 180f, DefaultValue.VAngleLimit, this) { StringToValue = stringToValue, ValueToString = valueToString });

            slider_thresholdAngleDifference.BindToFunctionController<EyeLookCharaController, float>(ctrl => ctrl.ThresholdAngleDifference, (ctrl, f) => ctrl.ThresholdAngleDifference = f);
            slider_bendingMultiplier.BindToFunctionController<EyeLookCharaController, float>(ctrl => ctrl.BendingMultiplier, (ctrl, f) => ctrl.BendingMultiplier = f);
            slider_maxAngleDifference.BindToFunctionController<EyeLookCharaController, float>(ctrl => ctrl.MaxAngleDifference, (ctrl, f) => ctrl.MaxAngleDifference = f);
            slider_upBendingAngle.BindToFunctionController<EyeLookCharaController, float>(ctrl => ctrl.UpBendingAngle, (ctrl, f) => ctrl.UpBendingAngle = f);
            slider_downBendingAngle.BindToFunctionController<EyeLookCharaController, float>(ctrl => ctrl.DownBendingAngle, (ctrl, f) => ctrl.DownBendingAngle = f);
            slider_minBendingAngle.BindToFunctionController<EyeLookCharaController, float>(ctrl => ctrl.MinBendingAngle, (ctrl, f) => ctrl.MinBendingAngle = f);
            slider_maxBendingAngle.BindToFunctionController<EyeLookCharaController, float>(ctrl => ctrl.MaxBendingAngle, (ctrl, f) => ctrl.MaxBendingAngle = f);
            slider_leapSpeed.BindToFunctionController<EyeLookCharaController, float>(ctrl => ctrl.LeapSpeed, (ctrl, f) => ctrl.LeapSpeed = f);
            slider_forntTagDis.BindToFunctionController<EyeLookCharaController, float>(ctrl => ctrl.ForntTagDis, (ctrl, f) => ctrl.ForntTagDis = f);
            slider_nearDis.BindToFunctionController<EyeLookCharaController, float>(ctrl => ctrl.NearDis, (ctrl, f) => ctrl.NearDis = f);
            slider_hAngleLimit.BindToFunctionController<EyeLookCharaController, float>(ctrl => ctrl.HAngleLimit, (ctrl, f) => ctrl.HAngleLimit = f);
            slider_vAngleLimit.BindToFunctionController<EyeLookCharaController, float>(ctrl => ctrl.VAngleLimit, (ctrl, f) => ctrl.VAngleLimit = f);
        }
    }
}
