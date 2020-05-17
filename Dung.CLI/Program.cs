using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.ExceptionServices;
using Dung.Lib;
using Dung.Plugin;
using Serilog;
using Serilog.Events;

namespace Dung.CLI
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            string tempLogFile = SetupLogging();
            string cwd = Environment.CurrentDirectory;
            Project? project = PluginHost<IBuildPlugin>.LoadPlugins()
                .Select(plugin => plugin.DetectProject(cwd, "src", "build"))
                .FirstOrDefault(proj => proj != null);
            if (project == null)
            {
                Log.Error($"Couldn't detect a project in the directory {cwd}.");
                Log.Error("Make sure you are in the right directory to call this command.");
                FinalizeLog();
                Console.WriteLine($"Log file written to {tempLogFile}");
            }
            else
            {
                project.WriteNinja();
                string destLogFile = Path.Combine(project.BuildDir, "dung.log");
                FinalizeLog();
                if (File.Exists(destLogFile)) File.Delete(destLogFile);
                File.Copy(tempLogFile, destLogFile);
            }
        }

        private static void FinalizeLog()
        {
            Log.CloseAndFlush();
        }

        private static string SetupLogging()
        {
            string tempFile = Path.GetTempFileName();
            AssemblyName name = typeof(Program).Assembly.GetName();
            var log = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .Enrich.WithProperty("Assembly Name", name.Name)
                .Enrich.WithProperty("Version", name.Version?.ToString() ?? "<unknown>")
                .WriteTo.Console(LogEventLevel.Information, "{Level:u1}: {Message:l}{NewLine}{Exception}")
                .WriteTo.File(tempFile)
                .CreateLogger();
            log.Information($"Using {name.Name} v{name.Version?.ToString() ?? "<unknown>"}");
            Log.Logger = log;
            AppDomain.CurrentDomain.FirstChanceException += OnFirstChangeException;
            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
            return tempFile;
        }

        private static void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject is Exception ex)
            {
                if (e.IsTerminating) Log.Fatal(ex, "Fatal: Unknown exception, aborting");
                else Log.Error("Unknown exception");
            }
            else
            {
                if (e.IsTerminating) Log.Fatal("Fatal: Unknown exception: {@string}", e.ExceptionObject.ToString());
                else Log.Error("Unknown exception: {@string}", e.ExceptionObject.ToString());
            }
        }

        private static void OnFirstChangeException(object? sender, FirstChanceExceptionEventArgs e)
        {
            Log.Error(e.Exception, "First chance exception");
        }
    }
}