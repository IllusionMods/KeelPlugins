using LightManager.Core;
using Studio;
using System.Collections.Generic;
using System.Globalization;
using UILib;
using UnityEngine;
using UnityEngine.UI;

namespace LightManager.Koikatu
{
    internal class LightManager : MonoBehaviour
    {
        private Image spotlightUI;
        private Image mainPanel;
        private Text targetText;
        private InputField speedField;

        private void Start()
        {
            var uiTransform = Studio.Studio.Instance.transform.Find("Canvas Main Menu/02_Manipulate/02_Light/Image Spot");
            spotlightUI = uiTransform.GetComponent<Image>();

            UIUtility.InitKOI(GetType().Assembly);
            ExtraLightUI(uiTransform);

            Studio.Studio.Instance.treeNodeCtrl.onSelect += OnSelectWork;
        }

        private void Update()
        {
            mainPanel.gameObject.SetActive(spotlightUI.isActiveAndEnabled);
        }

#if DEBUG
        private void OnDestroy()
        {
            Studio.Studio.Instance.treeNodeCtrl.onSelect -= OnSelectWork;
            DestroyImmediate(mainPanel.gameObject);

            foreach(var item in Resources.FindObjectsOfTypeAll<TrackTransform>())
                DestroyImmediate(item);
        }
#endif

        private void OnSelectWork(TreeNodeObject node)
        {
            if(Studio.Studio.Instance.dicInfo.TryGetValue(node, out ObjectCtrlInfo objectCtrlInfo))
            {
                if(objectCtrlInfo is OCILight ocilight)
                {
                    var tracker = ocilight.light.gameObject.GetComponent<TrackTransform>();
                    if(tracker)
                    {
                        targetText.text = tracker.targetName;
                        speedField.text = tracker.rotationSpeed.ToString(CultureInfo.InvariantCulture);
                    }
                    else
                    {
                        targetText.text = "None";
                        speedField.text = "1";
                    }
                }
            }
        }

        private void ExtraLightUI(Transform parent)
        {
            const float width = 50f;
            const float height = 50f;

            mainPanel = UIUtility.CreatePanel("LightManagerPanel", parent);
            mainPanel.color = new Color(0.41f, 0.42f, 0.43f, 1f);
            mainPanel.transform.SetRect(1f, 1f, 1f, 1f, 0f, -height * 2f, width * 2f, 0f);

            var targetTextPanel = UIUtility.CreatePanel("TargetTextPanel", mainPanel.transform);
            targetTextPanel.transform.SetRect(0.1f, 0.68f, 0.9f, 0.92f);
            targetText = UIUtility.CreateText("TargetText", targetTextPanel.transform, "None");
            targetText.transform.SetRect(0.01f, 0.01f, 0.99f, 0.99f);
            targetText.alignment = TextAnchor.MiddleCenter;

            var button = UIUtility.CreateButton("RetargetButton", mainPanel.transform, "Apply");
            button.transform.SetRect(0.1f, 0.38f, 0.9f, 0.62f);
            button.onClick.AddListener(() => SetTargetsForSelected(targetText));

            var speedTextPanel = UIUtility.CreatePanel("SpeedTextPanel", mainPanel.transform);
            speedTextPanel.transform.SetRect(0.1f, 0.08f, 0.6f, 0.32f);
            var speedText = UIUtility.CreateText("SpeedText", speedTextPanel.transform, "Speed");
            speedText.transform.SetRect(0.05f, 0.01f, 0.95f, 0.99f);
            speedText.alignment = TextAnchor.MiddleCenter;

            speedField = UIUtility.CreateInputField("RotationSpeedInput", mainPanel.transform, "");
            speedField.transform.SetRect(0.6f, 0.08f, 0.9f, 0.32f);
            speedField.text = "1";
            speedField.textComponent.alignment = TextAnchor.MiddleCenter;
            speedField.onEndEdit.AddListener(UpdateSelectedTrackers);
        }

        private void UpdateSelectedTrackers(string input)
        {
            if(!float.TryParse(input, out float parsedSpeed))
            {
                parsedSpeed = 1f;
                speedField.text = "1";
            }

            foreach(var objectCtrl in Studio.Studio.Instance.treeNodeCtrl.selectObjectCtrl)
            {
                if(objectCtrl is OCILight light)
                {
                    var tracker = light.light.gameObject.GetComponent<TrackTransform>();
                    if(tracker)
                        tracker.rotationSpeed = parsedSpeed;
                }
            }
        }

        private void SetTargetsForSelected(Text targetText)
        {
            var lightlist = new List<OCILight>();
            var charalist = new List<OCIChar>();

            foreach(var objectCtrl in Studio.Studio.Instance.treeNodeCtrl.selectObjectCtrl)
            {
                if(objectCtrl is OCILight light)
                {
                    if(light.lightType == LightType.Spot)
                        lightlist.Add(light);
                }
                else if(objectCtrl is OCIChar chara)
                {
                    charalist.Add(chara);
                }
            }

            if(charalist.Count > 0)
            {
                targetText.text = charalist[0].charInfo.chaFile.parameter.fullname;

                if(!float.TryParse(speedField.text, out float parsedSpeed))
                    parsedSpeed = 1f;

                foreach(var ocilight in lightlist)
                {
                    var tracker = ocilight.light.gameObject.GetComponent<TrackTransform>();
                    if(!tracker) tracker = ocilight.light.gameObject.AddComponent<TrackTransform>();
                    tracker.target = charalist[0].charInfo.objBody.transform;
                    tracker.targetKey = charalist[0].objectInfo.dicKey;
                    tracker.targetName = charalist[0].charInfo.chaFile.parameter.fullname;
                    tracker.rotationSpeed = parsedSpeed;
                }
            }
            else
            {
                targetText.text = "None";

                foreach(var ocilight in lightlist)
                {
                    var tracker = ocilight.light.gameObject.GetComponent<TrackTransform>();
                    if(tracker)
                        DestroyImmediate(tracker);
                }
            }
        }
    }
}
