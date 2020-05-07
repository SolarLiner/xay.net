using System.IO;
using System.Linq;

namespace Dung.Lib.Lang.C
{
    public class CProject : Project
    {
        public CProject(string root, string? name = null) : base(Path.Join(root, "build"))
        {
            Name = name ?? Path.GetFileName(root);
            var sourceDir = Path.Join(root, "src");

            Entrypoint = new CExe(Name, BuildDir, Directory.EnumerateFiles(sourceDir)
                .Where(f => f.EndsWith(".c"))
                .Select(s => new CObject(sourceDir,
                    s))
                .ToList());
        }

        public override IDependency Entrypoint { get; }

        public override string Name { get; }

        /*public string SourceRoot { get; }
        public string BuildRoot { get; }

        public IEnumerable<CObject> Objects => Sources.Select(s => new CObject(SourceRoot, s)).ToList();
        public CExe Executable => new CExe(Name, BuildRoot, Objects);

        public HashSet<Rule> Rules() =>
            Objects.SelectMany(o => o.Builds(BuildRoot))
                .Select(b => b.Rule)
                .ToHashSet(new Rule.Comparer());

        public List<string> Sources { get; }

        public static CProject? DetectProject(string root)
        {
            string sourceRoot = Path.Join(root, "src");
            if (!Directory.Exists(sourceRoot) ||
                !Directory.GetFiles(sourceRoot)
                    .Any(f => f.EndsWith(".c") || f.EndsWith(".h")))
                return null;
            string buildRoot = Path.Join(root, "build");
            return new CProject(Path.GetFileName(root), sourceRoot, buildRoot);
        }

        public CProject(string name, string sourceRoot, string buildRoot)
        {
            Name = name;
            SourceRoot = sourceRoot;
            BuildRoot = buildRoot;
            Sources = Directory.GetFiles(sourceRoot).Where(f => f.EndsWith(".c")).ToList();
        }

        public IEnumerable<Build> Builds(string buildDir) =>
            Objects.SelectMany(o => o.Builds(buildDir))
                .Concat(Executable.Builds(buildDir));

        public string Name { get; }*/
        public static CProject? DetectProject(string root)
        {
            var sourceDir = Path.Join(root, "src");
            if (!Directory.Exists(sourceDir) || !Directory.EnumerateFiles(sourceDir).Any(f => f.EndsWith(".c")))
                return null;
            return new CProject(root);
        }
    }
}