using BepInEx;
using BepInEx.Harmony;
using HarmonyLib;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace KeelPlugins
{
    [BepInPlugin(GUID, PluginName, Version)]
    internal class SoundEffects : BaseUnityPlugin
    {
        public const string GUID = "keelhauled.soundeffects";
        public const string PluginName = "SoundEffects";
        public const string Version = "1.0.1";

        private Harmony harmony;
        private static List<AudioClip> slaps = new List<AudioClip>();

        private void Awake()
        {
            harmony = HarmonyWrapper.PatchAll(typeof(Hooks));

            var ass = Assembly.GetExecutingAssembly();
            var soundDir = Path.Combine(Path.GetDirectoryName(ass.Location), PluginName);
            foreach(var filePath in Directory.GetFiles(soundDir))
            {
                var clip = ExternalAudioClip.Load(filePath);
                slaps.Add(clip);
            }
        }

#if DEBUG
        private void OnDestroy()
        {
            harmony.UnpatchAll();
        }
#endif

        private class Hooks
        {
            [HarmonyPrefix, HarmonyPatch(typeof(H_SE), nameof(H_SE.Play_Piston))]
            public static bool CustomSound(H_SE __instance, Female female)
            {
                if(slaps.Count > 0)
                {
                    var random = Random.Range(0, slaps.Count - 1);
                    var gameCtrl = Traverse.Create(__instance).Field("gameCtrl").GetValue<GameControl>();
                    gameCtrl.audioCtrl.Play3DSE(slaps[random], female.CrotchTrans.position);
                    return false;
                }

                return true;
            }
        }
    }
}
