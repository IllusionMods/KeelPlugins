using BepInEx;
using BepInEx.Configuration;
using BepInEx.Harmony;
using ChaCustom;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using UnityEngine;

namespace HideAllUI
{
    [BepInIncompatibility("HideStudioUI")]
    [BepInIncompatibility("HideHInterface")]
    [BepInPlugin(GUID, "HideAllUI", Version)]
    public class HideAllUI : BaseUnityPlugin
    {
        public const string GUID = "keelhauled.hideallui";
        public const string Version = "1.0.0";

        // must be static for the transpiler
        private static ConfigWrapper<KeyboardShortcut> HideHotkey { get; set; }

        private Harmony harmony;
        private static HideUI currentUIHandler;

        private void Start()
        {
            HideHotkey = Config.GetSetting("Keyboard Shortcuts", "Hide UI", new KeyboardShortcut(KeyCode.Space));
            harmony = new Harmony($"{GUID}.harmony");
            HarmonyWrapper.PatchAll(typeof(Hooks), harmony);
        }

#if DEBUG
        private void OnDestroy()
        {
            harmony.UnpatchAll(typeof(Hooks));
        }
#endif

        private void Update()
        {
            if(currentUIHandler != null && HideHotkey.Value.IsDown())
                currentUIHandler.ToggleUI();
        }

        private static bool HotkeyIsDown()
        {
            return HideHotkey.Value.IsDown();
        }

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
