using UnityEngine;

namespace KeelPlugins
{
    public class MaskComponent : MonoBehaviour
    {
        private int count = 0;
        private RenderTexture lastFrame = null;
        
        private void Start()
        {
            lastFrame = RenderTexture.GetTemporary(src.width, src.height);
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
                // Need to keep a copy of the last frame since we don't know when MaskFrames will be used
                Graphics.Blit(src, lastFrame);

                Graphics.Blit(src, dest);
            }
        }
    }
}
