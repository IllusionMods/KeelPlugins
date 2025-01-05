using System.Collections;
using System.Collections.Generic;
using BepInEx;
using HarmonyLib;
using UnityEngine;

[assembly: System.Reflection.AssemblyVersion(MaterialLink.KoikatuSunshine.MaterialLinkInfo.Version)]

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
            Log.SetLogSource(Logger);
            Harmony.CreateAndPatchAll(typeof(MaterialLinkInfo));
        }

        // color change
        [HarmonyPostfix, HarmonyPatch(typeof(ChaControl), nameof(ChaControl.ChangeCustomClothes)), HarmonyWrapSafe]
        private static void ChangeCustomClothes_Postfix(ChaControl __instance)
        {
            UpdateMaterials(__instance);
        }

        // coordinate change
        [HarmonyPostfix, HarmonyPatch(typeof(ChaControl), nameof(ChaControl.UpdateClothesStateAll))]
        private static void UpdateClothesStateAll_Postfix(ChaControl __instance)
        {
            UpdateMaterialsDelayed(__instance, 10);
        }
        
        // character start
        [HarmonyPostfix, HarmonyPatch(typeof(ChaControl), nameof(ChaControl.Initialize)), HarmonyWrapSafe]
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
                
                UpdateMaterials(chara);
            }
        }

        private static void UpdateMaterials(ChaControl chara)
        {
            if(!chara)
                return;

            // Find clothing that uses the skin shader
            foreach(var cloth in chara.gameObject.GetComponentsInChildren<ChaClothesComponent>())
            foreach(var rend in cloth.gameObject.GetComponentsInChildren<Renderer>())
            {
                if(rend && rend.material.shader.name == "Shader Forge/main_skin" && !ManagedRenderers.Contains(rend))
                {
                    Log.Info($"Managing {cloth.name}");
                    ManagedRenderers.Add(rend);
                }
            }

            // Edit found renderers to use skin material
            if(chara.customMatBody)
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
