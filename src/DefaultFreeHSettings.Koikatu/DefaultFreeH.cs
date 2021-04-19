using System;
using System.Collections;
using System.Collections.Generic;
using BepInEx;
using HarmonyLib;

namespace DefaultFreeHSettings.Koikatu
{
    [BepInPlugin(GUID, "DefaultFreeHSettings", Version)]
    public class DefaultFreeH : BaseUnityPlugin
    {
        public const string GUID = "keelhauled.defaultfreehsettings";
        public const string Version = "1.0.0." + BuildNumber.Version;

        private Harmony harmony;
        private static DefaultFreeH plugin;

        private void Awake()
        {
            plugin = this;
            Log.SetLogSource(Logger);
            harmony = Harmony.CreateAndPatchAll(typeof(DefaultFreeH));
        }

        private void OnDestroy()
        {
            harmony.UnpatchSelf();
        }

        [HarmonyPostfix, HarmonyPatch(typeof(FreeHScene), "Start")]
        private static void FreeHStart(FreeHScene __instance)
        {
            plugin.StartCoroutine(coroutineTest(__instance));
        }

        private static IEnumerator coroutineTest(FreeHScene __instance)
        {
            yield return null;
            yield return null;
            yield return null;
            yield return null;
            yield return null;
            
            var traverse = Traverse.Create(__instance);
            var member = traverse.Field("member").GetValue<FreeHScene.Member>();
            
            var chaFileControl = new ChaFileControl();
            chaFileControl.LoadCharaFile(@"C:\games\Koikatu\UserData\chara\female\_めぐみん_2019_0124_1955_42_062.png", 1, false, true);
            member.resultHeroine.Value = new SaveData.Heroine(chaFileControl, false);
        }
    }
}