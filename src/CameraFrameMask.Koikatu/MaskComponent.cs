using UnityEngine;

namespace KeelPlugins
{
    public class MaskComponent : MonoBehaviour
    {
        private int count = 0;
        private RenderTexture lastFrame = null;

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
                if(lastFrame) Graphics.Blit(lastFrame, dest);
                count--;
            }
            else
            {
                // Reroll the texture just in case the screen resolution changes
                RenderTexture.ReleaseTemporary(lastFrame);
                lastFrame = RenderTexture.GetTemporary(src.width, src.height);
                // Need to keep a copy of the last frame since it is not known when MaskFrames will be used
                Graphics.Blit(src, lastFrame);

                Graphics.Blit(src, dest);
            }
        }
    }
}
