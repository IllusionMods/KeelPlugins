using BepInEx;
using H;
using HarmonyLib;
using KeelPlugins;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using BepInEx.Configuration;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[assembly: System.Reflection.AssemblyVersion(AltAutoMode.PlayHome.AltAutoMode.Version)]

namespace AltAutoMode.PlayHome
{
    [BepInProcess(Constants.MainGameProcessName32bit)]
    [BepInProcess(Constants.MainGameProcessName64bit)]
    [BepInPlugin(GUID, "Alternative Automode", Version)]
    public class AltAutoMode : BaseUnityPlugin
    {
        public const string GUID = "keelhauled.altautomode";
        public const string Version = "1.0.2." + BuildNumber.Version;

        private static ConfigEntry<bool> _sUnlockPoses;
        private static ConfigEntry<bool> _sEyesOpen;

        private void Awake()
        {
            _sUnlockPoses = Config.Bind("H Scene", "Unlock all positions", false, "Unlock all H positions regardless of game progress and map. All special positions are always available.");
            _sEyesOpen = Config.Bind("H Scene", "Prevent eyes from staying closed", true);
            Harmony.CreateAndPatchAll(typeof(Hooks));
        }

        private class Hooks
        {
            private static float sliderVal;
            private static Slider slider;
            private static float time;
            private static float random;

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
                        random = UnityEngine.Random.Range(2f, 2.5f);
                    }
                    else
                    {
                        autoChangeSpeed.SetValue(0.1f);
                        random = UnityEngine.Random.Range(1f, 10f);
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
                return !Input.GetKey(KeyCode.LeftShift);
            }

            [HarmonyPrefix, HarmonyPatch(typeof(HStyleChangeUI), "StyleCheck")]
            public static bool UnlockPositions(HStyleChangeUI __instance, ref bool __result, H_StyleData data)
            {
                if(_sUnlockPoses.Value)
                {
                    __result = data.state == Traverse.Create(__instance).Field("nowState").GetValue<H_StyleData.STATE>();
                    return false;
                }
                return true;
            }

            [HarmonyPostfix, HarmonyPatch(typeof(H_ExpressionData), nameof(H_ExpressionData.ChangeExpression), typeof(Human), typeof(H_Expression.TYPE), typeof(H_Parameter), typeof(float))]
            public static void OpenEyes(Human human)
            {
                if(_sEyesOpen.Value)
                {
                    if(human.blink.LimitMax == 0f) 
                        human.blink.LimitMax = 0.7f;
                }
            }
        }
    }
}
