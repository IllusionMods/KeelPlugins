using HarmonyLib;
using Studio;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace KeelPlugins
{
    internal partial class StudioMono : LockOnBase
    {
        protected override float CameraMoveSpeed
        {
            get { return camera.moveSpeed; }
            set { camera.moveSpeed = value; }
        }

        protected override Vector3 CameraTargetPos
        {
            get { return camera.targetPos; }
            set { camera.targetPos = value; }
        }

        protected override Vector3 CameraAngle
        {
            get { return camera.cameraAngle; }
            set { camera.cameraAngle = value; }
        }

        protected override float CameraFov
        {
            get { return camera.fieldOfView; }
            set { camera.fieldOfView = value; }
        }

        protected override Vector3 CameraDir
        {
            get { return cameraData.distance; }
            set { cameraData.distance = value; }
        }

        protected override bool CameraTargetTex
        {
            set { camera.isConfigTargetTex = value; }
        }

        protected override float CameraZoomSpeed => defaultCameraSpeed * Studio.Studio.optionSystem.cameraSpeed;
        protected override Transform CameraTransform => camera.transform;
        protected override bool AllowTracking => !(guideObjectManager.isOperationTarget && guideObjectManager.mode == 1);
        protected override bool InputFieldSelected => base.InputFieldSelected || studio.isInputNow || guideObjectManager.isOperationTarget;
        protected override bool CameraEnabled => camera.enabled;

        private Studio.Studio studio = Studio.Studio.Instance;
        private Studio.CameraControl camera = Studio.Studio.Instance.cameraCtrl;
        private TreeNodeCtrl treeNodeCtrl = Studio.Studio.Instance.treeNodeCtrl;
        private GuideObjectManager guideObjectManager = GuideObjectManager.Instance;

        private Studio.CameraControl.CameraData cameraData;
        private OCIChar currentCharaOCI;

        protected override void Start()
        {
            base.Start();

            cameraData = Traverse.Create(camera).Field("cameraData").GetValue<Studio.CameraControl.CameraData>();
            treeNodeCtrl.onSelect += new Action<TreeNodeObject>(OnSelectWork);
            studio.onDelete += new Action<ObjectCtrlInfo>(OnDeleteWork);
            var systemMenuContent = studio.transform.Find("Canvas Main Menu/04_System/Viewport/Content");
            systemMenuContent.Find("Load").GetComponent<Button>().onClick.AddListener(ResetModState);
            Guitime.pos = new Vector2(1f, 1f);
        }

        private void OnSelectWork(TreeNodeObject node)
        {
            if(studio.dicInfo.TryGetValue(node, out ObjectCtrlInfo objectCtrlInfo))
            {
                if(objectCtrlInfo.kind == 0)
                {
                    var ocichar = objectCtrlInfo as OCIChar;

                    if(ocichar != currentCharaOCI)
                    {
                        currentCharaOCI = ocichar;
                        currentCharaInfo = ocichar.charInfo;
                        shouldResetLock = true;

                        if(LockOnPluginCore.AutoSwitchLock.Value && lockOnTarget)
                        {
                            if(LockOn(lockOnTarget.name, true, false))
                                shouldResetLock = false;
                            else
                                LockOnRelease();
                        }
                    }
                    else
                    {
                        currentCharaOCI = ocichar;
                        currentCharaInfo = ocichar.charInfo;
                    }

                    return;
                }
            }

            currentCharaOCI = null;
            currentCharaInfo = null;
        }

        private void OnDeleteWork(ObjectCtrlInfo info)
        {
            if(info.kind == 0)
            {
                currentCharaOCI = null;
                currentCharaInfo = null;
            }
        }

        protected override bool LockOn()
        {
            if(guideObjectManager.selectObject)
            {
                // hacky way to find out if the target is an FK/IK node
                if(!studio.dicObjectCtrl.TryGetValue(guideObjectManager.selectObject.dicKey, out _))
                {
                    LockOn(guideObjectManager.selectObject.transform.gameObject);
                    return true;
                }
            }

            if(base.LockOn()) return true;

            var charaNodes = LockOnPluginCore.ScrollThroughMalesToo.Value ? GetCharaNodes<OCIChar>() : GetCharaNodes<OCICharFemale>();
            if(charaNodes.Count > 0)
            {
                studio.treeNodeCtrl.SelectSingle(charaNodes[0]);
                if(base.LockOn()) return true;
            }

            return false;
        }

        protected override void CharaSwitch(bool scrollDown = true)
        {
            var charaNodes = LockOnPluginCore.ScrollThroughMalesToo.Value ? GetCharaNodes<OCIChar>() : GetCharaNodes<OCICharFemale>();

            for(int i = 0; i < charaNodes.Count; i++)
            {
                if(charaNodes[i] == treeNodeCtrl.selectNode)
                {
                    int next = i + 1 > charaNodes.Count - 1 ? 0 : i + 1;
                    if(!scrollDown) next = i - 1 < 0 ? charaNodes.Count - 1 : i - 1;
                    treeNodeCtrl.SelectSingle(charaNodes[next]);
                    return;
                }
            }

            if(charaNodes.Count > 0)
            {
                treeNodeCtrl.SelectSingle(charaNodes[0]);
            }
        }

        protected override void ResetModState()
        {
            base.ResetModState();
            currentCharaOCI = null;
            treeNodeCtrl.SelectSingle(null);
        }

        private List<TreeNodeObject> GetCharaNodes<CharaType>()
        {
            var charaNodes = new List<TreeNodeObject>();

            int n = 0; TreeNodeObject nthNode;
            while(nthNode = treeNodeCtrl.GetNode(n))
            {
                if(nthNode.visible && studio.dicInfo.TryGetValue(nthNode, out ObjectCtrlInfo objectCtrlInfo))
                {
                    if(objectCtrlInfo is CharaType)
                    {
                        charaNodes.Add(nthNode);
                    }
                }
                n++;
            }

            return charaNodes;
        }
    }
}
