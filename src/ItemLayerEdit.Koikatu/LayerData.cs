using MessagePack;
using UnityEngine;

namespace KeelPlugins
{
    internal class LayerDataContainer : MonoBehaviour
    {
        public int DefaultLayer;
    }

    [MessagePackObject(true)]
    internal class LayerSaveData
    {
        public int DefaultLayer;
        public int NewLayer;
        public int ObjectId;
    }
}
