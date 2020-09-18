using BepInEx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KeelPlugins
{
    [BepInPlugin(GUID, PluginName, Version)]
    public class ContextMenu : BaseUnityPlugin
    {
        public const string GUID = "keelhauled.cameraframemask";
        public const string PluginName = "CameraFrameMask";
        public const string Version = "1.0.0." + BuildNumber.Version;
    }
}
