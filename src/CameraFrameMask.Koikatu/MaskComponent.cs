using UnityEngine;

namespace KeelPlugins
{
    public class MaskComponent : MonoBehaviour
    {
        private int count = 0;
        private RenderTexture displayFrame = null;

        public void MaskFrames(int count)
        {
            if(this.count < count)
                this.count = count;
        }

        private void OnRenderImage(RenderTexture src, RenderTexture dest)
        {
            if(count > 0)
            {
                if(displayFrame == null)
                {
                    displayFrame = RenderTexture.GetTemporary(src.width, src.height);
                    Graphics.Blit(src, displayFrame);
                }

                Graphics.Blit(displayFrame, dest);
                count--;
            }
            else
            {
                if(displayFrame != null)
                {
                    RenderTexture.ReleaseTemporary(displayFrame);
                    displayFrame = null;
                }

                Graphics.Blit(src, dest);
            }
        }
    }
}
