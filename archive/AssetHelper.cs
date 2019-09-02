using System;
using System.IO;
using UnityEngine;

namespace ShaderMagic
{
    public static class AssetHelper
    {
        public static Shader LoadShaderAsset(string assetBundlePath, string assetName)
        {
            Shader shader = null;
            AssetBundle ab = null;

            try
            {
                var bytes = File.ReadAllBytes(assetBundlePath);
                ab = AssetBundle.LoadFromMemory(bytes);
                shader = ab.LoadAsset<Shader>(assetName);
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Error loading shader \"{assetName}\" from \"{assetBundlePath}\"");
                Console.WriteLine(ex);
            }

            ab?.Unload(false);
            return shader;
        }

        public static Texture2D LoadTexture2D(string filePath)
        {
            try
            {
                Texture2D tex = new Texture2D(2, 2);
                tex.LoadImage(File.ReadAllBytes(filePath));
                return tex;
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Error loading image from \"{filePath}\"");
                Console.WriteLine(ex);
                return null;
            }
        }
    }
}
