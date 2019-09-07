using BepInEx;
using HarmonyLib;

namespace KeelPlugins
{
    [BepInPlugin(GUID, "HideAllUI", Version)]
    public class HideAllUI : HideAllUICore
    {
        public const string Version = "1.0.0";

        private class Hooks
        {
            [HarmonyPostfix, HarmonyPatch(typeof(CharaCustom.CharaCustom), "Start")]
            public static void MakerEntrypoint()
            {
                currentUIHandler = new HideMakerUI();
            }
        }
    }
}
