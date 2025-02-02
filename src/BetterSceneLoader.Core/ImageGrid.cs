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
using KKAPI.Utilities;
using UniRx.Triggers;
using UniRx;
using UnityEngine.EventSystems;

namespace BetterSceneLoader
{
    public class ImageGrid
    {
        private readonly float buttonSize = 10f;
        private readonly float marginSize = 5f;
        private readonly float headerSize = 20f;
        private readonly float UIScale = 1.0f;
        private readonly float scrollOffsetX = -15f;
        private readonly float dropdownWidth = 250f;

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
        private Dropdown category;
        private Image infopanel;
        private Text infotext;
        private Dropdown sorting;

        private readonly Dictionary<string, CategoryData> sceneCache = new Dictionary<string, CategoryData>();
        private Button currentButton;
        private readonly string defaultPath;
        private string currentPath;
        private string currentCategoryFolder;
        private readonly Dictionary<string, string> CategoryFolders = new Dictionary<string, string>();

        public UnityAction OnSaveButtonClick;
        public UnityAction<string> OnLoadButtonClick;
        public UnityAction<string> OnDeleteButtonClick;
        public UnityAction<string> OnImportButtonClick;
        
        public class CategoryData
        {
            public Image Container;
            public SortBy SortBy;
        }

        public ImageGrid(string defaultPath, UnityAction onSaveButtonClick,
                         UnityAction<string> onLoadButtonClick, UnityAction<string> onImportButtonClick)
        {
            this.defaultPath = defaultPath;
            currentCategoryFolder = defaultPath;
            OnSaveButtonClick = onSaveButtonClick;
            OnLoadButtonClick = onLoadButtonClick;
            OnImportButtonClick = onImportButtonClick;
        }

        public void ShowWindow(bool flag)
        {
            UISystem.gameObject.SetActive(flag);
            toolbarToggle?.SetValue(flag);
            if(category != null)
                category.template.SetRect(0f, 1f, 0f, 0f, 0f, headerSize - marginSize, dropdownWidth, mainPanel.rectTransform.rect.height / 2);
        }

        public void UpdateWindow()
        {
            foreach(var scene in sceneCache.Values)
            {
                var gridlayout = scene.Container.gameObject.GetComponent<AutoGridLayout>();
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

        public void CreateUI(string name, int sortingOrder, string titleText)
        {
            UISystem = UIUtility.CreateNewUISystem(name);
            UISystem.GetComponent<CanvasScaler>().referenceResolution = new Vector2(1920f / UIScale, 1080f / UIScale);
            UISystem.sortingOrder = sortingOrder;
            ShowWindow(false);

            mainPanel = UIUtility.CreatePanel("Panel", UISystem.transform);
            mainPanel.color = backgroundColor;
            UIUtility.AddOutlineToObject(mainPanel.transform, outlineColor);

            var drag = UIUtility.CreatePanel("Draggable", mainPanel.transform);
            drag.transform.SetRect(0f, 1f, 1f, 1f, 0f, -headerSize);
            drag.color = dragColor;
            UIUtility.MakeObjectDraggable(drag.rectTransform, mainPanel.rectTransform);

            nametext = UIUtility.CreateText("Nametext", drag.transform, titleText);
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

            var curPos = 0f;
            category = UIUtility.CreateDropdown("CategoryDropdown", drag.transform, "Categories");
            category.transform.SetRect(0f, 0f, 0f, 1f, curPos, 0f, curPos+=dropdownWidth);
            category.captionText.transform.SetRect(0f, 0f, 1f, 1f, 0f, 2f, -15f, -2f);
            category.captionText.alignment = TextAnchor.MiddleCenter;
            category.template.GetComponent<ScrollRect>().scrollSensitivity = 40f;
            category.options = GetCategories();
            category.onValueChanged.AddListener(x =>
            {
                currentCategoryFolder = CategoryFolders[category.options[x].text];
                imagelist.content.GetComponentInChildren<Image>().gameObject.SetActive(false);
                imagelist.content.anchoredPosition = new Vector2(0f, 0f);
                PopulateGrid();
            });
            DropdownAutoScroll.Setup(category);
            DropdownFilter.AddFilterUI(category, "BetterSceneLoaderDropdown");

            sorting = UIUtility.CreateDropdown("SortDropdown", drag.transform, "Sort");
            sorting.transform.SetRect(0f, 0f, 0f, 1f, curPos, 0f, curPos+=150f);
            sorting.captionText.transform.SetRect(0f, 0f, 1f, 1f, 0f, 2f, -15f, -2f);
            sorting.captionText.alignment = TextAnchor.MiddleCenter;
            sorting.options = Enum.GetNames(typeof(SortBy)).Select(x => new Dropdown.OptionData(x)).ToList();
            sorting.onValueChanged.AddListener(x =>
            {
                sceneCache[currentCategoryFolder].SortBy = (SortBy)x;
                var container = imagelist.content.GetComponentInChildren<Image>();
                if(container && container.gameObject.activeSelf) // only reload when not changing category
                    ReloadImages();
            });

            var refresh = UIUtility.CreateButton("RefreshButton", drag.transform, "Refresh");
            refresh.transform.SetRect(0f, 0f, 0f, 1f, curPos, 0f, curPos+=80f);
            refresh.onClick.AddListener(ReloadImages);

            var save = UIUtility.CreateButton("SaveButton", drag.transform, "Save");
            save.transform.SetRect(0f, 0f, 0f, 1f, curPos, 0f, curPos+=80f);
            save.onClick.AddListener(() =>
            {
                OnSaveButtonClick();
                if(currentCategoryFolder == defaultPath)
                {
                    var dir = new DirectoryInfo(defaultPath);
                    var fileInfo = dir.GetFiles().OrderByDescending(f => f.LastWriteTime).First();
                    var gridContainer = imagelist.content.GetComponentInChildren<Image>();
                    var button = CreateSceneButton(gridContainer.transform, PngAssist.LoadTexture(fileInfo.FullName), fileInfo);
                    button.transform.SetAsFirstSibling();
                }
            });

            var folder = UIUtility.CreateButton("FolderButton", drag.transform, "Folder");
            folder.transform.SetRect(0f, 0f, 0f, 1f, curPos, 0f, curPos+=80f);
            folder.onClick.AddListener(() => Application.OpenURL($"file:///{EncodePath(currentCategoryFolder)}"));

            var autoCloseToggle = UIUtility.CreateToggle("AutoCloseToggle", drag.transform, "Auto Close");
            autoCloseToggle.transform.SetRect(0f, 0f, 0f, 1f, curPos, 0f, curPos+=80f);
            autoCloseToggle.GetComponentInChildren<Text>().color = new Color(1f, 1f, 1f, 1f);
            autoCloseToggle.onValueChanged.AddListener(x => BetterSceneLoader.AutoClose.Value = x);

            var loadingPanel = UIUtility.CreatePanel("LoadingIconPanel", drag.transform);
            loadingPanel.transform.SetRect(0f, 0f, 0f, 1f, curPos, 0f, curPos+=headerSize);
            loadingPanel.color = new Color(0f, 0f, 0f, 0f);
            var loadingIcon = UIUtility.CreatePanel("LoadingIcon", loadingPanel.transform);
            loadingIcon.transform.SetRect(0.1f, 0.1f, 0.9f, 0.9f);
            var loadiconTex = PngAssist.ChangeTextureFromByte(Resource.GetResourceAsBytes(typeof(ImageGrid).Assembly, "Resources.loadicon"));
            loadingIcon.sprite = Sprite.Create(loadiconTex, new Rect(0, 0, loadiconTex.width, loadiconTex.height), new Vector2(0.5f, 0.5f));
            LoadingIcon.Init(loadingPanel.gameObject, loadingIcon, -5f);

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
                RecycleBinUtil.MoveToRecycleBin(currentPath);
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
            deletebutton.onClick.AddListener(() =>
            {
                if(BetterSceneLoader.ConfirmDelete.Value)
                    confirmpanel.gameObject.SetActive(true);
                else
                {
                    RecycleBinUtil.MoveToRecycleBin(currentPath);
                    currentButton.gameObject.SetActive(false);
                }
            });

            infopanel = UIUtility.CreatePanel("InfoPanel", imagelist.transform);
            infopanel.gameObject.SetActive(false);

            infotext = UIUtility.CreateText("InfoText", infopanel.transform, "INFO");
            infotext.transform.SetRect(0f, 0f, 1f, 1f);
            infotext.alignment = TextAnchor.MiddleCenter;
            UIUtility.AddOutlineToObject(infotext.transform);

            ThreadingHelper.Instance.StartCoroutine(AddToolbarButton());

            UpdateWindow();
            PopulateGrid();
        }

        private IEnumerator AddToolbarButton()
        {
            var pluginiconTex = PngAssist.ChangeTextureFromByte(Resource.GetResourceAsBytes(typeof(ImageGrid).Assembly, "Resources.pluginicon"));
            toolbarToggle = CustomToolbarButtons.AddLeftToolbarToggle(pluginiconTex, false, ShowWindow);

            yield return new WaitUntil(() => toolbarToggle.ControlObject != null);

            var button = toolbarToggle.ControlObject.GetComponent<Button>();
            button.OnPointerClickAsObservable().Subscribe(e =>
            {
                if(e.button == PointerEventData.InputButton.Right)
                    OnSaveButtonClick();
            });
        }

        private List<Dropdown.OptionData> GetCategories()
        {
            if(!File.Exists(defaultPath))
                Directory.CreateDirectory(defaultPath);

            var folders = Directory.GetDirectories(defaultPath, "*", SearchOption.AllDirectories).OrderBy(x => x).ToList();
            folders.Insert(0, defaultPath);
            
            return folders.Select(x =>
            {
                var catname = x == defaultPath ? "/" : x.Remove(0, defaultPath.Length+1).Replace("\\", "/");
                CategoryFolders[catname] = x;
                return new Dropdown.OptionData(catname);
            }).ToList();
        }

        private void ReloadImages()
        {
            optionspanel.transform.SetParent(imagelist.transform);
            confirmpanel.transform.SetParent(imagelist.transform);
            infopanel.transform.SetParent(imagelist.transform);
            optionspanel.gameObject.SetActive(false);
            confirmpanel.gameObject.SetActive(false);
            infopanel.gameObject.SetActive(false);

            var newCats = GetCategories();
            var oldIndex = category.value;
            var newIndex = FirstIndexMatch(newCats, x => x.text == category.options[oldIndex].text);
            category.options = newCats;
            if(oldIndex != newIndex || newIndex == -1)
            {
                category.value = newIndex == -1 ? 0 : newIndex;
                category.RefreshShownValue();
            }
            else
            {
                GameObject.Destroy(imagelist.content.GetComponentInChildren<Image>().gameObject);
                imagelist.content.anchoredPosition = new Vector2(0f, 0f);
                PopulateGrid(true);
            }
        }

        private void PopulateGrid(bool forceUpdate = false)
        {
            var sortBy = BetterSceneLoader.SceneSorting.Value;
            
            if(forceUpdate)
            {
                if(sceneCache.TryGetValue(currentCategoryFolder, out var temp))
                    sortBy = temp.SortBy;
                sceneCache.Remove(currentCategoryFolder);
            }

            if(sceneCache.TryGetValue(currentCategoryFolder, out var catData))
            {
                sorting.value = (int)catData.SortBy;
                catData.Container.gameObject.SetActive(true);
            }
            else
            {
                var dirInfo = new DirectoryInfo(currentCategoryFolder);
                var scenefiles = dirInfo.GetFiles("*.png").ToList();
                switch(sortBy)
                {
                    case SortBy.DateAscending:
                        scenefiles = scenefiles.OrderBy(x => x.LastWriteTime).ToList();
                        break;
                    case SortBy.DateDescending:
                        scenefiles = scenefiles.OrderByDescending(x => x.LastWriteTime).ToList();
                        break;
                    case SortBy.SizeAscending:
                        scenefiles = scenefiles.OrderBy(x => x.Length).ToList();
                        break;
                    case SortBy.SizeDescending:
                        scenefiles = scenefiles.OrderByDescending(x => x.Length).ToList();
                        break;
                }

                var container = UIUtility.CreatePanel("GridContainer", imagelist.content.transform);
                container.transform.SetRect(0f, 0f, 1f, 1f);
                container.gameObject.AddComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;

                var gridlayout = container.gameObject.AddComponent<AutoGridLayout>();
                gridlayout.spacing = new Vector2(marginSize, marginSize);
                gridlayout.m_IsColumn = true;
                gridlayout.m_Column = BetterSceneLoader.ColumnAmount.Value;

                ThreadingHelper.Instance.StartCoroutine(LoadButtonsAsync(container.transform, scenefiles));
                sceneCache.Add(currentCategoryFolder, new CategoryData{ Container = container, SortBy = sortBy });
                sorting.value = (int)sortBy;
            }
        }

        private IEnumerator LoadButtonsAsync(Transform parent, List<FileInfo> scenefiles)
        {
            LoadingIcon.loadingCount++;
            foreach(var scene in scenefiles)
            {
                var uri = "file:///" + EncodePath(scene.FullName);
#if KKS
                using(var uwr = UnityWebRequestTexture.GetTexture(uri, true))
                {
                    yield return uwr.SendWebRequest();

                    if(uwr.isNetworkError || uwr.isHttpError)
                        throw new Exception(uwr.error);

                    var tex = DownloadHandlerTexture.GetContent(uwr);
                    CreateSceneButton(parent, tex, scene);
                }
#else
                using(var www = new WWW(uri))
                {
                    yield return www;

                    if(!string.IsNullOrEmpty(www.error))
                        throw new Exception(www.error);

                    var tex = PngAssist.ChangeTextureFromByte(www.bytes);
                    CreateSceneButton(parent, tex, scene);
                }
#endif
            }
            LoadingIcon.loadingCount--;
        }

        private Button CreateSceneButton(Transform parent, Texture2D texture, FileInfo fileInfo)
        {
            var button = UIUtility.CreateButton("ImageButton", parent, "");
            button.OnPointerEnterAsObservable().Subscribe(e =>
            {
                currentButton = button;
                currentPath = fileInfo.FullName;

                if(optionspanel.transform.parent != button.transform)
                {
                    optionspanel.transform.SetParent(button.transform);
                    optionspanel.transform.SetRect(0f, 0f, 1f, 0.15f);
                    optionspanel.gameObject.SetActive(true);

                    confirmpanel.transform.SetParent(button.transform);
                    confirmpanel.transform.SetRect(0.4f, 0.4f, 0.6f, 0.6f);

                    infopanel.transform.SetParent(button.transform);
                    infopanel.transform.SetRect(0f, 0.88f, 1f, 1f);
                    infopanel.gameObject.SetActive(true);
                    infotext.text = $"{FormatFilesize(fileInfo.Length)} {fileInfo.LastWriteTime}";
                }

                confirmpanel.gameObject.SetActive(false);
            });

            var sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            button.gameObject.GetComponent<Image>().sprite = sprite;

            return button;
        }

        private string EncodePath(string path)
        {
            return path.Replace("#", "%23").Replace("+", "%2B").Replace("&", "%26");
        }

        private int FirstIndexMatch<TItem>(IEnumerable<TItem> items, Func<TItem,bool> matchCondition)
        {
            var index = 0;
            foreach(var item in items)
            {
                if(matchCondition.Invoke(item))
                    return index;
                index++;
            }
            return -1;
        }

        private string FormatFilesize(long length)
        {
            string[] units = { "B", "KB", "MB", "GB", "TB" };
            var size = Convert.ToDecimal(length);

            int order = 0;
            while(size >= 1024 && order < units.Length - 1)
            {
                order++;
                size /= 1024;
            }

            return $"{size:0.#} {units[order]}";
        }
    }
}
