using HarmonyLib;
using Studio;
using System;

namespace KeelPlugins
{
    internal static class EtcInfoPatch
    {
        private static Harmony harmony;
        private static Type etcInfoType;

        public static void Patch(Harmony harmonyInstance)
        {
            harmony = harmonyInstance;
            etcInfoType = typeof(MPCharCtrl).GetNestedType("EtcInfo", AccessTools.all);
            PatchEtcInfoMethod("ChangeEyebrowsPtn");
            PatchEtcInfoMethod("OnValueChangedForegroundEyebrow");
            PatchEtcInfoMethod("ChangeEyesPtn");
            PatchEtcInfoMethod("OnValueChangedEyesOpen");
            PatchEtcInfoMethod("OnValueChangedEyesBlink");
            PatchEtcInfoMethod("OnValueChangedForegroundEyes");
            PatchEtcInfoMethod("ChangeMouthPtn");
            PatchEtcInfoMethod("OnValueChangedMouthOpen");
            PatchEtcInfoMethod("OnValueChangedLipSync");
        }

        private static void PatchEtcInfoMethod(string targetName)
        {
            var target = AccessTools.Method(etcInfoType, targetName);
            var patch = AccessTools.Method(typeof(EtcInfoPatch), $"Patch_{targetName}");
            harmony.Patch(target, null, new HarmonyMethod(patch));
        }

        private static void Patch_ChangeEyebrowsPtn(object __instance, ref int _no)
        {
            if(Utils.GetIsUpdateInfo(__instance)) return;

            foreach(var chara in Utils.GetAllSelectedButMain(__instance))
                chara.charInfo.ChangeEyebrowPtn(_no, true);
        }

        private static void Patch_OnValueChangedForegroundEyebrow(object __instance, ref int _value)
        {
            if(Utils.GetIsUpdateInfo(__instance)) return;

            foreach(var chara in Utils.GetAllSelectedButMain(__instance))
                chara.foregroundEyebrow = (byte)_value;
        }

        private static void Patch_ChangeEyesPtn(object __instance, ref int _no)
        {
            if(Utils.GetIsUpdateInfo(__instance)) return;

            foreach(var chara in Utils.GetAllSelectedButMain(__instance))
                chara.charInfo.ChangeEyesPtn(_no, true);
        }

        private static void Patch_OnValueChangedEyesOpen(object __instance, ref float _value)
        {
            if(Utils.GetIsUpdateInfo(__instance)) return;

            foreach(var chara in Utils.GetAllSelectedButMain(__instance))
                chara.ChangeEyesOpen(_value);
        }

        private static void Patch_OnValueChangedEyesBlink(object __instance, ref bool _value)
        {
            if(Utils.GetIsUpdateInfo(__instance)) return;

            foreach(var chara in Utils.GetAllSelectedButMain(__instance))
                chara.ChangeBlink(_value);
        }

        private static void Patch_OnValueChangedForegroundEyes(object __instance, ref int _value)
        {
            if(Utils.GetIsUpdateInfo(__instance)) return;

            foreach(var chara in Utils.GetAllSelectedButMain(__instance))
                chara.foregroundEyes = (byte)_value;
        }

        private static void Patch_ChangeMouthPtn(object __instance, ref int _no)
        {
            if(Utils.GetIsUpdateInfo(__instance)) return;

            foreach(var chara in Utils.GetAllSelectedButMain(__instance))
                chara.charInfo.ChangeMouthPtn(_no, true);
        }

        private static void Patch_OnValueChangedMouthOpen(object __instance, ref float _value)
        {
            if(Utils.GetIsUpdateInfo(__instance)) return;

            foreach(var chara in Utils.GetAllSelectedButMain(__instance))
                chara.ChangeMouthOpen(_value);
        }

        private static void Patch_OnValueChangedLipSync(object __instance, ref bool _value)
        {
            if(Utils.GetIsUpdateInfo(__instance)) return;

            foreach(var chara in Utils.GetAllSelectedButMain(__instance))
                chara.ChangeLipSync(_value);
        }
    }
}
