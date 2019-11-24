using UnityEngine;

namespace KeelPlugins
{
    internal static class UnityExtensions
    {
        public static void SetAllLayers(this GameObject gameObject, int layer)
        {
            gameObject.layer = layer;
            foreach(var child in gameObject.GetComponentsInChildren<Transform>())
                child.gameObject.layer = layer;
        }

        public static bool AddComponentIfNotExist<T>(this GameObject gameObject, out T component) where T : MonoBehaviour
        {
            if(gameObject.GetComponent<T>())
            {
                component = null;
                return false;
            }
            else
            {
                component = gameObject.AddComponent<T>();
                return true;
            }
        }
    }
}
