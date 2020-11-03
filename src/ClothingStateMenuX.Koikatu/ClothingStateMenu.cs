using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Diagnostics;

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
            UI.CreateUI();
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
