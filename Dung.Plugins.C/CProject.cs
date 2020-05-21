using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Dung.Lib;
using Serilog;
using YamlDotNet.RepresentationModel;

namespace Dung.Plugins.C
{
    public class CProject : Project
    {
        public CProject(string root, string sourceDir, string buildDir) : base(root, sourceDir, buildDir)
        {
            List<CObject> objects = Directory.EnumerateFiles(sourceDir)
                .Where(f => f.EndsWith(".c"))
                .Select(s => new CObject(sourceDir,
                    s))
                .ToList();
            /*if (Configuration.HasValue)
            {
                if (Configuration.Value.Exclude != null)
                {
                    var excluded = Configuration.Value.Exclude.Select(s => Path.Join(sourceDir, s)).ToHashSet();
                    objects = objects.Where(o => !excluded.Contains(o.SourceFile)).ToList();
                }

                if (Configuration.Value.Include != null)
                {
                    var included = Configuration.Value.Include.Select(s => Path.Join(sourceDir, s)).ToHashSet();
                    objects = objects.Where(o => included.Contains(o.SourceFile)).ToList();
                }

                if (Configuration.Value.SystemDependencies != null)
                {
                    var flags = new List<string>();
                    var libs = new List<string>();
                    foreach (string dep in
                        Configuration.Value.SystemDependencies)
                    {
                        if (dep == "math" || dep == "m")
                        {
                            libs.Add("-lm");
                            continue;
                        }
                        var pkgconfig = new PkgConfig(dep);
                        flags.Add(pkgconfig.CFlags);
                        libs.Add(pkgconfig.Libs);
                    }

                    Variables.Add("cflags", string.Join(" ", flags));
                    Variables.Add("clibs", string.Join(" ", libs));
                }
            }*/

            var buildType = ArtifactType.Executable;
            var buildMode = ArtifactLinkMode.Shared;

            if (Configuration?.RootNode is YamlMappingNode node)
            {
                var nodeExclude = new YamlScalarNode("exclude");
                if (node.Children.ContainsKey(nodeExclude))
                    if (node.Children[nodeExclude] is YamlSequenceNode seq)
                    {
                        var excluded = seq
                            .OfType<YamlScalarNode>()
                            .Select(s => Path.Join(sourceDir, s.ToString()))
                            .ToHashSet();
                        objects = objects.Where(o => !excluded.Contains(o.SourceFile)).ToList();
                    }

                var nodeInclude = new YamlScalarNode("include");
                if (node.Children.ContainsKey(nodeInclude))
                    if (node.Children[nodeInclude] is YamlSequenceNode seq)
                    {
                        var included = seq
                            .OfType<YamlScalarNode>()
                            .Select(s => Path.Join(sourceDir, s.ToString()))
                            .ToHashSet();
                        objects = objects.Where(o => included.Contains(o.SourceFile)).ToList();
                    }

                var nodeBuildType = new YamlScalarNode("type");
                if (node.Children.ContainsKey(nodeBuildType))
                    if (node.Children[nodeBuildType] is YamlScalarNode scalarNode)
                    {
                        buildType = scalarNode.Value switch
                        {
                            "library" => ArtifactType.Library,
                            "executable" => ArtifactType.Executable,
                            _ => throw new ConfiguationException(
                                "Build type can only be either 'library' or 'executable'")
                        };
                    }

                var nodeBuildMode = new YamlScalarNode("mode");
                if (node.Children.ContainsKey(nodeBuildMode))
                    if (node.Children[nodeBuildMode] is YamlScalarNode scalarNode)
                    {
                        buildMode = scalarNode.Value switch
                        {
                            "shared" => ArtifactLinkMode.Shared,
                            "static" => ArtifactLinkMode.Static,
                            _ => throw new ConfiguationException(
                                "Build mode can only be either 'static' or 'shared'")
                        };
                    }

                var nodeDependencies = new YamlScalarNode("dependencies");
                if (node.Children.ContainsKey(nodeDependencies))
                    if (node.Children[nodeDependencies] is YamlSequenceNode seq)
                    {
                        var flags = new List<string>();
                        var libs = new List<string>();
                        if(buildMode == ArtifactLinkMode.Shared && buildType == ArtifactType.Library)
                            flags.Add("-fpic");
                        foreach (string dep in seq.OfType<YamlScalarNode>().Select(n => n.ToString()))
                            if (dep == "math" || dep == "m")
                            {
                                libs.Add("-lm");
                            }
                            else
                            {
                                var pkg = new PkgConfig(dep);
                                if (pkg.Exists)
                                {
                                    flags.Add(pkg.CFlags);
                                    flags.Add($"-DHAVE_{dep.ToUpperInvariant()}");
                                    libs.Add(pkg.Libs);
                                    libs.Add(pkg.SharedLibrary);
                                }
                                else
                                {
                                    Console.Error.WriteLine(
                                        $"WARNING: Dependency {dep} not found on your system. Make sure to install it beforehand.");
                                }
                            }

                        Variables.Add("cflags", string.Join(" ", flags));
                        Variables.Add("clibs", string.Join(" ", libs));
                    }
            }

            if (!Variables.ContainsKey("cflags")) Variables.Add("cflags", "");
            if (!Variables.ContainsKey("clibs")) Variables.Add("clibs", "");
            Log.Information("Found sources:");
            foreach (string s in objects.Select(obj => obj.SourceFile)) Log.Information("\t{@string}", s);
            Entrypoint = buildType == ArtifactType.Executable
                ? (IBuildable) new CExe(Name,
                    buildMode,
                    objects)
                : new CLibrary(Name,
                    buildMode,
                    objects);
        }

        protected override IDependency Entrypoint { get; }
    }

    public class ConfiguationException : Exception
    {
        public ConfiguationException(string message) : base(message)
        {
        }
    }
}