using System;
using System.Diagnostics;
using System.IO;
using Serilog;

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
            Log.Information($"Found pkg-config at {_pkgConfig.FileName}");
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
            string stdout = proc.StandardOutput.ReadToEnd();
            Log.Verbose($"`pkg-config {startinfo.Arguments}` command exited with code {{@int}} - stdout:\n{{@string}}",
                proc.ExitCode, stdout);
            return (proc.ExitCode, stdout.Trim());
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
            string stdout = proc.StandardOutput.ReadToEnd();
            Log.Verbose("`which` command exited with code {@int} - stdout:\n{@string}", proc.ExitCode, stdout);
            if (proc.ExitCode != 0) throw new FileNotFoundException("pkg-config has not been found");
            return new ProcessStartInfo(stdout.Trim())
            {
                RedirectStandardOutput = true
            };
        }
    }
}