using HarmonyLib;
using Studio;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CharaStateX.Koikatu
{
    internal static class AnimationPatch
    {
        [HarmonyPostfix, HarmonyPatch(typeof(PauseRegistrationList), "OnClickLoad")]
        public static void PoseLoadPatch(PauseRegistrationList __instance)
        {
            var traverse = Traverse.Create(__instance);
            var listPath = traverse.Field("listPath").GetValue<List<string>>();
            var select = traverse.Field("select").GetValue<int>();

            foreach(var chara in Utils.GetSelectedCharacters().Where(chara => chara != __instance.ociChar))
                PauseCtrl.Load(chara, listPath[select]);
        }

        [HarmonyPrefix, HarmonyPatch(typeof(MPCharCtrl), "LoadAnime")]
        public static bool LoadAnimePrefix(MPCharCtrl __instance, ref int _group, ref int _category, ref int _no)
        {
            bool sexMatch = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);

            foreach(var chara in Utils.GetSelectedCharacters())
            {
                if(chara != __instance.ociChar)
                {
                    float param1 = chara.animeOptionParam1;
                    float param2 = chara.animeOptionParam2;

                    var soup = sexMatch ? MatchCategory(chara.sex, _group) : _group;
                    chara.LoadAnime(soup, _category, _no);

                    chara.animeOptionParam1 = param1;
                    chara.animeOptionParam2 = param2;
                }
            }

            float animeOptionParam1 = __instance.ociChar.animeOptionParam1;
            float animeOptionParam2 = __instance.ociChar.animeOptionParam2;

            int group = sexMatch ? MatchCategory(__instance.ociChar.sex, _group) : _group;
            __instance.ociChar.LoadAnime(group, _category, _no);
            Traverse.Create(__instance).Field("animeControl").Method("UpdateInfo").GetValue();

            __instance.ociChar.animeOptionParam1 = animeOptionParam1;
            __instance.ociChar.animeOptionParam2 = animeOptionParam2;

            return false;
        }

        private static int MatchCategory(int sex, int group)
        {
            if(sex == 1)
            {
                switch(group)
                {
                    case 3: return 2;
                    case 5: return 4;
                }
            }
            else if(sex == 0)
            {
                switch(group)
                {
                    case 2: return 3;
                    case 4: return 5;
                }
            }

            return group;
        }
    }
}
