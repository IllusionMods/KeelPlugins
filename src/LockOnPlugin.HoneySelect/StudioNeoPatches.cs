using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Linq;
using HarmonyLib;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;

namespace LockOnPlugin
{
    internal static class StudioNeoPatches
    {
        internal static void Init()
        {
            //HarmonyInstance.DEBUG = true;
            Harmony harmony = new Harmony("lockonplugin.studioneo");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }

        /// <summary>
        /// Prevents normal camera movement with keyboard if moveSpeed is 0f
        /// </summary>
        [HarmonyPatch(typeof(Studio.CameraControl))]
        [HarmonyPatch("InputKeyProc")]
        private static class NeoPatch1
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
                            new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(Studio.CameraControl), "moveSpeed")),
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
        /// Simplify UpdateDynamicBones
        /// </summary>
        //[HarmonyPatch(typeof(DynamicBone_Ver02))]
        //[HarmonyPatch("UpdateDynamicBones")]
        //[HarmonyPatch(new Type[] { typeof(float) })]
        //private static class NeoPatch2
        //{
        //    private static IEnumerable<CodeInstruction> Transpiler(ILGenerator ilGenerator, IEnumerable<CodeInstruction> instructions)
        //    {
        //        List<CodeInstruction> codes = new List<CodeInstruction>(instructions);
        //        FieldInfo field = AccessTools.Field(typeof(DynamicBone_Ver02), "ObjectPrevPosition");

        //        for(int i = 0; i < codes.Count; i++)
        //        {
        //            if(codes[i].opcode == OpCodes.Stfld && codes[i].operand == field)
        //            {
        //                List<CodeInstruction> newCodes = new List<CodeInstruction>()
        //                {
        //                    new CodeInstruction(OpCodes.Ldarg_0),
        //                    new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(DynamicBone_Ver02), "UpdateParticles1")),
        //                    new CodeInstruction(OpCodes.Ldarg_0),
        //                    new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(DynamicBone_Ver02), "UpdateParticles2")),
        //                    new CodeInstruction(OpCodes.Ldarg_0),
        //                    new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(DynamicBone_Ver02), "ApplyParticlesToTransforms")),
        //                    new CodeInstruction(OpCodes.Ret),
        //                };

        //                codes.InsertRange(i + 1, newCodes);
        //                break;
        //            }
        //        }

        //        return codes.AsEnumerable();
        //    }
        //}
    }
}
