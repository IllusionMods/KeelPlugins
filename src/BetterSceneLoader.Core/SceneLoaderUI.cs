using BepInEx;
using KKAPI.Studio.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UILib;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using KeelPlugins.Utils;
using UnityEngine.Networking;

// problem adjusting thumbnail size when certain number range of scenes

namespace BetterSceneLoader
{
    public class SceneLoaderUI
    {
        private static readonly string scenePath = BepInEx.Utility.CombinePaths(Paths.GameRootPath, "UserData", "Studio", "scene");

        private float buttonSize = 10f;
        private float marginSize = 5f;
        private float headerSize = 20f;
        private float UIScale = 1.0f;
        private float scrollOffsetX = -15f;

        private readonly Color dragColor = new Color(0.4f, 0.4f, 0.4f, 1f);
        private readonly Color backgroundColor = new Color(1f, 1f, 1f, 1f);
        private readonly Color outlineColor = new Color(0f, 0f, 0f, 1f);

        private Canvas UISystem;
        private Image mainPanel;
        private ScrollRect imagelist;
        private Image optionspanel;
        private Image confirmpanel;
        private Button yesbutton;
        private Button nobutton;
        private Text nametext;
        private ToolbarToggle toolbarToggle;

        private Dictionary<string, Image> sceneCache = new Dictionary<string, Image>();
        private Button currentButton;
        private string currentPath;
        private string currentCategoryFolder = scenePath;
        private Dictionary<string, string> CategoryFolders = new Dictionary<string, string>();

        public UnityAction OnSaveButtonClick;
        public UnityAction<string> OnLoadButtonClick;
        public UnityAction<string> OnDeleteButtonClick;
        public UnityAction<string> OnImportButtonClick;
        public UnityAction<string> OnFolderButtonClick;

        public void ShowWindow(bool flag)
        {
            UISystem.gameObject.SetActive(flag);
            toolbarToggle?.SetValue(flag);
        }

        public void UpdateWindow()
        {
            foreach(var scene in sceneCache)
            {
                var gridlayout = scene.Value.gameObject.GetComponent<AutoGridLayout>();
                if(gridlayout != null)
                {
                    gridlayout.m_Column = BetterSceneLoader.ColumnAmount.Value;
                    gridlayout.CalculateLayoutInputHorizontal();
                }
            }

            if(imagelist != null)
            {
                imagelist.scrollSensitivity = Mathf.Lerp(30f, 300f, BetterSceneLoader.ScrollSensitivity.Value / 10f);
            }

            if(mainPanel)
            {
                mainPanel.transform.SetRect(BetterSceneLoader.AnchorLeft.Value, BetterSceneLoader.AnchorBottom.Value,
                                            BetterSceneLoader.AnchorRight.Value, BetterSceneLoader.AnchorTop.Value,
                                            BetterSceneLoader.UIMargin.Value, BetterSceneLoader.UIMargin.Value,
                                            -BetterSceneLoader.UIMargin.Value, -BetterSceneLoader.UIMargin.Value);
            }
        }

        public void CreateUI()
        {
            UISystem = UIUtility.CreateNewUISystem("BetterSceneLoaderCanvas");
            UISystem.GetComponent<CanvasScaler>().referenceResolution = new Vector2(1920f / UIScale, 1080f / UIScale);
            UISystem.sortingOrder = 10;
            ShowWindow(false);

            mainPanel = UIUtility.CreatePanel("Panel", UISystem.transform);
            mainPanel.color = backgroundColor;
            UIUtility.AddOutlineToObject(mainPanel.transform, outlineColor);

            var drag = UIUtility.CreatePanel("Draggable", mainPanel.transform);
            drag.transform.SetRect(0f, 1f, 1f, 1f, 0f, -headerSize);
            drag.color = dragColor;
            UIUtility.MakeObjectDraggable(drag.rectTransform, mainPanel.rectTransform);

            nametext = UIUtility.CreateText("Nametext", drag.transform, "Scenes");
            nametext.transform.SetRect(0f, 0f, 1f, 1f, 340f, 0f, -buttonSize * 2f);
            nametext.alignment = TextAnchor.MiddleCenter;

            var close = UIUtility.CreateButton("CloseButton", drag.transform, "");
            close.transform.SetRect(1f, 0f, 1f, 1f, -buttonSize * 2f);
            close.onClick.AddListener(() => ShowWindow(false));

            var x1 = UIUtility.CreatePanel("x1", close.transform);
            x1.transform.SetRect(0f, 0f, 1f, 1f, 8f, 0f, -8f);
            x1.rectTransform.eulerAngles = new Vector3(0f, 0f, 45f);
            x1.color = new Color(0f, 0f, 0f, 1f);
            var x2 = UIUtility.CreatePanel("x2", close.transform);
            x2.transform.SetRect(0f, 0f, 1f, 1f, 8f, 0f, -8f);
            x2.rectTransform.eulerAngles = new Vector3(0f, 0f, -45f);
            x2.color = new Color(0f, 0f, 0f, 1f);

            var category = UIUtility.CreateDropdown("Dropdown", drag.transform, "Categories");
            category.transform.SetRect(0f, 0f, 0f, 1f, 0f, 0f, 100f);
            category.captionText.transform.SetRect(0f, 0f, 1f, 1f, 0f, 2f, -15f, -2f);
            category.captionText.alignment = TextAnchor.MiddleCenter;
            category.options = GetCategories();
            category.onValueChanged.AddListener(x =>
            {
                currentCategoryFolder = CategoryFolders[category.options[x].text];
                imagelist.content.GetComponentInChildren<Image>().gameObject.SetActive(false);
                imagelist.content.anchoredPosition = new Vector2(0f, 0f);
                PopulateGrid();
            });

            var refresh = UIUtility.CreateButton("RefreshButton", drag.transform, "Refresh");
            refresh.transform.SetRect(0f, 0f, 0f, 1f, 100f, 0f, 180f);
            refresh.onClick.AddListener(ReloadImages);

            var save = UIUtility.CreateButton("SaveButton", drag.transform, "Save");
            save.transform.SetRect(0f, 0f, 0f, 1f, 180f, 0f, 260f);
            save.onClick.AddListener(() =>
            {
                OnSaveButtonClick();
                if(currentCategoryFolder == scenePath)
                {
                    var dir = new DirectoryInfo(scenePath);
                    var path = dir.GetFiles().OrderByDescending(f => f.LastWriteTime).First().FullName;
                    var button = CreateSceneButton(imagelist.content.GetComponentInChildren<Image>().transform, PngAssist.LoadTexture(path), path);
                    button.transform.SetAsFirstSibling();
                }
            });

            var folder = UIUtility.CreateButton("FolderButton", drag.transform, "Folder");
            folder.transform.SetRect(0f, 0f, 0f, 1f, 260f, 0f, 340f);
            folder.onClick.AddListener(() => OnFolderButtonClick(scenePath));

            var autoCloseToggle = UIUtility.CreateToggle("AutoCloseToggle", drag.transform, "Auto Close");
            autoCloseToggle.transform.SetRect(0f, 0f, 0f, 1f, 340f, 0f, 420f);
            autoCloseToggle.GetComponentInChildren<Text>().color = new Color(1f, 1f, 1f, 1f);
            autoCloseToggle.onValueChanged.AddListener(x => BetterSceneLoader.AutoClose.Value = x);

            var loadingPanel = UIUtility.CreatePanel("LoadingIconPanel", drag.transform);
            loadingPanel.transform.SetRect(0f, 0f, 0f, 1f, 420f, 0f, 420f + headerSize);
            loadingPanel.color = new Color(0f, 0f, 0f, 0f);
            var loadingIcon = UIUtility.CreatePanel("LoadingIcon", loadingPanel.transform);
            loadingIcon.transform.SetRect(0.1f, 0.1f, 0.9f, 0.9f);
            var loadiconTex = PngAssist.ChangeTextureFromByte(Resource.GetResourceAsBytes(typeof(SceneLoaderUI).Assembly, "Resources.loadicon"));
            loadingIcon.sprite = Sprite.Create(loadiconTex, new Rect(0, 0, loadiconTex.width, loadiconTex.height), new Vector2(0.5f, 0.5f));
            LoadingIcon.Init(loadingIcon, -5f);

            imagelist = UIUtility.CreateScrollView("Imagelist", mainPanel.transform);
            imagelist.transform.SetRect(0f, 0f, 1f, 1f, marginSize, marginSize, -marginSize, -headerSize - marginSize / 2f);
            imagelist.gameObject.AddComponent<Mask>();
            imagelist.content.gameObject.AddComponent<VerticalLayoutGroup>();
            imagelist.content.gameObject.AddComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            imagelist.verticalScrollbar.GetComponent<RectTransform>().offsetMin = new Vector2(scrollOffsetX, 0f);
            imagelist.viewport.offsetMax = new Vector2(scrollOffsetX, 0f);
            imagelist.movementType = ScrollRect.MovementType.Clamped;

            optionspanel = UIUtility.CreatePanel("ButtonPanel", imagelist.transform);
            optionspanel.gameObject.SetActive(false);

            confirmpanel = UIUtility.CreatePanel("ConfirmPanel", imagelist.transform);
            confirmpanel.gameObject.SetActive(false);

            yesbutton = UIUtility.CreateButton("YesButton", confirmpanel.transform, "Y");
            yesbutton.transform.SetRect(0f, 0f, 0.5f, 1f);
            yesbutton.onClick.AddListener(() =>
            {
                OnDeleteButtonClick(currentPath);
                confirmpanel.gameObject.SetActive(false);
                currentButton.gameObject.SetActive(false);
            });

            nobutton = UIUtility.CreateButton("NoButton", confirmpanel.transform, "N");
            nobutton.transform.SetRect(0.5f, 0f, 1f, 1f);
            nobutton.onClick.AddListener(() => confirmpanel.gameObject.SetActive(false));

            var loadbutton = UIUtility.CreateButton("LoadButton", optionspanel.transform, "Load");
            loadbutton.transform.SetRect(0f, 0f, 0.3f, 1f);
            loadbutton.onClick.AddListener(() =>
            {
                confirmpanel.gameObject.SetActive(false);
                optionspanel.gameObject.SetActive(false);
                OnLoadButtonClick(currentPath);
                if(BetterSceneLoader.AutoClose.Value)
                    ShowWindow(false);
            });

            var importbutton = UIUtility.CreateButton("ImportButton", optionspanel.transform, "Import");
            importbutton.transform.SetRect(0.35f, 0f, 0.65f, 1f);
            importbutton.onClick.AddListener(() =>
            {
                OnImportButtonClick(currentPath);
                confirmpanel.gameObject.SetActive(false);
                optionspanel.gameObject.SetActive(false);
            });

            var deletebutton = UIUtility.CreateButton("DeleteButton", optionspanel.transform, "Delete");
            deletebutton.transform.SetRect(0.7f, 0f, 1f, 1f);
            deletebutton.onClick.AddListener(() => confirmpanel.gameObject.SetActive(true));

            var pluginiconTex = PngAssist.ChangeTextureFromByte(Resource.GetResourceAsBytes(typeof(SceneLoaderUI).Assembly, "Resources.pluginicon"));
            toolbarToggle = CustomToolbarButtons.AddLeftToolbarToggle(pluginiconTex, false, ShowWindow);

            UpdateWindow();
            PopulateGrid();
        }

        private List<Dropdown.OptionData> GetCategories()
        {
            if(!File.Exists(scenePath))
                Directory.CreateDirectory(scenePath);

            var folders = Directory.GetDirectories(scenePath).ToList();
            folders.Insert(0, scenePath);

            return folders.Select(x =>
            {
                var filename = Path.GetFileName(x);
                CategoryFolders[filename] = x;
                return new Dropdown.OptionData(filename);
            }).ToList();
        }

        private void ReloadImages()
        {
            optionspanel.transform.SetParent(imagelist.transform);
            confirmpanel.transform.SetParent(imagelist.transform);
            optionspanel.gameObject.SetActive(false);
            confirmpanel.gameObject.SetActive(false);

            GameObject.Destroy(imagelist.content.GetComponentInChildren<Image>().gameObject);
            imagelist.content.anchoredPosition = new Vector2(0f, 0f);
            PopulateGrid(true);
        }

        private void PopulateGrid(bool forceUpdate = false)
        {
            if(forceUpdate)
                sceneCache.Remove(currentCategoryFolder);

            if(sceneCache.TryGetValue(currentCategoryFolder, out Image sceneList))
            {
                sceneList.gameObject.SetActive(true);
            }
            else
            {
                var scenefiles = Directory.GetFiles(currentCategoryFolder, "*.png").Select(x => new KeyValuePair<DateTime, string>(File.GetLastWriteTime(x), x)).ToList();
                scenefiles.Sort((a, b) => b.Key.CompareTo(a.Key));

                var container = UIUtility.CreatePanel("GridContainer", imagelist.content.transform);
                container.transform.SetRect(0f, 0f, 1f, 1f);
                container.gameObject.AddComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;

                var gridlayout = container.gameObject.AddComponent<AutoGridLayout>();
                gridlayout.spacing = new Vector2(marginSize, marginSize);
                gridlayout.m_IsColumn = true;
                gridlayout.m_Column = BetterSceneLoader.ColumnAmount.Value;

                ThreadingHelper.Instance.StartCoroutine(LoadButtonsAsync(container.transform, scenefiles));
                sceneCache.Add(currentCategoryFolder, container);
            }
        }

        private IEnumerator LoadButtonsAsync(Transform parent, List<KeyValuePair<DateTime, string>> scenefiles)
        {
            foreach(var scene in scenefiles)
            {
                LoadingIcon.loadingState[currentCategoryFolder] = true;

#if KKS
                using(var uwr = UnityWebRequestTexture.GetTexture("file:///" + scene.Value, true))
                {
                    yield return uwr.SendWebRequest();

                    if(uwr.isNetworkError || uwr.isHttpError)
                        throw new Exception(uwr.error);

                    var texture = DownloadHandlerTexture.GetContent(uwr);
                    CreateSceneButton(parent, texture, scene.Value);
                }
#else
                using(var www = new WWW("file:///" + scene.Value))
                {
                    yield return www;

                    if(!string.IsNullOrEmpty(www.error))
                        throw new Exception(www.error);

                    CreateSceneButton(parent, PngAssist.ChangeTextureFromByte(www.bytes), scene.Value);
                }
#endif
            }

            LoadingIcon.loadingState[currentCategoryFolder] = false;
        }

        private Button CreateSceneButton(Transform parent, Texture2D texture, string path)
        {
            var button = UIUtility.CreateButton("ImageButton", parent, "");
            button.onClick.AddListener(() =>
            {
                currentButton = button;
                currentPath = path;

                if(optionspanel.transform.parent != button.transform)
                {
                    optionspanel.transform.SetParent(button.transform);
                    optionspanel.transform.SetRect(0f, 0f, 1f, 0.15f);
                    optionspanel.gameObject.SetActive(true);

                    confirmpanel.transform.SetParent(button.transform);
                    confirmpanel.transform.SetRect(0.4f, 0.4f, 0.6f, 0.6f);
                }
                else
                {
                    optionspanel.gameObject.SetActive(!optionspanel.gameObject.activeSelf);
                }

                confirmpanel.gameObject.SetActive(false);
            });

            var sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            button.gameObject.GetComponent<Image>().sprite = sprite;

            return button;
        }
    }
}
