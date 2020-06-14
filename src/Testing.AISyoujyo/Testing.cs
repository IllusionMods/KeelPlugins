using BepInEx;
using BepInEx.Harmony;
using HarmonyLib;

namespace KeelPlugins
{
    [BepInPlugin("keelhauled.testing", "TESTINGPLUGIN", "1.0.0")]
    public class Testing : BaseUnityPlugin
    {
        private Harmony harmony;

        private void Awake()
        {
            harmony = HarmonyWrapper.PatchAll(GetType());
        }

        private void OnDestroy()
        {
            harmony.UnpatchAll();
        }
    }
}
