using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HideAllUI
{
    internal class HideStudioUI : HideUI
    {
        private string[] targets = new[] { "Canvas", "Canvas Object List", "Canvas Main Menu", "Canvas Frame cap", "Canvas System Menu", "Canvas Guide Input" };
        private IEnumerable<Canvas> canvasList;
        private bool visible = true;

        public HideStudioUI()
        {
            canvasList = GameObject.FindObjectsOfType<Canvas>().Where(x => targets.Contains(x.name));
        }

        public override void ToggleUI()
        {
            visible = !visible;
            foreach(var canvas in canvasList.Where(x => x))
                canvas.gameObject.SetActive(visible);
        }
    }
}
