using BepInEx;
using ParadoxNotion.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using KKAPI.Utilities;

namespace LockOnPlugin
{
    internal class TargetData
    {
        private static readonly string savePath = Path.Combine(Paths.ConfigPath, "LockOnPluginData.json");

        private static TargetData _instance;
        public static TargetData Instance
        {
            get
            {
                if(_instance == null)
                {
                    if(File.Exists(savePath))
                    {
                        try
                        {
                            var json = File.ReadAllText(savePath);
                            _instance = JSONSerializer.Deserialize<TargetData>(json);
                            Log.Info("Loading custom target data.");
                        }
                        catch(Exception ex)
                        {
                            Log.Info($"Failed to deserialize custom target data. Loading default target data.\n{ex}");
                            _instance = LoadResourceData();
                        }
                    }
                    else
                    {
                        Log.Debug("Loading default target data.");
                        _instance = LoadResourceData();
                    }
                }

                return _instance;
            }
        }

        private static TargetData LoadResourceData()
        {
            var json = Encoding.UTF8.GetString(ResourceUtils.GetEmbeddedResource(savePath, typeof(LockOnPluginCore).Assembly));
            return JSONSerializer.Deserialize<TargetData>(json);
        }

#pragma warning disable 649 // disable never assigned warning
        public List<string> quickTargets;
        public List<CustomTarget> customTargets;
        public List<CenterWeigth> centerWeigths;
        public List<string> presenceTargets;

        public class CustomTarget
        {
            public string target;
            public string point1;
            public string point2;
            public float midpoint;
        }

        public class CenterWeigth
        {
            public string bone;
            public float weigth;
        }
#pragma warning restore 649
    }
}
