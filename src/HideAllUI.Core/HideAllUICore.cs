using BepInEx;
using BepInEx.Configuration;
using BepInEx.Harmony;
using HarmonyLib;
using UnityEngine;

namespace KeelPlugins
{
    public class HideAllUICore : BaseUnityPlugin
    {
        public const string GUID = "keelhauled.hideallui";

        // must be static for the transpiler
        protected static ConfigEntry<KeyboardShortcut> HideHotkey { get; set; }

        private Harmony harmony;
        internal static HideUIAction currentUIHandler;

        private void Update()
        {
            if(currentUIHandler != null && HideHotkey.Value.IsDown())
                currentUIHandler.ToggleUI();
        }

        private void Awake()
        {
            HideHotkey = Config.AddSetting("Keyboard shortcuts", "Hide UI", new KeyboardShortcut(KeyCode.Space));
            harmony = HarmonyWrapper.PatchAll();
        }

#if DEBUG
        private void OnDestroy()
        {
            harmony.UnpatchAll();
        }
#endif
    }
}
