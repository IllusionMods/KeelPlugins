using System;
using System.Collections.Generic;
using System.Linq;
using BepInEx;
using HarmonyLib;

namespace ClothingStateMenuX.Koikatu
{
    [BepInProcess(KeelPlugins.KoikatuConstants.MainGameProcessName)]
    [BepInProcess(KeelPlugins.KoikatuConstants.MainGameProcessNameSteam)]
    [BepInPlugin("keelhauled.clothingstatemenux", "ClothingStateMenuX", Version)]
    public class ClothingStateMenu : BaseUnityPlugin
    {
        public const string Version = "1.0.0." + BuildNumber.Version;

        private void Awake()
        {
            Harmony.CreateAndPatchAll(GetType());
        }

        [HarmonyPrefix, HarmonyPatch(typeof(CustomScene), "Start")]
        private static void MakerEntrypoint()
        {
            UI.CreateTitle("Clothing Sets", 0);
            UI.CreateClothingSets(1);
            UI.CreateSeparator(2);

            UI.CreateClothingOptions(VanillaUI.ClothingStateToggles.transform.GetSiblingIndex() + 1);
            UI.CreateAccessories(VanillaUI.AccessoryToggles.transform.GetSiblingIndex() + 1);
        }
    }
}
