using BepInEx;
using HarmonyLib;
using UnityEngine;

namespace KeelPlugins
{
    [BepInPlugin(GUID, "HideAllUI", Version)]
    public class HideAllUI : HideAllUICore
    {
        public const string Version = "1.1.0";

        private class Hooks
        {
            [HarmonyPostfix, HarmonyPatch(typeof(CharaCustom.CharaCustom), "Start")]
            public static void MakerEntrypoint()
            {
                currentUIHandler = new HideMakerUI();
            }
            
            [HarmonyPostfix, HarmonyPatch(typeof(CharaCustom.CharaCustom), "OnDestroy")]
            public static void MakerEnd()
            {
                currentUIHandler = null;
            }
            
            [HarmonyPostfix, HarmonyPatch(typeof(Studio.Studio), "Awake")]
            public static void StudioEntrypoint()
            {
                currentUIHandler = new HideStudioUI();
            }
            
            [HarmonyPostfix, HarmonyPatch(typeof(Studio.Studio), "OnApplicationQuit")]
            public static void StudioEnd()
            {
                currentUIHandler = null;
            }
            
            [HarmonyPrefix, HarmonyPatch(typeof(HScene), "SetStartVoice")]
            public static void HSceneStart(HScene __instance)
            {
                var traverse = Traverse.Create(__instance);
            
                CanvasGroup UIGroup = traverse.Field("sprite").Field("UIGroup").GetValue<CanvasGroup>();
                if (UIGroup == null)
                    return;
                
                currentUIHandler = new HideHSceneUI(UIGroup);
            }

            [HarmonyPrefix, HarmonyPatch(typeof(HScene), "OnDisable")]
            public static void HSceneEnd()
            {
                currentUIHandler = null;
            }
        }
    }
}
