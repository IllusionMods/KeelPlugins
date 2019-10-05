using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Linq;
using HarmonyLib;

namespace LockOnPlugin
{
    internal static class HoneySelectPatches
    {
        internal static void Init()
        {
            //HarmonyInstance.DEBUG = true;
            Harmony harmony = new Harmony("lockonplugin.honeyselect");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }

        /// <summary>
        /// Prevents normal camera movement with keyboard if moveSpeed is 0f
        /// </summary>
        [HarmonyPatch(typeof(CameraControl_Ver2))]
        [HarmonyPatch("InputKeyProc")]
        private static class HScenePatch1
        {
            private static IEnumerable<CodeInstruction> Transpiler(ILGenerator ilGenerator, IEnumerable<CodeInstruction> instructions)
            {
                List<CodeInstruction> codes = new List<CodeInstruction>(instructions);
                Label label = ilGenerator.DefineLabel();
                bool codeFound = false;

                for(int i = 0; i < codes.Count; i++)
                {
                    if(codes[i].opcode == OpCodes.Ldc_I4 && (int)codes[i].operand == 275)
                    {
                        List<CodeInstruction> newCodes = new List<CodeInstruction>()
                        {
                            new CodeInstruction(OpCodes.Ldarg_0) { labels = codes[i].labels },
                            new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(CameraControl_Ver2), "moveSpeed")),
                            new CodeInstruction(OpCodes.Ldc_R4, 0f),
                            new CodeInstruction(OpCodes.Ceq),
                            new CodeInstruction(OpCodes.Brtrue, label),
                        };

                        codes[i].labels = new List<Label>();
                        codes.InsertRange(i, newCodes);
                        i += newCodes.Count;
                        codeFound = true;
                    }

                    if(codeFound && codes[i].opcode == OpCodes.Ldc_R4 && (float)codes[i].operand == 10f)
                    {
                        codes[i].labels.Add(label);
                        break;
                    }
                }

                return codes.AsEnumerable();
            }
        }

        /// <summary>
        /// Set moveSpeed to 0f if lockedOn = true
        /// </summary>
        [HarmonyPatch(typeof(CustomControl))]
        [HarmonyPatch("Update")]
        private static class MakerPatch1
        {
            private static IEnumerable<CodeInstruction> Transpiler(ILGenerator ilGenerator, IEnumerable<CodeInstruction> instructions)
            {
                List<CodeInstruction> codes = new List<CodeInstruction>(instructions);
                Label label1 = ilGenerator.DefineLabel();
                Label label2 = ilGenerator.DefineLabel();
                bool codeFound = false;

                for(int i = 0; i < codes.Count; i++)
                {
                    if(codes[i].opcode == OpCodes.Stfld && codes[i].operand.ToString() == "System.Single yRotSpeed")
                    {
                        List<CodeInstruction> newCodes = new List<CodeInstruction>()
                        {
                            new CodeInstruction(OpCodes.Ldsfld, AccessTools.Field(typeof(LockOnBase), "lockedOn")),
                            new CodeInstruction(OpCodes.Brtrue_S, label1),
                        };

                        codes.InsertRange(i + 3, newCodes);
                        i += newCodes.Count + 3;
                        codeFound = true;
                    }

                    if(codeFound && codes[i].opcode == OpCodes.Stfld && codes[i].operand.ToString() == "System.Single moveSpeed")
                    {
                        List<CodeInstruction> newCodes = new List<CodeInstruction>()
                        {
                            new CodeInstruction(OpCodes.Br_S, label2),
                            new CodeInstruction(OpCodes.Ldc_R4, 0f),
                        };

                        newCodes[1].labels.Add(label1);
                        codes[i].labels.Add(label2);
                        codes.InsertRange(i, newCodes);
                        i += newCodes.Count;
                    }
                }
                return codes.AsEnumerable();
            }
        }

        /// <summary>
        /// Prevents normal camera movement with keyboard if moveSpeed is 0f
        /// </summary>
        [HarmonyPatch(typeof(BaseCameraControl))]
        [HarmonyPatch("InputKeyProc")]
        private static class MakerPatch2
        {
            private static IEnumerable<CodeInstruction> Transpiler(ILGenerator ilGenerator, IEnumerable<CodeInstruction> instructions)
            {
                List<CodeInstruction> codes = new List<CodeInstruction>(instructions);
                Label label = ilGenerator.DefineLabel();
                bool codeFound = false;

                for(int i = 0; i < codes.Count; i++)
                {
                    if(codes[i].opcode == OpCodes.Ldc_I4 && (int)codes[i].operand == 275)
                    {
                        List<CodeInstruction> newCodes = new List<CodeInstruction>()
                        {
                            new CodeInstruction(OpCodes.Ldarg_0) { labels = codes[i].labels },
                            new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(CameraControl), "moveSpeed")),
                            new CodeInstruction(OpCodes.Ldc_R4, 0f),
                            new CodeInstruction(OpCodes.Ceq),
                            new CodeInstruction(OpCodes.Brtrue, label),
                        };

                        codes[i].labels = new List<Label>();
                        codes.InsertRange(i, newCodes);
                        i += newCodes.Count;
                        codeFound = true;
                    }

                    if(codeFound && codes[i].opcode == OpCodes.Ldc_R4 && (float)codes[i].operand == 10f)
                    {
                        codes[i].labels.Add(label);
                        break;
                    }
                }

                return codes.AsEnumerable();
            }
        }
    }
}
