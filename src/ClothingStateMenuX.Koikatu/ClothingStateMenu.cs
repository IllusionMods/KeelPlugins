using System;
using System.Collections.Generic;
using System.Linq;
using BepInEx;
using HarmonyLib;
using UnityEngine;
using System.Diagnostics;
using BepInEx.Logging;
using UnityEngine.UI;

namespace ClothingStateMenuX.Koikatu
{
    [BepInProcess(KeelPlugins.KoikatuConstants.MainGameProcessName)]
    [BepInProcess(KeelPlugins.KoikatuConstants.MainGameProcessNameSteam)]
    [BepInPlugin("keelhauled.clothingstatemenux", "ClothingStateMenuX", Version)]
    public class ClothingStateMenu : BaseUnityPlugin
    {
        public const string Version = "1.0.0." + BuildNumber.Version;

        public static new ManualLogSource Logger;

        public static List<Action> delete = new List<Action>();

        private void Awake()
        {
            Logger = base.Logger;
#if DEBUG
            MakerEntrypoint();
#else
            Harmony.CreateAndPatchAll(GetType());
#endif
        }

        [HarmonyPrefix, HarmonyPatch(typeof(CustomScene), "Start")]
        private static void MakerEntrypoint()
        {
            var watch = new Stopwatch();
            watch.Start();

            UI.CreateTitle("Clothing Sets", 0);
            UI.CreateClothingSets(1);
            UI.CreateSeparator(2);
            UI.CreateClothingOptions(VanillaUI.ClothingStateToggles.transform.GetSiblingIndex() + 1);

            watch.Stop();
            Logger.LogInfo(watch.Elapsed);
        }

#if DEBUG
        private void OnDestroy()
        {
            foreach(var item in delete)
                item();
        }
#endif
    }
}
