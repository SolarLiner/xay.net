using System;

namespace Dung.Plugins.C
{
    internal static class EnvironmentHelpers
    {
        internal static string GetCompiler()
        {
            return Environment.GetEnvironmentVariable("CC") ?? "gcc";
        }
    }
}