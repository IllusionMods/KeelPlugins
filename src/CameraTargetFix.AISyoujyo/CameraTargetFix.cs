using BepInEx;
using BepInEx.Harmony;
using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;

namespace KeelPlugins
{
    [BepInProcess(AISyoujyoConstants.StudioProcessName)]
    [BepInPlugin(GUID, PluginName, Version)]
    public class CameraTargetFix : BaseUnityPlugin
    {
        public const string GUID = "keelhauled.cameratargetfix";
        public const string PluginName = "CameraTargetFix";
        public const string Version = "1.0.0." + BuildNumber.Version;

        private Harmony harmony;

        private void Awake()
        {
            harmony = new Harmony("keelhauled.cameratargetfix.harmony");
            harmony.PatchAll(GetType());
        }

#if DEBUG
        private void OnDestroy()
        {
            harmony.UnpatchAll();
        }
#endif

        [HarmonyTranspiler, HarmonyPatch(typeof(Studio.CameraControl), "InternalUpdateCameraState")]
        private static IEnumerable<CodeInstruction> StudioPatch(IEnumerable<CodeInstruction> instructions)
        {
            var codes = instructions.ToList();

            for(int i = codes.Count - 1; i >= 0; i--)
            {
                if(codes[i].opcode == OpCodes.Call && codes[i].operand.ToString().Contains("get_isOutsideTargetTex()"))
                {
                    codes[i - 1].opcode = OpCodes.Nop;
                    codes[i].opcode = OpCodes.Ldc_I4_1;
                    break;
                }
            }

            return codes;
        }
    }
}
