using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using KeelPlugins.Koikatu;
using KKAPI;
using KKAPI.Maker;
using UnityEngine;

[assembly: System.Reflection.AssemblyFileVersion(TesselationSetting.Koikatu.TesselationSetting.Version)]

namespace TesselationSetting.Koikatu
{
    [BepInProcess(Constants.MainGameProcessName)]
    [BepInDependency(KoikatuAPI.GUID)]
    [BepInPlugin(GUID, PluginName, Version)]
    public class TesselationSetting : BaseUnityPlugin
    {
        public const string GUID = "keelhauled.tesselationsetting";
        public const string PluginName = "TesselationSetting";
        public const string Version = "1.0.0." + BuildNumber.Version;

        public static ConfigEntry<float> TessSmooth { get; set; }

        private static TesselationSetting plugin;
        private static readonly List<Renderer> ManagedRenderers = new List<Renderer>();

        private void Awake()
        {
            plugin = this;
            Log.SetLogSource(Logger);

            TessSmooth = Config.Bind("General", "TessSmooth", 0f, new ConfigDescription("", new AcceptableValueRange<float>(0f, 1f)));

            KKAPI.Chara.CharacterApi.RegisterExtraBehaviour<CharaExtra>(GUID);
        }
    }
}
