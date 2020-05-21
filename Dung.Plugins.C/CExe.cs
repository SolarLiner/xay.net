using System;
using System.Collections.Generic;
using System.Linq;
using Dung.Lib;
using Dung.Ninja.Objects;

namespace Dung.Plugins.C
{
    public class CExe : IBuildable
    {
        public CExe(string name, ArtifactLinkMode buildMode, IEnumerable<CObject> objects)
        {
            Name = name + GetExtension();
            Objects = objects;
            BuildMode = buildMode;
        }

        public IEnumerable<CObject> Objects { get; }
        public ArtifactLinkMode BuildMode { get; }
        public string Name { get; }
        public IEnumerable<IDependency>? Dependencies => Objects;

        public IEnumerable<IDependency> FlattenDependencies()
        {
            return Dependencies.Concat(Dependencies.SelectMany(d => d.FlattenDependencies()));
        }

        public Rule Rule => new Rule
        {
            Name = "clink_exe", Command = $"{EnvironmentHelpers.GetCompiler()} {LinkArgs}$in $clibs -o $out",
            Description = "Linking to $out"
        };

        public string LinkArgs => BuildMode == ArtifactLinkMode.Static ? "-static " : "";

        private static string GetExtension() => Environment.OSVersion.Platform == PlatformID.Unix ? "" : ".exe";
    }
}