using UnityEngine;

namespace KeelPlugins
{
    public class MaskComponent : MonoBehaviour
    {
        private int count = 0;
        private RenderTexture lastFrame = null;
        private int lastWidth;
        private int lastHeight;

        private void Start()
        {
            lastWidth = Screen.width;
            lastHeight = Screen.height;
            lastFrame = RenderTexture.GetTemporary(lastWidth, lastHeight);
        }
        
        private void OnDestroy()
        {
            RenderTexture.ReleaseTemporary(lastFrame);
        }

        public void MaskFrames(int count)
        {
            if(this.count < count)
                this.count = count;
        }

        private void OnRenderImage(RenderTexture src, RenderTexture dest)
        {
            if(count > 0)
            {
                // Display the last frame again to hide the actual screen
                Graphics.Blit(lastFrame, dest);
                count--;
            }
            else
            {
                var newWidth = Screen.width;
                var newHeight = Screen.height;

                if(lastWidth != newWidth || lastHeight != newHeight)
                {
                    RenderTexture.ReleaseTemporary(lastFrame);
                    lastFrame = RenderTexture.GetTemporary(newWidth, newHeight);

                    lastWidth = newWidth;
                    lastHeight = newHeight;
                }

                // Need to keep a copy of the last frame since we don't know when MaskFrames will be used
                Graphics.Blit(src, lastFrame);

                Graphics.Blit(src, dest);
            }
        }
    }
}
