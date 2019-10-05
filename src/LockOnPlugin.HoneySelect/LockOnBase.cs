using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.EventSystems;
using XInputDotNetPure;
using UnityEngine.UI;
using KeelPlugins;

namespace LockOnPlugin
{
    internal abstract class LockOnBase : MonoBehaviour
    {
        public const string VERSION = "2.6.0";
        public const string NAME_HSCENEMAKER = "LockOnPlugin";
        public const string NAME_NEO = "LockOnPluginNeo";

        public static LockOnBase instance;
        public static bool lockedOn = false;

        protected abstract float CameraMoveSpeed { get; set; }
        protected abstract Vector3 CameraTargetPos { get; set; }
        protected abstract Vector3 CameraAngle { get; set; }
        protected abstract float CameraFov { get; set; }
        protected abstract Vector3 CameraDir { get; set; }
        protected abstract bool CameraTargetTex { set; }
        protected abstract float CameraZoomSpeed { get; }
        protected abstract Transform CameraTransform { get; }
        protected virtual Vector3 CameraForward => CameraTransform.forward;
        protected virtual Vector3 CameraRight => CameraTransform.right;
        protected virtual Vector3 CameraUp => CameraTransform.up;
        protected virtual bool CameraEnabled => true;
        protected virtual Vector3 LockOnTargetPos => lockOnTarget.transform.position;
        protected virtual bool AllowTracking => true;
        protected virtual bool InputFieldSelected => EventSystem.current.currentSelectedGameObject != null && EventSystem.current.currentSelectedGameObject.GetComponent<InputField>() != null;

        protected KeyCode lockOnKey = KeyCode.Mouse4;
        protected KeyCode lockOnGuiKey = KeyCode.None;
        protected KeyCode prevCharaKey = KeyCode.None;
        protected KeyCode nextCharaKey = KeyCode.None;
        protected KeyboardShortcutHotkey lockOnHotkey;
        protected KeyboardShortcutHotkey lockOnGuiHotkey;
        protected KeyboardShortcutHotkey prevCharaHotkey;
        protected KeyboardShortcutHotkey nextCharaHotkey;

        protected float trackingSpeedNormal;
        protected float trackingSpeedMax = 0.3f;
        protected bool manageCursorVisibility;
        protected bool scrollThroughMalesToo;
        protected bool showInfoMsg;
        protected float breastCounterForce;
        protected float lockLeashLength;
        protected bool autoSwitchLock;
        protected bool unlockHSceneCam;

        protected CharInfo currentCharaInfo;
        public GameObject lockOnTarget;
        protected Vector3? lastTargetPos;
        protected float defaultCameraSpeed;

        protected Vector3 targetOffsetSize = new Vector3();
        protected Vector3 targetOffsetSizeAdded = new Vector3();
        protected float offsetKeyHeld = 0f;
        protected bool reduceOffset = false;

        protected bool mouseButtonDown0 = false;
        protected bool mouseButtonDown1 = false;
        protected WinCursor.Point lockPos;
        protected bool cursorLocked = false;
        protected bool shouldResetLock = false;

        protected bool controllerEnabled;
        protected float controllerMoveSpeed;
        protected float controllerZoomSpeed;
        protected float controllerRotSpeed;
        protected bool controllerInvertX;
        protected bool controllerInvertY;
        protected bool controllerSwapSticks;
        protected GamePadState gamepadStatePrev;
        protected GamePadState gamepadState;
        protected PlayerIndex controllerIndex = PlayerIndex.One;

        protected bool moving = false;
        protected bool animSwitched;
        protected int animMoveSetCurrent = 0;
        protected List<MoveSetData> animMoveSets = new List<MoveSetData>
        {
            new MoveSetData("tachi_pose_03", "tachi_pose_04", 2.5f, 10.3f), // hands in front
            new MoveSetData("tachi_pose_01", "tachi_pose_02", 5f, 9.6f), // hands on the side
            new MoveSetData("tachi_pose_05", "tachi_pose_06", 2.5f, 11.1f), // catwalk
            new MoveSetData("tachi_pose_07", "tachi_pose_02", 5f, 9.6f), // hands on the side alt
        };
        
        protected virtual void Start()
        {
            instance = this;
            defaultCameraSpeed = CameraMoveSpeed;
            LoadSettings();
        }

        protected virtual void OnDestroy()
        {
            ResetModState();
        }

        protected virtual bool LoadSettings()
        {
            trackingSpeedNormal = LockOnPlugin.TrackingSpeedNormal.Value;
            showInfoMsg = LockOnPlugin.ShowInfoMsg.Value;
            manageCursorVisibility = true;
            CameraTargetTex = false;
            scrollThroughMalesToo = LockOnPlugin.ScrollThroughMalesToo.Value;
            breastCounterForce = 0.07f;
            lockLeashLength = LockOnPlugin.LockLeashLength.Value;
            autoSwitchLock = LockOnPlugin.AutoSwitchLock.Value;
            unlockHSceneCam = true;

            try
            {
                for(int i = 0; i < 4; i++)
                {
                    if(GamePad.GetState((PlayerIndex)i).IsConnected)
                    {
                        controllerIndex = (PlayerIndex)i;
                        break;
                    }
                }

                controllerEnabled = true;
                controllerMoveSpeed = 0.3f;
                controllerZoomSpeed = 0.2f;
                controllerRotSpeed = 0.4f;
                controllerInvertX = false;
                controllerInvertY = false;
                controllerSwapSticks = false;
            }
            catch(DllNotFoundException ex)
            {
                Console.WriteLine(ex);
                controllerEnabled = false;
            }
            
            lockOnHotkey = new KeyboardShortcutHotkey(LockOnPlugin.LockOnKey.Value, 0.4f);
            lockOnGuiHotkey = new KeyboardShortcutHotkey(LockOnPlugin.GuiHotkey.Value);
            prevCharaHotkey = new KeyboardShortcutHotkey(LockOnPlugin.PrevCharaKey.Value);
            nextCharaHotkey = new KeyboardShortcutHotkey(LockOnPlugin.NextCharaKey.Value);

            return true;
        }

        protected virtual void Update()
        {
            if(!lockOnTarget && lockedOn)
            {
                Console.WriteLine("[{0}] Reset mod state", NAME_HSCENEMAKER);
                ResetModState();
            }

            GamepadControls();
            lockOnHotkey.KeyHoldAction(LockOnRelease);
            lockOnHotkey.KeyUpAction(() => LockOn());
            lockOnGuiHotkey.KeyDownAction(ToggleLockOnGUI);
            prevCharaHotkey.KeyDownAction(() => CharaSwitch(false));
            nextCharaHotkey.KeyDownAction(() => CharaSwitch(true));

            if(lockOnTarget && CameraEnabled)
            {
                if(Input.GetMouseButton(0) && Input.GetMouseButton(1))
                {
                    float x = Input.GetAxis("Mouse X");
                    float y = Input.GetAxis("Mouse Y");
                    if(Mathf.Abs(x) > 0f || Mathf.Abs(y) > 0f)
                    {
                        targetOffsetSize += (CameraRight * x * defaultCameraSpeed) + (CameraForward * y * defaultCameraSpeed);
                        reduceOffset = false;
                    }
                }
                else if(Input.GetMouseButton(1))
                {
                    float x = Input.GetAxis("Mouse X");
                    if(Input.GetKey(KeyCode.LeftControl))
                    {
                        Guitime.angle = 0.1f;
                        if(Mathf.Abs(x) > 0f)
                        {
                            //camera tilt adjustment
                            float newAngle = CameraAngle.z - x;
                            newAngle = Mathf.Repeat(newAngle, 360f);
                            CameraAngle = new Vector3(CameraAngle.x, CameraAngle.y, newAngle);
                        }
                    }
                    else if(Input.GetKey(KeyCode.LeftShift))
                    {
                        Guitime.fov = 0.1f;
                        if(Mathf.Abs(x) > 0f)
                        {
                            //fov adjustment
                            float newFov = CameraFov + x;
                            CameraFov = Mathf.Clamp(newFov, 1f, 160f);
                        }
                    }
                    else if(!InputFieldSelected)
                    {
                        if(Mathf.Abs(x) > 0f)
                        {
                            //handle zooming manually when camera.movespeed == 0
                            float newDir = CameraDir.z - x * CameraZoomSpeed;
                            newDir = Mathf.Clamp(newDir, float.MinValue, 0f);
                            CameraDir = new Vector3(0f, 0f, newDir);
                            reduceOffset = false;
                        }

                        float y = Input.GetAxis("Mouse Y");
                        if(Mathf.Abs(y) > 0f)
                        {
                            targetOffsetSize += (Vector3.up * y * defaultCameraSpeed);
                            reduceOffset = false;
                        }
                    }
                }
                
                bool RightArrow = Input.GetKey(KeyCode.RightArrow), LeftArrow = Input.GetKey(KeyCode.LeftArrow);
                bool UpArrow = Input.GetKey(KeyCode.UpArrow), DownArrow = Input.GetKey(KeyCode.DownArrow);
                bool PageUp = Input.GetKey(KeyCode.PageUp), PageDown = Input.GetKey(KeyCode.PageDown);

                if(!InputFieldSelected && Hotkey.allowHotkeys && (RightArrow || LeftArrow || UpArrow || DownArrow || PageUp || PageDown))
                {
                    reduceOffset = false;
                    offsetKeyHeld += Time.deltaTime / 3f;
                    if(offsetKeyHeld > 1f) offsetKeyHeld = 1f;
                    float speed = Time.deltaTime * Mathf.Lerp(0.2f, 2f, offsetKeyHeld);

                    if(RightArrow) targetOffsetSize += CameraRight * speed;
                    else if(LeftArrow) targetOffsetSize += CameraRight * -speed;

                    if(UpArrow) targetOffsetSize += CameraForward * speed;
                    else if(DownArrow) targetOffsetSize += CameraForward * -speed;

                    if(PageUp) targetOffsetSize += CameraUp * speed;
                    else if(PageDown)targetOffsetSize += CameraUp * -speed;
                }
                else
                {
                    offsetKeyHeld -= Time.deltaTime * 2f;
                    if(offsetKeyHeld < 0f) offsetKeyHeld = 0f;
                }

                if(reduceOffset)
                {
                    // add this as a setting
                    if(targetOffsetSize.magnitude > 0.00001f)
                    {
                        float trackingSpeed = lockOnTarget.name == CameraTargetManager.MOVEMENTPOINT_NAME ? trackingSpeedMax : trackingSpeedNormal;
                        targetOffsetSize = Vector3.MoveTowards(targetOffsetSize, new Vector3(), targetOffsetSize.magnitude / (1f / trackingSpeed)); 
                    }
                    else
                    {
                        targetOffsetSize = new Vector3();
                        reduceOffset = false;
                    }
                }

                if(AllowTracking)
                {
                    float trackingSpeed;
                    float leash;
                    if(lockOnTarget.name == CameraTargetManager.MOVEMENTPOINT_NAME)
                    {
                        trackingSpeed = trackingSpeedMax;
                        leash = 0f;
                    }
                    else
                    {
                        trackingSpeed = trackingSpeedNormal;
                        leash = lockLeashLength;
                    }

                    float distance = Vector3.Distance(CameraTargetPos, lastTargetPos.Value);
                    if(distance > leash + 0.00001f) CameraTargetPos = Vector3.MoveTowards(CameraTargetPos, LockOnTargetPos + targetOffsetSize, (distance - leash) * trackingSpeed * Time.deltaTime * 60f);
                    CameraTargetPos += targetOffsetSize - targetOffsetSizeAdded;
                    targetOffsetSizeAdded = targetOffsetSize;
                    lastTargetPos = LockOnTargetPos + targetOffsetSize; 
                }
            }

            if(Input.GetKeyDown(KeyCode.Escape))
            {
                HideLockOnTargets();
            }

            if(manageCursorVisibility)
            {
                if(!cursorLocked)
                {
                    if(GUIUtility.hotControl == 0 && !EventSystem.current.IsPointerOverGameObject() && Hotkey.allowHotkeys)
                    {
                        bool mouseDown0 = Input.GetMouseButtonDown(0);
                        bool mouseDown1 = Input.GetMouseButtonDown(1);

                        if(mouseDown0 || mouseDown1)
                        {
                            if(mouseDown0) mouseButtonDown0 = true;
                            if(mouseDown1) mouseButtonDown1 = true;

                            Cursor.visible = false;
                            Cursor.lockState = CursorLockMode.Confined;

                            cursorLocked = true;
                            WinCursor.GetCursorPos(out lockPos);
                        }
                    }
                }

                if(cursorLocked)
                {
                    bool mouseUp0 = Input.GetMouseButtonUp(0);
                    bool mouseUp1 = Input.GetMouseButtonUp(1);

                    if((mouseButtonDown0 || mouseButtonDown1) && (mouseUp0 || mouseUp1))
                    {
                        if(mouseUp0) mouseButtonDown0 = false;
                        if(mouseUp1) mouseButtonDown1 = false;

                        if(!mouseButtonDown0 && !mouseButtonDown1)
                        {
                            Cursor.lockState = CursorLockMode.None;
                            Cursor.visible = true;
                            cursorLocked = false;
                        }
                    }

                    if(cursorLocked) WinCursor.SetCursorPos(lockPos.x, lockPos.y);
                }
            }
        }
        
        protected virtual void OnGUI()
        {
            if(showInfoMsg && Guitime.info > 0f)
            {
                Utils.DebugGUI(Guitime.pos.x, Guitime.pos.y, 200f, 45f, Guitime.msg);
                Guitime.info -= Time.deltaTime;
            }

            if(Guitime.angle > 0f)
            {
                Utils.DebugGUI(0.5f, 0.5f, 100f, 50f, "Camera tilt\n" + CameraAngle.z.ToString("0.0"));
                Guitime.angle -= Time.deltaTime;
            }

            if(Guitime.fov > 0f)
            {
                Utils.DebugGUI(0.5f, 0.5f, 100f, 50f, "Field of view\n" + CameraFov.ToString("0.0"));
                Guitime.fov -= Time.deltaTime;
            }
        }

        protected virtual bool LockOn()
        {
            if(currentCharaInfo)
            {
                List<string> targetList = currentCharaInfo is CharFemale ? FileManager.GetQuickFemaleTargetNames() : FileManager.GetQuickMaleTargetNames();

                if(shouldResetLock)
                {
                    shouldResetLock = false;
                    return LockOn(targetList[0]);
                }

                if(reduceOffset == true)
                {
                    CameraTargetPos += targetOffsetSize;
                    targetOffsetSize = new Vector3();
                }
                else if(targetOffsetSize.magnitude > 0f)
                {
                    reduceOffset = true;
                    return true;
                }
                
                if(!lockOnTarget)
                {
                    return LockOn(targetList[0]);
                }
                else
                {
                    for(int i = 0; i < targetList.Count; i++)
                    {
                        if(lockOnTarget.name == targetList[i])
                        {
                            int next = i + 1 > targetList.Count - 1 ? 0 : i + 1;
                            return LockOn(targetList[next]);
                        }
                    }
                    
                    return LockOn(targetList[0]);
                }
            }

            return false;
        }

        protected virtual bool LockOn(string targetName, bool lockOnAnyway = false, bool resetOffset = true)
        {
            foreach(GameObject target in CameraTargetManager.GetTargetManager(currentCharaInfo).GetAllTargets())
            {
                if(target.name.Substring(3) == targetName.Substring(3))
                {
                    if(LockOn(target, resetOffset))
                    {
                        return true;
                    }
                }
            }

            if(lockOnAnyway)
            {
                if(LockOn())
                {
                    return true;
                }
            }

            return false;
        }

        public virtual bool LockOn(GameObject target, bool resetOffset = true)
        {
            if(target)
            {
                if(resetOffset) reduceOffset = true;
                lockedOn = true;
                lockOnTarget = target;
                if(lastTargetPos == null) lastTargetPos = LockOnTargetPos + targetOffsetSize;
                CameraMoveSpeed = 0f;
                Guitime.InfoMsg("Locked to \"" + lockOnTarget.name + "\"");
                return true;
            }

            return false;
        }

        protected virtual void LockOnRelease()
        {
            if(lockOnTarget)
            {
                lockedOn = false;
                reduceOffset = true;
                lockOnTarget = null;
                lastTargetPos = null;
                CameraMoveSpeed = defaultCameraSpeed;
                Guitime.InfoMsg("Camera unlocked");
            }
        }

        protected virtual void ToggleLockOnGUI()
        {
            if(currentCharaInfo)
            {
                var targetManager = CameraTargetManager.GetTargetManager(currentCharaInfo);
                targetManager.showLockOnTargets = !targetManager.showLockOnTargets;
            }
        }

        protected void HideLockOnTargets()
        {
            foreach(var targetManager in FindObjectsOfType<CameraTargetManager>())
            {
                targetManager.showLockOnTargets = false;
            }
        }

        protected virtual void CharaSwitch(bool scrollDown = true)
        {
            Console.WriteLine("Character switching not implemented in this version");
        }

        protected virtual void ResetModState()
        {
            HideLockOnTargets();
            lockedOn = false;
            reduceOffset = true;
            lockOnTarget = null;
            lastTargetPos = null;
            CameraMoveSpeed = defaultCameraSpeed;
            Guitime.InfoMsg("Camera unlocked");
            currentCharaInfo = null;
        }

        protected virtual void GamepadControls()
        {
            if(!controllerEnabled) return;
            gamepadStatePrev = gamepadState;
            gamepadState = GamePad.GetState(controllerIndex, GamePadDeadZone.Circular);
            if(!gamepadState.IsConnected) return;
            animSwitched = false;

            if(gamepadStatePrev.Buttons.A == ButtonState.Released && gamepadState.Buttons.A == ButtonState.Pressed)
            {
                LockOn();
            }

            if(gamepadStatePrev.Buttons.B == ButtonState.Released && gamepadState.Buttons.B == ButtonState.Pressed)
            {
                LockOnRelease();
            }

            if(gamepadStatePrev.Buttons.Y == ButtonState.Released && gamepadState.Buttons.Y == ButtonState.Pressed)
            {
                CharaSwitch(true);
            }

            if(gamepadStatePrev.Buttons.X == ButtonState.Released && gamepadState.Buttons.X == ButtonState.Pressed)
            {
                int next = animMoveSetCurrent + 1 > animMoveSets.Count - 1 ? 0 : animMoveSetCurrent + 1;
                animMoveSetCurrent = next;
                animSwitched = true;
            }

            if(gamepadStatePrev.Buttons.RightStick == ButtonState.Released && gamepadState.Buttons.RightStick == ButtonState.Pressed)
            {
                Utils.InvokePluginMethod("TogglePOV.BaseMono", "TogglePOV");
            }

            if(gamepadStatePrev.Buttons.LeftStick == ButtonState.Released && gamepadState.Buttons.LeftStick == ButtonState.Pressed)
            {
                LockOn(CameraTargetManager.MOVEMENTPOINT_NAME);
            }

            if(gamepadState.DPad.Right == ButtonState.Pressed)
            {
                Guitime.angle = 1f;
                float newAngle = CameraAngle.z - 1f * Time.deltaTime * 60f;
                newAngle = Mathf.Repeat(newAngle, 360f);
                CameraAngle = new Vector3(CameraAngle.x, CameraAngle.y, newAngle);
            }
            else if(gamepadState.DPad.Left == ButtonState.Pressed)
            {
                Guitime.angle = 1f;
                float newAngle = CameraAngle.z + 1f * Time.deltaTime * 60f;
                newAngle = Mathf.Repeat(newAngle, 360f);
                CameraAngle = new Vector3(CameraAngle.x, CameraAngle.y, newAngle);
            }
            else if(gamepadState.DPad.Up == ButtonState.Pressed)
            {
                if(gamepadState.Buttons.LeftShoulder == ButtonState.Pressed)
                {
                    Guitime.fov = 1f;
                    float newFov = CameraFov - 1f * Time.deltaTime * 60f;
                    CameraFov = Mathf.Clamp(newFov, 1f, 160f);
                }
                else
                {
                    float newDir = CameraDir.z + 1f * CameraZoomSpeed;
                    newDir = Mathf.Clamp(newDir, float.MinValue, 0f);
                    CameraDir = new Vector3(0f, 0f, newDir);
                    reduceOffset = false;
                }
            }
            else if(gamepadState.DPad.Down == ButtonState.Pressed)
            {
                if(gamepadState.Buttons.LeftShoulder == ButtonState.Pressed)
                {
                    Guitime.fov = 1f;
                    float newFov = CameraFov + 1f * Time.deltaTime * 60f;
                    CameraFov = Mathf.Clamp(newFov, 1f, 160f);
                }
                else
                {
                    float newDir = CameraDir.z - 1f * CameraZoomSpeed;
                    newDir = Mathf.Clamp(newDir, float.MinValue, 0f);
                    CameraDir = new Vector3(0f, 0f, newDir);
                    reduceOffset = false;
                }
            }

            Vector2 leftStick = new Vector2(gamepadState.ThumbSticks.Left.X, -gamepadState.ThumbSticks.Left.Y);
            Vector2 rightStick = new Vector2(gamepadState.ThumbSticks.Right.X, -gamepadState.ThumbSticks.Right.Y);

            if(controllerSwapSticks)
            {
                GamepadMovement(rightStick);
                GamepadCamera(leftStick);
            }
            else
            {
                GamepadMovement(leftStick);
                GamepadCamera(rightStick);
            }
        }

        protected virtual void GamepadMovement(Vector2 stick)
        {
            if(stick.magnitude > 0.1f)
            {
                Console.WriteLine("Movement not implemented in this version");
            }
        }

        protected virtual void GamepadCamera(Vector2 stick)
        {
            if(stick.magnitude > 0f && CameraEnabled)
            {
                bool L1 = gamepadState.Buttons.LeftShoulder == ButtonState.Pressed;
                bool R1 = gamepadState.Buttons.RightShoulder == ButtonState.Pressed;

                if(!L1 && !R1)
                {
                    float speed = Mathf.Lerp(1f, 4f, controllerRotSpeed) * Time.deltaTime * 60f;
                    float newX = Mathf.Repeat((!controllerInvertY || CameraDir.z == 0f ? stick.y : -stick.y) * speed, 360f);
                    float newY = Mathf.Repeat((!controllerInvertX || CameraDir.z == 0f ? stick.x : -stick.x) * speed, 360f);
                    CameraAngle += new Vector3(newX, newY, 0f);
                }
                else if(lockOnTarget)
                {
                    reduceOffset = false;

                    if(R1)
                    {
                        float speed = Mathf.Lerp(0.01f, 0.4f, controllerZoomSpeed) * Time.deltaTime * 60f;
                        targetOffsetSize += (CameraForward * -stick.y * speed);
                    }
                    else
                    {
                        float speed = Mathf.Lerp(0.001f, 0.04f, controllerMoveSpeed) * Time.deltaTime * 60f;
                        targetOffsetSize += (CameraRight * stick.x * speed) + (Vector3.up * -stick.y * speed);
                    }
                }
                else
                {
                    reduceOffset = false;

                    if(R1)
                    {
                        float speed = Mathf.Lerp(0.01f, 0.4f, controllerZoomSpeed) * Time.deltaTime * 60f;
                        CameraTargetPos += (CameraForward * -stick.y * speed);
                    }
                    else
                    {
                        float speed = Mathf.Lerp(0.001f, 0.04f, controllerMoveSpeed) * Time.deltaTime * 60f;
                        CameraTargetPos += (CameraRight * stick.x * speed) + (Vector3.up * -stick.y * speed);
                    }
                }
            }
        }

        protected class MoveSetData
        {
            public string idle;
            public string move;
            public float animSpeed;
            public float speedMult;

            public MoveSetData(string idle, string move, float animSpeed, float speedMult)
            {
                this.idle = idle;
                this.move = move;
                this.animSpeed = animSpeed;
                this.speedMult = speedMult;
            }
        }

        protected static class Guitime
        {
            public static float angle = 0f;
            public static float fov = 0f;
            public static float info = 0f;
            public static string msg = "";
            public static Vector2 pos = new Vector2(0.5f, 0f);

            public static void InfoMsg(string newMsg, float time = 3f)
            {
                msg = newMsg;
                info = time;
            }
        }

        protected static class WinCursor
        {
            [DllImport("user32.dll")]
            public static extern bool SetCursorPos(int X, int Y);

            [DllImport("user32.dll")]
            public static extern bool GetCursorPos(out Point pos);

            public struct Point
            {
                public int x;
                public int y;

                public static implicit operator Vector2(Point point)
                {
                    return new Vector2(point.x, point.y);
                }
            }
        }
    }
}
