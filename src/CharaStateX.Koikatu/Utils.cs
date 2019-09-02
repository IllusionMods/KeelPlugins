using HarmonyLib;
using Studio;
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
            return Traverse.Create(__instance).Property("ociChar").GetValue<OCIChar>();
        }

        public static bool GetIsUpdateInfo(object __instance)
        {
            return Traverse.Create(__instance).Property("isUpdateInfo").GetValue<bool>();
        }
    }
}
