using BepInEx;
using BepInEx.Logging;
using KKAPI.Maker;

namespace ClothingStateMenuX.Koikatu
{
    [BepInProcess(KeelPlugins.KoikatuConstants.MainGameProcessName)]
    [BepInProcess(KeelPlugins.KoikatuConstants.MainGameProcessNameSteam)]
    [BepInPlugin("keelhauled.clothingstatemenux", "ClothingStateMenuX", Version)]
    public class ClothingStateMenu : BaseUnityPlugin
    {
        public const string Version = "1.0.0." + BuildNumber.Version;
        internal static new ManualLogSource Logger;

        private void Awake()
        {
            Logger = base.Logger;
            MakerAPI.MakerStartedLoading += (x, y) => UI.CreateUI();
        }
    }
}
