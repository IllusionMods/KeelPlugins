using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ChaCustom;
using HarmonyLib;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace ClothingStateMenuX.Koikatu
{
    public static class UI
    {
        public static GameObject CreateClothingSets(int index)
        {
            var container = CreateContainer(25, index);

            var btn1 = CreateButton("1", 14, () => VanillaUI.OutfitDropDown.value = 0, container.transform);
            var btn2 = CreateButton("2", 14, () => VanillaUI.OutfitDropDown.value = 1, container.transform);
            var btn3 = CreateButton("3", 14, () => VanillaUI.OutfitDropDown.value = 2, container.transform);
            var btn4 = CreateButton("4", 14, () => VanillaUI.OutfitDropDown.value = 3, container.transform);
            var btn5 = CreateButton("5", 14, () => VanillaUI.OutfitDropDown.value = 4, container.transform);
            var btn6 = CreateButton("6", 14, () => VanillaUI.OutfitDropDown.value = 5, container.transform);
            var btn7 = CreateButton("7", 14, () => VanillaUI.OutfitDropDown.value = 6, container.transform);

            var pos = 0.03f;
            var step = (1f - pos * 2) / 7f;
            btn1.transform.SetRect(pos, 0f, pos += step, 1f);
            btn2.transform.SetRect(pos, 0f, pos += step, 1f);
            btn3.transform.SetRect(pos, 0f, pos += step, 1f);
            btn4.transform.SetRect(pos, 0f, pos += step, 1f);
            btn5.transform.SetRect(pos, 0f, pos += step, 1f);
            btn6.transform.SetRect(pos, 0f, pos += step, 1f);
            btn7.transform.SetRect(pos, 0f, pos += step, 1f);

            return container;
        }

        public static List<GameObject> CreateClothingOptions(int index)
        {
            int counter = 0;

            var delete = new List<GameObject>
            {
                CreateClothingStateButtons("Top", ChaFileDefine.ClothesKind.top, 3),
                CreateClothingStateButtons("Bottom", ChaFileDefine.ClothesKind.bot, 3),
                CreateClothingStateButtons("Bra", ChaFileDefine.ClothesKind.bra, 3),
                CreateClothingStateButtons("Underwear", ChaFileDefine.ClothesKind.shorts, 4),
                CreateClothingStateButtons("Pantyhose", ChaFileDefine.ClothesKind.panst, 3),
                CreateClothingStateButtons("Gloves", ChaFileDefine.ClothesKind.gloves, 2),
                CreateClothingStateButtons("Legwear", ChaFileDefine.ClothesKind.socks, 2),
                CreateClothingStateButtons("Shoes", ChaFileDefine.ClothesKind.shoes_inner, 2)
            };

            return delete;

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
            var copyTxt = GameObject.Instantiate(VanillaUI.TitleTextTemplate, VanillaUI.Sidebar.transform);
            copyTxt.transform.SetSiblingIndex(index);
            copyTxt.GetComponentInChildren<TextMeshProUGUI>().text = text;
            return copyTxt;
        }

        public static GameObject CreateText(string text, float fontSize, Transform parent)
        {
            var copyTxt = GameObject.Instantiate(VanillaUI.NormalTextTemplate, parent);

            var textMesh = copyTxt.GetComponentInChildren<TextMeshProUGUI>();
            textMesh.text = text;
            textMesh.enableAutoSizing = false;
            textMesh.fontSize = fontSize;
            textMesh.margin = new Vector4(0, 0);
            textMesh.transform.SetRect();
            textMesh.alignment = TextAlignmentOptions.Center;

            return copyTxt;
        }

        public static GameObject CreateSeparator(int index)
        {
            var sep = GameObject.Instantiate(VanillaUI.SeparatorTemplate, VanillaUI.Sidebar.transform);
            sep.transform.SetSiblingIndex(index);
            return sep;
        }

        public static GameObject CreateButton(string text, float fontSize, UnityAction onClick, Transform parent)
        {
            var copyBtn = GameObject.Instantiate(VanillaUI.ButtonTemplate, parent);

            var textMesh = copyBtn.GetComponentInChildren<TextMeshProUGUI>();
            textMesh.text = text;
            textMesh.enableAutoSizing = false;
            textMesh.fontSize = fontSize;
            textMesh.margin = new Vector4(0, 0);
            textMesh.transform.SetRect();

            var btnComp = copyBtn.GetComponent<Button>();
            btnComp.onClick.RemoveAllListeners();
            btnComp.onClick.AddListener(onClick);

            return copyBtn;
        }

        public static GameObject CreateContainer(float minHeight, int index)
        {
            var copy = GameObject.Instantiate(VanillaUI.ButtonContainerTemplate, VanillaUI.Sidebar.transform);
            copy.transform.SetSiblingIndex(index);
            copy.GetComponent<LayoutElement>().minHeight = minHeight;

            foreach(Transform t in copy.transform)
                GameObject.DestroyImmediate(t.gameObject);

            return copy;
        }

        public static void SetRect(this Transform self, float anchorLeft = 0f, float anchorBottom = 0f, float anchorRight = 1f, float anchorTop = 1f, float offsetLeft = 0f, float offsetBottom = 0f, float offsetRight = 0f, float offsetTop = 0f)
        {
            RectTransform rt = self as RectTransform;
            rt.anchorMin = new Vector2(anchorLeft, anchorBottom);
            rt.anchorMax = new Vector2(anchorRight, anchorTop);
            rt.offsetMin = new Vector2(offsetLeft, offsetBottom);
            rt.offsetMax = new Vector2(offsetRight, offsetTop);
        }
    }
}
