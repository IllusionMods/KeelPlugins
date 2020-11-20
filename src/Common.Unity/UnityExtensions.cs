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
    }
}
