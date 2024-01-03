using System.Collections;
using KKAPI;
using KKAPI.Chara;
using UnityEngine;

namespace TesselationSetting.Koikatu
{
    public class CharaExtra : CharaCustomFunctionController
    {
        protected override void OnCardBeingSaved(GameMode currentGameMode)
        {
            
        }

        protected override void OnReload(GameMode currentGameMode)
        {
            StartCoroutine(Coroutine());

            IEnumerator Coroutine()
            {
                for(int i = 0; i < 10; i++)
                    yield return null;
                
                SetRendererTesselation(ChaControl.rendBody);
                SetRendererTesselation(ChaControl.rendFace);
            }
        }

        private static void SetRendererTesselation(Renderer rend)
        {
            if(rend.material && rend.material.shader.name == "xukmi/SkinPlusTess")
            {
                if(rend.material.HasProperty("_TessSmooth"))
                {
                    Log.Info($"Set TessSmooth {TesselationSetting.TessSmooth.Value} '{rend.material.name}'");
                    rend.material.SetFloat("_TessSmooth", TesselationSetting.TessSmooth.Value);
                }
            }
        }
    }
}
