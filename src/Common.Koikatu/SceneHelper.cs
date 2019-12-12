using UnityEngine.SceneManagement;

namespace KeelPlugins
{
    internal static class SceneHelper
    {
        public static bool StudioInitializing => SceneManager.GetActiveScene().name == "StudioStart";
    }
}
