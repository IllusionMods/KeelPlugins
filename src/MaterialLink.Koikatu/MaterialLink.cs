using System;
using System.Collections;
using BepInEx;
using HarmonyLib;
using UnityEngine;

[assembly: System.Reflection.AssemblyFileVersion(MaterialLink.Koikatu.MaterialLinkInfo.Version)]

namespace MaterialLink.Koikatu
{
    [BepInPlugin(GUID, PluginName, Version)]
    public class MaterialLinkInfo : BaseUnityPlugin
    {
        public const string GUID = "keelhauled.materiallink";
        public const string PluginName = "MaterialLink";
        public const string Version = "1.0.0." + BuildNumber.Version;

        public static Action<ChaControl> UpdateMaterials;

        private static MaterialLinkInfo plugin;

        private void Awake()
        {
            plugin = this;
            Harmony.CreateAndPatchAll(typeof(MaterialLinkInfo));
        }

        // color change
        [HarmonyPostfix, HarmonyPatch(typeof(ChaControl), nameof(ChaControl.ChangeCustomClothes))]
        private static void ChangeCustomClothes_Postfix(ChaControl __instance)
        {
            UpdateMaterials?.Invoke(__instance);
        }
        
        // coordinate change
        [HarmonyPostfix, HarmonyPatch(typeof(ChaControl), nameof(ChaControl.UpdateClothesStateAll))]
        private static void UpdateClothesStateAll_Postfix(ChaControl __instance)
        {
            UpdateMaterialsDelayed(__instance, 2);
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
                
                UpdateMaterials?.Invoke(chara);
            }
        }
    }

    public class MaterialLink : MonoBehaviour
    {
        public Renderer[] ManagedRenderers;

        private ChaControl chaControl;

        private void Start()
        {
            chaControl = gameObject.GetComponentInParent<ChaControl>();
            MaterialLinkInfo.UpdateMaterials += UpdateMaterialsIfSelf;
        }

        private void OnDestroy()
        {
            MaterialLinkInfo.UpdateMaterials -= UpdateMaterialsIfSelf;
        }

        private void UpdateMaterialsIfSelf(ChaControl chara)
        {
            if(chara == chaControl)
                UpdateMaterials();
        }

        private void UpdateMaterials()
        {
            if(chaControl && chaControl.customMatBody)
            {
                foreach(var rend in ManagedRenderers)
                {
                    if(rend && chaControl.customMatBody && rend.material != chaControl.customMatBody)
                        rend.material = chaControl.customMatBody;
                }
            }
        }
    }
}