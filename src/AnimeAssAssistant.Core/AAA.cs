using BepInEx;
using BepInEx.Configuration;
using KeelPlugins;
using KeelPlugins.Utils;
using KKAPI.Maker;
using KKAPI.Maker.UI.Sidebar;
using UniRx;
using UnityEngine;

[assembly: System.Reflection.AssemblyFileVersion(AnimeAssAssistant.AAA.Version)]

namespace AnimeAssAssistant
{
    [BepInDependency(KKAPI.KoikatuAPI.GUID)]
    [BepInProcess(Constants.MainGameProcessName)]
    [BepInProcess(Constants.VRProcessName)]
    [BepInProcess(Constants.MainGameProcessNameSteam)]
    [BepInProcess(Constants.VRProcessNameSteam)]
    [BepInPlugin(GUID, "Anime Ass Assistant", Version)]
    public class AAA : BaseUnityPlugin
    {
        public const string GUID = "keelhauled.animeassassistant";
        public const string Version = "1.0.2." + BuildNumber.Version;

        private const string SECTION_HOTKEY = "Keyboard shortcuts";
        private const string SECTION_FOLDER = "Folders";
        private const string DESCRIPTION_RYCYCLEBIN = "Moves the current character to the recycle bin";
        private const string DESCRIPTION_SAVECHARA = "Moves the current character to the save folder";

        internal static ConfigEntry<KeyboardShortcut> HotkeyKill { get; set; }
        internal static ConfigEntry<KeyboardShortcut> HotkeyNext { get; set; }
        internal static ConfigEntry<KeyboardShortcut> HotkeyPrev { get; set; }
        internal static ConfigEntry<KeyboardShortcut> HotkeySave { get; set; }
        internal static ConfigEntry<string> SearchFolder { get; set; }
        internal static ConfigEntry<string> SaveFolder { get; set; }

        internal static bool EnableAAA;

        private void Awake()
        {
            Log.SetLogSource(Logger);

            HotkeyNext = Config.Bind(SECTION_HOTKEY, "Select next character", new KeyboardShortcut(KeyCode.RightArrow), new ConfigDescription("", null, new ConfigurationManagerAttributes { Order = 10 }));
            HotkeyPrev = Config.Bind(SECTION_HOTKEY, "Select previous character", new KeyboardShortcut(KeyCode.LeftArrow), new ConfigDescription("", null, new ConfigurationManagerAttributes { Order = 9 }));
            HotkeyKill = Config.Bind(SECTION_HOTKEY, "Move character to recycle bin", new KeyboardShortcut(KeyCode.DownArrow), new ConfigDescription(DESCRIPTION_RYCYCLEBIN, null, new ConfigurationManagerAttributes { Order = 8 }));
            HotkeySave = Config.Bind(SECTION_HOTKEY, "Move character to save folder", new KeyboardShortcut(KeyCode.UpArrow), new ConfigDescription(DESCRIPTION_SAVECHARA, null, new ConfigurationManagerAttributes { Order = 7 }));

            SearchFolder = Config.Bind(SECTION_FOLDER, "Search folder path", "", new ConfigDescription("The folder where the plugin draws characters from"));
            SaveFolder = Config.Bind(SECTION_FOLDER, "Save folder path", "", new ConfigDescription("The folder where characters are saved to when pressing the save hotkey"));
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
