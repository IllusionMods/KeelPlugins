using BepInEx;
using KKAPI.Chara;
using KKAPI.Maker;
using KKAPI.Maker.UI;
using System;
using UnityEngine;

[assembly: System.Reflection.AssemblyVersion(EyeLookEditor.EyeLookEditor.Version)]

namespace EyeLookEditor
{
    [BepInPlugin(GUID, "EyeLookEditor", Version)]
    [BepInDependency(KKAPI.KoikatuAPI.GUID, KKAPI.KoikatuAPI.VersionConst)]
    public class EyeLookEditor : BaseUnityPlugin
    {
        public const string GUID = "keelhauled.eyelookeditor";
        public const string Version = "1.0.1." + BuildNumber.Version;

        private void Start()
        {
            Log.SetLogSource(Logger);
            MakerAPI.RegisterCustomSubCategories += RegisterCustomSubCategories;
            CharacterApi.RegisterExtraBehaviour<EyeLookCharaController>(GUID);
        }

        private void RegisterCustomSubCategories(object sender, RegisterSubCategoriesEvent e)
        {
            var irisCategory = MakerConstants.Face.Iris;
            var category = new MakerCategory(irisCategory.CategoryName, "eyelookeditor", irisCategory.Position+1, "Eye Look");
            e.AddSubCategory(category);

            var stringToValue = new Func<string, float>(float.Parse);
            var valueToString = new Func<float, string>(f => f.ToString("0.#"));
            var mouseScrollFunc = new Func<Vector2, float>(x => x.y > 0 ? -1 : 1);

            var slider_thresholdAngleDifference = e.AddControl(new MakerSlider(category, "thresholdAngleDifference", -360f, 360f, DefaultValue.ThresholdAngleDifference, this)
                { StringToValue = stringToValue, ValueToString = valueToString, WholeNumbers = true });
            var slider_bendingMultiplier = e.AddControl(new MakerSlider(category, "bendingMultiplier", -100f, 100f, DefaultValue.BendingMultiplier, this)
                { StringToValue = stringToValue, ValueToString = valueToString, WholeNumbers = true });
            var slider_maxAngleDifference = e.AddControl(new MakerSlider(category, "maxAngleDifference", -360f, 360f, DefaultValue.MaxAngleDifference, this)
                { StringToValue = stringToValue, ValueToString = valueToString, WholeNumbers = true });
            var slider_upBendingAngle = e.AddControl(new MakerSlider(category, "upBendingAngle", -360f, 360f, DefaultValue.UpBendingAngle, this)
                { StringToValue = stringToValue, ValueToString = valueToString, WholeNumbers = true });
            var slider_downBendingAngle = e.AddControl(new MakerSlider(category, "downBendingAngle", -360f, 360f, DefaultValue.DownBendingAngle, this)
                { StringToValue = stringToValue, ValueToString = valueToString, WholeNumbers = true });
            var slider_minBendingAngle = e.AddControl(new MakerSlider(category, "minBendingAngle", -360f, 360f, DefaultValue.MinBendingAngle, this)
                { StringToValue = stringToValue, ValueToString = valueToString, WholeNumbers = true });
            var slider_maxBendingAngle = e.AddControl(new MakerSlider(category, "maxBendingAngle", -360f, 360f, DefaultValue.MaxBendingAngle, this)
                { StringToValue = stringToValue, ValueToString = valueToString, WholeNumbers = true });
            var slider_leapSpeed = e.AddControl(new MakerSlider(category, "Tracking Speed", 0f, 50f, DefaultValue.LeapSpeed, this)
                { StringToValue = stringToValue, ValueToString = valueToString, WholeNumbers = true, MouseScrollValueChange = mouseScrollFunc });
            var slider_forntTagDis = e.AddControl(new MakerSlider(category, "forntTagDis", -100f, 100f, DefaultValue.ForntTagDis, this)
                { StringToValue = stringToValue, ValueToString = valueToString, WholeNumbers = true });
            var slider_nearDis = e.AddControl(new MakerSlider(category, "nearDis", -100f, 100f, DefaultValue.NearDis, this)
                { StringToValue = stringToValue, ValueToString = valueToString, WholeNumbers = true });
            var slider_hAngleLimit = e.AddControl(new MakerSlider(category, "Horizontal Tracking Limit", 0f, 360f, DefaultValue.HAngleLimit, this)
                { StringToValue = stringToValue, ValueToString = valueToString, WholeNumbers = true });
            var slider_vAngleLimit = e.AddControl(new MakerSlider(category, "Vertical Tracking Limit", 0f, 360f, DefaultValue.VAngleLimit, this)
                { StringToValue = stringToValue, ValueToString = valueToString, WholeNumbers = true });

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
