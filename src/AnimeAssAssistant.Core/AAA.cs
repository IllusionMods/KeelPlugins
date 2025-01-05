using BepInEx;
using BepInEx.Configuration;
using KeelPlugins;
using KeelPlugins.Utils;
using KKAPI.Maker;
using KKAPI.Maker.UI.Sidebar;
using UniRx;
using UnityEngine;
using HarmonyLib;
using ChaCustom;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Linq;

[assembly: System.Reflection.AssemblyVersion(AnimeAssAssistant.AAA.Version)]

namespace AnimeAssAssistant
{
    [BepInDependency(KKAPI.KoikatuAPI.GUID)]
    [BepInProcess(Constants.MainGameProcessName)]
    [BepInProcess(Constants.VRProcessName)]
    [BepInPlugin(GUID, "Anime Ass Assistant", Version)]
    public class AAA : BaseUnityPlugin
    {
        public const string GUID = "keelhauled.animeassassistant";
        public const string Version = "1.1.0." + BuildNumber.Version;

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

            SearchFolder.SettingChanged += (x, y) =>
            {
                var ass = gameObject.GetComponent<Assistant>();
                if(ass != null) ass.ClearLoadedCharas();
            };
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

            Harmony.CreateAndPatchAll(typeof(Hooks));
        }

        private class Hooks
        {
            [HarmonyPostfix, HarmonyPatch(typeof(CustomCharaFile), nameof(CustomCharaFile.OnChangeSelect)), HarmonyWrapSafe]
            private static void OnChangeSelect()
            {
                FindObjectOfType<Assistant>().ClearLoadedCharas();
            }

            [HarmonyTranspiler, HarmonyPatch(typeof(BaseCameraControl_Ver2), nameof(BaseCameraControl_Ver2.InputKeyProc)), HarmonyWrapSafe]
            private static IEnumerable<CodeInstruction> CameraControlBlock(IEnumerable<CodeInstruction> instructions)
            {
                return new CodeMatcher(instructions)
                    // match all arrow key KeyCode
                    .MatchForward(false, new CodeMatch(i => i.opcode == OpCodes.Ldc_I4 && new[]{273, 274, 275, 276}.Contains((int)i.operand)))
                    .Repeat(matcher =>
                    {
                        matcher.Advance(2);
                        // save GetKeyDown label
                        var label = matcher.Operand;
                        matcher.Advance(1);
                        // add branch to the same label
                        matcher.Insert(new CodeInstruction(OpCodes.Ldsfld, AccessTools.Field(typeof(AAA), nameof(EnableAAA))),
                                       new CodeInstruction(OpCodes.Brtrue, label));
                    }).InstructionEnumeration();
            }
        }
    }
}
