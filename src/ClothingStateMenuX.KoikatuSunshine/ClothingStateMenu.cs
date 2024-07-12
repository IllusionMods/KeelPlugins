using BepInEx;
using BepInEx.Bootstrap;
using HarmonyLib;
using KeelPlugins;
using KKAPI;
using KKAPI.Maker;

[assembly: System.Reflection.AssemblyFileVersion(ClothingStateMenuX.Koikatu.ClothingStateMenu.Version)]

namespace ClothingStateMenuX.Koikatu
{
    [BepInProcess(Constants.MainGameProcessName)]
    [BepInDependency(KoikatuAPI.GUID)]
    [BepInDependency(MoreOutfitsGUID, BepInDependency.DependencyFlags.SoftDependency)]
    [BepInPlugin("keelhauled.clothingstatemenux", "ClothingStateMenuX", Version)]
    public class ClothingStateMenu : BaseUnityPlugin
    {
        public const string Version = "1.0.4." + BuildNumber.Version;

        private const string MoreOutfitsGUID = "com.deathweasel.bepinex.moreoutfits";

        private void Awake()
        {
            Log.SetLogSource(Logger);
            MakerAPI.MakerStartedLoading += (x, y) => UI.Setup();

            if(Chainloader.PluginInfos.ContainsKey(MoreOutfitsGUID))
                Harmony.CreateAndPatchAll(typeof(ClothingStateMenu));
            else
                MakerAPI.ReloadCustomInterface += (x, y) => UI.ReloadClothingSets();
        }

        [HarmonyPostfix, HarmonyPatch("KK_Plugins.MoreOutfits.MakerUI, KKS_MoreOutfits", "UpdateMakerUI"), HarmonyWrapSafe]
        private static void MoreOutfitsHook()
        {
            UI.ReloadClothingSets();
        }
    }
}
