using BepInEx.Configuration;
using Studio;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace KeelPlugins
{
    internal class ObjMoveRotAssistMgr : MonoBehaviour
    {
        private KEY mode = KEY.NONE;
        private Vector2 beginMousePos;
        private Vector2 lastMousePos;
        private Dictionary<int, Vector3> oldPos;
        private Dictionary<int, Vector3> oldRot;
        private Dictionary<int, Vector3> oldScale;
        private GuideObject firstTarget;
        private Dictionary<int, GuideObject> targets = new Dictionary<int, GuideObject>();

        private Dictionary<KEY, KeyboardShortcut> rotationKeys = new Dictionary<KEY, KeyboardShortcut>()
        {
            { KEY.OBJ_ROT_X, StudioAddonLite.KEY_OBJ_ROT_X.Value },
            { KEY.OBJ_ROT_Y, StudioAddonLite.KEY_OBJ_ROT_Y.Value },
            { KEY.OBJ_ROT_Z, StudioAddonLite.KEY_OBJ_ROT_Z.Value },
            { KEY.OBJ_ROT_X_2, StudioAddonLite.KEY_OBJ_ROT_X_2.Value },
            { KEY.OBJ_ROT_Y_2, StudioAddonLite.KEY_OBJ_ROT_Y_2.Value },
            { KEY.OBJ_ROT_Z_2, StudioAddonLite.KEY_OBJ_ROT_Z_2.Value }
        };

        public GuideObject GetTargetObject()
        {
            GuideObject guideObject = GuideObjectManager.Instance.operationTarget;
            if(guideObject == null)
            {
                guideObject = GuideObjectManager.Instance.selectObject;
            }

            return guideObject;
        }

        private void Update()
        {
            var instance = GuideObjectManager.Instance;
            Vector2 vector = GetMousePos() - lastMousePos;

            if(mode == KEY.NONE && (instance.selectObjects == null || instance.selectObjects.Length == 0))
            {
                return;
            }

            if(mode == KEY.OBJ_MOVE_XZ)
            {
                var delta = new Vector3(-vector.x, 0f, -vector.y);
                Move(delta);

                if(!StudioAddonLite.KEY_OBJ_MOVE_XZ.Value.IsPressed() || instance.mode != 0)
                {
                    FinishMove();
                    mode = KEY.NONE;
                }
            }
            else if(mode == KEY.OBJ_MOVE_Y)
            {
                var delta2 = new Vector3(0f, vector.y, 0f);
                Move(delta2);

                if(!StudioAddonLite.KEY_OBJ_MOVE_Y.Value.IsPressed() || instance.mode != 0)
                {
                    FinishMove();
                    mode = KEY.NONE;
                }
            }
            else if(mode == KEY.OBJ_ROT_X)
            {
                float num = (vector.x + vector.y) / 2f;
                var delta3 = new Vector3(num, 0f, 0f);
                Rotate(delta3);

                if(!StudioAddonLite.KEY_OBJ_ROT_X.Value.IsPressed() || instance.mode != 1)
                {
                    FinishRotate();
                    mode = KEY.NONE;
                }
            }
            else if(mode == KEY.OBJ_ROT_Y)
            {
                float num2 = (vector.x + vector.y) / 2f;
                var delta4 = new Vector3(0f, num2, 0f);
                Rotate(delta4);

                if(!StudioAddonLite.KEY_OBJ_ROT_Y.Value.IsPressed() || instance.mode != 1)
                {
                    FinishRotate();
                    mode = KEY.NONE;
                }
            }
            else if(mode == KEY.OBJ_ROT_Z)
            {
                float num3 = (vector.x + vector.y) / 2f;
                var delta5 = new Vector3(0f, 0f, num3);
                Rotate(delta5);

                if(!StudioAddonLite.KEY_OBJ_ROT_Z.Value.IsPressed() || instance.mode != 1)
                {
                    FinishRotate();
                    mode = KEY.NONE;
                }
            }
            else if(mode == KEY.OBJ_ROT_X_2)
            {
                float delta6 = (vector.x + vector.y) / 2f;
                Rotate2(Vector3.right, delta6);

                if(!StudioAddonLite.KEY_OBJ_ROT_X_2.Value.IsPressed() || instance.mode != 1)
                {
                    FinishRotate();
                    mode = KEY.NONE;
                }
            }
            else if(mode == KEY.OBJ_ROT_Y_2)
            {
                float delta7 = (vector.x + vector.y) / 2f;
                Rotate2(Vector3.up, delta7);

                if(!StudioAddonLite.KEY_OBJ_ROT_Y_2.Value.IsPressed() || instance.mode != 1)
                {
                    FinishRotate();
                    mode = KEY.NONE;
                }
            }
            else if(mode == KEY.OBJ_ROT_Z_2)
            {
                float delta8 = (vector.x + vector.y) / 2f;
                Rotate2(Vector3.forward, delta8);

                if(!StudioAddonLite.KEY_OBJ_ROT_Z_2.Value.IsPressed() || instance.mode != 1)
                {
                    FinishRotate();
                    mode = KEY.NONE;
                }
            }
            else if(mode == KEY.OBJ_SCALE_ALL)
            {
                Vector3 vector2 = GetMousePos() - beginMousePos;
                float num4 = (vector2.x + vector2.y) / 2f;
                Scale(Vector3.one * num4);

                if(!StudioAddonLite.KEY_OBJ_SCALE_ALL.Value.IsPressed() || instance.mode != 2)
                {
                    FinishScale();
                    mode = KEY.NONE;
                }
            }
            else if(mode == KEY.OBJ_SCALE_X)
            {
                Vector3 vector3 = GetMousePos() - beginMousePos;
                float num5 = (vector3.x + vector3.y) / 2f;
                Scale(Vector3.left * num5);

                if(!StudioAddonLite.KEY_OBJ_SCALE_ALL.Value.IsPressed() || instance.mode != 2)
                {
                    FinishScale();
                    mode = KEY.NONE;
                }
            }
            else if(mode == KEY.OBJ_SCALE_Y)
            {
                Vector3 vector4 = GetMousePos() - beginMousePos;
                float num6 = (vector4.x + vector4.y) / 2f;
                Scale(Vector3.up * num6);

                if(!StudioAddonLite.KEY_OBJ_SCALE_ALL.Value.IsPressed() || instance.mode != 2)
                {
                    FinishScale();
                    mode = KEY.NONE;
                }
            }
            else if(mode == KEY.OBJ_SCALE_Z)
            {
                Vector3 vector5 = GetMousePos() - beginMousePos;
                float num7 = (vector5.x + vector5.y) / 2f;
                Scale(Vector3.forward * num7);

                if(!StudioAddonLite.KEY_OBJ_SCALE_ALL.Value.IsPressed() || instance.mode != 2)
                {
                    FinishScale();
                    mode = KEY.NONE;
                }
            }
            else if(mode == KEY.NONE)
            {
                if(instance.mode == 0)
                {
                    if(StudioAddonLite.KEY_OBJ_MOVE_XZ.Value.IsPressed())
                    {
                        mode = KEY.OBJ_MOVE_XZ;
                        oldPos = CollectOldPos();
                    }
                    else if(StudioAddonLite.KEY_OBJ_MOVE_Y.Value.IsPressed())
                    {
                        mode = KEY.OBJ_MOVE_Y;
                        oldPos = CollectOldPos();
                    }
                }
                else if(instance.mode == 1)
                {
                    foreach(var key in rotationKeys)
                    {
                        if(key.Value.IsPressed())
                        {
                            mode = key.Key;
                            firstTarget = GetTargetObject();
                            oldPos = CollectOldPos();
                            oldRot = CollectOldRot();
                            break;
                        }
                    }
                }
                else if(instance.mode == 2)
                {
                    if(StudioAddonLite.KEY_OBJ_SCALE_ALL.Value.IsPressed())
                    {
                        mode = KEY.OBJ_SCALE_ALL;
                        oldScale = CollectOldScale();
                        beginMousePos = GetMousePos();
                    }
                    else if(StudioAddonLite.KEY_OBJ_SCALE_X.Value.IsPressed())
                    {
                        mode = KEY.OBJ_SCALE_X;
                        oldScale = CollectOldScale();
                        beginMousePos = GetMousePos();
                    }
                    else if(StudioAddonLite.KEY_OBJ_SCALE_Y.Value.IsPressed())
                    {
                        mode = KEY.OBJ_SCALE_Y;
                        oldScale = CollectOldScale();
                        beginMousePos = GetMousePos();
                    }
                    else if(StudioAddonLite.KEY_OBJ_SCALE_Z.Value.IsPressed())
                    {
                        mode = KEY.OBJ_SCALE_Z;
                        oldScale = CollectOldScale();
                        beginMousePos = GetMousePos();
                    }
                }
            }
            else
            {
                Console.WriteLine("Unknown mode ");
                mode = KEY.NONE;
            }

            lastMousePos = GetMousePos();
        }

        private Dictionary<int, Vector3> CollectOldPos()
        {
            var dictionary = new Dictionary<int, Vector3>();
            targets = new Dictionary<int, GuideObject>();

            foreach(GuideObject guideObject in GuideObjectManager.Instance.selectObjects)
            {
                if(guideObject.enablePos)
                {
                    dictionary.Add(guideObject.dicKey, guideObject.changeAmount.pos);
                    targets.Add(guideObject.dicKey, guideObject);
                }
            }

            return dictionary;
        }

        private Dictionary<int, Vector3> CollectOldRot()
        {
            var dictionary = new Dictionary<int, Vector3>();
            targets = new Dictionary<int, GuideObject>();

            foreach(GuideObject guideObject in GuideObjectManager.Instance.selectObjects)
            {
                if(guideObject.enableRot)
                {
                    dictionary.Add(guideObject.dicKey, guideObject.changeAmount.rot);
                    targets.Add(guideObject.dicKey, guideObject);
                }
            }

            return dictionary;
        }

        private Dictionary<int, Vector3> CollectOldScale()
        {
            var dictionary = new Dictionary<int, Vector3>();
            targets = new Dictionary<int, GuideObject>();

            foreach(GuideObject guideObject in GuideObjectManager.Instance.selectObjects)
            {
                if(guideObject.enableScale)
                {
                    dictionary.Add(guideObject.dicKey, guideObject.changeAmount.scale);
                    targets.Add(guideObject.dicKey, guideObject);
                }
            }

            return dictionary;
        }

        private void Move(Vector3 delta)
        {
            Camera mainCmaera = Studio.Studio.Instance.cameraCtrl.mainCmaera;
            if(mainCmaera != null)
            {
                var vector = new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, Input.mousePosition.z);
                Ray ray = mainCmaera.ScreenPointToRay(vector);
                ray.direction = new Vector3(ray.direction.x, 0f, ray.direction.z);
                Vector3 vector2 = ray.direction * -1f * delta.z;
                ray.direction = Quaternion.LookRotation(ray.direction) * Vector3.right;
                vector2 += ray.direction * -1f * delta.x;
                vector2.y = delta.y;
                delta = vector2;
            }

            delta = delta * Studio.Studio.optionSystem.manipuleteSpeed * StudioAddonLite.MOVE_RATIO.Value;
            foreach(GuideObject guideObject in targets.Values)
            {
                if(guideObject.enablePos)
                {
                    guideObject.transformTarget.position = guideObject.transformTarget.position + delta;
                    guideObject.changeAmount.pos = guideObject.transformTarget.localPosition;
                }
            }
        }

        private void FinishMove()
        {
            var array = (from v in targets select new GuideCommand.EqualsInfo { dicKey = v.Key, oldValue = oldPos[v.Key], newValue = v.Value.changeAmount.pos }).ToArray();
            UndoRedoManager.Instance.Push(new GuideCommand.MoveEqualsCommand(array));
        }

        private void Rotate(Vector3 delta)
        {
            delta = delta * Studio.Studio.optionSystem.manipuleteSpeed * StudioAddonLite.ROTATE_RATIO.Value;

            foreach(GuideObject guideObject in targets.Values)
            {
                if(guideObject.enableRot)
                {
                    Vector3 localEulerAngles = guideObject.transformTarget.localEulerAngles += delta;
                    guideObject.transformTarget.localEulerAngles = localEulerAngles;
                    guideObject.changeAmount.rot = guideObject.transformTarget.localEulerAngles;
                }
            }
        }

        private void FinishRotate()
        {
            var array = (from v in targets select new GuideCommand.EqualsInfo { dicKey = v.Key, oldValue = oldRot[v.Key], newValue = v.Value.changeAmount.rot }).ToArray();
            UndoRedoManager.Instance.Push(new GuideCommand.RotationEqualsCommand(array));
        }

        private void Rotate2(Vector3 axis, float delta)
        {
            delta = delta * Studio.Studio.optionSystem.manipuleteSpeed * StudioAddonLite.ROTATE_RATIO.Value;
            Vector3 position = firstTarget.transformTarget.position;

            foreach(GuideObject guideObject in targets.Values)
            {
                Vector3 localPosition = Vector3.zero;
                Vector3 localEulerAngles = Vector3.zero;

                if(!guideObject.enableRot)
                {
                    localEulerAngles = guideObject.transformTarget.localEulerAngles;
                }

                if(!guideObject.enablePos)
                {
                    localPosition = guideObject.transformTarget.localPosition;
                }

                if(guideObject.enableRot || guideObject.enablePos)
                {
                    guideObject.transformTarget.RotateAround(position, axis, delta);
                    if(!guideObject.enablePos)
                    {
                        guideObject.transformTarget.localPosition = localPosition;
                    }
                    if(!guideObject.enableRot)
                    {
                        guideObject.transformTarget.localEulerAngles = localEulerAngles;
                    }
                    guideObject.changeAmount.rot = guideObject.transformTarget.localEulerAngles;
                    guideObject.changeAmount.pos = guideObject.transformTarget.localPosition;
                }
            }
        }

        private void FinishRotate2()
        {
            var posChangeAmountInfo = (from v in targets select new GuideCommand.EqualsInfo { dicKey = v.Key, oldValue = oldPos[v.Key], newValue = v.Value.changeAmount.pos }).ToArray();
            var rotChangeAmountInfo = (from v in targets select new GuideCommand.EqualsInfo { dicKey = v.Key, oldValue = oldRot[v.Key], newValue = v.Value.changeAmount.rot }).ToArray();
            UndoRedoManager.Instance.Push(new MoveRotEqualsCommand(posChangeAmountInfo, rotChangeAmountInfo));
        }

        private void Scale(Vector3 scaleDelta)
        {
            Vector3 vector = scaleDelta * Studio.Studio.optionSystem.manipuleteSpeed * StudioAddonLite.SCALE_RATIO.Value;
            foreach(GuideObject guideObject in targets.Values)
            {
                if(guideObject.enableRot)
                {
                    Vector3 vector2 = oldScale[guideObject.dicKey];
                    vector2.x *= 1f + vector.x;
                    vector2.y *= 1f + vector.y;
                    vector2.z *= 1f + vector.z;
                    guideObject.transformTarget.localScale = vector2;
                    guideObject.changeAmount.scale = vector2;
                }
            }
        }

        private void FinishScale()
        {
            var array = (from v in targets select new GuideCommand.EqualsInfo { dicKey = v.Key, oldValue = oldScale[v.Key], newValue = v.Value.changeAmount.scale }).ToArray();
            UndoRedoManager.Instance.Push(new GuideCommand.ScaleEqualsCommand(array));
        }

        private Vector2 GetMousePos()
        {
            Vector2 vector = Input.mousePosition;
            return new Vector2(vector.x / Screen.width, vector.y / Screen.height);
        }

        public enum KEY
        {
            OBJ_MOVE_XZ,
            OBJ_MOVE_Y,
            OBJ_ROT_X,
            OBJ_ROT_Y,
            OBJ_ROT_Z,
            OBJ_SCALE_ALL,
            OBJ_SCALE_X,
            OBJ_SCALE_Y,
            OBJ_SCALE_Z,
            OBJ_ROT_X_2,
            OBJ_ROT_Y_2,
            OBJ_ROT_Z_2,
            NONE
        }

        public class MoveRotEqualsCommand : ICommand
        {
            private GuideCommand.EqualsInfo[] posChangeAmountInfo;
            private GuideCommand.EqualsInfo[] rotChangeAmountInfo;

            public MoveRotEqualsCommand(GuideCommand.EqualsInfo[] posChangeAmountInfo, GuideCommand.EqualsInfo[] rotChangeAmountInfo)
            {
                this.posChangeAmountInfo = posChangeAmountInfo;
                this.rotChangeAmountInfo = rotChangeAmountInfo;
            }

            public void Do()
            {
                for(int i = 0; i < posChangeAmountInfo.Length; i++)
                {
                    ChangeAmount changeAmount = Studio.Studio.GetChangeAmount(posChangeAmountInfo[i].dicKey);
                    if(changeAmount != null)
                    {
                        changeAmount.pos = posChangeAmountInfo[i].newValue;
                    }
                }

                for(int j = 0; j < rotChangeAmountInfo.Length; j++)
                {
                    ChangeAmount changeAmount2 = Studio.Studio.GetChangeAmount(rotChangeAmountInfo[j].dicKey);
                    if(changeAmount2 != null)
                    {
                        changeAmount2.rot = rotChangeAmountInfo[j].newValue;
                    }
                }
            }

            public void Redo()
            {
                Do();
            }

            public void Undo()
            {
                for(int i = 0; i < posChangeAmountInfo.Length; i++)
                {
                    ChangeAmount changeAmount = Studio.Studio.GetChangeAmount(posChangeAmountInfo[i].dicKey);
                    if(changeAmount != null)
                    {
                        changeAmount.pos = posChangeAmountInfo[i].oldValue;
                    }
                }

                for(int j = 0; j < rotChangeAmountInfo.Length; j++)
                {
                    ChangeAmount changeAmount2 = Studio.Studio.GetChangeAmount(rotChangeAmountInfo[j].dicKey);
                    if(changeAmount2 != null)
                    {
                        changeAmount2.rot = rotChangeAmountInfo[j].oldValue;
                    }
                }
            }
        }
    }
}
