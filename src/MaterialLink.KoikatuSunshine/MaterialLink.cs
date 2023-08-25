using System;
using System.Collections;
using System.Collections.Generic;
using BepInEx;
using HarmonyLib;
using UnityEngine;

[assembly: System.Reflection.AssemblyFileVersion(MaterialLink.KoikatuSunshine.MaterialLinkInfo.Version)]

namespace MaterialLink.KoikatuSunshine
{
    [BepInPlugin(GUID, PluginName, Version)]
    public class MaterialLinkInfo : BaseUnityPlugin
    {
        public const string GUID = "keelhauled.materiallink";
        public const string PluginName = "MaterialLink";
        public const string Version = "1.0.0." + BuildNumber.Version;

        private static MaterialLinkInfo plugin;

        private static readonly List<Renderer> ManagedRenderers = new List<Renderer>();

        private void Awake()
        {
            plugin = this;
            Harmony.CreateAndPatchAll(typeof(MaterialLinkInfo));
        }

        // color change
        [HarmonyPostfix, HarmonyPatch(typeof(ChaControl), nameof(ChaControl.ChangeCustomClothes))]
        private static void ChangeCustomClothes_Postfix(ChaControl __instance)
        {
            UpdateMaterialsDelayed(__instance, 10);
        }
        
        // coordinate change
        [HarmonyPostfix, HarmonyPatch(typeof(ChaControl), nameof(ChaControl.UpdateClothesStateAll))]
        private static void UpdateClothesStateAll_Postfix(ChaControl __instance)
        {
            UpdateMaterialsDelayed(__instance, 10);
        }
        
        // character start
        [HarmonyPostfix, HarmonyPatch(typeof(ChaControl), nameof(ChaControl.Initialize))]
        private static void Initialize_Postfix(ChaControl __instance)
        {
            UpdateMaterialsDelayed(__instance, 10);
        }

        private static void UpdateMaterialsDelayed(ChaControl chara, int framesToWait)
        {
            plugin.StartCoroutine(Coroutine());

            IEnumerator Coroutine()
            {
                for(int i = 0; i < framesToWait; i++)
                    yield return null;
                
                GatherRenderers(chara);
                UpdateRenderers(chara);
            }
        }

        private static void GatherRenderers(ChaControl chara)
        {
            if(chara)
            {
                var clothesComponents = chara.gameObject.GetComponentsInChildren<ChaClothesComponent>();
                foreach(var clothComp in clothesComponents)
                {
                    var renderers = clothComp.gameObject.GetComponentsInChildren<Renderer>();
                    foreach(var rend in renderers)
                    {
                        if(rend && rend.material.shader.name == "Shader Forge/main_skin" && !ManagedRenderers.Contains(rend))
                        {
                            Console.WriteLine($"Managing {clothComp.name}");
                            ManagedRenderers.Add(rend);
                        }
                    }
                }
            }
        }

        private static void UpdateRenderers(ChaControl chara)
        {
            if(chara && chara.customMatBody)
            {
                foreach(var rend in ManagedRenderers)
                {
                    if(rend && rend.material != chara.customMatBody)
                        rend.material = chara.customMatBody;
                }
            }
        }
    }
}
