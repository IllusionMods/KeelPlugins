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

        private static List<GameObject> delete;

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

            var go1 = UI.CreateTitle("Clothing Sets", 0);
            var go2 = UI.CreateClothingSets(1);
            var go3 = UI.CreateSeparator(2);
            var go4 = UI.CreateClothingOptions(VanillaUI.ClothingStateToggles.transform.GetSiblingIndex() + 1);

            delete = new List<GameObject> { go1, go2, go3 };
            delete.AddRange(go4);

            watch.Stop();
            Logger.LogInfo(watch.Elapsed);
        }

#if DEBUG
        private void OnDestroy()
        {
            foreach(var item in delete)
                DestroyImmediate(item);
        }
#endif
    }
}
