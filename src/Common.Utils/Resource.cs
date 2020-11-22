using System.IO;
using System.Reflection;

namespace KeelPlugins.Utils
{
    internal static class Resource
    {
        public static byte[] GetResourceAsBytes(Assembly assembly, string resourceName)
        {
            using(var stream = GetManifestResourceStream(assembly, resourceName))
            using(var mem = new MemoryStream())
            {
                CopyTo(stream, mem);
                return mem.ToArray();
            }
        }

        public static string GetResourceAsString(Assembly assembly, string resourceName)
        {
            using(var stream = GetManifestResourceStream(assembly, resourceName))
            using(var reader = new StreamReader(stream))
                return reader.ReadToEnd();
        }

        private static Stream GetManifestResourceStream(Assembly assembly, string resourceName)
        {
            var resourcePath = $"{assembly.GetName().Name}.{resourceName}";
            var res = assembly.GetManifestResourceStream(resourcePath);

            if(res == null)
                throw new FileNotFoundException(resourcePath);

            return res;
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
