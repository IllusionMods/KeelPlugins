using System.Collections.Generic;
using System.Linq;
using Manager;

namespace LockOnPlugin
{
    internal partial class HSceneMono : LockOnBase
    {
        private CameraControl_Ver2 camera = Singleton<CameraControl_Ver2>.Instance;
        private Character charaManager = Character.Instance;
        private int activeCharaCount;

        protected override void Start()
        {
            base.Start();

            if(unlockHSceneCam)
            {
                camera.isLimitPos = false;
                camera.isLimitDir = false; 
            }

            currentCharaInfo = charaManager.dictFemale[0];
            activeCharaCount = GetActiveCharaCount();
        }

        protected override void Update()
        {
            base.Update();

            int count = GetActiveCharaCount();
            if(activeCharaCount != count)
            {
                currentCharaInfo = charaManager.dictFemale[0];
                LockOnRelease();
                activeCharaCount = count;
            }
        }

        protected override void CharaSwitch(bool scrollDown = true)
        {
            if(!lockOnTarget && LockOn()) return;

            List<CharInfo> characters = new List<CharInfo>();
            for(int i = 0; i < charaManager.dictFemale.Count; i++)
            {
                if(charaManager.dictFemale[i].animBody != null)
                {
                    characters.Add(charaManager.dictFemale[i]);
                }
            }

            if(scrollThroughMalesToo)
            {
                for(int i = 0; i < charaManager.dictMale.Count; i++)
                {
                    if(charaManager.dictMale[i].animBody != null)
                    {
                        characters.Add(charaManager.dictMale[i]);
                    }
                }
            }

            for(int i = 0; i < characters.Count; i++)
            {
                if(characters[i] == currentCharaInfo)
                {
                    int next = i + 1 > characters.Count - 1 ? 0 : i + 1;
                    if(!scrollDown) next = i - 1 < 0 ? characters.Count - 1 : i - 1;
                    currentCharaInfo = characters[next];
                    if(autoSwitchLock && lockOnTarget) LockOn(lockOnTarget.name, true);
                    return;
                }
            }
        }

        protected override void ResetModState()
        {
            base.ResetModState();
            currentCharaInfo = charaManager.dictFemale[0];
            activeCharaCount = GetActiveCharaCount();
        }

        private int GetActiveCharaCount()
        {
            int females = charaManager.dictFemale.Count(x => x.Value.animBody != null);
            int males = charaManager.dictMale.Count(x => x.Value.animBody != null);
            return females + males;
        }
    }
}
