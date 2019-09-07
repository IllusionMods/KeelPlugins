using BepInEx;
using BepInEx.Configuration;
using BepInEx.Harmony;
using BepInEx.Logging;
using System.IO;
using UnityEngine;

namespace KeelPlugins
{
    public class MakerBridgeCore : BaseUnityPlugin
    {
        public const string GUID = "keelhauled.makerbridge";
        public const string PluginName = "MakerBridge";
        internal static new ManualLogSource Logger;

        internal static string MakerCardPath;
        internal static string OtherCardPath;
        protected static GameObject bepinex;

        internal static ConfigWrapper<KeyboardShortcut> SendChara { get; set; }

        private void Awake()
        {
            Logger = base.Logger;
            bepinex = gameObject;

            SendChara = Config.GetSetting("Keyboard Shortcuts", "Send character", new KeyboardShortcut(KeyCode.B));
            
            MakerCardPath = Path.Combine(Paths.ConfigPath, "makerbridge1.png");
            OtherCardPath = Path.Combine(Paths.ConfigPath, "makerbridge2.png");

            HarmonyWrapper.PatchAll();
        }
    }
}
