using UnityEngine;

namespace ItemLayerEdit.Koikatu
{
    public static class Extensions
    {
        public static void SetAllLayers(this GameObject gameObject, int layer)
        {
            gameObject.layer = layer;
            foreach(var child in gameObject.GetComponentsInChildren<Transform>())
                child.gameObject.layer = layer;
        }
    }
}
