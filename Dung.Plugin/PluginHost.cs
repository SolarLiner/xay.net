using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Serilog;

namespace Dung.Plugin
{
    public static class PluginHost<TPlugin> where TPlugin: class, IPlugin
    {
        public static IEnumerable<TPlugin> LoadPlugins()
        {
            List<TPlugin> plugins = GetPluginDirectories()
                .SelectMany(GetPluginsInDirectory)
                .SelectMany(path => LoadPlugin(path)
                    .GetTypes())
                .Where(type => typeof(TPlugin).IsAssignableFrom(type))
                .Select(assemblyType => Activator.CreateInstance(assemblyType) as TPlugin)
                .Where(plugin => plugin != null && plugin.Enabled)
                .OfType<TPlugin>()
                .ToList();
            Log.Information($"Loaded {plugins.Count} plugins.");
            return plugins;
        }

        private static IEnumerable<string> GetPluginsInDirectory(string arg)
        {
            List<string> files = Directory.GetFiles(arg, "*.dll", SearchOption.AllDirectories).ToList();
            Log.Debug($"Searching in {arg}:");
            foreach (string file in files)
            {
                Log.Debug($"\t{file}");
            }
            return files;
        }

        private static IEnumerable<string> GetPluginDirectories()
        {
            var directories = new List<string>();
            Dictionary<string, string> variables = Environment.GetEnvironmentVariables()
                .Cast<DictionaryEntry>()
                .Where(envVar => envVar.Value != null)
                .ToDictionary(environmentVariable => environmentVariable.Key.ToString()!,
                    environmentVariable => environmentVariable.Value!.ToString()!);
            if(variables.ContainsKey("XDG_DATA_HOME"))
            {
                directories.Add(Path.Join(variables["XDG_DATA_HOME"], "dung", "plugins"));
            }

            if (variables.ContainsKey("XDG_DATA_DIRS"))
            {
                directories.AddRange(variables["XDG_DATA_DIRS"].Split(":").Select(path => Path.Join(path, "dung", "plugins")));
            }

            if (variables.ContainsKey("DUNG_CLI_PLUGIN_PATH"))
            {
                directories.AddRange(variables["DUNG_CLI_PLUGIN_PATH"].Split(":"));
            }

            Log.Debug("Searching in these directories:");
            foreach (string directory in directories)
            {
                Log.Debug($"\t{directory}");
            }
            return directories.Where(Directory.Exists);
        }

        private static Assembly LoadPlugin(string path)
        {
            var loadContext = new PluginLoadContext(path);
            return loadContext.LoadFromAssemblyName(new AssemblyName(Path.GetFileNameWithoutExtension(path)));
        }
    }
}