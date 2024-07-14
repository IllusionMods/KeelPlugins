using UnityEngine;
using UnityEngine.UI;

namespace BetterSceneLoader
{
    public class LoadingIcon : MonoBehaviour
    {
        public static void Init(GameObject parent, Image image, float speed)
        {
            var icon = parent.AddComponent<LoadingIcon>();
            icon.image = image;
            icon.speed = speed;
        }

        public static int loadingCount = 0;
        private Image image;
        private float speed;

        private void Update()
        {
            if(loadingCount > 0)
            {
                if(!image.enabled) image.enabled = true;
                image.rectTransform.rotation *= Quaternion.Euler(0f, 0f, speed);
            }
            else
            {
                if(image.enabled) image.enabled = false;
            }
        }
    }
}
