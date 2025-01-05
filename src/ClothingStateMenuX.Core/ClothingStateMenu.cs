using System;
using BepInEx;
using BepInEx.Bootstrap;
using BepInEx.Configuration;
using HarmonyLib;
using KeelPlugins;
using KKAPI;
using KKAPI.Maker;

[assembly: System.Reflection.AssemblyVersion(ClothingStateMenuX.ClothingStateMenu.Version)]

namespace ClothingStateMenuX
{
    [BepInProcess(Constants.MainGameProcessName)]
    [BepInDependency(KoikatuAPI.GUID)]
    [BepInDependency(MoreOutfitsGUID, BepInDependency.DependencyFlags.SoftDependency)]
    [BepInPlugin("keelhauled.clothingstatemenux", "ClothingStateMenuX", Version)]
    public class ClothingStateMenu : BaseUnityPlugin
    {
        public const string Version = "1.1.0." + BuildNumber.Version;

        public static ConfigEntry<bool> ShowClothingSets { get; set; }

        private const string MoreOutfitsGUID = "com.deathweasel.bepinex.moreoutfits";
        private Harmony harmony;

        private void Awake()
        {
            Log.SetLogSource(Logger);
            ShowClothingSets = Config.Bind("General", "Show Clothing Sets", true);
            ShowClothingSets.SettingChanged += (sender, args) =>
            {
                if(ShowClothingSets.Value)
                {
                    StartClothingSetEvents();
                    UI.ReloadClothingSets();
                }
                else
                {
                    harmony?.UnpatchSelf();
                    MakerAPI.ReloadCustomInterface -= OnReloadCustomInterface;
                    UI.RemoveClothingSets();
                }
            };

            MakerAPI.MakerStartedLoading += (sender, args) => UI.Setup();

            if(ShowClothingSets.Value)
                StartClothingSetEvents();
        }

        private void StartClothingSetEvents()
        {
            if(Chainloader.PluginInfos.ContainsKey(MoreOutfitsGUID))
                harmony = Harmony.CreateAndPatchAll(typeof(Hooks));
            else
                MakerAPI.ReloadCustomInterface += OnReloadCustomInterface;
        }

        private void OnReloadCustomInterface(object sender, EventArgs args)
        {
            UI.ReloadClothingSets();
        }

        private static class Hooks
        {
#if KK
            [HarmonyPostfix, HarmonyPatch("KK_Plugins.MoreOutfits.MakerUI, KK_MoreOutfits", "UpdateMakerUI"), HarmonyWrapSafe]
#else
            [HarmonyPostfix, HarmonyPatch("KK_Plugins.MoreOutfits.MakerUI, KKS_MoreOutfits", "UpdateMakerUI"), HarmonyWrapSafe]
#endif
            private static void MoreOutfitsHook()
            {
                UI.ReloadClothingSets();
            }
        }
    }
}
