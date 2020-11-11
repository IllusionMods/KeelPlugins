using HarmonyLib;
using Studio;
using System;

namespace CharaStateX.Koikatu
{
    internal static class StateInfoPatch
    {
        private static Harmony harmony;
        private static Type stateInfoType;

        public static void Patch(Harmony harmonyInstance)
        {
            harmony = harmonyInstance;
            stateInfoType = typeof(MPCharCtrl).GetNestedType("StateInfo", AccessTools.all);
            PatchStateInfoMethod("OnClickCosType");
            PatchStateInfoMethod("OnClickShoesType");
            PatchStateInfoMethod("OnClickCosState");
            PatchStateInfoMethod("OnClickClothingDetails");
            PatchStateInfoMethod("OnClickAccessories");
            PatchStateInfoMethod("OnClickLiquid");
            PatchStateInfoMethod("OnClickTears");
            PatchStateInfoMethod("OnValueChangedCheek");
            PatchStateInfoMethod("OnValueChangedNipple");
            PatchStateInfoMethod("OnValueChangedSon");
            PatchStateInfoMethod("OnValueChangedSonLength");
        }

        private static void PatchStateInfoMethod(string targetName)
        {
            var target = AccessTools.Method(stateInfoType, targetName);
            var patch = AccessTools.Method(typeof(StateInfoPatch), $"Patch_{targetName}");
            harmony.Patch(target, null, new HarmonyMethod(patch));
        }

#pragma warning disable IDE0051 // Remove unused private members
        private static void Patch_OnClickCosType(object __instance, ref int _value)
        {
            if(Utils.GetIsUpdateInfo(__instance)) return;

            foreach(var chara in Utils.GetAllSelectedButMain(__instance))
                chara.SetCoordinateInfo((ChaFileDefine.CoordinateType)_value, false);
        }

        private static void Patch_OnClickShoesType(object __instance, ref int _value)
        {
            if(Utils.GetIsUpdateInfo(__instance)) return;

            foreach(var chara in Utils.GetAllSelectedButMain(__instance))
                chara.SetShoesType(_value);
        }

        private static void Patch_OnClickCosState(object __instance, ref int _value)
        {
            if(Utils.GetIsUpdateInfo(__instance)) return;

            foreach(var chara in Utils.GetAllSelectedButMain(__instance))
                chara.SetClothesStateAll(_value);
        }

        private static void Patch_OnClickClothingDetails(object __instance, ref int _id, ref byte _state)
        {
            if(Utils.GetIsUpdateInfo(__instance)) return;

            foreach(var chara in Utils.GetAllSelectedButMain(__instance))
                chara.SetClothesState(_id, _state);
        }

        private static void Patch_OnClickAccessories(object __instance, ref int _id, ref bool _flag)
        {
            if(Utils.GetIsUpdateInfo(__instance)) return;

            foreach(var chara in Utils.GetAllSelectedButMain(__instance))
                chara.ShowAccessory(_id, _flag);
        }

        private static void Patch_OnClickLiquid(object __instance, ref ChaFileDefine.SiruParts _parts, ref byte _state)
        {
            if(Utils.GetIsUpdateInfo(__instance)) return;

            foreach(var chara in Utils.GetAllSelectedButMain(__instance))
                chara.SetSiruFlags(_parts, _state);
        }

        private static void Patch_OnClickTears(object __instance, ref byte _state)
        {
            if(Utils.GetIsUpdateInfo(__instance)) return;

            foreach(var chara in Utils.GetAllSelectedButMain(__instance))
                chara.SetTearsLv(_state);
        }

        private static void Patch_OnValueChangedCheek(object __instance, ref float _value)
        {
            if(Utils.GetIsUpdateInfo(__instance)) return;

            foreach(var chara in Utils.GetAllSelectedButMain(__instance))
                chara.SetHohoAkaRate(_value);
        }

        private static void Patch_OnValueChangedNipple(object __instance, ref float _value)
        {
            if(Utils.GetIsUpdateInfo(__instance)) return;

            foreach(var chara in Utils.GetAllSelectedButMain(__instance))
                chara.SetNipStand(_value);
        }

        private static void Patch_OnValueChangedSon(object __instance, ref bool _value)
        {
            if(Utils.GetIsUpdateInfo(__instance)) return;

            foreach(var chara in Utils.GetAllSelectedButMain(__instance))
                chara.SetVisibleSon(_value);
        }

        private static void Patch_OnValueChangedSonLength(object __instance, ref float _value)
        {
            if(Utils.GetIsUpdateInfo(__instance)) return;

            foreach(var chara in Utils.GetAllSelectedButMain(__instance))
                chara.SetSonLength(_value);
        }
#pragma warning restore IDE0051 // Remove unused private members
    }
}
