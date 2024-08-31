using HarmonyLib;
using Studio;
using System.Linq;
using UnityEngine;

namespace DefaultParamEditor.Koikatu
{
    internal class CharacterParam
    {
        private static ParamData.CharaData _charaData;

        public static void Init(ParamData.CharaData data)
        {
            _charaData = data;
            Harmony.CreateAndPatchAll(typeof(Hooks));
        }

        public static void Save()
        {
            var selected = GuideObjectManager.Instance.selectObjectKey
                .Select(x => Studio.Studio.GetCtrlInfo(x) as OCIChar).Where(x => x != null).ToList();

            if(selected.Count > 0)
            {
                var status = selected[0].charFileStatus;

                _charaData.clothesState = status.clothesState.ToArray();
                _charaData.shoesType = status.shoesType;
                _charaData.hohoAkaRate = status.hohoAkaRate;
                _charaData.nipStandRate = status.nipStandRate;
                _charaData.tearsLv = status.tearsLv;

                _charaData.eyesLookPtn = status.eyesLookPtn;
                _charaData.neckLookPtn = status.neckLookPtn;
                _charaData.eyebrowPtn = status.eyebrowPtn;
                _charaData.eyesPtn = status.eyesPtn;
                _charaData.eyesOpenMax = status.eyesOpenMax;
                _charaData.eyesBlink = status.eyesBlink;
                _charaData.mouthPtn = status.mouthPtn;

                _charaData.saved = true;
                Log.Info("Default character settings saved");
            }
            else
            {
                Log.Message("Warning: Select a character to save default settings");
            }
        }

        public static void Reset()
        {
            _charaData.saved = false;
        }

        public static void Load()
        {
            if(_charaData.saved)
            {
                var selected = GuideObjectManager.Instance.selectObjectKey
                    .Select(x => Studio.Studio.GetCtrlInfo(x) as OCIChar).Where(x => x != null).ToList();

                if(selected.Count > 0)
                {
                    Log.Info("Loading chara defaults");
                    foreach(var chara in selected)
                        UpdateCharaState(chara);
                }
            }
            else
            {
                Log.Message("Character defaults have not been saved yet");
            }
        }

        private static void UpdateChaFileStatus(ChaFileStatus chaFileStatus)
        {
            chaFileStatus.clothesState = _charaData.clothesState.ToArray();
            chaFileStatus.shoesType = _charaData.shoesType;
            chaFileStatus.hohoAkaRate = _charaData.hohoAkaRate;
            chaFileStatus.nipStandRate = _charaData.nipStandRate;
            chaFileStatus.tearsLv = _charaData.tearsLv;

            chaFileStatus.eyesLookPtn = _charaData.eyesLookPtn;
            chaFileStatus.neckLookPtn = _charaData.neckLookPtn;
            chaFileStatus.eyebrowPtn = _charaData.eyebrowPtn;
            chaFileStatus.eyesPtn = _charaData.eyesPtn;
            chaFileStatus.eyesOpenMax = _charaData.eyesOpenMax;
            chaFileStatus.eyesBlink = _charaData.eyesBlink;
            chaFileStatus.mouthPtn = _charaData.mouthPtn;
        }

        private static void UpdateCharaState(OCIChar chara)
        {
            UpdateChaFileStatus(chara.charFileStatus);

            // This could work but disables accessories
            //AddObjectAssist.UpdateState(chara, chara.charFileStatus);
            chara.ChangeLookEyesPtn(chara.charFileStatus.eyesLookPtn, true);
            chara.ChangeLookNeckPtn(chara.charFileStatus.neckLookPtn);
            chara.charInfo.ChangeEyebrowPtn(chara.charFileStatus.eyebrowPtn);
            chara.charInfo.ChangeEyesPtn(chara.charFileStatus.eyesPtn);
            chara.ChangeEyesOpen(chara.charFileStatus.eyesOpenMax);
            chara.ChangeBlink(chara.charFileStatus.eyesBlink);
            chara.charInfo.ChangeMouthPtn(chara.charFileStatus.mouthPtn);
            //chara.ChangeMouthOpen(chara.oiCharInfo.mouthOpen);
            //chara.ChangeHandAnime(0, chara.oiCharInfo.handPtn[0]);
            //chara.ChangeHandAnime(1, chara.oiCharInfo.handPtn[1]);
            
            var mpCharCtrl = GameObject.FindObjectOfType<MPCharCtrl>();
            if(mpCharCtrl && (mpCharCtrl.select == 0 || mpCharCtrl.select == 1))
                mpCharCtrl.OnClickRoot(mpCharCtrl.select);
        }

        private class Hooks
        {
            [HarmonyPostfix, HarmonyPatch(typeof(ChaFileStatus), nameof(ChaFileStatus.MemberInit))]
            public static void HarmonyPatch_ChaFileStatus_MemberInit(ChaFileStatus __instance)
            {
                if(_charaData.saved)
                {
                    Log.Debug("Loading defaults for a new character");
                    UpdateChaFileStatus(__instance);
                }
            }
        }
    }
}
