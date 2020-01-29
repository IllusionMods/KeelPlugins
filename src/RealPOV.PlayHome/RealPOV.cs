﻿using BepInEx;
using BepInEx.Bootstrap;
using BepInEx.Configuration;
using HarmonyLib;

namespace KeelPlugins
{
    [BepInPlugin(GUID, PluginName, Version)]
    public class RealPOV : RealPOVCore
    {
        private const string SECTION_OFFSETS = "Offsets";

        internal static ConfigEntry<float> FemaleOffsetX { get; set; }
        internal static ConfigEntry<float> FemaleOffsetY { get; set; }
        internal static ConfigEntry<float> FemaleOffsetZ { get; set; }
        internal static ConfigEntry<float> MaleOffsetX { get; set; }
        internal static ConfigEntry<float> MaleOffsetY { get; set; }
        internal static ConfigEntry<float> MaleOffsetZ { get; set; }

        protected override void Awake()
        {
            base.Awake();

            FemaleOffsetX = Config.Bind(SECTION_OFFSETS, "Female offset X", 0f, new ConfigDescription("", null, new ConfigurationManagerAttributes { IsAdvanced = true }));
            FemaleOffsetY = Config.Bind(SECTION_OFFSETS, "Female offset Y", 0.0315f, new ConfigDescription("", null, new ConfigurationManagerAttributes { IsAdvanced = true }));
            FemaleOffsetZ = Config.Bind(SECTION_OFFSETS, "Female offset Z", 0f, new ConfigDescription("", null, new ConfigurationManagerAttributes { IsAdvanced = true }));

            MaleOffsetX = Config.Bind(SECTION_OFFSETS, "Male offset X", 0f, new ConfigDescription("", null, new ConfigurationManagerAttributes { IsAdvanced = true }));
            MaleOffsetY = Config.Bind(SECTION_OFFSETS, "Male offset Y", 0.092f, new ConfigDescription("", null, new ConfigurationManagerAttributes { IsAdvanced = true }));
            MaleOffsetZ = Config.Bind(SECTION_OFFSETS, "Male offset Z", 0.12f, new ConfigDescription("", null, new ConfigurationManagerAttributes { IsAdvanced = true }));
        }

        [HarmonyPostfix, HarmonyPatch(typeof(H_Scene), "Awake")]
        private static void HSceneInit()
        {
            Chainloader.ManagerObject.GetOrAddComponent<PointOfView>();
        }

        [HarmonyPostfix, HarmonyPatch(typeof(H_Scene), "OnDestroy")]
        private static void HSceneDestroy()
        {
            Destroy(Chainloader.ManagerObject.GetComponent<PointOfView>());
        }
    }
}
