using System;
using System.Collections.Generic;
using System.IO;
using Mono.Cecil;

namespace AssOverride
{
    public class AssOverride
    {
        public const string Version = "1.0.0." + BuildNumber.Version;

        private static readonly Dictionary<string, string> asses = new Dictionary<string, string>();

        public static IEnumerable<string> TargetDLLs => GetDLLs();
        public static IEnumerable<string> GetDLLs()
        {
            foreach(var file in Directory.GetFiles("blaablaa", "*.dll"))
            {
                var filename = Path.GetFileName(file);
                var filenameNoExtension = Path.GetFileNameWithoutExtension(file);
                asses.Add(filenameNoExtension, file);
                yield return filename;
            }
        }

        public static void Patch(ref AssemblyDefinition ass)
        {
            if(asses.TryGetValue(ass.Name.Name, out var path))
            {
                ass = AssemblyDefinition.ReadAssembly(path);
                Console.Write($"Replacing {ass.Name.Name}");
            }
        }
    }
}