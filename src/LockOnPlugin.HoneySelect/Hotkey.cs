using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace LockOnPlugin
{
    internal class Hotkey
    {
        public static bool allowHotkeys = true;

        private KeyCode key = KeyCode.None;
        private float procTime = 0f;
        private float timeHeld = 0f;
        private bool released = true;
        private bool enabled = true;

        public Hotkey(KeyCode newKey, float newProcTime = 0f)
        {
            key = newKey;
            if(key == KeyCode.None)
                enabled = false;

            if(newProcTime > 0f)
                procTime = newProcTime;
        }

        public void KeyDownAction(UnityAction action)
        {
            if(ResetIfShould()) return;

            if(enabled && Input.GetKeyDown(key) && !GetModifiers())
            {
                action();
                released = false;
            }
        }

        // this always needs at least KeyUpAction(null) after it
        public void KeyHoldAction(UnityAction action)
        {
            if(ResetIfShould()) return;

            if(enabled && procTime > 0f && Input.GetKey(key) && !GetModifiers())
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

            if(enabled && Input.GetKeyUp(key) && !GetModifiers())
            {
                if(released)
                {
                    action();
                }

                timeHeld = 0f;
                released = true;
            }
        }

        private bool GetModifiers()
        {
            return Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.LeftControl);
        }

        private bool ResetIfShould()
        {
            bool shouldReset = false;

            if(!allowHotkeys)
            {
                shouldReset = true;
            }

            if(GUIUtility.keyboardControl > 0)
            {
                shouldReset = true;
            }
            
            if(EventSystem.current.currentSelectedGameObject != null && EventSystem.current.currentSelectedGameObject.GetComponent<InputField>() != null)
            {
                shouldReset = true;
            }

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
