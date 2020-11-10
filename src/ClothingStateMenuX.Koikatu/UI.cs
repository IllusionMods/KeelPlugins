using System.Collections.Generic;
using TMPro;
using UILib;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace ClothingStateMenuX.Koikatu
{
    public static class UI
    {
        private static string ID = "(CSMX)";

        public static void CreateUI()
        {
            CreateTitle("Clothing Sets", 0);
            CreateClothingSets(1);
            CreateSeparator(2);
            CreateClothingOptions(VanillaUI.ClothingStateToggles.transform.GetSiblingIndex() + 1);
        }

        public static void CreateClothingSets(int index)
        {
            var container = CreateContainer(25, index);

            var buttons = new List<GameObject>
            {
                CreateButton("1", 14, () => VanillaUI.OutfitDropDown.value = 0, container.transform),
                CreateButton("2", 14, () => VanillaUI.OutfitDropDown.value = 1, container.transform),
                CreateButton("3", 14, () => VanillaUI.OutfitDropDown.value = 2, container.transform),
                CreateButton("4", 14, () => VanillaUI.OutfitDropDown.value = 3, container.transform),
                CreateButton("5", 14, () => VanillaUI.OutfitDropDown.value = 4, container.transform),
                CreateButton("6", 14, () => VanillaUI.OutfitDropDown.value = 5, container.transform),
                CreateButton("7", 14, () => VanillaUI.OutfitDropDown.value = 6, container.transform),
            };

            var pos = 0.03f;
            var step = (1f - pos * 2) / 7f;

            foreach(var button in buttons)
                button.transform.SetRect(pos, 0f, pos += step, 1f);

            buttons[VanillaUI.OutfitDropDown.value].GetComponent<Button>().SetColorMultiplier(0.7f);

            VanillaUI.OutfitDropDown.onValueChanged.AddListener(SetMultipliers);

            void SetMultipliers(int x)
            {
                foreach(var button in buttons)
                    button.GetComponent<Button>().SetColorMultiplier(1f);

                buttons[x].GetComponent<Button>().SetColorMultiplier(0.7f);
            }
        }

        public static void CreateClothingOptions(int index)
        {
            int counter = 0;

            CreateClothingStateButtons("Top", ChaFileDefine.ClothesKind.top, 3);
            CreateClothingStateButtons("Bottom", ChaFileDefine.ClothesKind.bot, 3);
            CreateClothingStateButtons("Bra", ChaFileDefine.ClothesKind.bra, 3);
            CreateClothingStateButtons("Underwear", ChaFileDefine.ClothesKind.shorts, 4);
            CreateClothingStateButtons("Pantyhose", ChaFileDefine.ClothesKind.panst, 3);
            CreateClothingStateButtons("Gloves", ChaFileDefine.ClothesKind.gloves, 2);
            CreateClothingStateButtons("Legwear", ChaFileDefine.ClothesKind.socks, 2);
            CreateClothingStateButtons("Shoes", ChaFileDefine.ClothesKind.shoes_inner, 2);

            GameObject CreateClothingStateButtons(string text, ChaFileDefine.ClothesKind kind, int buttons)
            {
                var container = CreateContainer(22, index + counter++);

                var margin = 0.03f;
                var pos = 0.4f;
                var step = (1f - pos - margin) / 4f;

                var textElem = CreateText(text, 12, container.transform);
                textElem.transform.SetRect(margin, 0f, pos, 1f);

                var buttonOn = CreateButton("On", 10, () => VanillaUI.Character.SetClothesState((int)kind, 0), container.transform);
                buttonOn.transform.SetRect(pos, 0f, pos += step, 1f);

                var buttonHalf1 = CreateButton("½", 10, () => VanillaUI.Character.SetClothesState((int)kind, 1), container.transform);
                buttonHalf1.transform.SetRect(pos, 0f, pos += step, 1f);
                if(buttons < 3) buttonHalf1.GetComponent<Button>().interactable = false;

                var buttonHalf2 = CreateButton("½", 10, () => VanillaUI.Character.SetClothesState((int)kind, 2), container.transform);
                buttonHalf2.transform.SetRect(pos, 0f, pos += step, 1f);
                if(buttons < 4) buttonHalf2.GetComponent<Button>().interactable = false;

                var buttonOff = CreateButton("Off", 10, () => VanillaUI.Character.SetClothesState((int)kind, 3), container.transform);
                buttonOff.transform.SetRect(pos, 0f, pos += step, 1f);

                return container;
            }
        }

        public static GameObject CreateTitle(string text, int index)
        {
            var copy = GameObject.Instantiate(VanillaUI.TitleTextTemplate, VanillaUI.Sidebar.transform);
            copy.name += ID;
            copy.transform.SetSiblingIndex(index);
            copy.GetComponentInChildren<TextMeshProUGUI>().text = text;
            return copy;
        }

        public static GameObject CreateText(string text, float fontSize, Transform parent)
        {
            var copy = GameObject.Instantiate(VanillaUI.NormalTextTemplate, parent);
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
            var copy = GameObject.Instantiate(VanillaUI.SeparatorTemplate, VanillaUI.Sidebar.transform);
            copy.name += ID;
            copy.transform.SetSiblingIndex(index);
            return copy;
        }

        public static GameObject CreateButton(string text, float fontSize, UnityAction onClick, Transform parent)
        {
            var copy = GameObject.Instantiate(VanillaUI.ButtonTemplate, parent);
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
            var copy = GameObject.Instantiate(VanillaUI.ButtonContainerTemplate, VanillaUI.Sidebar.transform);
            copy.name += ID;
            copy.transform.SetSiblingIndex(index);
            copy.GetComponent<LayoutElement>().minHeight = minHeight;

            foreach(Transform t in copy.transform)
                GameObject.DestroyImmediate(t.gameObject);

            return copy;
        }

        public static void SetColorMultiplier(this Button button, float value)
        {
            var color = button.colors;
            color.colorMultiplier = value;
            button.colors = color;
        }
    }
}
