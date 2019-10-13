using BepInEx;
using BepInEx.Harmony;
using H;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace KeelPlugins
{
    [BepInProcess(PlayHomeConstants.MainGameProcessName32bit)]
    [BepInProcess(PlayHomeConstants.MainGameProcessName64bit)]
    [BepInPlugin(GUID, "Alternative Automode", Version)]
    public class AltAutoMode : BaseUnityPlugin
    {
        public const string GUID = "keelhauled.altautomode";
        public const string Version = "1.0.0";

        private Harmony harmony;

        private void Awake()
        {
            harmony = HarmonyWrapper.PatchAll(typeof(Hooks));
        }

#if DEBUG
        private void OnDestroy()
        {
            harmony.UnpatchAll();
        }
#endif

        private class Hooks
        {
            private static float sliderVal;
            private static Slider slider;
            private static float time = 0f;
            private static float random = 0f;

            [HarmonyPrefix, HarmonyPatch(typeof(MixController), "Update")]
            public static void SaveSliderVal(MixController __instance)
            {
                time += Time.deltaTime;

                if(time > random)
                {
                    time = 0f;
                    var autoChangeSpeed = Traverse.Create(__instance).Field("autoChangeSpeed");
                    var speed = autoChangeSpeed.GetValue<float>();

                    if(speed == 0.1f)
                    {
                        autoChangeSpeed.SetValue(0.7f);
                        random = Random.Range(2f, 2.5f);
                    }
                    else
                    {
                        autoChangeSpeed.SetValue(0.1f);
                        random = Random.Range(1f, 10f);
                    }
                }

                if(!slider)
                    slider = Traverse.Create(__instance).Field("slider").GetValue<Slider>();

                if(slider)
                    sliderVal = slider.value;
            }

            [HarmonyPostfix, HarmonyPatch(typeof(MixController), "Update")]
            public static void SetSliderVal(MixController __instance)
            {
                if(__instance.mode == MixController.MODE.FULL_AUTO && slider)
                {
                    if(EventSystem.current && EventSystem.current.IsPointerOverGameObject())
                    {
                        slider.value = sliderVal;
                    }
                    else
                    {
                        var wheelSpeed = Traverse.Create(__instance).Field("wheelSpeed").GetValue<float>();
                        slider.value = sliderVal + (Input.mouseScrollDelta.y * wheelSpeed);
                    }
                }
            }

            [HarmonyPostfix, HarmonyPatch(typeof(MixController), "ChangeMode")]
            public static void KeepSlider(MixController __instance)
            {
                var sliderRoot = Traverse.Create(__instance).Field("sliderRoot").GetValue<GameObject>();
                sliderRoot.SetActive(true);
            }

            [HarmonyPrefix, HarmonyPatch(typeof(AutoMode), "NextPos2")]
            public static void AdjustYLimit(AutoMode __instance, ref float gage)
            {
                gage = 0f;
                var instance = Traverse.Create(__instance);
                instance.Field("strokeMin").SetValue(-1f);
                instance.Field("strokeMax").SetValue(1f);
            }

            [HarmonyTranspiler, HarmonyPatch(typeof(MixController), "OnScroll")]
            public static IEnumerable<CodeInstruction> AllowScrolling(IEnumerable<CodeInstruction> instructions)
            {
                var codes = instructions.ToList();

                for(int i = 0; i < codes.Count; i++)
                    if(i >= 0 && i <= 2)
                        codes[i] = new CodeInstruction(OpCodes.Nop);

                return codes;
            }

            [HarmonyPrefix, HarmonyPatch(typeof(H_Members), "SetH_Pos")]
            public static bool StopMovement()
            {
                return Input.GetKey(KeyCode.LeftShift) ? true : false;
            }

            [HarmonyPrefix, HarmonyPatch(typeof(HStyleChangeUI), "StyleCheck")]
            public static bool UnlockPositions(HStyleChangeUI __instance, ref bool __result, H_StyleData data)
            {
                __result = data.state == Traverse.Create(__instance).Field("nowState").GetValue<H_StyleData.STATE>();
                return false;
            }

            [HarmonyPostfix, HarmonyPatch(typeof(H_ExpressionData), nameof(H_ExpressionData.ChangeExpression), new Type[] { typeof(Human), typeof(H_Expression.TYPE), typeof(H_Parameter), typeof(float) })]
            public static void OpenEyes(Human human)
            {
                if(human.blink.LimitMax == 0f)
                    human.blink.LimitMax = 0.7f;
            }
        }
    }
}
