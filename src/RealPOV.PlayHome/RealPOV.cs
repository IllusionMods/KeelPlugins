using BepInEx;
using BepInEx.Bootstrap;
using BepInEx.Configuration;
using HarmonyLib;
using KeelPlugins.Utils;
using RealPOV.Core;
using UnityEngine;

[assembly: System.Reflection.AssemblyVersion(RealPOV.PlayHome.RealPOV.Version)]

namespace RealPOV.PlayHome
{
    [BepInPlugin(GUID, PluginName, Version)]
    public class RealPOV : RealPOVCore
    {
        public const string Version = "1.1.1." + BuildNumber.Version;

        private const string SECTION_OFFSETS = "Offsets";

        internal static ConfigEntry<float> FemaleOffsetX { get; set; }
        internal static ConfigEntry<float> FemaleOffsetY { get; set; }
        internal static ConfigEntry<float> FemaleOffsetZ { get; set; }
        internal static ConfigEntry<float> MaleOffsetX { get; set; }
        internal static ConfigEntry<float> MaleOffsetY { get; set; }
        internal static ConfigEntry<float> MaleOffsetZ { get; set; }
        internal static ConfigEntry<KeyboardShortcut> CycleNextHotkey { get; set; }
        internal static ConfigEntry<KeyboardShortcut> CyclePrevHotkey { get; set; }
        internal static ConfigEntry<bool> IncludeFemalePOV { get; set; }
        internal static ConfigEntry<float> DefaultNearClip { get; set; }

        protected static float defaultNearClip = 0.06f;

        protected override void Awake()
        {
            base.Awake();

            FemaleOffsetX = Config.Bind(SECTION_OFFSETS, "Female offset X", 0f, new ConfigDescription("", null, new ConfigurationManagerAttributes { IsAdvanced = true }));
            FemaleOffsetY = Config.Bind(SECTION_OFFSETS, "Female offset Y", 0.0315f, new ConfigDescription("", null, new ConfigurationManagerAttributes { IsAdvanced = true }));
            FemaleOffsetZ = Config.Bind(SECTION_OFFSETS, "Female offset Z", 0f, new ConfigDescription("", null, new ConfigurationManagerAttributes { IsAdvanced = true }));

            MaleOffsetX = Config.Bind(SECTION_OFFSETS, "Male offset X", 0f, new ConfigDescription("", null, new ConfigurationManagerAttributes { IsAdvanced = true }));
            MaleOffsetY = Config.Bind(SECTION_OFFSETS, "Male offset Y", 0.092f, new ConfigDescription("", null, new ConfigurationManagerAttributes { IsAdvanced = true }));
            MaleOffsetZ = Config.Bind(SECTION_OFFSETS, "Male offset Z", 0.12f, new ConfigDescription("", null, new ConfigurationManagerAttributes { IsAdvanced = true }));

            CycleNextHotkey = Config.Bind(SECTION_HOTKEYS, "Cycle next character POV", new KeyboardShortcut(KeyCode.KeypadPlus));
            CyclePrevHotkey = Config.Bind(SECTION_HOTKEYS, "Cycle previous character POV", new KeyboardShortcut(KeyCode.KeypadMinus));
            IncludeFemalePOV = Config.Bind(SECTION_GENERAL, "Include female POV when cycling", false);
            DefaultNearClip = Config.Bind(SECTION_GENERAL, "Default camera near clip", defaultNearClip, new ConfigDescription("", new AcceptableValueRange<float>(0.01f, 0.1f)));

            Harmony.CreateAndPatchAll(GetType());
        }

        protected override void EnablePov()
        {
            
        }

        protected override void DisablePov()
        {
            
        }

        [HarmonyPostfix, HarmonyPatch(typeof(H_Scene), "Awake")]
        private static void HSceneInit()
        {
            Chainloader.ManagerObject.GetOrAddComponent<PointOfView>();
        }

        [HarmonyPostfix, HarmonyPatch(typeof(StudioScene), "Start")]
        private static void StudioSceneInit()
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
