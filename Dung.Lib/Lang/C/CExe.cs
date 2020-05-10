using System.Collections.Generic;
using System.Linq;
using Dung.Ninja.Objects;

namespace Dung.Lib.Lang.C
{
    public class CExe : IBuildable
    {
        public CExe(string name, string buildDir, IEnumerable<CObject> objects)
        {
            Name = name;
            BuildDir = buildDir;
            Objects = objects;
        }

        public string BuildDir { get; }
        public IEnumerable<CObject> Objects { get; }
        public string Name { get; }
        public IEnumerable<IDependency>? Dependencies => Objects;

        public IEnumerable<IDependency> FlattenDependencies()
        {
            return Dependencies.Concat(Dependencies.SelectMany(d => d.FlattenDependencies()));
        }

        public Rule Rule => new Rule
        {
            Name = "link", Command = $"{EnvironmentHelpers.GetCompiler()} $clibs $in -o $out",
            Description = "Linking to $out"
        };
    }
}