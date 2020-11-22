using System;
using System.IO;
using System.Reflection;

namespace KeelPlugins.Utils
{
    public static class Resource
    {
        public static byte[] GetResourceAsBytes(Assembly assembly, string resourceName)
        {
            var resourcePath = $"{assembly.GetName().Name}.{resourceName}";

            using(var res = assembly.GetManifestResourceStream(resourcePath))
            {
                if(res == null)
                    throw new FileNotFoundException(resourcePath);

                using(var mem = new MemoryStream())
                {
                    CopyTo(res, mem);
                    return mem.ToArray();
                }
            }
        }

        public static string GetResourceAsString(Assembly assembly, string resourceName)
        {
            var resourcePath = $"{assembly.GetName().Name}.{resourceName}";

            using(var stream = assembly.GetManifestResourceStream(resourcePath))
            using(var reader = new StreamReader(stream))
                return reader.ReadToEnd();
        }

        private static void CopyTo(Stream input, Stream outputStream)
        {
            byte[] buffer = new byte[16 * 1024]; // Fairly arbitrary size
            int bytesRead;
            while((bytesRead = input.Read(buffer, 0, buffer.Length)) > 0)
            {
                outputStream.Write(buffer, 0, bytesRead);
            }
        }
    }
}
