using System;
using System.IO;
using System.Reflection;

namespace UILib
{
    internal static class UILibResource
    {
        public static byte[] LoadEmbeddedResource(Assembly callingAssembly, string resourceName)
        {
            try
            {
                using(var stream = callingAssembly.GetManifestResourceStream(resourceName))
                {
                    return ReadFully(stream);
                }
            }
            catch(Exception)
            {
                Console.WriteLine($"Error accessing resources ({resourceName})");
                throw;
            }
        }

        public static byte[] ReadFully(Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using(MemoryStream ms = new MemoryStream())
            {
                int read;
                while((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }
    }
}
