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

        private const string DESCRIPTION_SENDCHARA = "Sends the selected character to the other open koikatu application.";
        private const string DESCRIPTION_SHOWMSG = "Show on screen messages about things the plugin is doing.";

        internal static string MakerCardPath;
        internal static string OtherCardPath;
        protected static GameObject bepinex;

        internal static ConfigEntry<KeyboardShortcut> SendChara { get; set; }
        internal static ConfigEntry<bool> ShowMessages { get; set; }

        private void Awake()
        {
            Logger = base.Logger;
            bepinex = gameObject;

            SendChara = Config.AddSetting("Keyboard shortcuts", "Send character", new KeyboardShortcut(KeyCode.B), new ConfigDescription(DESCRIPTION_SENDCHARA));
            ShowMessages = Config.AddSetting("General", "Show messages", true, new ConfigDescription(DESCRIPTION_SHOWMSG));

            var tempFolder = Path.GetTempPath();
            MakerCardPath = Path.Combine(tempFolder, "makerbridge1.png");
            OtherCardPath = Path.Combine(tempFolder, "makerbridge2.png");

            HarmonyWrapper.PatchAll();
        }

        internal static void Log(object data)
        {
            Logger.Log(ShowMessages.Value ? LogLevel.Message : LogLevel.Info, data);
        }
    }
}
