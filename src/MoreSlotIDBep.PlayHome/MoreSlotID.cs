using BepInEx;
using HarmonyLib;
using System.Collections.Generic;
using System.IO;

namespace KeelPlugins
{
    [BepInPlugin(GUID, "PH_MoreSlotID_Bep", Version)]
    public class MoreSlotID : BaseUnityPlugin
    {
        public const string GUID = "keelhauled.phmoreslotidbep";
        public const string Version = "1.0.0";

        internal static string ModFolder = Path.Combine(Paths.GameRootPath, "mods");
        internal static Dictionary<string, string> AssetList = new Dictionary<string, string>();
        internal static Dictionary<string, string> ThumbList = new Dictionary<string, string>();

        private void Awake()
        {
            var harmony = new Harmony("keelhauled.phmoreslotidbep.harmony");
            harmony.PatchAll(typeof(EditModePatch));
            harmony.PatchAll(typeof(ItemDataBasePatch));
            CustomDataSetupLoaderPatch.Patch(harmony);
        }
    }
}
