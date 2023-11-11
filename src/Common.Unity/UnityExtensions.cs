namespace UnityEngine
{
    internal static class UnityExtensions
    {
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

        public static Transform FindRecursive(this Transform parent, string childName)
        {
            foreach(Transform child in parent)
            {
                if(child.name == childName)
                {
                    return child;
                }
                else
                {
                    var found = FindRecursive(child, childName);
                    if (found != null)
                        return found;
                }
            }
            return null;
        }
    }
}
