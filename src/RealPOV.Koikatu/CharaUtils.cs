namespace RealPOV.Koikatu
{
    internal static class CharaUtils
    {
        public static void SetNeckLook(this ChaControl chara, NECK_LOOK_TYPE_VER2 necktype)
        {
            for(int i = 0; i < chara.neckLookCtrl.neckLookScript.neckTypeStates.Length; i++)
            {
                if(chara.neckLookCtrl.neckLookScript.neckTypeStates[i].lookType == necktype)
                    chara.neckLookCtrl.ptnNo = i;
            }
        }

        public static void SetEyeLook(this ChaControl chara, EYE_LOOK_TYPE eyetype)
        {
            for(int i = 0; i < chara.eyeLookCtrl.eyeLookScript.eyeTypeStates.Length; i++)
            {
                if(chara.eyeLookCtrl.eyeLookScript.eyeTypeStates[i].lookType == eyetype)
                    chara.eyeLookCtrl.ptnNo = i;
            }
        }

        public static NECK_LOOK_TYPE_VER2 GetNeckLook(this ChaControl chara)
        {
            return chara.neckLookCtrl.neckLookScript.neckTypeStates[chara.neckLookCtrl.ptnNo].lookType;
        }

        public static EYE_LOOK_TYPE GetEyeLook(this ChaControl chara)
        {
            return chara.eyeLookCtrl.eyeLookScript.eyeTypeStates[chara.eyeLookCtrl.ptnNo].lookType;
        }
    }
}
