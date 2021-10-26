using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace MockServer.Environment.Extensions
{
    public static class FileExtensions
    {
        public static string GetFullPath(string location)
        {
            if (location.StartsWith("\\"))
            {
                var pathes = location.Split('\\', StringSplitOptions.RemoveEmptyEntries).ToList();
                pathes.Insert(0, Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
                return Path.Combine(pathes.ToArray());
            }

            return location;
        }
    }
}
