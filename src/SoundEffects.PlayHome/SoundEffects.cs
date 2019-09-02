using BepInEx;
using HarmonyLib;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Random = UnityEngine.Random;

namespace KeelPlugins
{
    [BepInPlugin(GUID, PluginName, Version)]
    internal class SoundLoader : BaseUnityPlugin
    {
        public const string GUID = "keelhauled.soundloader";
        public const string PluginName = "SoundLoader";
        public const string Version = "1.0.0";

        private Harmony harmony;
        private static List<AudioClip> slaps = new List<AudioClip>();

        private void Start()
        {
            harmony = new Harmony($"{GUID}.harmony");
            harmony.PatchAll(typeof(Hooks));

            var soundDir = Path.Combine(Paths.PluginPath, PluginName);
            foreach(var filePath in Directory.GetFiles(soundDir))
            {
                var clip = ExternalAudioClip.Load(filePath);
                slaps.Add(clip);
            }
        }

#if DEBUG
        private void OnDestroy()
        {
            harmony.UnpatchAll(typeof(Hooks));
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
