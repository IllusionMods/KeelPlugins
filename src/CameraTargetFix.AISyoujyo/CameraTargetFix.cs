using BepInEx;
using CharaCustom;
using HarmonyLib;

namespace KeelPlugins
{
    [BepInPlugin(GUID, PluginName, Version)]
    public class CameraTargetFix : CameraTargetFixCore
    {
        protected override void Awake()
        {
            base.Awake();

            if(Paths.ProcessName == AISyoujyoConstants.StudioProcessName)
            {
                Harmony.Patch(typeof(Studio.CameraControl).GetMethod("InternalUpdateCameraState", AccessTools.all),
                                  transpiler: new HarmonyMethod(typeof(CameraTargetFixCore).GetMethod(nameof(StudioPatch), AccessTools.all))); 
            }
            else
            {
                Harmony.Patch(typeof(CustomControl).GetMethod("Start", AccessTools.all),
                              postfix: new HarmonyMethod(GetType().GetMethod(nameof(MakerPatch), AccessTools.all)));
            }
        }

        private static void MakerPatch()
        {
            Singleton<CustomBase>.Instance.centerDraw = Manager.Config.ActData.Look;
        }
    }
}
