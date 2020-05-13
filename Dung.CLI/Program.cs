using System;
using System.Reflection;
using Dung.Lib.Lang.C;
using Serilog;
using Serilog.Events;

namespace Dung.CLI
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            SetupLogging();
            string cwd = Environment.CurrentDirectory;
            CProject? project = CProject.DetectProject(cwd);
            if (project == null)
            {
                Console.WriteLine($"Couldn't detect a project in the directory {cwd}.");
                Console.WriteLine("Make sure you are in the right directory to call this command.");
            }
            else
            {
                project.WriteNinja();
            }

            FinalizeLog();
        }

        private static void FinalizeLog()
        {
            Log.CloseAndFlush();
        }

        private static void SetupLogging()
        {
            AssemblyName name = typeof(Program).Assembly.GetName();
            var log = new LoggerConfiguration()
                .WriteTo.Console(LogEventLevel.Information)
                .Enrich.WithProperty("Assembly Name", name.Name)
                .Enrich.WithProperty("Version", name.Version?.ToString() ?? "<unknown>")
                .CreateLogger();
            log.Information($"Using {name.Name} v{name.Version?.ToString() ?? "<unknown>"}");
            Log.Logger = log;
        }
    }
}