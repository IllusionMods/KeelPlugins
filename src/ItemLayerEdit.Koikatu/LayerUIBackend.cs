using TMPro;
using UILib;
using UnityEngine;
using UnityEngine.UI;

namespace KeelPlugins
{
    internal static class LayerUIBackend
    {
        private const string panelTemplatePath = "StudioScene/Canvas Main Menu/02_Manipulate/01_Item/Image Color1 Background";
        private const string textTemplatePath = "StudioScene/Canvas Main Menu/02_Manipulate/01_Item/Image Line/TextMeshPro Width";
        private const string sliderTemplatePath = "StudioScene/Canvas Main Menu/02_Manipulate/01_Item/Image Line/Slider Width";
        private const string inputTemplatePath = "StudioScene/Canvas Main Menu/02_Manipulate/01_Item/Image Line/TextMeshPro - InputField Width";
        private const string defButtonTemplatePath = "StudioScene/Canvas Main Menu/02_Manipulate/01_Item/Image Line/Button Width Default";

        internal static GameObject CreatePanel()
        {
            var panelTemplate = GameObject.Find(panelTemplatePath);

            var panel = GameObject.Instantiate(panelTemplate, panelTemplate.transform.parent, true);
            panel.SetActive(true);
            panel.name = "ItemLayerEdit";
            panel.transform.localScale = Vector3.one;
            var layoutElement = panel.GetComponent<LayoutElement>();
            layoutElement.preferredHeight = 30f;
            layoutElement.minHeight = 30f;
            foreach(Transform child in panel.transform)
                GameObject.Destroy(child.gameObject);

            return panel;
        }

        internal static TextMeshProUGUI CreateText(Transform parent)
        {
            var textTemplate = GameObject.Find(textTemplatePath);

            var layerTextObject = GameObject.Instantiate(textTemplate, parent, true);
            layerTextObject.name = "LayerTextObject";
            layerTextObject.transform.SetRect(0.01f, 0.1f, 0.13f, 0.9f);
            var layerTextComponent = layerTextObject.GetComponent<TextMeshProUGUI>();
            layerTextComponent.text = "Layer";

            return layerTextComponent;
        }

        internal static Slider CreateSlider(Transform parent)
        {
            var sliderTemplate = GameObject.Find(sliderTemplatePath);

            var layerSliderObject = GameObject.Instantiate(sliderTemplate, parent, true);
            layerSliderObject.name = "LayerSliderObject";
            layerSliderObject.transform.SetRect(0.15f, 0.35f, 0.71f, 0.65f);
            var layerSliderComponent = layerSliderObject.GetComponent<Slider>();
            layerSliderComponent.wholeNumbers = true;
            layerSliderComponent.minValue = 0;
            layerSliderComponent.maxValue = 30;

            return layerSliderComponent;
        }

        internal static TMP_InputField CreateInputfield(Transform parent)
        {
            var inputTemplate = GameObject.Find(inputTemplatePath);

            var layerInputObject = GameObject.Instantiate(inputTemplate, parent, true);
            layerInputObject.name = "LayerInputObject";
            layerInputObject.transform.SetRect(0.72f, 0.15f, 0.84f, 0.85f);
            var layerInputComponent = layerInputObject.GetComponent<TMP_InputField>();

            return layerInputComponent;
        }

        internal static Button CreateButton(Transform parent)
        {
            var defButtonTemplate = GameObject.Find(defButtonTemplatePath);

            var layerDefButtonObject = GameObject.Instantiate(defButtonTemplate, parent, true);
            layerDefButtonObject.name = "LayerDefButtonObject";
            layerDefButtonObject.transform.SetRect(0.86f, 0.15f, 0.97f, 0.85f);
            var layerDefButtonComponent = layerDefButtonObject.GetComponent<Button>();

            return layerDefButtonComponent;
        }
    }
}
