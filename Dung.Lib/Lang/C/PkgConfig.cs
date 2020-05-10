using System;
using System.Diagnostics;
using System.IO;

namespace Dung.Lib.Lang.C
{
    public class PkgConfig
    {
        private readonly ProcessStartInfo _pkgConfig;
        private string? _cflags;
        private string? _clibs;
        private bool? _exists;
        private string? _sharedlib;
        private string? _staticlib;

        public PkgConfig(string dependency)
        {
            _pkgConfig = FindPkgConfig();
            Dependency = dependency;
        }

        public string Dependency { get; }

        public bool Exists => _exists.GetValueOrDefault((bool) (_exists = Run("--exists").Item1 == 0));
        public string CFlags => _cflags ??= Run("--cflags").Item2;
        public string Libs => _clibs ??= Run("--libs").Item2;
        public string SharedLibrary => _sharedlib ??= $"{Run("--variable=libdir").Item2}/lib{Dependency}.so";
        public string StaticLibrary => _staticlib ??= $"{Run("--variable=libdir").Item2}/lib{Dependency}.a";

        private (int, string) Run(string arguments)
        {
            var startinfo = _pkgConfig;
            startinfo.Arguments = $"{arguments} {Dependency}";
            var proc = new Process {StartInfo = startinfo};
            proc.Start();
            proc.WaitForExit();
            return (proc.ExitCode, proc.StandardOutput.ReadToEnd().Trim());
        }

        private static ProcessStartInfo FindPkgConfig()
        {
            string command = Environment.OSVersion.Platform == PlatformID.Unix ? "which" : "where";
            var proc = new Process
            {
                StartInfo = new ProcessStartInfo(command,
                    "pkg-config")
                {
                    RedirectStandardOutput = true
                }
            };
            proc.Start();
            proc.WaitForExit();
            if (proc.ExitCode != 0) throw new FileNotFoundException("pkg-config has not been found");
            return new ProcessStartInfo(proc.StandardOutput.ReadToEnd().Trim())
            {
                RedirectStandardOutput = true
            };
        }
    }
}