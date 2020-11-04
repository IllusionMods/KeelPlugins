using BepInEx;
using HarmonyLib;
using KKAPI.Studio.SaveLoad;
using UnityEngine;

namespace KeelPlugins
{
    [BepInDependency(KKAPI.KoikatuAPI.GUID)]
    [BepInProcess(KoikatuConstants.StudioProcessName)]
    [BepInPlugin(GUID, "Light Manager", Version)]
    public class LightManagerPlugin : BaseUnityPlugin
    {
        public const string GUID = "keelhauled.lightmanager";
        public const string Version = "1.0.0." + BuildNumber.Version;

#pragma warning disable IDE0052 // Remove unread private members
        private Harmony harmony;
#pragma warning restore IDE0052 // Remove unread private members
        private static GameObject bepinex;

        private void Awake()
        {
            bepinex = gameObject;
            harmony = Harmony.CreateAndPatchAll(typeof(Hooks));
            StudioSaveLoadApi.RegisterExtraBehaviour<SceneDataController>(GUID);
        }

#if DEBUG
        private void OnDestroy()
        {
            harmony.UnpatchAll();
        }
#endif

        private class Hooks
        {
            [HarmonyPrefix, HarmonyPatch(typeof(StudioScene), "Start")]
            public static void Entrypoint()
            {
                bepinex.GetOrAddComponent<LightManager>();
            }
        }
    }
}
