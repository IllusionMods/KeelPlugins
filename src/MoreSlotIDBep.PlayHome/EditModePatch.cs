using HarmonyLib;
using System;
using System.IO;
using UnityEngine;

namespace KeelPlugins
{
    internal static class EditModePatch
    {
        [HarmonyPrefix, HarmonyPatch(typeof(EditMode), "CreateData", new Type[] { typeof(PrefabData), typeof(Rect), typeof(AssetBundleController) })]
        public static bool CreateData_PrefabData_Patch(ref CustomSelectSet __result, ref PrefabData item, ref Rect smallRect, ref AssetBundleController abc)
        {
            Vector2 vector = new Vector2(256f, 256f);
            Texture2D texture = LoadAsset(abc, item.prefab);
            Sprite thumbnail_L = Sprite.Create(texture, new Rect(Vector2.zero, vector), vector * 0.5f, 100f, 0u, SpriteMeshType.FullRect);
            Sprite thumbnail_S = Sprite.Create(texture, smallRect, smallRect.size * 0.5f, 100f, 0u, SpriteMeshType.FullRect);
            __result = new CustomSelectSet(item.id, item.name_LineFeed, thumbnail_S, thumbnail_L, item.isNew);

            return false;
        }

        [HarmonyPrefix, HarmonyPatch(typeof(EditMode), "CreateData", new Type[] { typeof(CombineTextureData), typeof(Rect), typeof(AssetBundleController) })]
        public static bool CreateData_CombineTextureData_Patch(ref CustomSelectSet __result, ref CombineTextureData item, ref Rect smallRect, ref AssetBundleController abc)
        {
            Vector2 vector = new Vector2(256f, 256f);
            Texture2D texture = LoadAsset(abc, item.textureName);
            Sprite thumbnail_L = Sprite.Create(texture, new Rect(Vector2.zero, vector), vector * 0.5f, 100f, 0u, SpriteMeshType.FullRect);
            Sprite thumbnail_S = Sprite.Create(texture, smallRect, smallRect.size * 0.5f, 100f, 0u, SpriteMeshType.FullRect);
            __result = new CustomSelectSet(item.id, item.name_LineFeed, thumbnail_S, thumbnail_L, item.isNew);

            return false;
        }

        [HarmonyPrefix, HarmonyPatch(typeof(EditMode), "CreateData", new Type[] { typeof(HeadData), typeof(Rect), typeof(AssetBundleController) })]
        public static bool CreateData_PrefabData_Patch(ref CustomSelectSet __result, ref HeadData item, ref Rect smallRect, ref AssetBundleController abc)
        {
            Vector2 vector = new Vector2(256f, 256f);
            Texture2D texture = LoadAsset(abc, item.path);
            Sprite thumbnail_L = Sprite.Create(texture, new Rect(Vector2.zero, vector), vector * 0.5f, 100f, 0u, SpriteMeshType.FullRect);
            Sprite thumbnail_S = Sprite.Create(texture, smallRect, smallRect.size * 0.5f, 100f, 0u, SpriteMeshType.FullRect);
            __result = new CustomSelectSet(item.id, item.name_LineFeed, thumbnail_S, thumbnail_L, item.isNew);

            return false;
        }

        [HarmonyPrefix, HarmonyPatch(typeof(EditMode), "CreateData", new Type[] { typeof(AccessoryData), typeof(Rect), typeof(AssetBundleController) })]
        public static bool CreateData_PrefabData_Patch(ref CustomSelectSet __result, ref AccessoryData item, ref Rect smallRect, ref AssetBundleController abc)
        {
            Vector2 vector = new Vector2(256f, 256f);
            Texture2D texture = LoadAsset(abc, item.prefab_F);
            Sprite thumbnail_L = Sprite.Create(texture, new Rect(Vector2.zero, vector), vector * 0.5f, 100f, 0u, SpriteMeshType.FullRect);
            Sprite thumbnail_S = Sprite.Create(texture, smallRect, smallRect.size * 0.5f, 100f, 0u, SpriteMeshType.FullRect);
            __result = new CustomSelectSet(item.id, item.name_LineFeed, thumbnail_S, thumbnail_L, item.isNew);

            return false;
        }

        private static Texture2D LoadAsset(AssetBundleController abc, string assetName)
        {
            if(abc.assetBundleName.Contains("thumnbnail/thumbnail_") || abc.assetBundleName.Contains("thumnbnail/thumnbs_"))
            {
                if(MoreSlotID.ThumbList.TryGetValue(assetName, out string abdataDir))
                {
                    string text = FileUtils.CombinePaths(abdataDir, "thumnbnail_R", assetName + ".png");
                    if(File.Exists(text))
                    {
                        Console.WriteLine("Load thumb: " + text);
                        return LoadPNG(text);
                    }
                    else
                    {
                        Console.WriteLine($"Thumbnail '{text}' does not exist");
                    }
                }
            }

            return abc.LoadAsset<Texture2D>(assetName);
        }

        private static Texture2D LoadPNG(string file)
        {
            byte[] array;
            using(BinaryReader binaryReader = new BinaryReader(new FileStream(file, FileMode.Open, FileAccess.Read)))
            {
                try
                {
                    array = binaryReader.ReadBytes((int)binaryReader.BaseStream.Length);
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex);
                    return null;
                }

                binaryReader.Close();
            }

            var texture2D = new Texture2D(1, 1, TextureFormat.ARGB32, false);
            texture2D.LoadImage(array);
            return texture2D;
        }
    }
}
