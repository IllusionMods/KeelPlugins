using System.Collections.Generic;
using ChaCustom;
using TMPro;
using UILib;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace ClothingStateMenuX
{
    public static class UI
    {
        private const string ID = "(CSMX)";
        private static TMP_Dropdown outfitDropDown;
        private static GameObject sidebar;
        private static GameObject clothingStateToggles;
        private static GameObject titleTextTemplate;
        private static GameObject normalTextTemplate;
        private static GameObject buttonTemplate;
        private static GameObject buttonContainerTemplate;
        private static GameObject separatorTemplate;

        private static readonly List<GameObject> clothingSetObjects = new List<GameObject>();

        public static void Setup()
        {
            outfitDropDown = Singleton<CustomControl>.Instance.ddCoordinate;
            var scrollView = GameObject.Find("CustomScene/CustomRoot/FrontUIGroup/CvsDraw/Top/Scroll View");
            scrollView.GetComponent<ScrollRect>().scrollSensitivity = 60f;
            sidebar = scrollView.transform.Find("Viewport/Content").gameObject;
            var rbClothesStateTransform = sidebar.transform.Find("rbClothesState");
            clothingStateToggles = rbClothesStateTransform.gameObject;
            titleTextTemplate = sidebar.transform.Find("txtClothesState").gameObject;
            normalTextTemplate = rbClothesStateTransform.transform.Find("imgRbCol00/textRbSelect").gameObject;
            var btnLightingInitializeTransform = sidebar.transform.Find("btnLightingInitialize");
            buttonContainerTemplate = btnLightingInitializeTransform.gameObject;
            buttonTemplate = btnLightingInitializeTransform.transform.Find("btnDefault").gameObject;
            separatorTemplate = sidebar.transform.Find("Separate").gameObject;

            var btnDelete = GameObject.Find("CustomScene/CustomRoot/FrontUIGroup/CustomUIGroup/CvsMenuTree/06_SystemTop/charaFileControl/charaFileWindow/WinRect/Save/btnDelete");
            var buttonTemplateBtn = buttonTemplate.GetComponent<Button>();
            var tempSprites = buttonTemplateBtn.spriteState;
            tempSprites.disabledSprite = btnDelete.GetComponent<Button>().spriteState.disabledSprite;
            buttonTemplateBtn.spriteState = tempSprites;
            
            CreateClothingOptions(clothingStateToggles.transform.GetSiblingIndex() + 1);

            foreach(var trigger in sidebar.GetComponentsInChildren<ObservableScrollTrigger>())
                trigger.enabled = false;
        }

        public static void ReloadClothingSets()
        {
            RemoveClothingSets();

            clothingSetObjects.Add(CreateTitle("Clothing Sets", 0));
            
            var index = 1;
            for (int i = 0; i < outfitDropDown.options.Count; i++)
            {
                var container = CreateContainer(25, index++);
                clothingSetObjects.Add(container);
                var option = outfitDropDown.options[i];
                var optionIndex = i;
                var button = CreateButton(option.text, 14, () => outfitDropDown.value = optionIndex, container.transform);
                button.transform.SetRect(0f, 0f, 1f, 1f);
            }

            clothingSetObjects.Add(CreateSeparator(index));
        }

        public static void RemoveClothingSets()
        {
            foreach (var obj in clothingSetObjects)
                GameObject.Destroy(obj);
            clothingSetObjects.Clear();
        }

        public static void CreateClothingOptions(int index)
        {
            CreateClothingStateButtons("Top", ChaFileDefine.ClothesKind.top, 3);
            CreateClothingStateButtons("Bottom", ChaFileDefine.ClothesKind.bot, 3);
            CreateClothingStateButtons("Bra", ChaFileDefine.ClothesKind.bra, 3);
            CreateClothingStateButtons("Underwear", ChaFileDefine.ClothesKind.shorts, 4);
            CreateClothingStateButtons("Pantyhose", ChaFileDefine.ClothesKind.panst, 3);
            CreateClothingStateButtons("Gloves", ChaFileDefine.ClothesKind.gloves, 2);
            CreateClothingStateButtons("Legwear", ChaFileDefine.ClothesKind.socks, 2);
            CreateClothingStateButtons("Shoes", ChaFileDefine.ClothesKind.shoes_inner, 2);

            void CreateClothingStateButtons(string text, ChaFileDefine.ClothesKind kind, int buttons)
            {
                var container = CreateContainer(22, index++);

                const float margin = 0.03f;
                var pos = 0.4f;
                var step = (1f - pos - margin) / 4f;

                var textElem = CreateText(text, 12, container.transform);
                textElem.transform.SetRect(margin, 0f, pos, 1f);

                var character = KKAPI.Maker.MakerAPI.GetCharacterControl();

                var buttonOn = CreateButton("On", 10, () => character.SetClothesState((int)kind, 0), container.transform);
                buttonOn.transform.SetRect(pos, 0f, pos += step, 1f);

                var buttonHalf1 = CreateButton("½", 10, () => character.SetClothesState((int)kind, 1), container.transform);
                buttonHalf1.transform.SetRect(pos, 0f, pos += step, 1f);
                if(buttons < 3) buttonHalf1.GetComponent<Button>().interactable = false;

                var buttonHalf2 = CreateButton("½", 10, () => character.SetClothesState((int)kind, 2), container.transform);
                buttonHalf2.transform.SetRect(pos, 0f, pos += step, 1f);
                if(buttons < 4) buttonHalf2.GetComponent<Button>().interactable = false;

                var buttonOff = CreateButton("Off", 10, () => character.SetClothesState((int)kind, 3), container.transform);
                buttonOff.transform.SetRect(pos, 0f, pos += step, 1f);
            }
        }

        public static GameObject CreateTitle(string text, int index)
        {
            var copy = GameObject.Instantiate(titleTextTemplate, sidebar.transform);
            copy.name += ID;
            copy.transform.SetSiblingIndex(index);
            copy.GetComponentInChildren<TextMeshProUGUI>().text = text;
            return copy;
        }

        public static GameObject CreateText(string text, float fontSize, Transform parent)
        {
            var copy = GameObject.Instantiate(normalTextTemplate, parent);
            copy.name += ID;

            var textMesh = copy.GetComponentInChildren<TextMeshProUGUI>();
            textMesh.text = text;
            textMesh.enableAutoSizing = false;
            textMesh.fontSize = fontSize;
            textMesh.transform.SetRect();
            textMesh.alignment = TextAlignmentOptions.Center;

            return copy;
        }

        public static GameObject CreateSeparator(int index)
        {
            var copy = GameObject.Instantiate(separatorTemplate, sidebar.transform);
            copy.name += ID;
            copy.transform.SetSiblingIndex(index);
            return copy;
        }

        public static GameObject CreateButton(string text, float fontSize, UnityAction onClick, Transform parent)
        {
            var copy = GameObject.Instantiate(buttonTemplate, parent);
            copy.name += ID;

            var textMesh = copy.GetComponentInChildren<TextMeshProUGUI>();
            textMesh.text = text;
            textMesh.enableAutoSizing = false;
            textMesh.fontSize = fontSize;
            textMesh.transform.SetRect();

            var button = copy.GetComponent<Button>();
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(onClick);

            return copy;
        }

        public static GameObject CreateContainer(float minHeight, int index)
        {
            var copy = GameObject.Instantiate(buttonContainerTemplate, sidebar.transform);
            copy.name += ID;
            copy.transform.SetSiblingIndex(index);
            copy.GetComponent<LayoutElement>().minHeight = minHeight;

            foreach(Transform t in copy.transform)
                GameObject.DestroyImmediate(t.gameObject);

            return copy;
        }
    }
}
