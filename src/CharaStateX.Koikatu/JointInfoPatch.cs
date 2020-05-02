using HarmonyLib;
using Studio;
using System;
using System.Linq;
using UnityEngine.UI;

namespace KeelPlugins
{
    internal static class JointInfoPatch
    {
        public static void Patch(Harmony harmony)
        {
            var jointInfoType = typeof(MPCharCtrl).GetNestedType("JointInfo", AccessTools.all);
            var target = AccessTools.Method(jointInfoType, "OnValueChanged");
            var patch = AccessTools.Method(typeof(JointInfoPatch), nameof(JointInfoPostfix));
            harmony.Patch(target, null, new HarmonyMethod(patch));

            PatchKKPE(harmony);
        }

        private static void JointInfoPostfix(object __instance, ref int _group, ref bool _value)
        {
            if(Utils.GetIsUpdateInfo(__instance)) return;

            foreach(var chara in Utils.GetAllSelectedButMain(__instance))
                chara.EnableExpressionCategory(_group, _value);
        }

        private static void PatchKKPE(Harmony harmony)
        {
            var ass = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault((x) => x.FullName.Contains("KKPE,"));

            if(ass != null)
            {
                var type = ass.GetType("HSPE.MainWindow");
                var target = AccessTools.Method(type, "SpawnGUI");
                var patch = AccessTools.Method(typeof(JointInfoPatch), nameof(KKPESpawnGUIPostfix));
                harmony.Patch(target, null, new HarmonyMethod(patch));
            }
        }

        private static void KKPESpawnGUIPostfix(object __instance)
        {
            var traverse = Traverse.Create(__instance);
            var _crotchCorrectionToggle = traverse.Field("_crotchCorrectionToggle").GetValue<Toggle>();
            var _leftFootCorrectionToggle = traverse.Field("_leftFootCorrectionToggle").GetValue<Toggle>();
            var _rightFootCorrectionToggle = traverse.Field("_rightFootCorrectionToggle").GetValue<Toggle>();

            _crotchCorrectionToggle.onValueChanged.AddListener((x) =>
            {
                try
                {
                    foreach (var chara in Utils.GetSelectedCharacters())
                    {
                        var poseTarget = chara.charInfo.gameObject.GetComponent("CharaPoseController")
                                         ?? throw new ArgumentNullException("Could not find CharaPoseController");
                        var propertyCrotch = AccessTools.Property(poseTarget.GetType(), "crotchJointCorrection")
                                             ?? throw new ArgumentNullException("Could not find property CharaPoseController.crotchJointCorrection");
                        propertyCrotch.SetValue(poseTarget, x, null);
                    }
                }
                catch (Exception ex)
                {
                    UnityEngine.Debug.LogError(ex);
                }
            });

            _leftFootCorrectionToggle.onValueChanged.AddListener((x) =>
            {
                try
                {
                    foreach (var chara in Utils.GetSelectedCharacters())
                    {
                        var poseTarget = chara.charInfo.gameObject.GetComponent("CharaPoseController")
                                         ?? throw new ArgumentNullException("Could not find CharaPoseController");
                        var propertyLeft = AccessTools.Property(poseTarget.GetType(), "leftFootJointCorrection")
                                           ?? throw new ArgumentNullException("Could not find property CharaPoseController.leftFootJointCorrection");
                        propertyLeft.SetValue(poseTarget, x, null);
                    }
                }
                catch (Exception ex)
                {
                    UnityEngine.Debug.LogError(ex);
                }
            });

            _rightFootCorrectionToggle.onValueChanged.AddListener((x) =>
            {
                try
                {
                    foreach (var chara in Utils.GetSelectedCharacters())
                    {
                        var poseTarget = chara.charInfo.gameObject.GetComponent("CharaPoseController")
                                         ?? throw new ArgumentNullException("Could not find CharaPoseController");
                        var propertyRight = AccessTools.Property(poseTarget.GetType(), "rightFootJointCorrection")
                                            ?? throw new ArgumentNullException(
                                                "Could not find property CharaPoseController.rightFootJointCorrection");
                        propertyRight.SetValue(poseTarget, x, null);
                    }
                }
                catch (Exception ex)
                {
                    UnityEngine.Debug.LogError(ex);
                }
            });
        }
    }
}
