using UnityEngine;

namespace KeelPlugins
{
    internal class HideHSceneUI : HideUIAction
    {
        private CanvasGroup group;
        private bool visible = true;

        public HideHSceneUI(CanvasGroup UIGroup)
        {
            group = UIGroup;
        }

        public override void ToggleUI()
        {
            visible = !visible;

            group.alpha = visible ? 1f : 0f;
        }
    }
}