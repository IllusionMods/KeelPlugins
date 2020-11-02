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
            var container = CreateContainer(27, index);

            var btn1 = CreateButton("1", () => VanillaUI.OutfitDropDown.value = 0, container.transform);
            var btn2 = CreateButton("2", () => VanillaUI.OutfitDropDown.value = 1, container.transform);
            var btn3 = CreateButton("3", () => VanillaUI.OutfitDropDown.value = 2, container.transform);
            var btn4 = CreateButton("4", () => VanillaUI.OutfitDropDown.value = 3, container.transform);
            var btn5 = CreateButton("5", () => VanillaUI.OutfitDropDown.value = 4, container.transform);
            var btn6 = CreateButton("6", () => VanillaUI.OutfitDropDown.value = 5, container.transform);
            var btn7 = CreateButton("7", () => VanillaUI.OutfitDropDown.value = 6, container.transform);

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
                CreateClothingStateButtons("Top", ChaFileDefine.ClothesKind.top),
                CreateClothingStateButtons("Bottom", ChaFileDefine.ClothesKind.shorts),
                CreateClothingStateButtons("Bra", ChaFileDefine.ClothesKind.bra),
                CreateClothingStateButtons("Underwear", ChaFileDefine.ClothesKind.bot),
                CreateClothingStateButtons("Pantyhose", ChaFileDefine.ClothesKind.panst),
                CreateClothingStateButtons("Gloves", ChaFileDefine.ClothesKind.gloves),
                CreateClothingStateButtons("Legwear", ChaFileDefine.ClothesKind.socks),
                CreateClothingStateButtons("Shoes", ChaFileDefine.ClothesKind.shoes_inner)
            };

            return delete;

            GameObject CreateClothingStateButtons(string text, ChaFileDefine.ClothesKind kind)
            {
                var container = CreateContainer(25, index + counter++);

                var pos = 0.4f;
                var step = (1f - pos - 0.03f) / 4f;

                var textElem = CreateText(text, container.transform);
                textElem.transform.SetRect(0.03f, 0f, pos, 1f);

                var buttonOn = CreateButton("On", () => VanillaUI.Character.SetClothesState((int)kind, 0), container.transform);
                buttonOn.transform.SetRect(pos, 0f, pos += step, 1f);

                var buttonHalf1 = CreateButton("½", () => VanillaUI.Character.SetClothesState((int)kind, 1), container.transform);
                buttonHalf1.transform.SetRect(pos, 0f, pos += step, 1f);

                var buttonHalf2 = CreateButton("½", () => VanillaUI.Character.SetClothesState((int)kind, 2), container.transform);
                buttonHalf2.transform.SetRect(pos, 0f, pos += step, 1f);

                var buttonOff = CreateButton("Off", () => VanillaUI.Character.SetClothesState((int)kind, 3), container.transform);
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

        public static GameObject CreateText(string text, Transform parent)
        {
            var copyTxt = GameObject.Instantiate(VanillaUI.NormalTextTemplate, parent);

            var textMesh = copyTxt.GetComponentInChildren<TextMeshProUGUI>();
            textMesh.text = text;
            textMesh.enableAutoSizing = false;
            textMesh.fontSize = 12;
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

        public static GameObject CreateButton(string text, UnityAction onClick, Transform parent)
        {
            var copyBtn = GameObject.Instantiate(VanillaUI.ButtonTemplate, parent);

            var textMesh = copyBtn.GetComponentInChildren<TextMeshProUGUI>();
            textMesh.text = text;
            textMesh.enableAutoSizing = false;
            textMesh.fontSize = 10;
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
    }
}
