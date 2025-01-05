using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using System.IO;
using UnityEngine;
using KKAPI.Maker;
using KKAPI.Studio;

[assembly: System.Reflection.AssemblyVersion(MakerBridge.MakerBridge.Version)]

namespace MakerBridge
{
    [BepInPlugin(GUID, PluginName, Version)]
    [BepInDependency(KKAPI.KoikatuAPI.GUID)]
    public class MakerBridge : BaseUnityPlugin
    {
        public const string GUID = "keelhauled.makerbridge";
        public const string PluginName = "MakerBridge";
        public const string Version = "1.0.5." + BuildNumber.Version;

        private const string DESCRIPTION_SENDCHARA = "Sends the selected character to the other open koikatu application.";
        private const string DESCRIPTION_SHOWMSG = "Show on screen messages about things the plugin is doing.";

        internal static string MakerCardPath;
        internal static string OtherCardPath;
        protected static GameObject bepinex;

        internal static ConfigEntry<KeyboardShortcut> SendChara { get; set; }
        internal static ConfigEntry<bool> ShowMessages { get; set; }

        protected virtual void Awake()
        {
            Log.SetLogSource(Logger);
            bepinex = gameObject;

            SendChara = Config.Bind("Keyboard shortcuts", "Send character", new KeyboardShortcut(KeyCode.B), new ConfigDescription(DESCRIPTION_SENDCHARA));
            ShowMessages = Config.Bind("General", "Show messages", true, new ConfigDescription(DESCRIPTION_SHOWMSG));

            MakerCardPath = Path.Combine(Paths.CachePath, "makerbridge1.png");
            OtherCardPath = Path.Combine(Paths.CachePath, "makerbridge2.png");

            MakerAPI.MakerFinishedLoading += (sender, e) => bepinex.GetOrAddComponent<MakerHandler>();
            MakerAPI.MakerExiting += (sender, e) => Destroy(bepinex.GetComponent<MakerHandler>());
            StudioAPI.StudioLoadedChanged += (sender, e) => bepinex.GetOrAddComponent<StudioHandler>();
        }

        internal static void LogMsg(object data)
        {
            Log.Level(ShowMessages.Value ? LogLevel.Message : LogLevel.Info, data);
        }
    }
}
