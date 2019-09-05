using HarmonyLib;
using System;

namespace KeelPlugins
{
    internal static class ItemDataBasePatch
    {
        [HarmonyPrefix, HarmonyPatch(typeof(ItemDataBase), new Type[] { typeof(int), typeof(string), typeof(string), typeof(int), typeof(bool) })]
        public static bool ConstructorPatch(ItemDataBase __instance, ref int id, ref string name, ref string assetbundleName, ref int order, ref bool isNew)
        {
            name = name.Replace("\\n", "\n");
            __instance.id = ((id > 999999 && id < 1000000000) ? id : (id % 1000));
            __instance.name_LineFeed = name;
            __instance.name = name.Replace("\n", string.Empty);

            if(MoreSlotID.AssetList.TryGetValue(assetbundleName, out string abdataDir))
            {
                __instance.assetbundleDir = abdataDir;
                Console.WriteLine($"Found asset bundle {assetbundleName} in {abdataDir}");
            }
            else
            {
                __instance.assetbundleDir = GlobalData.assetBundlePath;
            }

            __instance.assetbundleName = assetbundleName;
            __instance.order = order;
            __instance.isNew = isNew;

            return false;
        }
    }
}
