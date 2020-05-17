using System.IO;
using System.Linq;
using Dung.Lib;
using Dung.Lib.Lang.C;
using Dung.Plugin;

namespace Dung.Plugins.C
{
    public class CBuildPlugin : IBuildPlugin
    {
        public string Name => "C build plugin";
        public string Version => typeof(CBuildPlugin).Assembly.GetName().Version?.ToString() ?? "<unknown>";
        public string Author => "SolarLiner";

        public Project? DetectProject(string root, string sourceDirRelative, string buildDirRelative)
        {
            var sourceDir = Path.Join(root, sourceDirRelative);
            var buildDir = Path.Join(root, buildDirRelative);
            if (!Directory.Exists(sourceDir) || !Directory.EnumerateFiles(sourceDir).Any(f => f.EndsWith(".c")))
                return null;
            return new CProject(root, sourceDir, buildDir);
        }
    }
}