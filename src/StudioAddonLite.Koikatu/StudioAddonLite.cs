using BepInEx;
using BepInEx.Configuration;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace KeelPlugins
{
    [BepInProcess(KoikatuConstants.StudioProcessName)]
    [BepInPlugin(GUID, "StudioAddonLite", Version)]
    public class StudioAddonLite : BaseUnityPlugin
    {
        public const string GUID = "studioaddonlite";
        public const string Version = "1.0.0";

        private const string SECTION_GENERAL = "General";
        private const string SECTION_HOTKEYS = "Keyboard Shortcuts";

        internal static ConfigWrapper<float> MOVE_RATIO { get; set; }
        internal static ConfigWrapper<float> ROTATE_RATIO { get; set; }
        internal static ConfigWrapper<float> SCALE_RATIO { get; set; }

        internal static ConfigWrapper<KeyboardShortcut> KEY_OBJ_MOVE_XZ { get; set; }
        internal static ConfigWrapper<KeyboardShortcut> KEY_OBJ_MOVE_Y { get; set; }

        internal static ConfigWrapper<KeyboardShortcut> KEY_OBJ_ROT_X { get; set; }
        internal static ConfigWrapper<KeyboardShortcut> KEY_OBJ_ROT_Y { get; set; }
        internal static ConfigWrapper<KeyboardShortcut> KEY_OBJ_ROT_Z { get; set; }

        internal static ConfigWrapper<KeyboardShortcut> KEY_OBJ_ROT_X_2 { get; set; }
        internal static ConfigWrapper<KeyboardShortcut> KEY_OBJ_ROT_Y_2 { get; set; }
        internal static ConfigWrapper<KeyboardShortcut> KEY_OBJ_ROT_Z_2 { get; set; }

        internal static ConfigWrapper<KeyboardShortcut> KEY_OBJ_SCALE_ALL { get; set; }
        internal static ConfigWrapper<KeyboardShortcut> KEY_OBJ_SCALE_X { get; set; }
        internal static ConfigWrapper<KeyboardShortcut> KEY_OBJ_SCALE_Y { get; set; }
        internal static ConfigWrapper<KeyboardShortcut> KEY_OBJ_SCALE_Z { get; set; }

        private StudioAddonLite()
        {
            MOVE_RATIO = Config.GetSetting(SECTION_GENERAL, "MOVE_RATIO", 2.5f, new ConfigDescription("", new AcceptableValueRange<float>(0f, 10f)));
            ROTATE_RATIO = Config.GetSetting(SECTION_GENERAL, "ROTATE_RATIO", 90f, new ConfigDescription("", new AcceptableValueRange<float>(0f, 360f)));
            SCALE_RATIO = Config.GetSetting(SECTION_GENERAL, "SCALE_RATIO", 0.5f, new ConfigDescription("", new AcceptableValueRange<float>(0f, 2f)));

            KEY_OBJ_MOVE_XZ = Config.GetSetting(SECTION_HOTKEYS, "KEY_OBJ_MOVE_XZ", new KeyboardShortcut(KeyCode.G));
            KEY_OBJ_MOVE_Y = Config.GetSetting(SECTION_HOTKEYS, "KEY_OBJ_MOVE_Y", new KeyboardShortcut(KeyCode.H));

            KEY_OBJ_ROT_X = Config.GetSetting(SECTION_HOTKEYS, "KEY_OBJ_ROT_X", new KeyboardShortcut(KeyCode.G, KeyCode.LeftShift));
            KEY_OBJ_ROT_Y = Config.GetSetting(SECTION_HOTKEYS, "KEY_OBJ_ROT_Y", new KeyboardShortcut(KeyCode.H, KeyCode.LeftShift));
            KEY_OBJ_ROT_Z = Config.GetSetting(SECTION_HOTKEYS, "KEY_OBJ_ROT_Z", new KeyboardShortcut(KeyCode.Y, KeyCode.LeftShift));
            KEY_OBJ_ROT_X_2 = Config.GetSetting(SECTION_HOTKEYS, "KEY_OBJ_ROT_X_2", new KeyboardShortcut(KeyCode.G));
            KEY_OBJ_ROT_Y_2 = Config.GetSetting(SECTION_HOTKEYS, "KEY_OBJ_ROT_Y_2", new KeyboardShortcut(KeyCode.H));
            KEY_OBJ_ROT_Z_2 = Config.GetSetting(SECTION_HOTKEYS, "KEY_OBJ_ROT_Z_2", new KeyboardShortcut(KeyCode.Y));

            KEY_OBJ_SCALE_ALL = Config.GetSetting(SECTION_HOTKEYS, "KEY_OBJ_SCALE_ALL", new KeyboardShortcut(KeyCode.T));
            KEY_OBJ_SCALE_X = Config.GetSetting(SECTION_HOTKEYS, "KEY_OBJ_SCALE_X", new KeyboardShortcut(KeyCode.G));
            KEY_OBJ_SCALE_Y = Config.GetSetting(SECTION_HOTKEYS, "KEY_OBJ_SCALE_Y", new KeyboardShortcut(KeyCode.H));
            KEY_OBJ_SCALE_Z = Config.GetSetting(SECTION_HOTKEYS, "KEY_OBJ_SCALE_Z", new KeyboardShortcut(KeyCode.Y));
        }

        private void Awake()
        {
            SceneManager.sceneLoaded += SceneLoaded;
        }

#if DEBUG
        private void OnDestroy() // for ScriptEngine
        {
            SceneManager.sceneLoaded -= SceneLoaded;
        }
#endif

        private bool done = false;

        private void SceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if(!done && FindObjectOfType<StudioScene>())
            {
                done = true;
                gameObject.AddComponent<ObjMoveRotAssistMgr>();
            }
        }
    }
}
