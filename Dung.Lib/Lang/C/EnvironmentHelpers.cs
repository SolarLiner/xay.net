using System;

namespace Dung.Lib.Lang.C
{
    internal static class EnvironmentHelpers
    {
        internal static string GetCompiler()
        {
            return Environment.GetEnvironmentVariable("CC") ?? "gcc";
        }
    }
}