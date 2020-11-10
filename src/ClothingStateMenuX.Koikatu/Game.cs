using ChaCustom;
using HarmonyLib;
using KeelPlugins;
using TMPro;
using UnityEngine;

namespace ClothingStateMenuX.Koikatu
{
    public static class Game
    {
        public static Lazy<ChaControl> Character = new Lazy<ChaControl>(() => GameObject.FindObjectOfType<ChaControl>());
        public static Lazy<GameObject> Sidebar = new Lazy<GameObject>(() => GameObject.Find("CustomScene/CustomRoot/FrontUIGroup/CvsDraw/Top"));
        public static Lazy<GameObject> AccessoryToggles = new Lazy<GameObject>(() => Sidebar.Value.transform.Find("tglAcsGrp").gameObject);
        public static Lazy<GameObject> ClothingStateToggles = new Lazy<GameObject>(() => Sidebar.Value.transform.Find("rbClothesState").gameObject);
        public static Lazy<TMP_Dropdown> OutfitDropDown = new Lazy<TMP_Dropdown>(() => Traverse.Create(Singleton<CustomControl>.Instance).Field("ddCoordinate").GetValue<TMP_Dropdown>());
        public static Lazy<GameObject> TitleTextTemplate = new Lazy<GameObject>(() => Sidebar.Value.transform.Find("txtClothesState").gameObject);
        public static Lazy<GameObject> NormalTextTemplate = new Lazy<GameObject>(() => Sidebar.Value.transform.Find("rbClothesState/imgRbCol00/textRbSelect").gameObject);
        public static Lazy<GameObject> ButtonTemplate = new Lazy<GameObject>(() => Sidebar.Value.transform.Find("btnLightingInitialize/btnSelect").gameObject);
        public static Lazy<GameObject> ButtonContainerTemplate = new Lazy<GameObject>(() => Sidebar.Value.transform.Find("btnLightingInitialize").gameObject);
        public static Lazy<GameObject> SeparatorTemplate = new Lazy<GameObject>(() => Sidebar.Value.transform.Find("Separate").gameObject);
    }
}
