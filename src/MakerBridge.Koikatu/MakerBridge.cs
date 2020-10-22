using BepInEx;
using HarmonyLib;

namespace KeelPlugins
{
    [BepInPlugin(GUID, PluginName, Version)]
    public class MakerBridge : MakerBridgeCore
    {
        public const string Version = "1.0.2." + BuildNumber.Version;

        protected override void Awake()
        {
            base.Awake();
            Harmony.CreateAndPatchAll(typeof(Hooks));
        }

        private class Hooks
        {
            [HarmonyPrefix, HarmonyPatch(typeof(CustomScene), "Start")]
            public static void MakerEntrypoint()
            {
                bepinex.GetOrAddComponent<MakerHandler>();
            }

            [HarmonyPrefix, HarmonyPatch(typeof(CustomScene), "OnDestroy")]
            public static void MakerEnd()
            {
                Destroy(bepinex.GetComponent<MakerHandler>());
            }

            [HarmonyPrefix, HarmonyPatch(typeof(StudioScene), "Start")]
            public static void StudioEntrypoint()
            {
                bepinex.GetOrAddComponent<StudioHandler>();
            }
        }
    }
}
