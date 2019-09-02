using BepInEx;
using BepInEx.Harmony;
using HarmonyLib;

namespace KeelPlugins
{
    [BepInProcess(KoikatuConstants.KoikatuStudioProcessName)]
    [BepInPlugin(GUID, "CharaStateX", Version)]
    public class CharaStateX : BaseUnityPlugin
    {
        public const string GUID = "keelhauled.charastatex";
        public const string Version = "1.0.1";

        private void Awake()
        {
            var harmony = new Harmony("keelhauled.charastatex.harmony");
            HarmonyWrapper.PatchAll(typeof(AnimationPatch), harmony);
            StateInfoPatch.Patch(harmony);
            NeckLookPatch.Patch(harmony);
            EtcInfoPatch.Patch(harmony);
            HandInfoPatch.Patch(harmony);
            JointInfoPatch.Patch(harmony);
            FKIKPatch.Patch(harmony);
        }
    }
}
