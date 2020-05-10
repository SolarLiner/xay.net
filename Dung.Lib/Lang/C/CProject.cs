using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Dung.Lib.Lang.C
{
    public class CProject : Project
    {
        public CProject(string root, string? name = null) : base(root)
        {
            var sourceDir = Path.Join(root, "src");

            List<CObject> objects = Directory.EnumerateFiles(sourceDir)
                .Where(f => f.EndsWith(".c"))
                .Select(s => new CObject(sourceDir,
                    s))
                .ToList();
            if (Configuration.HasValue)
            {
                if (Configuration.Value.Exclude != null)
                {
                    var excluded = Configuration.Value.Exclude.Select(s => Path.Join(sourceDir, s)).ToHashSet();
                    objects = objects.Where(o => excluded.Contains(o.SourceFile)).ToList();
                }

                if (Configuration.Value.Include != null)
                {
                    var included = Configuration.Value.Include.Select(s => Path.Join(sourceDir, s)).ToHashSet();
                    objects = objects.Where(o => included.Contains(o.SourceFile)).ToList();
                }
            }

            Name = Configuration?.Name ?? name ?? Path.GetFileName(root);
            Entrypoint = new CExe(Name, BuildDir, objects);
        }

        protected override IDependency Entrypoint { get; }

        public override string Name { get; }

        public static CProject? DetectProject(string root)
        {
            var sourceDir = Path.Join(root, "src");
            if (!Directory.Exists(sourceDir) || !Directory.EnumerateFiles(sourceDir).Any(f => f.EndsWith(".c")))
                return null;
            return new CProject(root);
        }
    }
}