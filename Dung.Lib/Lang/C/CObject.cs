using System.Collections.Generic;
using System.IO;
using System.Linq;
using Dung.Ninja.Objects;

namespace Dung.Lib.Lang.C
{
    public class CObject : IBuildable
    {
        private readonly string sourceDir;

        private readonly string sourceRel;

        public CObject(string sourceDir, string sourceFile)
        {
            this.sourceDir = sourceDir;
            sourceRel = Path.GetRelativePath(sourceDir, sourceFile);
        }

        public string SourceFile => Path.Join(sourceDir, sourceRel);
        public string Name => Path.ChangeExtension(Path.GetFileName(SourceFile), ".o");
        public IEnumerable<IDependency>? Dependencies => new[] {new Source(SourceFile)};

        public IEnumerable<IDependency> FlattenDependencies()
        {
            return Dependencies.Concat(Dependencies.SelectMany(d => d.FlattenDependencies()));
        }

        Rule IBuildable.Rule => new Rule
        {
            Command = "gcc -MMD -MT $out -MF $out.d -c $in -o $out",
            Name = "cc",
            Description = "Compiling C file $in",
            Depfile = "$out.d"
        };
    }
}