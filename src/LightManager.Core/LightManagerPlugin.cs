using BepInEx;
using HarmonyLib;
using KeelPlugins.Koikatu;
using KKAPI.Studio.SaveLoad;
using UnityEngine;

[assembly: System.Reflection.AssemblyFileVersion(LightManager.Koikatu.LightManagerPlugin.Version)]

namespace LightManager.Koikatu
{
    [BepInDependency(KKAPI.KoikatuAPI.GUID)]
    [BepInProcess(Constants.StudioProcessName)]
    [BepInPlugin(GUID, "Light Manager", Version)]
    public class LightManagerPlugin : BaseUnityPlugin
    {
        public const string GUID = "keelhauled.lightmanager";
        public const string Version = "1.0.1." + BuildNumber.Version;

        private static GameObject bepinex;

        private void Awake()
        {
            bepinex = gameObject;
            Harmony.CreateAndPatchAll(typeof(Hooks));
            StudioSaveLoadApi.RegisterExtraBehaviour<SceneDataController>(GUID);
        }

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
