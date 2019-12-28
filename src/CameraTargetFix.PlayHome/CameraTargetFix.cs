using BepInEx;
using HarmonyLib;

namespace KeelPlugins
{
    [BepInProcess(PlayHomeConstants.StudioProcessName64bit)]
    [BepInProcess(PlayHomeConstants.StudioProcessName32bit)]
    [BepInPlugin(GUID, PluginName, Version)]
    public class CameraTargetFix : CameraTargetFixCore
    {
        protected override void Awake()
        {
            base.Awake();

            Harmony.Patch(typeof(Studio.CameraControl).GetMethod("LateUpdate", AccessTools.all),
                          transpiler: new HarmonyMethod(typeof(CameraTargetFixCore).GetMethod(nameof(StudioPatch), AccessTools.all)));
        }
    }
}
