using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using UnityEngine;

namespace RealPOV.Core
{
    public abstract class RealPOVCore : BaseUnityPlugin
    {
        public const string GUID = "keelhauled.realpov";
        public const string PluginName = "RealPOV";

        internal const string SECTION_GENERAL = "General";
        internal const string SECTION_HOTKEYS = "Keyboard shortcuts";

        internal static new ManualLogSource Logger;

        internal static ConfigEntry<float> ViewOffset { get; set; }
        internal static ConfigEntry<float> DefaultFOV { get; set; }
        internal static ConfigEntry<float> MouseSens { get; set; }
        internal static ConfigEntry<KeyboardShortcut> POVHotkey { get; set; }

        internal static bool POVEnabled = false;
        internal static float CurrentFOV = -1;
        internal static Vector3 LookRotation = new Vector3();
        internal static Camera GameCamera { get; set; }

        private static float backupFOV;
        private static float backupNearClip;

        protected static float defaultViewOffset = 0.03f;
        protected static float defaultFov = 70f;

        protected virtual void Awake()
        {
            Logger = base.Logger;

            POVHotkey = Config.Bind(SECTION_HOTKEYS, "Toggle POV", new KeyboardShortcut(KeyCode.Backspace));
            DefaultFOV = Config.Bind(SECTION_GENERAL, "Default FOV", defaultFov, new ConfigDescription("", new AcceptableValueRange<float>(20f, 120f)));
            MouseSens = Config.Bind(SECTION_GENERAL, "Mouse sensitivity", 1f, new ConfigDescription("", new AcceptableValueRange<float>(0.1f, 2f)));
            ViewOffset = Config.Bind(SECTION_GENERAL, "View offset", defaultViewOffset);
        }

        private void Update()
        {
            if(POVHotkey.Value.IsDown())
            {
                if(POVEnabled)
                    DisablePOV();
                else
                    EnablePOV();
            }

            if(POVEnabled)
            {
                if(Input.GetMouseButton(0))
                {
                    var x = Input.GetAxis("Mouse X") * MouseSens.Value;
                    var y = -Input.GetAxis("Mouse Y") * MouseSens.Value;
                    LookRotation += new Vector3(y, x, 0f);
                }
                else if(Input.GetMouseButton(1))
                {
                    CurrentFOV += Input.GetAxis("Mouse X");
                }
            }
        }

        internal virtual void EnablePOV()
        {
            POVEnabled = true;
            backupFOV = GameCamera.fieldOfView;
            backupNearClip = GameCamera.nearClipPlane;

            if(CurrentFOV == -1)
                CurrentFOV = DefaultFOV.Value;
        }

        internal virtual void DisablePOV()
        {
            POVEnabled = false;
            GameCamera.fieldOfView = backupFOV;
            GameCamera.nearClipPlane = backupNearClip;
        }
    }
}
