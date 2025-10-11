using BepInEx;
using KKAPI.Studio.UI;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using KeelPlugins.Utils;
using Studio;
using UniRx.Triggers;
using UniRx;
using UnityEngine.EventSystems;

namespace BetterSceneLoader
{
    public class SceneGrid : ImageGrid
    {
        private ToolbarToggle toolbarToggle;
        private AddButtonCtrl addButtonCtrl;

        public SceneGrid() : base(
            defaultPath: BepInEx.Utility.CombinePaths(Paths.GameRootPath, "UserData", "Studio", "scene"),
            onSaveButtonClick: () => Studio.Studio.Instance.systemButtonCtrl.OnClickSave(),
            onLoadButtonClick: x => Studio.Studio.Instance.StartCoroutine(Studio.Studio.Instance.LoadSceneCoroutine(x)),
            onImportButtonClick: x => Studio.Studio.Instance.ImportScene(x))
        { }

        public override void ShowWindow(bool flag)
        {
            base.ShowWindow(flag);
            toolbarToggle?.SetValue(flag);
            if(flag && (addButtonCtrl.select == 0 || addButtonCtrl.select == 1))
                addButtonCtrl.OnClick(addButtonCtrl.select);
        }

        public override void CreateUI(string name, int sortingOrder, string titleText)
        {
            base.CreateUI(name, sortingOrder, titleText);
            ThreadingHelper.Instance.StartCoroutine(AddToolbarButton());
            addButtonCtrl = GameObject.Find("StudioScene/Canvas Main Menu/01_Add").GetComponent<AddButtonCtrl>();
        }

        private IEnumerator AddToolbarButton()
        {
            if (!BetterSceneLoader.AddButton.Value) yield break;

            var pluginiconTex = PngAssist.ChangeTextureFromByte(Resource.GetResourceAsBytes(typeof(ImageGrid).Assembly, "Resources.pluginicon"));
            toolbarToggle = CustomToolbarButtons.AddLeftToolbarToggle(pluginiconTex, false, ShowWindow);

            yield return new WaitUntil(() => toolbarToggle.ControlObject);

            var button = toolbarToggle.ControlObject.GetComponent<Button>();
            button.OnPointerClickAsObservable().Subscribe(e =>
            {
                if(e.button == PointerEventData.InputButton.Right)
                    OnSaveButtonClick?.Invoke();
            });
        }
    }
}
