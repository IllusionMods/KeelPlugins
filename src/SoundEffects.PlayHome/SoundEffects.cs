using BepInEx;
using HarmonyLib;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

[assembly: System.Reflection.AssemblyVersion(SoundEffects.PlayHome.SoundEffects.Version)]

namespace SoundEffects.PlayHome
{
    [BepInPlugin(GUID, PluginName, Version)]
    internal class SoundEffects : BaseUnityPlugin
    {
        public const string GUID = "keelhauled.soundeffects";
        public const string PluginName = "SoundEffects";
        public const string Version = "1.0.2." + BuildNumber.Version;

        private Harmony harmony;
        private static List<AudioClip> slaps = new List<AudioClip>();

        private void Awake()
        {
            harmony = Harmony.CreateAndPatchAll(GetType());
            var soundDir = Path.Combine(Path.GetDirectoryName(Info.Location), PluginName);
            slaps = Directory.GetFiles(soundDir, "*.wav").Select(ExternalAudioClip.Load).ToList();
        }

#if DEBUG
        private void OnDestroy()
        {
            harmony.UnpatchSelf();
        }
#endif

        [HarmonyPrefix, HarmonyPatch(typeof(H_SE), nameof(H_SE.Play_Piston))]
        private static bool CustomSound(H_SE __instance, Female female)
        {
            if(slaps.Count > 0)
            {
                var random = Random.Range(0, slaps.Count - 1);
                __instance.gameCtrl.audioCtrl.Play3DSE(slaps[random], female.CrotchTrans.position);
                return false;
            }

            return true;
        }
    }
}
