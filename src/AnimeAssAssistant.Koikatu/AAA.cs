using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using KKAPI.Maker;
using KKAPI.Maker.UI.Sidebar;
using UniRx;
using UnityEngine;

namespace KeelPlugins
{
    [BepInDependency(KKAPI.KoikatuAPI.GUID)]
    [BepInProcess(KoikatuConstants.MainGameProcessName)]
    [BepInProcess(KoikatuConstants.VRProcessName)]
    [BepInProcess(KoikatuConstants.MainGameProcessNameSteam)]
    [BepInProcess(KoikatuConstants.VRProcessNameSteam)]
    [BepInPlugin(GUID, "Anime Ass Assistant", Version)]
    public class AAA : BaseUnityPlugin
    {
        public const string GUID = "keelhauled.animeassassistant";
        public const string Version = "1.0.0";
        internal static new ManualLogSource Logger;

        private const string SECTION_HOTKEY = "Keyboard Shortcuts";
        private const string SECTION_FOLDER = "Folders";

        internal static ConfigWrapper<KeyboardShortcut> HotkeyOutfit1 { get; set; }
        internal static ConfigWrapper<KeyboardShortcut> HotkeyOutfit2 { get; set; }
        internal static ConfigWrapper<KeyboardShortcut> HotkeyOutfit3 { get; set; }
        internal static ConfigWrapper<KeyboardShortcut> HotkeyOutfit4 { get; set; }
        internal static ConfigWrapper<KeyboardShortcut> HotkeyOutfit5 { get; set; }
        internal static ConfigWrapper<KeyboardShortcut> HotkeyOutfit6 { get; set; }
        internal static ConfigWrapper<KeyboardShortcut> HotkeyOutfit7 { get; set; }

        internal static ConfigWrapper<KeyboardShortcut> HotkeyKill { get; set; }
        internal static ConfigWrapper<KeyboardShortcut> HotkeyNext { get; set; }
        internal static ConfigWrapper<KeyboardShortcut> HotkeyPrev { get; set; }
        internal static ConfigWrapper<KeyboardShortcut> HotkeySave { get; set; }

        internal static ConfigWrapper<string> SearchFolder { get; set; }
        internal static ConfigWrapper<string> SaveFolder { get; set; }

        internal static bool EnableAAA;

        private void Awake()
        {
            Logger = base.Logger;

            HotkeyNext = Config.GetSetting(SECTION_HOTKEY, "Select next character", new KeyboardShortcut(KeyCode.RightArrow));
            HotkeyPrev = Config.GetSetting(SECTION_HOTKEY, "Select previous character", new KeyboardShortcut(KeyCode.LeftArrow));
            HotkeyKill = Config.GetSetting(SECTION_HOTKEY, "Move character to recycle bin", new KeyboardShortcut(KeyCode.DownArrow), new ConfigDescription("Moves the current character to the recycle bin"));
            HotkeySave = Config.GetSetting(SECTION_HOTKEY, "Move character to save folder", new KeyboardShortcut(KeyCode.UpArrow), new ConfigDescription("Moves the current character to the save folder"));

            HotkeyOutfit1 = Config.GetSetting(SECTION_HOTKEY, "Select outfit 1", new KeyboardShortcut(KeyCode.Alpha1));
            HotkeyOutfit2 = Config.GetSetting(SECTION_HOTKEY, "Select outfit 2", new KeyboardShortcut(KeyCode.Alpha2));
            HotkeyOutfit3 = Config.GetSetting(SECTION_HOTKEY, "Select outfit 3", new KeyboardShortcut(KeyCode.Alpha3));
            HotkeyOutfit4 = Config.GetSetting(SECTION_HOTKEY, "Select outfit 4", new KeyboardShortcut(KeyCode.Alpha4));
            HotkeyOutfit5 = Config.GetSetting(SECTION_HOTKEY, "Select outfit 5", new KeyboardShortcut(KeyCode.Alpha5));
            HotkeyOutfit6 = Config.GetSetting(SECTION_HOTKEY, "Select outfit 6", new KeyboardShortcut(KeyCode.Alpha6));
            HotkeyOutfit7 = Config.GetSetting(SECTION_HOTKEY, "Select outfit 7", new KeyboardShortcut(KeyCode.Alpha7));

            SearchFolder = Config.GetSetting(SECTION_FOLDER, "Search folder path", "", new ConfigDescription("The folder where the plugin draws characters from"));
            SaveFolder = Config.GetSetting(SECTION_FOLDER, "Save folder path", "", new ConfigDescription("The folder where characters are saved to when pressing the save hotkey"));
        }

        private void Start()
        {
            MakerAPI.RegisterCustomSubCategories += (sender, e) =>
            {
                EnableAAA = false;
                var toggle = new SidebarToggle("Anime Ass Assistant", false, this);
                e.AddSidebarControl(toggle).ValueChanged.Subscribe(x => EnableAAA = x);
                gameObject.GetOrAddComponent<Assistant>();
            };

            MakerAPI.MakerExiting += (sender, e) => Destroy(gameObject.GetComponent<Assistant>());
        }
    }
}
