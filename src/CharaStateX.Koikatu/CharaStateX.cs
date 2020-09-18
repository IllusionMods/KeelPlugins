using BepInEx;
using HarmonyLib;

namespace KeelPlugins
{
    [BepInProcess(KoikatuConstants.StudioProcessName)]
    [BepInPlugin(GUID, "CharaStateX", Version)]
    public class CharaStateX : BaseUnityPlugin
    {
        public const string GUID = "keelhauled.charastatex";
        public const string Version = "1.0.2." + BuildNumber.Version;

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
