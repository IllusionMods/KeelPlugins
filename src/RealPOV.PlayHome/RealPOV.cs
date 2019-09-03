using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;

namespace KeelPlugins
{
    [BepInPlugin(GUID, "RealPOV", Version)]
    public class RealPOV : BaseUnityPlugin
    {
        public const string GUID = "keelhauled.togglepov";
        public const string Version = "1.0.0";
        internal static new ManualLogSource Logger;

        private const string SECTION_HOTKEYS = "Keyboard Shortcuts";
        private const string SECTION_GENERAL = "General";
        private const string SECTION_OFFSETS = "Offsets";

        private const string DESCRIPTION_POVHOTKEY = "This hotkey enables the POV mode and then switches between different rotation modes. Hold the key to disable POV mode.";
        private const string DESCRIPTION_DEFAULTROTATION = "A character has two bones in their neck that this plugin rotates to shift the first person view. " +
                                                           "By changing this setting you can choose to rotate the bones individually or both at the same time.\n" +
                                                           "Only rotating the second bone can be useful in positions where the partner is very close to the camera and rotating both bones would cause clipping.\n" +
                                                           "Only rotating the first bone can be useful in positions where it would be helpful to extend the character's neck forward to see the partner better.";

        internal static ConfigWrapper<KeyboardShortcut> POVKey { get; set; }
        internal static KeyboardShortcutHotkey POVHotkey;
        internal static ConfigWrapper<float> DefaultFOV { get; set; }
        internal static ConfigWrapper<float> MouseSensitivity { get; set; }
        internal static ConfigWrapper<NeckMode> DefaultNeckMode { get; set; }
        internal static ConfigWrapper<float> FemaleOffsetX { get; set; }
        internal static ConfigWrapper<float> FemaleOffsetY { get; set; }
        internal static ConfigWrapper<float> FemaleOffsetZ { get; set; }
        internal static ConfigWrapper<float> MaleOffsetX { get; set; }
        internal static ConfigWrapper<float> MaleOffsetY { get; set; }
        internal static ConfigWrapper<float> MaleOffsetZ { get; set; }

        private Harmony harmony;
        private static GameObject bepinex;

        private void Awake()
        {
            Logger = base.Logger;
            bepinex = gameObject;

            POVKey = Config.GetSetting(SECTION_HOTKEYS, "TogglePOVMode", new KeyboardShortcut(KeyCode.Backspace), new ConfigDescription(DESCRIPTION_POVHOTKEY));
            POVHotkey = new KeyboardShortcutHotkey(POVKey.Value, 0.3f);
            DefaultFOV = Config.GetSetting(SECTION_GENERAL, "DefaultFOV", 70f, new ConfigDescription("", new AcceptableValueRange<float>(20f, 120f)));
            MouseSensitivity = Config.GetSetting(SECTION_GENERAL, "MouseSensitivity", 1f, new ConfigDescription("", new AcceptableValueRange<float>(0.1f, 2f)));
            DefaultNeckMode = Config.GetSetting(SECTION_GENERAL, "DefaultRotationMode", NeckMode.Both);

            FemaleOffsetX = Config.GetSetting(SECTION_OFFSETS, "FemaleOffsetX", 0f, new ConfigDescription("", null, "Advanced"));
            FemaleOffsetY = Config.GetSetting(SECTION_OFFSETS, "FemaleOffsetY", 0.0315f, new ConfigDescription("", null, "Advanced"));
            FemaleOffsetZ = Config.GetSetting(SECTION_OFFSETS, "FemaleOffsetZ", 0f, new ConfigDescription("", null, "Advanced"));

            MaleOffsetX = Config.GetSetting(SECTION_OFFSETS, "MaleOffsetX", 0f, new ConfigDescription("", null, "Advanced"));
            MaleOffsetY = Config.GetSetting(SECTION_OFFSETS, "MaleOffsetY", 0.092f, new ConfigDescription("", null, "Advanced"));
            MaleOffsetZ = Config.GetSetting(SECTION_OFFSETS, "MaleOffsetZ", 0.12f, new ConfigDescription("", null, "Advanced"));

            harmony = new Harmony($"{GUID}.harmony");
            harmony.PatchAll(typeof(PointOfView));
            harmony.PatchAll(typeof(Hooks));
        }

#if DEBUG
        private void OnDestroy()
        {
            harmony.UnpatchAll(typeof(PointOfView));
            harmony.UnpatchAll(typeof(Hooks));
        } 
#endif

        private class Hooks
        {
            [HarmonyPostfix, HarmonyPatch(typeof(H_Scene), "Awake")]
            public static void HSceneInit()
            {
                bepinex.GetOrAddComponent<PointOfView>();
            }

            [HarmonyPostfix, HarmonyPatch(typeof(H_Scene), "OnDestroy")]
            public static void HSceneDestroy()
            {
                Destroy(bepinex.GetComponent<PointOfView>());
            }
        }
    }
}
