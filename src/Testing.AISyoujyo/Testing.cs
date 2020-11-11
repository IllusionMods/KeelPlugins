using BepInEx;
using HarmonyLib;

namespace Testing.AISyoujyo
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
