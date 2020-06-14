using HarmonyLib;
using Studio;
using System;
using System.Collections.Generic;
using System.Linq;

namespace KeelPlugins
{
    internal static class Utils
    {
        public static IEnumerable<OCIChar> GetSelectedCharacters()
        {
            return GuideObjectManager.Instance.selectObjectKey.Select(x => Studio.Studio.GetCtrlInfo(x) as OCIChar).Where(x => x != null);
        }

        public static IEnumerable<OCIChar> GetAllSelectedButMain(object __instance)
        {
            var mainChara = GetMainChara(__instance);
            return GetSelectedCharacters().Where((chara) => chara != mainChara);
        }

        public static OCIChar GetMainChara(object __instance)
        {
            if(__instance == null) throw new ArgumentNullException(nameof(__instance));
            var property = __instance.GetType().GetProperty("ociChar", AccessTools.all);
            if(property == null) throw new ArgumentException("Could not find property ociChar");
            return (OCIChar)property.GetValue(__instance, null);
        }

        public static bool GetIsUpdateInfo(object __instance)
        {
            var propInfo = AccessTools.Property(__instance.GetType(), "isUpdateInfo");
            return (bool)propInfo.GetValue(__instance, null);
        }
    }
}
