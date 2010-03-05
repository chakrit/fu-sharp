
using System;
using System.IO;

namespace Fu
{
    public static class PathResolution
    {
        public static string ResolvePath(this IFuContext c, string appPath)
        { return ResolvePath(c.Settings.BasePath, appPath); }

        public static string ResolvePath(this IFuContext c, string appPath, bool disablePathProtection)
        { return ResolvePath(c.Settings.BasePath, appPath, disablePathProtection); }

        public static string ResolvePath(string basePath, string appPath)
        { return ResolvePath(basePath, appPath, false); }

        public static string ResolvePath(string basePath, string appPath, bool disablePathProtection)
        {
            // always resolve the basePath itself first
            // we can assume that basepath is safe since it'll be set by the developers
            // and not some malicious 3rd-party
            basePath = Path.Combine(Environment.CurrentDirectory, basePath);
            basePath = Path.GetFullPath(basePath);

            // null check
            if (string.IsNullOrEmpty(appPath))
                return basePath;

            // the root should be replaced with basePath
            if (appPath.StartsWith(@"~/") ||
                appPath.StartsWith(@"~\"))
                appPath = appPath.Substring(2);

            else if (appPath.StartsWith(@"\") ||
                appPath.StartsWith(@"/") ||
                appPath.StartsWith(@"~"))
                appPath = appPath.Substring(1);

            // uses Path.Combine and Path.GetFullPath to do the heavy lifting
            // GetFullPath is for resolving relative paths (. and ..)
            var result = Path.Combine(basePath, appPath);
            result = Path.GetFullPath(result);

            // Ensure nobody can escape outside basePath after all resolution
            // if path protection is not disabled
            if (!disablePathProtection)
                if (!result.StartsWith(basePath))
                    return basePath;

            return result;
        }
    }
}
