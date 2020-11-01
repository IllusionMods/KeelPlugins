using ChaCustom;
using HarmonyLib;
using TMPro;
using UnityEngine;

namespace ClothingStateMenuX.Koikatu
{
    public static class VanillaUI
    {
        private static GameObject _sidebar;
        public static GameObject Sidebar
        {
            get
            {
                if(_sidebar == null)
                    _sidebar = GameObject.Find("CustomScene/CustomRoot/FrontUIGroup/CvsDraw/Top");
                return _sidebar;
            }
        }

        private static GameObject _accessoryToggles;
        public static GameObject AccessoryToggles
        {
            get
            {
                if(_accessoryToggles == null)
                    _accessoryToggles = Sidebar.transform.Find("tglAcsGrp").gameObject;
                return _accessoryToggles;
            }
        }

        private static GameObject _clothingStateToggles;
        public static GameObject ClothingStateToggles
        {
            get
            {
                if(_clothingStateToggles == null)
                    _clothingStateToggles = Sidebar.transform.Find("rbClothesState").gameObject;
                return _clothingStateToggles;
            }
        }

        private static TMP_Dropdown _outfitDropDown;
        public static TMP_Dropdown OutfitDropDown
        {
            get
            {
                if(_outfitDropDown == null)
                    _outfitDropDown = Traverse.Create(Singleton<CustomControl>.Instance).Field("ddCoordinate").GetValue<TMP_Dropdown>();
                return _outfitDropDown;
            }
        }

        private static GameObject _titleTextTemplate;
        public static GameObject TitleTextTemplate
        {
            get
            {
                if(_titleTextTemplate == null)
                    _titleTextTemplate = Sidebar.transform.Find("txtClothesState").gameObject;
                return _titleTextTemplate;
            }
        }

        private static GameObject _normalTextTemplate;
        public static GameObject NormalTextTemplate
        {
            get
            {
                if(_normalTextTemplate == null)
                    _normalTextTemplate = Sidebar.transform.Find("rbClothesState/imgRbCol00/textRbSelect").gameObject;
                return _normalTextTemplate;
            }
        }

        private static GameObject _buttonTemplate;
        public static GameObject ButtonTemplate
        {
            get
            {
                if(_buttonTemplate == null)
                    _buttonTemplate = Sidebar.transform.Find("btnLightingInitialize/btnSelect").gameObject;
                return _buttonTemplate;
            }
        }

        private static GameObject _separatorTemplate;
        public static GameObject SeparatorTemplate
        {
            get
            {
                if(_separatorTemplate == null)
                    _separatorTemplate = Sidebar.transform.Find("Separate").gameObject;
                return _separatorTemplate;
            }
        }
    }
}
