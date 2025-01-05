using BepInEx;
using HarmonyLib;
using KeelPlugins;

[assembly: System.Reflection.AssemblyVersion(CharaStateX.Koikatu.CharaStateX.Version)]

namespace CharaStateX.Koikatu
{
    [BepInProcess(Constants.StudioProcessName)]
    [BepInPlugin(GUID, "CharaStateX", Version)]
    public class CharaStateX : BaseUnityPlugin
    {
        public const string GUID = "keelhauled.charastatex";
        public const string Version = "1.1.2." + BuildNumber.Version;

        private void Awake()
        {
            var harmony = new Harmony("keelhauled.charastatex.harmony");
            harmony.PatchAll(typeof(AnimationPatch));
            StateInfoPatch.Patch(harmony);
            NeckLookPatch.Patch(harmony);
            EtcInfoPatch.Patch(harmony);
            HandInfoPatch.Patch(harmony);
            JointInfoPatch.Patch(harmony);
            FKIKPatch.Patch(harmony);
        }
    }
}
