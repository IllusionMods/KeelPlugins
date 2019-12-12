using BepInEx;
using HarmonyLib;

namespace KeelPlugins
{
    [BepInProcess(AISyoujyoConstants.StudioProcessName)]
    [BepInPlugin(GUID, PluginName, Version)]
    public class CameraTargetFix : CameraTargetFixCore
    {
        protected override void Awake()
        {
            base.Awake();

            Harmony.Patch(typeof(Studio.CameraControl).GetMethod("InternalUpdateCameraState", AccessTools.all),
                          transpiler: new HarmonyMethod(typeof(CameraTargetFixCore).GetMethod(nameof(StudioPatch), AccessTools.all)));
        }
    }
}
