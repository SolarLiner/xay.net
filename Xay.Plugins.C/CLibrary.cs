using System;
using System.Collections.Generic;
using System.Linq;
using Xay.Lib;
using Xay.Ninja.Objects;

namespace Xay.Plugins.C
{
    public class CLibrary: IBuildable
    {
        public CLibrary(string name, ArtifactLinkMode buildMode, IEnumerable<CObject> objects)
        {
            BuildMode = buildMode;
            Name = name + GetExtension();
            Objects = objects;
        }

        public IEnumerable<CObject> Objects { get; }

        public string Name { get; }
        public IEnumerable<IDependency>? Dependencies => Objects;
        public IEnumerable<IDependency> FlattenDependencies() =>
            Dependencies.Concat(Dependencies.SelectMany(d => d.FlattenDependencies()));

        public Rule Rule => new Rule
        {
            Name = "clink_lib", Command = $"{EnvironmentHelpers.GetCompiler()} {LinkArgs}$in $clibs -o $out",
            Description = "Linking to $out"
        };

        public string LinkArgs => BuildMode == ArtifactLinkMode.Shared ? "-shared " : "";
        public ArtifactLinkMode BuildMode { get; }

        private string GetExtension() => Environment.OSVersion.Platform == PlatformID.Unix
            ? GetUnixExtension()
            : GetWindowExtension();

        private string GetWindowExtension() => BuildMode == ArtifactLinkMode.Shared ? ".dll" : ".lib";

        private string GetUnixExtension() => BuildMode == ArtifactLinkMode.Shared ? ".so" : ".a";
    }
}