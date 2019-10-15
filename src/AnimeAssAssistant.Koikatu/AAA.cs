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

        private const string SECTION_HOTKEY = "Keyboard shortcuts";
        private const string SECTION_FOLDER = "Folders";

        private const string DESCRIPTION_RYCYCLEBIN = "Moves the current character to the recycle bin";
        private const string DESCRIPTION_SAVECHARA = "Moves the current character to the save folder";

        internal static ConfigEntry<KeyboardShortcut> HotkeyOutfit1 { get; set; }
        internal static ConfigEntry<KeyboardShortcut> HotkeyOutfit2 { get; set; }
        internal static ConfigEntry<KeyboardShortcut> HotkeyOutfit3 { get; set; }
        internal static ConfigEntry<KeyboardShortcut> HotkeyOutfit4 { get; set; }
        internal static ConfigEntry<KeyboardShortcut> HotkeyOutfit5 { get; set; }
        internal static ConfigEntry<KeyboardShortcut> HotkeyOutfit6 { get; set; }
        internal static ConfigEntry<KeyboardShortcut> HotkeyOutfit7 { get; set; }

        internal static ConfigEntry<KeyboardShortcut> HotkeyKill { get; set; }
        internal static ConfigEntry<KeyboardShortcut> HotkeyNext { get; set; }
        internal static ConfigEntry<KeyboardShortcut> HotkeyPrev { get; set; }
        internal static ConfigEntry<KeyboardShortcut> HotkeySave { get; set; }

        internal static ConfigEntry<string> SearchFolder { get; set; }
        internal static ConfigEntry<string> SaveFolder { get; set; }

        internal static bool EnableAAA;

        private void Awake()
        {
            Logger = base.Logger;

            HotkeyNext = Config.AddSetting(SECTION_HOTKEY, "Select next character", new KeyboardShortcut(KeyCode.RightArrow), new ConfigDescription("", null, new ConfigurationManagerAttributes { Order = 10 }));
            HotkeyPrev = Config.AddSetting(SECTION_HOTKEY, "Select previous character", new KeyboardShortcut(KeyCode.LeftArrow), new ConfigDescription("", null, new ConfigurationManagerAttributes { Order = 9 }));
            HotkeyKill = Config.AddSetting(SECTION_HOTKEY, "Move character to recycle bin", new KeyboardShortcut(KeyCode.DownArrow), new ConfigDescription(DESCRIPTION_RYCYCLEBIN, null, new ConfigurationManagerAttributes { Order = 8 }));
            HotkeySave = Config.AddSetting(SECTION_HOTKEY, "Move character to save folder", new KeyboardShortcut(KeyCode.UpArrow), new ConfigDescription(DESCRIPTION_SAVECHARA, null, new ConfigurationManagerAttributes { Order = 7 }));

            HotkeyOutfit1 = Config.AddSetting(SECTION_HOTKEY, "Select outfit 1", new KeyboardShortcut(KeyCode.Alpha1));
            HotkeyOutfit2 = Config.AddSetting(SECTION_HOTKEY, "Select outfit 2", new KeyboardShortcut(KeyCode.Alpha2));
            HotkeyOutfit3 = Config.AddSetting(SECTION_HOTKEY, "Select outfit 3", new KeyboardShortcut(KeyCode.Alpha3));
            HotkeyOutfit4 = Config.AddSetting(SECTION_HOTKEY, "Select outfit 4", new KeyboardShortcut(KeyCode.Alpha4));
            HotkeyOutfit5 = Config.AddSetting(SECTION_HOTKEY, "Select outfit 5", new KeyboardShortcut(KeyCode.Alpha5));
            HotkeyOutfit6 = Config.AddSetting(SECTION_HOTKEY, "Select outfit 6", new KeyboardShortcut(KeyCode.Alpha6));
            HotkeyOutfit7 = Config.AddSetting(SECTION_HOTKEY, "Select outfit 7", new KeyboardShortcut(KeyCode.Alpha7));

            SearchFolder = Config.AddSetting(SECTION_FOLDER, "Search folder path", "", new ConfigDescription("The folder where the plugin draws characters from"));
            SaveFolder = Config.AddSetting(SECTION_FOLDER, "Save folder path", "", new ConfigDescription("The folder where characters are saved to when pressing the save hotkey"));
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
