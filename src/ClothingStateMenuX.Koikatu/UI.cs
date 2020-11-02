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
            var container = CreateContainer("ClothingSetsContainer", 28, index);

            var btn1 = CreateButton(container.transform, "1", () => VanillaUI.OutfitDropDown.value = 0);
            var btn2 = CreateButton(container.transform, "2", () => VanillaUI.OutfitDropDown.value = 1);
            var btn3 = CreateButton(container.transform, "3", () => VanillaUI.OutfitDropDown.value = 2);
            var btn4 = CreateButton(container.transform, "4", () => VanillaUI.OutfitDropDown.value = 3);
            var btn5 = CreateButton(container.transform, "5", () => VanillaUI.OutfitDropDown.value = 4);
            var btn6 = CreateButton(container.transform, "6", () => VanillaUI.OutfitDropDown.value = 5);
            var btn7 = CreateButton(container.transform, "7", () => VanillaUI.OutfitDropDown.value = 6);

            var pos = 0f;
            var step = 1f / 7f;
            btn1.transform.SetRect(pos, 0f, pos += step, 1f);
            btn2.transform.SetRect(pos, 0f, pos += step, 1f);
            btn3.transform.SetRect(pos, 0f, pos += step, 1f);
            btn4.transform.SetRect(pos, 0f, pos += step, 1f);
            btn5.transform.SetRect(pos, 0f, pos += step, 1f);
            btn6.transform.SetRect(pos, 0f, pos += step, 1f);
            btn7.transform.SetRect(pos, 0f, pos += step, 1f);

            return container;
        }

        public static GameObject CreateClothingOptions()
        {
            var container = CreateContainer("ClothingOptionsContainer", 175, index);
            container.AddComponent<VerticalLayoutGroup>();

            CreateClothingStateButtons("Top", ChaFileDefine.ClothesKind.top);
            CreateClothingStateButtons("Bottom", ChaFileDefine.ClothesKind.shorts);
            CreateClothingStateButtons("Bra", ChaFileDefine.ClothesKind.bra);
            CreateClothingStateButtons("Underwear", ChaFileDefine.ClothesKind.bot);
            CreateClothingStateButtons("Pantyhose", ChaFileDefine.ClothesKind.panst);
            CreateClothingStateButtons("Gloves", ChaFileDefine.ClothesKind.gloves);
            CreateClothingStateButtons("Legwear", ChaFileDefine.ClothesKind.socks);
            CreateClothingStateButtons("Shoes", ChaFileDefine.ClothesKind.shoes_inner);

            return container;

            void CreateClothingStateButtons(string text, ChaFileDefine.ClothesKind kind)
            {
                var pos = 0.3f;
                var step = (1f - pos) / 4f;

                var containerX = CreateContainer(text, 25, container.transform);
                var textElem = CreateText(containerX.transform, text);
                textElem.transform.SetRect(0f, 0f, pos, 1f);

                var buttonOn = CreateButton(containerX.transform, "On", () => VanillaUI.Character.SetClothesState((int)kind, 0));
                buttonOn.transform.SetRect(pos, 0f, pos += step, 1f);

                var buttonHalf1 = CreateButton(containerX.transform, "½", () => VanillaUI.Character.SetClothesState((int)kind, 1));
                buttonHalf1.transform.SetRect(pos, 0f, pos += step, 1f);

                var buttonHalf2 = CreateButton(containerX.transform, "½", () => VanillaUI.Character.SetClothesState((int)kind, 2));
                buttonHalf2.transform.SetRect(pos, 0f, pos += step, 1f);

                var buttonOff = CreateButton(containerX.transform, "Off", () => VanillaUI.Character.SetClothesState((int)kind, 3));
                buttonOff.transform.SetRect(pos, 0f, pos += step, 1f);
            }
        }

        public static GameObject CreateAccessories()
        {
            return new GameObject();
        }

        public static GameObject CreateTitle(string text, int index)
        {
            var copyTxt = GameObject.Instantiate(VanillaUI.TitleTextTemplate, VanillaUI.Sidebar.transform);
            copyTxt.transform.SetSiblingIndex(index);
            copyTxt.GetComponentInChildren<TextMeshProUGUI>().text = text;
            return copyTxt;
        }

        public static GameObject CreateText(Transform parent, string text)
        {
            var copyTxt = GameObject.Instantiate(VanillaUI.TitleTextTemplate, parent);
            copyTxt.GetComponentInChildren<TextMeshProUGUI>().text = text;
            return copyTxt;
        }

        public static GameObject CreateSeparator(int index)
        {
            var sep = GameObject.Instantiate(VanillaUI.SeparatorTemplate, VanillaUI.Sidebar.transform);
            sep.transform.SetSiblingIndex(index);
            return sep;
        }

        public static GameObject CreateButton(Transform parent, string name, UnityAction onClick)
        {
            var copyBtn = GameObject.Instantiate(VanillaUI.ButtonTemplate, parent);
            copyBtn.GetComponentInChildren<TextMeshProUGUI>().text = name;
            var btnComp = copyBtn.GetComponent<Button>();
            btnComp.onClick.RemoveAllListeners();
            btnComp.onClick.AddListener(onClick);
            return copyBtn;
        }

        public static GameObject CreateContainer(string name, float minHeight, int index)
        {
            var container = new GameObject(name);
            container.transform.parent = VanillaUI.Sidebar.transform;
            container.transform.SetSiblingIndex(index);
            container.AddComponent<LayoutElement>().minHeight = minHeight;
            return container;
        }

        public static GameObject CreateContainer(string name, float minHeight, Transform parent)
        {
            var container = new GameObject(name);
            container.transform.parent = parent;
            container.AddComponent<LayoutElement>().minHeight = minHeight;
            return container;
        }
    }
}
