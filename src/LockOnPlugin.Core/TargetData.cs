using BepInEx;
using ParadoxNotion.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using KKAPI.Utilities;

namespace LockOnPlugin.Core
{
    internal class TargetData
    {
        internal static TargetData data;
        private const string dataFileName = "LockOnPluginData.json";

        public static void LoadData()
        {
            var dataPath = Path.Combine(Paths.ConfigPath, dataFileName);

            if(File.Exists(dataPath))
            {
                try
                {
                    var json = File.ReadAllText(dataPath);
                    data = JSONSerializer.Deserialize<TargetData>(json);
                    Log.Info("Loading custom target data.");
                }
                catch(Exception ex)
                {
                    Log.Info($"Failed to deserialize custom target data. Loading default target data.\n{ex}");
                    LoadResourceData();
                }
            }
            else
            {
                Log.Debug("Loading default target data.");
                LoadResourceData();
            }
        }

        private static void LoadResourceData()
        {
            var json = Encoding.UTF8.GetString(ResourceUtils.GetEmbeddedResource(dataFileName, typeof(LockOnPluginCore).Assembly));
            data = JSONSerializer.Deserialize<TargetData>(json);
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
