using BepInEx.Configuration;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace KeelPlugins
{
    internal class KeyboardShortcutHotkey
    {
        private KeyboardShortcut key;
        private float procTime = 0f;
        private float timeHeld = 0f;
        private bool released = true;
        private bool enabled = true;

        public KeyboardShortcutHotkey(KeyboardShortcut newKey, float newProcTime = 0f)
        {
            key = newKey;
            if(key.MainKey == KeyCode.None)
                enabled = false;

            if(newProcTime > 0f)
                procTime = newProcTime;
        }

        public void KeyDownAction(UnityAction action)
        {
            if(ResetIfShould()) return;

            if(enabled && key.IsDown())
            {
                action();
                released = false;
            }
        }
        
        /// <summary>
        /// This always needs at least KeyUpAction(null) after it.
        /// </summary>
        /// <param name="action"></param>
        public void KeyHoldAction(UnityAction action)
        {
            if(ResetIfShould()) return;

            if(enabled && procTime > 0f && key.IsPressed())
            {
                timeHeld += Time.deltaTime;
                if(timeHeld >= procTime && released)
                {
                    action();
                    released = false;
                }
            }
        }

        public void KeyUpAction(UnityAction action)
        {
            if(ResetIfShould()) return;

            if(enabled && key.IsUp())
            {
                if(released)
                    action?.Invoke();

                timeHeld = 0f;
                released = true;
            }
        }

        private bool ResetIfShould()
        {
            bool shouldReset = false;

            if(GUIUtility.keyboardControl > 0)
                shouldReset = true;

            if(EventSystem.current.currentSelectedGameObject != null && EventSystem.current.currentSelectedGameObject.GetComponent<InputField>() != null)
                shouldReset = true;

            if(shouldReset)
            {
                timeHeld = 0f;
                released = true;
                return true;
            }

            return false;
        }
    }
}
