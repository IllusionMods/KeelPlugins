using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace KeelPlugins
{
    internal class HideMakerUI : HideUIAction
    {
        private IEnumerable<Canvas> canvasList;
        private bool visible = true;

        public HideMakerUI()
        {
            var go = GameObject.Find("CustomControl");
            canvasList = go.GetComponentsInChildren<Canvas>().Where(x => x.gameObject.name.Contains("Canvas"));
        }

        public override void ToggleUI()
        {
            visible = !visible;
            foreach(var canvas in canvasList.Where(x => x))
                canvas.enabled = visible;
        }
    }
}
