using BepInEx;
using HarmonyLib;

namespace KeelPlugins
{
    [BepInPlugin(GUID, "LockOnPlugin", Version)]
    public class LockOnPlugin : LockOnPluginCore
    {
        private class Entrypoints
        {
            [HarmonyPrefix, HarmonyPatch(typeof(CustomScene), "Start")]
            public static void MakerEntrypoint()
            {
                bepinex.GetOrAddComponent<MakerMono>();
            }

            [HarmonyPrefix, HarmonyPatch(typeof(CustomScene), "OnDestroy")]
            public static void MakerEnd()
            {
                Destroy(bepinex.GetComponent<MakerMono>());
            }

            [HarmonyPrefix, HarmonyPatch(typeof(StudioScene), "Start")]
            public static void StudioEntrypoint()
            {
                bepinex.GetOrAddComponent<StudioMono>();
            }

            [HarmonyPrefix, HarmonyPatch(typeof(HSceneProc), "SetShortcutKey")]
            public static void HSceneEntrypoint()
            {
                bepinex.GetOrAddComponent<HSceneMono>();
            }

            [HarmonyPrefix, HarmonyPatch(typeof(HSceneProc), "OnDestroy")]
            public static void HSceneEnd()
            {
                Destroy(bepinex.GetComponent<HSceneMono>());
            }
        }
    }
}
