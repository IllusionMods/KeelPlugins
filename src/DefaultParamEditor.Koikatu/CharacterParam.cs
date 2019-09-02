using BepInEx.Logging;
using HarmonyLib;
using Studio;
using System.Linq;
using UnityEngine;

namespace KeelPlugins
{
    internal class CharacterParam
    {
        private static ParamData.CharaData _charaData;

        public static void Init(ParamData.CharaData data)
        {
            _charaData = data;
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
                DefaultParamEditor.Logger.LogMessage("Default character settings saved");
            }
            else
            {
                DefaultParamEditor.Logger.LogMessage("Warning: Select character to save default settings");
            }
        }

        public static void Reset()
        {
            _charaData.saved = false;
        }

        public static void LoadDefaults()
        {
            if(_charaData.saved)
            {
                var selected = GuideObjectManager.Instance.selectObjectKey
                    .Select(x => Studio.Studio.GetCtrlInfo(x) as OCIChar).Where(x => x != null).ToList();

                if(selected.Count > 0)
                {
                    foreach(var chara in selected)
                        SetCharaValues(chara.charFileStatus);

                    UpdateStateInfo();
                    DefaultParamEditor.Logger.LogInfo("Loading chara defaults");
                }
            }
        }

        private static void SetCharaValues(ChaFileStatus chaFileStatus)
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

        private static void UpdateStateInfo()
        {
            var mpCharCtrl = GameObject.FindObjectOfType<MPCharCtrl>();
            if(mpCharCtrl)
            {
                int select = Traverse.Create(mpCharCtrl).Field("select").GetValue<int>();
                if(select == 0) mpCharCtrl.OnClickRoot(0);
            }
        }

        private class Hooks
        {
            [HarmonyPostfix, HarmonyPatch(typeof(ChaFileStatus), nameof(ChaFileStatus.MemberInit))]
            public static void HarmonyPatch_ChaFileStatus_MemberInit(ChaFileStatus __instance)
            {
                if(_charaData.saved)
                {
                    DefaultParamEditor.Logger.LogDebug("Loading defaults for a new character");
                    SetCharaValues(__instance);
                }
            }
        }
    }
}
