using System;
using System.IO;
using System.Linq;

namespace KeelPlugins
{
    internal static class FileUtils
    {
        public static string CombinePaths(string path1, params string[] paths)
        {
            if(path1 == null) throw new ArgumentNullException(nameof(path1));
            if(paths == null) throw new ArgumentNullException(nameof(paths));

            return paths.Aggregate(path1, (acc, p) => Path.Combine(acc, p));
        }
    }
}
