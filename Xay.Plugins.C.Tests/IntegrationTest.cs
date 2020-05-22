using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using Xay.Lib;
using Xay.Plugin;
using NUnit.Framework;
using Snapper;
using Snapper.Attributes;
using Snapper.Core;

namespace Xay.Plugins.C.Tests
{
    [TestFixture("content/simple")]
    [TestFixture("content/multiple")]
    [TestFixture("content/reallife-small")]
    [TestFixture("content/library")]
    public class IntegrationTest
    {
        public string Path { get; }

        public string CurrentPlatform => Environment.OSVersion.Platform == PlatformID.Unix ? "unix" : "windows";

        public IntegrationTest(string path)
        {
            Path = System.IO.Path.GetFullPath(path);
            Console.Error.WriteLine($"Using path {path}: {Path}");
        }

        [Test]
        [UpdateSnapshots]
        public void BuildDirectoryIsNominal()
        {
            Project project = new CBuildPlugin().DetectProject(Path, "src", "build");
            Assert.NotNull(project);
            var stream = new StreamWriter(new MemoryStream());
            project.WriteNinja(stream);

            FileTreeSnapshot().ShouldMatchChildSnapshot($"path={Path};platform={CurrentPlatform};tree");
            stream.Flush();
            var memstream = (MemoryStream) stream.BaseStream;
            memstream.GetBuffer()
                .ShouldMatchChildSnapshot($"path={Path};platform={CurrentPlatform};ninja");
        }

        public IEnumerable<string> FileTreeSnapshot() =>
            Directory.EnumerateFiles(Path,
                    "**",
                    SearchOption.AllDirectories)
                .Select(file => FileSnapshot(file));

        private string FileSnapshot(string file)
        {
            using var hasher = SHA256.Create();
            using FileStream stream = File.OpenRead(file);
            byte[] hash = hasher.ComputeHash(stream);
            string sHash = string.Join("", hash.Select(b => b.ToString("x")));
            return $"{System.IO.Path.GetRelativePath(Path, file)}: {sHash}";
        }
    }
}