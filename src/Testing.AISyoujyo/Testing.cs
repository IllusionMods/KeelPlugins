using BepInEx;
using HarmonyLib;

namespace KeelPlugins
{
    [BepInPlugin("keelhauled.testing", "TESTINGPLUGIN", "1.0.0")]
    public class Testing : BaseUnityPlugin
    {
        private Harmony harmony;

        private void Awake()
        {
            harmony = Harmony.CreateAndPatchAll(GetType());
        }

        private void OnDestroy()
        {
            harmony.UnpatchAll();
        }
    }
}
