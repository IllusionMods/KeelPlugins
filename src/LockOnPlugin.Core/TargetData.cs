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
        private const string dataName = "LockOnPluginData.json";

        private static TargetData _instance;
        public static TargetData Instance
        {
            get
            {
                if(_instance == null)
                {
                    var dataPath = Path.Combine(Paths.ConfigPath, dataName);
                    if(File.Exists(dataPath))
                    {
                        try
                        {
                            var json = File.ReadAllText(dataPath);
                            _instance = JSONSerializer.Deserialize<TargetData>(json);
                            Log.Info("Loading custom target data.");
                        }
                        catch(Exception ex)
                        {
                            Log.Info($"Failed to deserialize custom target data. Loading default target data.\n{ex}");
                            _instance = GetEmbeddedData();
                        }
                    }
                    else
                    {
                        Log.Debug("Loading default target data.");
                        _instance = GetEmbeddedData();
                    }
                }

                return _instance;
            }
        }

        private static TargetData GetEmbeddedData()
        {
            var json = Encoding.UTF8.GetString(ResourceUtils.GetEmbeddedResource(dataName, typeof(LockOnPluginCore).Assembly));
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
