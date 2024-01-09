using System.Linq;
using BepInEx;
using HarmonyLib;
using KKAPI.Studio;
using KKAPI.Studio.SaveLoad;
using KKAPI.Utilities;
using Sideloader.AutoResolver;
using Studio;
using UnityEngine;

namespace DefaultParamEditor.Koikatu
{
    internal static class SceneParam
    {
        private static ParamData.SceneData _sceneData;

        public static void Init(ParamData.SceneData data)
        {
            _sceneData = data;
            Harmony.CreateAndPatchAll(typeof(Hooks));
        }

        public static void Save()
        {
            var sceneInfo = Studio.Studio.Instance.sceneInfo;
            var systemButtonCtrl = Studio.Studio.Instance.systemButtonCtrl;

            if(sceneInfo != null && systemButtonCtrl != null)
            {
                _sceneData.aceNo = sceneInfo.aceNo;
                _sceneData.aceBlend = sceneInfo.aceBlend;

                var aceInfo = UniversalAutoResolver.LoadedStudioResolutionInfo.FirstOrDefault(x => x.ResolveItem && x.LocalSlot == sceneInfo.aceNo);
                if(aceInfo != null)
                {
                    _sceneData.aceNo = aceInfo.Slot;
                    _sceneData.aceNo_GUID = aceInfo.GUID;
                }

                // Second ace/ramp dropdown added by the TwoLut Plugin
                _sceneData.ace2No = null;
                _sceneData.ace2No_GUID = null;
                var tlp = Traverse.CreateWithType("KK_Plugins.TwoLutPlugin");
                if (tlp.TypeExists())
                {
                    var prop = tlp.Property("CurrentLut2LocalSlot");
                    if (prop.PropertyExists())
                    {
                        _sceneData.ace2No = prop.GetValue<int>();

                        var ace2Info = UniversalAutoResolver.LoadedStudioResolutionInfo.FirstOrDefault(x => x.ResolveItem && x.LocalSlot == _sceneData.ace2No);
                        if (ace2Info != null)
                        {
                            _sceneData.ace2No = ace2Info.Slot;
                            _sceneData.ace2No_GUID = ace2Info.GUID;
                        }
                    }
                }

                _sceneData.enableAOE = systemButtonCtrl.amplifyOcculusionEffectInfo.aoe.enabled;
                _sceneData.aoeColor = sceneInfo.aoeColor;
                _sceneData.aoeRadius = sceneInfo.aoeRadius;
                _sceneData.enableBloom = sceneInfo.enableBloom;
                _sceneData.bloomIntensity = sceneInfo.bloomIntensity;
                _sceneData.bloomThreshold = sceneInfo.bloomThreshold;
                _sceneData.bloomBlur = sceneInfo.bloomBlur;
                _sceneData.enableDepth = sceneInfo.enableDepth;
                _sceneData.depthFocalSize = sceneInfo.depthFocalSize;
                _sceneData.depthAperture = sceneInfo.depthAperture;
                _sceneData.enableVignette = sceneInfo.enableVignette;
                _sceneData.enableFog = sceneInfo.enableFog;
                _sceneData.fogColor = sceneInfo.fogColor;
                _sceneData.fogHeight = sceneInfo.fogHeight;
                _sceneData.fogStartDistance = sceneInfo.fogStartDistance;
                _sceneData.enableSunShafts = sceneInfo.enableSunShafts;
                _sceneData.sunThresholdColor = sceneInfo.sunThresholdColor;
                _sceneData.sunColor = sceneInfo.sunColor;
                _sceneData.enableShadow = systemButtonCtrl.selfShadowInfo.toggleEnable.isOn;

                _sceneData.rampG = sceneInfo.rampG;
                var rampGInfo = UniversalAutoResolver.TryGetResolutionInfo(ChaListDefine.CategoryNo.mt_ramp, sceneInfo.rampG);
                if(rampGInfo != null)
                {
                    _sceneData.rampG = rampGInfo.Slot;
                    _sceneData.rampG_GUID = rampGInfo.GUID;
                }

                _sceneData.ambientShadowG = sceneInfo.ambientShadowG;
                _sceneData.lineWidthG = sceneInfo.lineWidthG;
                _sceneData.lineColorG = sceneInfo.lineColorG;
                _sceneData.ambientShadow = sceneInfo.ambientShadow;
                _sceneData.cameraNearClip = Camera.main.nearClipPlane;
                _sceneData.fov = Studio.Studio.Instance.cameraCtrl.fieldOfView;

                _sceneData.saved = true;
                Log.Info("Default scene settings saved");
            }
        }

        public static void Reset()
        {
            _sceneData.saved = false;
        }

        public static void LoadDefaults()
        {
            if(_sceneData.saved && Studio.Studio.Instance)
            {
                Log.Info("Loading scene defaults");
                SetSceneInfoValues(Studio.Studio.Instance.sceneInfo);
                Studio.Studio.Instance.systemButtonCtrl.UpdateInfo();
            }
            else
            {
                Log.Message("Scene defaults have not been saved yet");
            }
        }

        private static void SetSceneInfoValues(SceneInfo sceneInfo)
        {
            sceneInfo.aceNo = _sceneData.aceNo;
            if(!string.IsNullOrEmpty(_sceneData.aceNo_GUID))
            {
                var aceInfo = UniversalAutoResolver.LoadedStudioResolutionInfo.FirstOrDefault(x => x.GUID == _sceneData.aceNo_GUID && x.Slot == _sceneData.aceNo);
                if(aceInfo != null)
                    sceneInfo.aceNo = aceInfo.LocalSlot;
            }

            // Second ace/ramp dropdown added by the TwoLut Plugin
            if(_sceneData.ace2No != null)
            {
                var tlp = Traverse.CreateWithType("KK_Plugins.TwoLutPlugin");
                if(tlp.TypeExists())
                {
                    var prop = tlp.Property("CurrentLut2LocalSlot");
                    if(prop.PropertyExists())
                    {
                        // Need to do a delayed load because on studio startup this is called before studio controls and TwoLut are initialized
                        ThreadingHelper.Instance.StartCoroutine(new WaitUntil(() => StudioAPI.StudioLoaded)
                            .AppendCo(new WaitForEndOfFrame())
                            .AppendCo(() =>
                            {
                                prop.SetValue(_sceneData.ace2No);

                                if (!string.IsNullOrEmpty(_sceneData.ace2No_GUID))
                                {
                                    var ace2Info = UniversalAutoResolver.LoadedStudioResolutionInfo.FirstOrDefault(x => x.GUID == _sceneData.ace2No_GUID && x.Slot == _sceneData.ace2No);
                                    if (ace2Info != null)
                                        prop.SetValue(ace2Info.LocalSlot);
                                }
                            }));
                    }
                }
            }

            sceneInfo.aceBlend = _sceneData.aceBlend;
            sceneInfo.enableAOE = _sceneData.enableAOE;
            sceneInfo.aoeColor = _sceneData.aoeColor;
            sceneInfo.aoeRadius = _sceneData.aoeRadius;
            sceneInfo.enableBloom = _sceneData.enableBloom;
            sceneInfo.bloomIntensity = _sceneData.bloomIntensity;
            sceneInfo.bloomThreshold = _sceneData.bloomThreshold;
            sceneInfo.bloomBlur = _sceneData.bloomBlur;
            sceneInfo.enableDepth = _sceneData.enableDepth;
            sceneInfo.depthFocalSize = _sceneData.depthFocalSize;
            sceneInfo.depthAperture = _sceneData.depthAperture;
            sceneInfo.enableVignette = _sceneData.enableVignette;
            sceneInfo.enableFog = _sceneData.enableFog;
            sceneInfo.fogColor = _sceneData.fogColor;
            sceneInfo.fogHeight = _sceneData.fogHeight;
            sceneInfo.fogStartDistance = _sceneData.fogStartDistance;
            sceneInfo.enableSunShafts = _sceneData.enableSunShafts;
            sceneInfo.sunThresholdColor = _sceneData.sunThresholdColor;
            sceneInfo.sunColor = _sceneData.sunColor;
            sceneInfo.enableShadow = _sceneData.enableShadow;

            sceneInfo.rampG = _sceneData.rampG;
            if(!string.IsNullOrEmpty(_sceneData.rampG_GUID))
            {
                var rampGInfo = UniversalAutoResolver.TryGetResolutionInfo(_sceneData.rampG, ChaListDefine.CategoryNo.mt_ramp, _sceneData.rampG_GUID);
                if(rampGInfo != null)
                    sceneInfo.rampG = rampGInfo.LocalSlot;
            }

            sceneInfo.ambientShadowG = _sceneData.ambientShadowG;
            sceneInfo.lineWidthG = _sceneData.lineWidthG;
            sceneInfo.lineColorG = _sceneData.lineColorG;
            sceneInfo.ambientShadow = _sceneData.ambientShadow;
        }

        private static class Hooks
        {
            [HarmonyPostfix, HarmonyPatch(typeof(SceneInfo), nameof(SceneInfo.Init))]
            public static void HarmonyPatch_SceneInfo_Init(SceneInfo __instance)
            {
                if(_sceneData.saved && !StudioSaveLoadApi.LoadInProgress && !StudioSaveLoadApi.ImportInProgress)
                {
                    Log.Debug("Loading defaults for a new scene");
                    SetSceneInfoValues(__instance);
                    Camera.main.nearClipPlane = _sceneData.cameraNearClip;
                    Studio.Studio.Instance.cameraCtrl.fieldOfView = _sceneData.fov;
                }
            }
        }
    }
}
