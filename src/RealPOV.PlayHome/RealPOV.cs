using BepInEx;
using BepInEx.Bootstrap;
using BepInEx.Configuration;
using BepInEx.Harmony;
using HarmonyLib;
using UnityEngine;

namespace KeelPlugins
{
    [BepInPlugin(GUID, PluginName, Version)]
    public class RealPOV : RealPOVCore
    {
        private const string SECTION_OFFSETS = "Offsets";

        private const string DESCRIPTION_POVHOTKEY = "This hotkey enables the POV mode and then switches between different rotation modes. Hold the key to disable POV mode.";
        private const string DESCRIPTION_DEFAULTROTATION = "A character has two bones in their neck that this plugin rotates to shift the first person view. " +
                                                           "By changing this setting you can choose to rotate the bones individually or both at the same time.\n" +
                                                           "Only rotating the second bone can be useful in positions where the partner is very close to the camera and rotating both bones would cause clipping.\n" +
                                                           "Only rotating the first bone can be useful in positions where it would be helpful to extend the character's neck forward to see the partner better.";

        internal static ConfigEntry<KeyboardShortcut> POVKey { get; set; }
        internal static KeyboardShortcutHotkey POVHotkey;
        internal static ConfigEntry<float> DefaultFOV { get; set; }
        internal static ConfigEntry<float> MouseSensitivity { get; set; }
        internal static ConfigEntry<NeckMode> DefaultNeckMode { get; set; }
        internal static ConfigEntry<float> FemaleOffsetX { get; set; }
        internal static ConfigEntry<float> FemaleOffsetY { get; set; }
        internal static ConfigEntry<float> FemaleOffsetZ { get; set; }
        internal static ConfigEntry<float> MaleOffsetX { get; set; }
        internal static ConfigEntry<float> MaleOffsetY { get; set; }
        internal static ConfigEntry<float> MaleOffsetZ { get; set; }

        protected override void Awake()
        {
            base.Awake();

            POVKey = Config.Bind(SECTION_HOTKEYS, "Toggle POV", new KeyboardShortcut(KeyCode.Backspace), new ConfigDescription(DESCRIPTION_POVHOTKEY));
            POVHotkey = new KeyboardShortcutHotkey(POVKey.Value, 0.3f);
            DefaultFOV = Config.Bind(SECTION_GENERAL, "Default FOV", 70f, new ConfigDescription("", new AcceptableValueRange<float>(20f, 120f)));
            MouseSensitivity = Config.Bind(SECTION_GENERAL, "Mouse sensitivity", 1f, new ConfigDescription("", new AcceptableValueRange<float>(0.1f, 2f)));
            DefaultNeckMode = Config.Bind(SECTION_GENERAL, "Default rotation mode", NeckMode.Both);

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
