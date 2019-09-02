using BepInEx;
using Harmony;
using Studio;
using UnityEngine;
using SharedPluginCode;

namespace SelfieCam
{
    [BepInProcess(KoikatuConstants.KoikatuStudioProcessName)]
    [BepInPlugin("keelhauled.selfiecam", "SelfieCam", Version)]
    public class SelfieCam : BaseUnityPlugin
    {
        public const string Version = "1.0.0";

        Studio.Studio studio = Studio.Studio.Instance;
        Studio.CameraControl camera = Studio.Studio.Instance.cameraCtrl;
        TreeNodeCtrl treeNodeCtrl = Studio.Studio.Instance.treeNodeCtrl;
        GuideObjectManager guideObjectManager = GuideObjectManager.Instance;
        Studio.CameraControl.CameraData cameraData;

        bool lockCamera = true;
        SavedKeyboardShortcut ToggleCam { get; }

        SelfieCam()
        {
            ToggleCam = new SavedKeyboardShortcut("ToggleCam", this, new KeyboardShortcut(KeyCode.V));
        }

        void Start()
        {
            cameraData = Traverse.Create(camera).Field("cameraData").GetValue<Studio.CameraControl.CameraData>();
        }

        void Update()
        {
            if(ToggleCam.IsDown()) lockCamera = !lockCamera;
            MoveCamera();
        }

        void MoveCamera()
        {
            if(lockCamera && treeNodeCtrl.selectObjectCtrl.Length > 0)
            {
                var selected = treeNodeCtrl.selectObjectCtrl[0];

                if(selected is OCIChar chara)
                {
                    foreach(var list in chara.oiCharInfo.child.Values)
                    {
                        foreach(var item in list)
                        {
                            if(studio.dicObjectCtrl.TryGetValue(item.dicKey, out var info))
                            {
                                if(info is OCIItem ociItem && ociItem.objectItem.name == "p_koi_stu_smaho00_00") // if smartphone found
                                {
                                    camera.targetPos = ociItem.guideObject.transform.position + -ociItem.guideObject.transform.forward * 0.05f;
                                    Camera.main.nearClipPlane = 0.01f;
                                    cameraData.distance = new Vector3();
                                    var newRot = Quaternion.Euler(ociItem.guideObject.transform.eulerAngles) * Quaternion.Euler(270f, 90f, 0f);
                                    camera.cameraAngle = newRot.eulerAngles;
                                    //camera.fieldOfView = 80f;
                                    return;
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
