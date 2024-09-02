using HarmonyLib;
using Studio;
using System.Linq;
using UnityEngine;

namespace DefaultParamEditor.Koikatu
{
    public static class CharacterParam
    {
        private static readonly ParamData.CharaData _charaData = ParamData.Instance.charaParamData;

        public static void Save()
        {
            var selected = GuideObjectManager.Instance.selectObjectKey
                .Select(x => Studio.Studio.GetCtrlInfo(x) as OCIChar).Where(x => x != null).ToList();

            if(selected.Count > 0)
            {
                var chara = selected[0];
                var status = chara.charFileStatus;

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
                _charaData.handPtn = chara.oiCharInfo.handPtn.ToArray();
                _charaData.mouthOpen = chara.oiCharInfo.mouthOpen;

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
                        UpdateState(chara);

                    var mpCharCtrl = GameObject.FindObjectOfType<MPCharCtrl>();
                    if(mpCharCtrl && (mpCharCtrl.select == 0 || mpCharCtrl.select == 1))
                        mpCharCtrl.OnClickRoot(mpCharCtrl.select);
                }
            }
            else
            {
                Log.Message("Character defaults have not been saved yet");
            }
        }

        public static void UpdateState(OCIChar ociChar)
        {
            ociChar.charInfo.fileStatus.clothesState = _charaData.clothesState.ToArray();
            ociChar.charInfo.fileStatus.shoesType = _charaData.shoesType;
            ociChar.ChangeLookNeckPtn(_charaData.neckLookPtn);
            ociChar.charInfo.ChangeEyebrowPtn(_charaData.eyebrowPtn);
            ociChar.charInfo.ChangeEyesPtn(_charaData.eyesPtn);
            ociChar.ChangeEyesOpen(_charaData.eyesOpenMax);
            ociChar.ChangeBlink(_charaData.eyesBlink);
            ociChar.charInfo.ChangeMouthPtn(_charaData.mouthPtn);
            ociChar.ChangeMouthOpen(_charaData.mouthOpen);
            ociChar.ChangeHandAnime(0, _charaData.handPtn[0]);
            ociChar.ChangeHandAnime(1, _charaData.handPtn[1]);
            ociChar.SetHohoAkaRate(_charaData.hohoAkaRate);
            ociChar.SetNipStand(_charaData.nipStandRate);
            ociChar.SetTearsLv(_charaData.tearsLv);
            ociChar.ChangeLookEyesPtn(_charaData.eyesLookPtn, true);
        }

        public class Hooks
        {
            [HarmonyPostfix, HarmonyPatch(typeof(AddObjectFemale), nameof(AddObjectFemale.Add), typeof(string))]
            public static void HarmonyPatch_AddObjectFemale_Add(OCICharFemale __result)
            {
                if(_charaData.saved)
                {
                    Log.Debug("Loading defaults for a new character");
                    UpdateState(__result);
                }
            }
        }
    }
}
