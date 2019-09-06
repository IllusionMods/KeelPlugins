using BepInEx;
using BepInEx.Logging;
using System.Collections.Generic;

namespace KeelPlugins
{
    public abstract class DragAndDropCore : BaseUnityPlugin
    {
        public const string PluginName = "Drag & Drop";
        public const string GUID = "keelhauled.draganddrop";
        internal static new ManualLogSource Logger;

        private UnityDragAndDropHook hook;

        private void Awake()
        {
            Logger = base.Logger;
        }

        private void OnEnable()
        {
            hook = new UnityDragAndDropHook();
            hook.InstallHook();
            hook.OnDroppedFiles += (aFiles, aPos) => ThreadingHelper.Instance.StartSyncInvoke(() => OnFiles(aFiles, aPos));
        }

        private void OnDisable()
        {
            hook.UninstallHook();
        }

        internal abstract void OnFiles(List<string> aFiles, POINT aPos);
    }
}
