using System.Collections.Generic;
using BepInEx;
using BepInEx.Configuration;
using UnityEngine;
using UnityEngine.EventSystems;

namespace RealPOV.Core
{
    public abstract class RealPOVCore : BaseUnityPlugin
    {
        public const string GUID = "keelhauled.realpov";
        public const string PluginName = "RealPOV";

        protected const string SECTION_GENERAL = "General";
        protected const string SECTION_HOTKEYS = "Keyboard shortcuts";

        internal static ConfigEntry<float> ViewOffset { get; set; }
        internal static ConfigEntry<float> DefaultFOV { get; set; }
        internal static ConfigEntry<float> MouseSens { get; set; }
        internal static ConfigEntry<KeyboardShortcut> POVHotkey { get; set; }

        protected static bool POVEnabled;
        protected static float? CurrentFOV;
        protected static readonly Dictionary<GameObject, Vector3> LookRotation = new Dictionary<GameObject, Vector3>();
        protected static GameObject currentCharaGo;
        protected static Camera GameCamera;
        protected static float defaultViewOffset = 0.03f;
        protected static float defaultFov = 70f;

        private static float backupFOV;
        private static float backupNearClip;
        private static bool allowCamera;
        private bool mouseButtonDown0;
        private bool mouseButtonDown1;

        protected virtual void Awake()
        {
            Log.SetLogSource(Logger);

            POVHotkey = Config.Bind(SECTION_HOTKEYS, "Toggle POV", new KeyboardShortcut(KeyCode.Backspace));
            DefaultFOV = Config.Bind(SECTION_GENERAL, "Default FOV", defaultFov, new ConfigDescription("", new AcceptableValueRange<float>(20f, 120f)));
            MouseSens = Config.Bind(SECTION_GENERAL, "Mouse sensitivity", 1f, new ConfigDescription("", new AcceptableValueRange<float>(0.1f, 2f)));
            ViewOffset = Config.Bind(SECTION_GENERAL, "View offset", defaultViewOffset, new ConfigDescription("Move the camera backward or forward", new AcceptableValueRange<float>(-0.5f, 0.5f)));
        }

        private void Update()
        {
            if(POVHotkey.Value.IsDown())
            {
                if(POVEnabled)
                    DisablePov();
                else
                    EnablePov();
            }
        }
        
        private void LateUpdate()
        {
            if(POVEnabled)
            {
                if(!allowCamera)
                {
                    if(GUIUtility.hotControl == 0 && !EventSystem.current.IsPointerOverGameObject())
                    {
                        if(Input.GetMouseButtonDown(0))
                        {
                            mouseButtonDown0 = true;
                            allowCamera = true;
                            if(GameCursor.IsInstance())
                                GameCursor.Instance.SetCursorLock(true);
                        }

                        if(Input.GetMouseButtonDown(1))
                        {
                            mouseButtonDown1 = true;
                            allowCamera = true;
                            if(GameCursor.IsInstance())
                                GameCursor.Instance.SetCursorLock(true);
                        }
                    }
                }

                if(allowCamera)
                {
                    bool mouseUp0 = Input.GetMouseButtonUp(0);
                    bool mouseUp1 = Input.GetMouseButtonUp(1);

                    if((mouseButtonDown0 || mouseButtonDown1) && (mouseUp0 || mouseUp1))
                    {
                        if(mouseUp0) mouseButtonDown0 = false;
                        if(mouseUp1) mouseButtonDown1 = false;

                        if(!mouseButtonDown0 && !mouseButtonDown1)
                        {
                            allowCamera = false;
                            if(GameCursor.IsInstance())
                                GameCursor.Instance.SetCursorLock(false);
                        }
                    }
                }

                if(allowCamera)
                {
                    if(mouseButtonDown0)
                    {
                        if(LookRotation.ContainsKey(currentCharaGo))
                        {
                            var x = Input.GetAxis("Mouse X") * MouseSens.Value;
                            var y = -Input.GetAxis("Mouse Y") * MouseSens.Value;
                            LookRotation[currentCharaGo] += new Vector3(y, x, 0f);
                        }
                    }
                    else if(mouseButtonDown1)
                    {
                        CurrentFOV += Input.GetAxis("Mouse X");
                    }
                }
            }
        }

        protected virtual void EnablePov()
        {
            POVEnabled = true;
            backupFOV = GameCamera.fieldOfView;
            backupNearClip = GameCamera.nearClipPlane;

            if(CurrentFOV == null)
                CurrentFOV = DefaultFOV.Value;
        }

        protected virtual void DisablePov()
        {
            currentCharaGo = null;
            POVEnabled = false;

            if(GameCamera != null)
            {
                GameCamera.fieldOfView = backupFOV;
                GameCamera.nearClipPlane = backupNearClip;

                if(GameCursor.IsInstance())
                    GameCursor.Instance.SetCursorLock(false);
            }
        }
    }
}
