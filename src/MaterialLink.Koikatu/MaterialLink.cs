using BepInEx;
using UnityEngine;

[assembly: System.Reflection.AssemblyFileVersion(MaterialLink.Koikatu.MaterialLinkInfo.Version)]

namespace MaterialLink.Koikatu
{
    [BepInPlugin(GUID, PluginName, Version)]
    public class MaterialLinkInfo : BaseUnityPlugin
    {
        public const string GUID = "keelhauled.materiallink";
        public const string PluginName = "MaterialLink";
        public const string Version = "1.0.0";
    }

    public class MaterialLink : MonoBehaviour
    {
        public Renderer[] ManagedRenderers;

        private ChaControl chaControl;

        private void Start()
        {
            chaControl = gameObject.GetComponentInParent<ChaControl>();
        }

        private void Update()
        {
            if(chaControl != null && chaControl.customMatBody != null)
            {
                foreach(var rend in ManagedRenderers)
                {
                    if(rend && chaControl.customMatBody && rend.material != chaControl.customMatBody)
                        rend.material = chaControl.customMatBody;
                }
            }
        }
    }
}