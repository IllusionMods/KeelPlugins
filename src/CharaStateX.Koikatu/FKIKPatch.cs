using HarmonyLib;
using Studio;

namespace KeelPlugins
{
    internal static class FKIKPatch
    {
        public static void Patch(Harmony harmony)
        {
            {
                var type = typeof(MPCharCtrl).GetNestedType("FKInfo", AccessTools.all);
                var target = AccessTools.Method(type, "OnChangeValueFunction");
                var patch = AccessTools.Method(typeof(FKIKPatch), nameof(Patch_FKInfo_OnChangeValueFunction));
                harmony.Patch(target, null, new HarmonyMethod(patch));
            }

            {
                var type = typeof(MPCharCtrl).GetNestedType("IKInfo", AccessTools.all);
                var target = AccessTools.Method(type, "OnChangeValueFunction");
                var patch = AccessTools.Method(typeof(FKIKPatch), nameof(Patch_IKInfo_OnChangeValueFunction));
                harmony.Patch(target, null, new HarmonyMethod(patch));
            }
        }

        private static void Patch_FKInfo_OnChangeValueFunction(object __instance, ref bool _value)
        {
            if(Utils.GetIsUpdateInfo(__instance)) return;

            foreach(var chara in Utils.GetAllSelectedButMain(__instance))
                chara.ActiveKinematicMode(OICharInfo.KinematicMode.FK, _value, false);
        }

        private static void Patch_IKInfo_OnChangeValueFunction(object __instance, ref bool _value)
        {
            if(Utils.GetIsUpdateInfo(__instance)) return;

            foreach(var chara in Utils.GetAllSelectedButMain(__instance))
                chara.ActiveKinematicMode(OICharInfo.KinematicMode.IK, _value, false);
        }
    }
}
