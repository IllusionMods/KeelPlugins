using BepInEx;
using ChaCustom;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using UnityEngine;

namespace KeelPlugins
{
    [BepInIncompatibility("HideStudioUI")]
    [BepInIncompatibility("HideHInterface")]
    [BepInPlugin(GUID, "HideAllUI", Version)]
    public class HideAllUI : HideAllUICore
    {
        public const string Version = "1.0.0";

        private static bool HotkeyIsDown() => HideHotkey.Value.IsDown();

        private class Hooks
        {
            [HarmonyTranspiler, HarmonyPatch(typeof(CustomControl), "Update")]
            public static IEnumerable<CodeInstruction> SetMakerHotkey(IEnumerable<CodeInstruction> instructions)
            {
                var codes = instructions.ToList();
                var inputGetKeyDown = AccessTools.Method(typeof(Input), nameof(Input.GetKeyDown), new Type[] { typeof(KeyCode) });

                for(int i = 0; i < codes.Count; i++)
                {
                    if(codes[i].opcode == OpCodes.Ldc_I4_S && codes[i].operand is sbyte val && val == (sbyte)KeyCode.Space)
                    {
                        if(codes[i + 1].opcode == OpCodes.Call && codes[i + 1].operand == inputGetKeyDown)
                        {
                            codes[i].opcode = OpCodes.Nop;
                            codes[i + 1] = new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(HideAllUI), nameof(HotkeyIsDown)));
                            break;
                        }
                    }
                }

                return codes;
            }

            [HarmonyPostfix, HarmonyPatch(typeof(Studio.Studio), "Init")]
            public static void StudioInit()
            {
                currentUIHandler = new HideStudioUI();
            }

            [HarmonyPrefix, HarmonyPatch(typeof(HSceneProc), "SetShortcutKey")]
            public static void HSceneStart()
            {
                currentUIHandler = new HideHSceneUI();
            }

            [HarmonyPrefix, HarmonyPatch(typeof(HSceneProc), "OnDestroy")]
            public static void HSceneEnd()
            {
                currentUIHandler = null;
            }
        }
    }
}
