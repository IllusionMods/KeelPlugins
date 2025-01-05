using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using KeelPlugins;
using UnityEngine;

[assembly: System.Reflection.AssemblyVersion(StudioAddonLite.Koikatu.StudioAddonLite.Version)]

namespace StudioAddonLite.Koikatu
{
    [BepInProcess(Constants.StudioProcessName)]
    [BepInPlugin(GUID, "StudioAddonLite", Version)]
    public class StudioAddonLite : BaseUnityPlugin
    {
        public const string GUID = "keelhauled.studioaddonlite";
        public const string Version = "1.1.1." + BuildNumber.Version;

        private const string SECTION_GENERAL = "General";
        private const string SECTION_HOTKEYS = "Keyboard shortcuts";

        internal static ConfigEntry<float> MOVE_RATIO { get; set; }
        internal static ConfigEntry<float> ROTATE_RATIO { get; set; }
        internal static ConfigEntry<float> SCALE_RATIO { get; set; }

        internal static ConfigEntry<KeyboardShortcut> KEY_OBJ_MOVE_XZ { get; set; }
        internal static ConfigEntry<KeyboardShortcut> KEY_OBJ_MOVE_Y { get; set; }

        internal static ConfigEntry<KeyboardShortcut> KEY_OBJ_ROT_X { get; set; }
        internal static ConfigEntry<KeyboardShortcut> KEY_OBJ_ROT_Y { get; set; }
        internal static ConfigEntry<KeyboardShortcut> KEY_OBJ_ROT_Z { get; set; }

        internal static ConfigEntry<KeyboardShortcut> KEY_OBJ_ROT_X_2 { get; set; }
        internal static ConfigEntry<KeyboardShortcut> KEY_OBJ_ROT_Y_2 { get; set; }
        internal static ConfigEntry<KeyboardShortcut> KEY_OBJ_ROT_Z_2 { get; set; }

        internal static ConfigEntry<KeyboardShortcut> KEY_OBJ_SCALE_ALL { get; set; }
        internal static ConfigEntry<KeyboardShortcut> KEY_OBJ_SCALE_X { get; set; }
        internal static ConfigEntry<KeyboardShortcut> KEY_OBJ_SCALE_Y { get; set; }
        internal static ConfigEntry<KeyboardShortcut> KEY_OBJ_SCALE_Z { get; set; }

        private static GameObject bepinex;

        private void Awake()
        {
            MOVE_RATIO = Config.Bind(SECTION_GENERAL, "Move ratio", 2.5f, new ConfigDescription("", new AcceptableValueRange<float>(0f, 10f)));
            ROTATE_RATIO = Config.Bind(SECTION_GENERAL, "Rotate ratio", 90f, new ConfigDescription("", new AcceptableValueRange<float>(0f, 360f)));
            SCALE_RATIO = Config.Bind(SECTION_GENERAL, "Scaling ratio", 0.5f, new ConfigDescription("", new AcceptableValueRange<float>(0f, 2f)));

            KEY_OBJ_MOVE_XZ = Config.Bind(SECTION_HOTKEYS, "Move XZ", new KeyboardShortcut(KeyCode.G));
            KEY_OBJ_MOVE_Y = Config.Bind(SECTION_HOTKEYS, "Move Y", new KeyboardShortcut(KeyCode.H));

            KEY_OBJ_ROT_X = Config.Bind(SECTION_HOTKEYS, "Rotate X (Local)", new KeyboardShortcut(KeyCode.G, KeyCode.LeftShift));
            KEY_OBJ_ROT_Y = Config.Bind(SECTION_HOTKEYS, "Rotate Y (Local)", new KeyboardShortcut(KeyCode.H, KeyCode.LeftShift));
            KEY_OBJ_ROT_Z = Config.Bind(SECTION_HOTKEYS, "Rotate Z (Local)", new KeyboardShortcut(KeyCode.Y, KeyCode.LeftShift));
            KEY_OBJ_ROT_X_2 = Config.Bind(SECTION_HOTKEYS, "Rotate X (World)", new KeyboardShortcut(KeyCode.G));
            KEY_OBJ_ROT_Y_2 = Config.Bind(SECTION_HOTKEYS, "Rotate Y (World)", new KeyboardShortcut(KeyCode.H));
            KEY_OBJ_ROT_Z_2 = Config.Bind(SECTION_HOTKEYS, "Rotate Z (World)", new KeyboardShortcut(KeyCode.Y));

            KEY_OBJ_SCALE_ALL = Config.Bind(SECTION_HOTKEYS, "Scale All", new KeyboardShortcut(KeyCode.T));
            KEY_OBJ_SCALE_X = Config.Bind(SECTION_HOTKEYS, "Scale X", new KeyboardShortcut(KeyCode.G));
            KEY_OBJ_SCALE_Y = Config.Bind(SECTION_HOTKEYS, "Scale Y", new KeyboardShortcut(KeyCode.H));
            KEY_OBJ_SCALE_Z = Config.Bind(SECTION_HOTKEYS, "Scale Z", new KeyboardShortcut(KeyCode.Y));

            bepinex = gameObject;
            Harmony.CreateAndPatchAll(GetType());
        }

        [HarmonyPrefix, HarmonyPatch(typeof(StudioScene), "Start")]
        private static void StudioEntrypoint()
        {
            bepinex.GetOrAddComponent<ObjMoveRotAssistMgr>();
        }
    }
}
