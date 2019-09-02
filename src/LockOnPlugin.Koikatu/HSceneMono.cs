using IllusionUtility.GetUtility;
using UnityEngine;

namespace KeelPlugins
{
    internal partial class HSceneMono : LockOnBase
    {
        private CameraControl_Ver2 camera = Singleton<CameraControl_Ver2>.Instance;

        private ChaInfo GetClosestChara()
        {
            ChaInfo closestChara = null;
            float smallestMagnitude = 0f;

            foreach(var chara in FindObjectsOfType<ChaControl>())
            {
                float magnitude = 0f;
                foreach(var targetname in TargetData.data.presenceTargets)
                {
                    var target = chara.objBodyBone.transform.FindLoop(targetname);
                    float distance = Vector3.Distance(camera.TargetPos, camera.transBase.InverseTransformPoint(target.transform.position));
                    magnitude += distance;
                }

                if(!closestChara)
                {
                    closestChara = chara;
                    smallestMagnitude = magnitude;
                }
                else
                {
                    if(magnitude < smallestMagnitude)
                    {
                        closestChara = chara;
                        smallestMagnitude = magnitude;
                    }
                }
            }

            return closestChara;
        }

        protected override bool LockOn()
        {
            if(!lockedOn) currentCharaInfo = GetClosestChara();
            return base.LockOn();
        }
    }
}
