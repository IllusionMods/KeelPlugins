using System;
using BepInEx;
using HarmonyLib;
using KKAPI.Maker;
using LockOnPlugin.Core;

[assembly: System.Reflection.AssemblyFileVersion(LockOnPlugin.Koikatu.LockOnPlugin.Version)]

namespace LockOnPlugin.Koikatu
{
    [BepInPlugin(GUID, PluginName, Version)]
    public class LockOnPlugin : LockOnPluginCore
    {
        public const string Version = "2.6.3." + BuildNumber.Version;

        protected override void Awake()
        {
            base.Awake();

            Harmony.CreateAndPatchAll(typeof(Entrypoints));
            MakerAPI.MakerBaseLoaded += Entrypoints.MakerEntrypoint;
            MakerAPI.MakerExiting += Entrypoints.MakerEnd;
        }

        private class Entrypoints
        {
            public static void MakerEntrypoint(object sender, RegisterCustomControlsEvent e)
            {
                bepinex.GetOrAddComponent<MakerMono>();
            }

            public static void MakerEnd(object sender, EventArgs e)
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
