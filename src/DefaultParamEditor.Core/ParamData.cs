using System;
using System.IO;
using BepInEx;
using ParadoxNotion.Serialization;
using UnityEngine;

namespace DefaultParamEditor.Koikatu
{
    public class ParamData
    {
        private static readonly string savePath = Path.Combine(Paths.ConfigPath, "DefaultParamEditorData.json");

        private static ParamData _instance;
        public static ParamData Instance
        {
            get
            {
                if(_instance == null)
                {
                    _instance = new ParamData();

                    if(File.Exists(savePath))
                    {
                        try
                        {
                            var json = File.ReadAllText(savePath);
                            _instance = JSONSerializer.Deserialize<ParamData>(json);
                        }
                        catch(Exception ex)
                        {
                            Log.Error($"Failed to load settings from {savePath} with error: " + ex);
                            _instance = new ParamData();
                        }
                    }
                }
                
                return _instance;
            }
        }

        public static void SaveToFile()
        {
            var json = JSONSerializer.Serialize(typeof(ParamData), Instance, true);
            File.WriteAllText(savePath, json);
        }

        public readonly CharaData charaParamData = new CharaData();
        public readonly SceneData sceneParamData = new SceneData();

        public class CharaData
        {
            public bool saved = false;

            /// <summary>
            /// Make sure to make copies, not use by reference
            /// </summary>
            public byte[] clothesState;
            public byte shoesType;
            public float hohoAkaRate;
            public float nipStandRate;
            public byte tearsLv;
            public int eyesLookPtn;
            public int neckLookPtn;
            public int eyebrowPtn;
            public int eyesPtn;
            public float eyesOpenMax;
            public bool eyesBlink;
            public int mouthPtn;
            public int[] handPtn;
            public float mouthOpen;

            // parameters to add
            // default animation
            // siru
            // eyebrow and eye overlaying
            // donger options
        }

        public class SceneData
        {
            public bool saved = false;

            public int aceNo;
            public string aceNo_GUID;
            // Added by the TwoLut plugin
            public int? ace2No;
            // Added by the TwoLut plugin
            public string ace2No_GUID;
            public float aceBlend;
            public bool enableAOE;
            public Color aoeColor;
            public float aoeRadius;
            public bool enableBloom;
            public float bloomIntensity;
            public float bloomThreshold;
            public float bloomBlur;
            public bool enableDepth;
            public float depthFocalSize;
            public float depthAperture;
            public bool enableVignette;
            public bool enableFog;
            public Color fogColor;
            public float fogHeight;
            public float fogStartDistance;
            public bool enableSunShafts;
            public Color sunThresholdColor;
            public Color sunColor;
            public bool enableShadow;
            public float lineColorG;
            public Color ambientShadow;
            public float lineWidthG;
            public int rampG;
            public string rampG_GUID;
            public float ambientShadowG;
            public float cameraNearClip;
            public float fov;

            // parameters to add
            // all character lighting options
        }
    }
}
