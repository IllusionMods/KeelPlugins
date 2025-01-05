using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using KeelPlugins;
using KeelPlugins.Utils;
using KKAPI;
#if KKS
using TMPro;
#endif
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[assembly: System.Reflection.AssemblyVersion(DefaultParamEditor.Koikatu.DefaultParamEditor.Version)]

namespace DefaultParamEditor.Koikatu
{
    [BepInPlugin(GUID, "DefaultParamEditor", Version)]
    [BepInProcess(Constants.StudioProcessName)]
    [BepInDependency(KoikatuAPI.GUID, KoikatuAPI.VersionConst)]
    [BepInDependency(Sideloader.Sideloader.GUID)]
    public class DefaultParamEditor : BaseUnityPlugin
    {
        public const string GUID = "keelhauled.defaultparameditor";
        public const string Version = "1.4.2." + BuildNumber.Version;

        private static ConfigEntry<bool> CreateUISaveButtons { get; set; }

        private void Awake()
        {
            Log.SetLogSource(Logger);

            CreateUISaveButtons = Config.Bind("General", "Create UI Save Buttons", false, new ConfigDescription("Create save buttons in the studio UI next to the load buttons.\nRequires restart", null, new ConfigurationManagerAttributes { Order = 1 }));
            Config.Bind("General", "Character Parameters", "", new ConfigDescription("", null, new ConfigurationManagerAttributes { Order = 4, HideDefaultButton = true, CustomDrawer = CharaParamDrawer }));
            Config.Bind("General", "Scene Parameters", "", new ConfigDescription("", null, new ConfigurationManagerAttributes { Order = 5, HideDefaultButton = true, CustomDrawer = SceneParamDrawer }));

            Harmony.CreateAndPatchAll(typeof(Hooks));
            Harmony.CreateAndPatchAll(typeof(CharacterParam.Hooks));
            Harmony.CreateAndPatchAll(typeof(SceneParam.Hooks));
        }

        private void SceneParamDrawer(ConfigEntryBase configEntry)
        {
            if(GUILayout.Button("Save", GUILayout.ExpandWidth(true)))
            {
                SceneParam.Save();
                ParamData.SaveToFile();
            }

            if(GUILayout.Button("Reset", GUILayout.ExpandWidth(false)))
            {
                SceneParam.Reset();
                ParamData.SaveToFile();
            }
        }

        private void CharaParamDrawer(ConfigEntryBase configEntry)
        {
            if(GUILayout.Button("Save", GUILayout.ExpandWidth(true)))
            {
                CharacterParam.Save();
                ParamData.SaveToFile();
            }

            if(GUILayout.Button("Reset", GUILayout.ExpandWidth(false)))
            {
                CharacterParam.Reset();
                ParamData.SaveToFile();
            }
        }

        private class Hooks
        {
            [HarmonyPostfix, HarmonyPatch(typeof(Studio.Studio), nameof(Studio.Studio.Init))]
            public static void CreateUI()
            {
                var mainlist = SetupList("StudioScene/Canvas Main Menu/04_System");
                CreateMainButton("Load scene param", mainlist, SceneParam.Load);
                var charalist = SetupList("StudioScene/Canvas Main Menu/02_Manipulate/00_Chara/00_Root");
                CreateCharaButton("Load chara param", charalist, CharacterParam.Load);

                if(CreateUISaveButtons.Value)
                {
                    CreateMainButton("Save scene param", mainlist, () =>
                    {
                        SceneParam.Save();
                        ParamData.SaveToFile();
                    });
                    CreateCharaButton("Save chara param", charalist, () =>
                    {
                        CharacterParam.Save();
                        ParamData.SaveToFile();
                    });
                }
            }

            private static ScrollRect SetupList(string goPath)
            {
                var listObject = GameObject.Find(goPath);
                var scrollRect = listObject.GetComponent<ScrollRect>();
                scrollRect.content.gameObject.GetOrAddComponent<VerticalLayoutGroup>();
                scrollRect.content.gameObject.GetOrAddComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;
                scrollRect.scrollSensitivity = 25;

                foreach (Transform item in scrollRect.content.transform)
                {
                    var layoutElement = item.gameObject.GetOrAddComponent<LayoutElement>();
                    layoutElement.preferredHeight = 40;
                }

                return scrollRect;
            }

            private static Button CreateMainButton(string name, ScrollRect scrollRect, UnityAction onClickEvent)
            {
                return CreateButton(name, scrollRect, onClickEvent, "StudioScene/Canvas Main Menu/04_System/Viewport/Content/End");
            }

            private static Button CreateCharaButton(string name, ScrollRect scrollRect, UnityAction onClickEvent)
            {
                return CreateButton(name, scrollRect, onClickEvent, "StudioScene/Canvas Main Menu/02_Manipulate/00_Chara/00_Root/Viewport/Content/State");
            }

            private static Button CreateButton(string name, ScrollRect scrollRect, UnityAction onClickEvent, string goPath)
            {
                var template = GameObject.Find(goPath);
                var newObject = Instantiate(template, scrollRect.content.transform);
                newObject.name = "NewObject";
                var textComponent = newObject.GetComponentInChildren<Text>();
                if (textComponent != null)
                {
                    textComponent.text = name;
                }
#if KKS
                else
                {
                    var textComponent2 = newObject.GetComponentInChildren<TextMeshProUGUI>();
                    textComponent2.text = name;
                }
#endif
                var buttonComponent = newObject.GetComponent<Button>();
                buttonComponent.onClick.ActuallyRemoveAllListeners();
                buttonComponent.onClick.AddListener(onClickEvent);
                return buttonComponent;
            }
        }
    }
}
