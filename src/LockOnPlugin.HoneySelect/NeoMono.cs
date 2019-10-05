using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Studio;
using XInputDotNetPure;

namespace LockOnPlugin
{
    internal partial class NeoMono : LockOnBase
    {
        private Studio.Studio studio = Studio.Studio.Instance;
        private Studio.CameraControl camera = Studio.Studio.Instance.cameraCtrl;
        private TreeNodeCtrl treeNodeCtrl = Studio.Studio.Instance.treeNodeCtrl;
        private GuideObjectManager guideObjectManager = Singleton<GuideObjectManager>.Instance;

        private Studio.CameraControl.CameraData cameraData;
        private OCIChar currentCharaOCI;
        private List<DynamicBone_Ver02> boobs;
        private AnimatorOverrideController overrideController;

        protected override void Start()
        {
            base.Start();

            cameraData = Utils.GetSecureField<Studio.CameraControl.CameraData, Studio.CameraControl>("cameraData", camera);
            treeNodeCtrl.onSelect += new Action<TreeNodeObject>(OnSelectWork);
            studio.onDelete += new Action<ObjectCtrlInfo>(OnDeleteWork);
            Transform systemMenuContent = studio.transform.Find("Canvas Main Menu/04_System/Viewport/Content");
            systemMenuContent.Find("Load").GetComponent<Button>().onClick.AddListener(() => ResetModState());
            systemMenuContent.Find("End").GetComponent<Button>().onClick.AddListener(() => HideLockOnTargets());
            OverrideControllerCreate();
        }

        protected override bool LoadSettings()
        {
            base.LoadSettings();

            manageCursorVisibility = false;
            Guitime.pos = new Vector2(1f, 1f);
            return true;
        }

        private void OnSelectWork(TreeNodeObject node)
        {
            ObjectCtrlInfo objectCtrlInfo = null;
            if(studio.dicInfo.TryGetValue(node, out objectCtrlInfo))
            {
                if(objectCtrlInfo.kind == 0)
                {
                    OCIChar ocichar = objectCtrlInfo as OCIChar;

                    if(ocichar != currentCharaOCI)
                    {
                        currentCharaOCI = ocichar;
                        currentCharaInfo = ocichar.charInfo;
                        shouldResetLock = true;

                        boobs = null;
                        if(ocichar is OCICharFemale)
                        {
                            CharFemaleBody body = (CharFemaleBody)ocichar.charBody;
                            boobs = new List<DynamicBone_Ver02>
                            {
                                body.getDynamicBone(CharFemaleBody.DynamicBoneKind.BreastL),
                                body.getDynamicBone(CharFemaleBody.DynamicBoneKind.BreastR),
                            };
                        }

                        if(autoSwitchLock && lockOnTarget)
                        {
                            if(LockOn(lockOnTarget.name, true, false))
                            {
                                shouldResetLock = false;
                            }
                            else
                            {
                                LockOnRelease();
                            }
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
            if(base.LockOn()) return true;

            List<TreeNodeObject> charaNodes = scrollThroughMalesToo ? GetCharaNodes<OCIChar>() : GetCharaNodes<OCICharFemale>();
            if(charaNodes.Count > 0)
            {
                studio.treeNodeCtrl.SelectSingle(charaNodes[0]);
                if(base.LockOn()) return true;
            }

            return false;
        }

        protected override void CharaSwitch(bool scrollDown = true)
        {
            List<TreeNodeObject> charaNodes = scrollThroughMalesToo ? GetCharaNodes<OCIChar>() : GetCharaNodes<OCICharFemale>();

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
            treeNodeCtrl?.SelectSingle(null);
        }

        private List<TreeNodeObject> GetCharaNodes<CharaType>()
        {
            List<TreeNodeObject> charaNodes = new List<TreeNodeObject>();

            int n = 0; TreeNodeObject nthNode;
            while(nthNode = treeNodeCtrl.GetNode(n))
            {
                ObjectCtrlInfo objectCtrlInfo = null;
                if(nthNode.visible && studio.dicInfo.TryGetValue(nthNode, out objectCtrlInfo))
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

        protected override void GamepadMovement(Vector2 stick)
        {
            if(stick.magnitude > 0.1f)
            {
                if(currentCharaOCI != null)
                {
                    if(gamepadState.Buttons.LeftShoulder == ButtonState.Released)
                    {
                        bool rotatingInPov = !CameraEnabled && Mathf.Abs(stick.y) < 0.2f;

                        if((!moving || animSwitched) && !rotatingInPov)
                        {
                            moving = true;
                            currentCharaOCI.charInfo.animBody.runtimeAnimatorController = overrideController;
                            currentCharaOCI.charInfo.animBody.CrossFadeInFixedTime(animMoveSets[animMoveSetCurrent].move, 0.2f);
                            currentCharaOCI.optionItemCtrl.LoadAnimeItem(null, "", 0f, 0f);
                            //currentCharaOCI.ActiveKinematicMode(OICharInfo.KinematicMode.FK, false, false);
                            //currentCharaOCI.ActiveKinematicMode(OICharInfo.KinematicMode.IK, false, false);
                            //GameObject toggle = GameObject.Find("Toggle Function");
                            //if(toggle) toggle.GetComponent<Toggle>().isOn = false;
                        }

                        float refer = 0.6f;
                        float scale = 0.53f;
                        float heightMult = Mathf.LerpUnclamped(scale / refer, 1f, currentCharaInfo.chaCustom.GetShapeBodyValue(0) / refer);
                        
                        float animSpeed = animMoveSets[animMoveSetCurrent].animSpeed * currentCharaOCI.guideObject.changeAmount.scale.z;
                        if(!rotatingInPov) currentCharaOCI.animeSpeed = stick.magnitude * animSpeed / currentCharaOCI.guideObject.changeAmount.scale.z / heightMult;
                        stick = stick * 0.04f;

                        if(CameraEnabled)
                        {
                            Vector3 forward = Vector3.Scale(Camera.main.transform.forward, new Vector3(1f, 0f, 1f)).normalized;
                            Vector3 right = Vector3.Scale(Camera.main.transform.right, new Vector3(1f, 0f, 1f)).normalized;
                            Vector3 lookDirection = right * stick.x + forward * -stick.y;
                            lookDirection = new Vector3(lookDirection.x, 0f, lookDirection.z);
                            currentCharaOCI.guideObject.changeAmount.pos += lookDirection * Time.deltaTime * (animSpeed * animMoveSets[animMoveSetCurrent].speedMult);
                            Quaternion lookRotation = Quaternion.LookRotation(lookDirection, Vector3.up);
                            Quaternion finalRotation = Quaternion.RotateTowards(Quaternion.Euler(currentCharaOCI.guideObject.changeAmount.rot), lookRotation, Time.deltaTime * 60f * 10f);
                            currentCharaOCI.guideObject.changeAmount.rot = finalRotation.eulerAngles;
                        }
                        else
                        {
                            Vector3 forward = Vector3.Scale(currentCharaInfo.transform.forward, new Vector3(1f, 0f, 1f)).normalized;
                            Vector3 lookDirection = forward * -stick.y;
                            lookDirection = new Vector3(lookDirection.x, 0f, lookDirection.z);
                            currentCharaOCI.guideObject.changeAmount.pos += lookDirection * Time.deltaTime * (animSpeed * animMoveSets[animMoveSetCurrent].speedMult);
                            currentCharaOCI.guideObject.changeAmount.rot += new Vector3(0f, stick.x * Time.deltaTime * 60f * 100f, 0f);
                        }

                        if(currentCharaOCI is OCICharFemale)
                        {
                            Vector3 charaforward = Vector3.Scale(currentCharaInfo.transform.forward, new Vector3(1f, 0f, 1f)).normalized;
                            for(int i = 0; i < boobs.Count; i++)
                            {
                                boobs[i].Force = charaforward * stick.magnitude * breastCounterForce * (animMoveSets[animMoveSetCurrent].animSpeed / 2.5f);
                                //Utils.GetSecureField<List<DynamicBone_Ver02.Particle>, DynamicBone_Ver02>("Particles", boobs[i]).ForEach(x => x.Inert = 0.5f);
                            }
                        }
                    }
                    else
                    {
                        currentCharaOCI.guideObject.changeAmount.pos -= new Vector3{ y = 1f } * stick.y * Time.deltaTime;
                    }
                }
            }
            else if(moving || animSwitched)
            {
                moving = false;
                currentCharaOCI.charInfo.animBody.runtimeAnimatorController = overrideController;
                currentCharaOCI.charInfo.animBody.CrossFadeInFixedTime(animMoveSets[animMoveSetCurrent].idle, 0.4f);
                currentCharaOCI.animeSpeed = 1f;
                for(int i = 0; i < boobs.Count; i++) boobs[i].Force = new Vector3();
            }
        }

        private void OverrideControllerCreate()
        {
            var infoBase = GetAnimeInfo(0, 0, 1);
            var infoWalk = GetAnimeInfo(1, 6, 0);
            var infoGentle = GetAnimeInfo(2, 10, 0);
            var infoActive = GetAnimeInfo(2, 17, 7);
            var infoEnergetic = GetAnimeInfo(2, 16, 3);

            var controllerBase = CommonLib.LoadAsset<RuntimeAnimatorController>(infoBase.bundlePath, infoBase.fileName);
            var controllerWalk = CommonLib.LoadAsset<RuntimeAnimatorController>(infoWalk.bundlePath, infoWalk.fileName);
            var controllerGentle = CommonLib.LoadAsset<RuntimeAnimatorController>(infoGentle.bundlePath, infoGentle.fileName);
            var controllerActive = CommonLib.LoadAsset<RuntimeAnimatorController>(infoActive.bundlePath, infoActive.fileName);
            var controllerEnergetic = CommonLib.LoadAsset<RuntimeAnimatorController>(infoEnergetic.bundlePath, infoEnergetic.fileName);

            overrideController = new AnimatorOverrideController();
            overrideController.runtimeAnimatorController = controllerBase;
            overrideController[animMoveSets[1].move] = controllerWalk.animationClips[0];
            overrideController[animMoveSets[0].idle] = controllerGentle.animationClips[0];
            overrideController[animMoveSets[0].move] = controllerWalk.animationClips[1];
            overrideController[animMoveSets[2].idle] = controllerActive.animationClips[7];
            overrideController[animMoveSets[2].move] = controllerWalk.animationClips[3];
            overrideController[animMoveSets[3].idle] = controllerEnergetic.animationClips[5];
        }

        private Info.AnimeLoadInfo GetAnimeInfo(int group, int category, int no)
        {
            Dictionary<int, Dictionary<int, Info.AnimeLoadInfo>> dictionary = null;
            if(!Info.Instance.dicFemaleAnimeLoadInfo.TryGetValue(group, out dictionary)) return null;

            Dictionary<int, Info.AnimeLoadInfo> dictionary2 = null;
            if(!dictionary.TryGetValue(category, out dictionary2)) return null;

            Info.AnimeLoadInfo animeLoadInfo = null;
            if(!dictionary2.TryGetValue(no, out animeLoadInfo)) return null;

            return animeLoadInfo;
        }
    }
}
