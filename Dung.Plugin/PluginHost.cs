using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Serilog;

namespace Dung.Plugin
{
    /// <summary>
    ///     Plugin host for <see cref="IPlugin" /> instance and derived interfaces.
    /// </summary>
    /// <typeparam name="TPlugin">Derived plugin interface</typeparam>
    public static class PluginHost<TPlugin> where TPlugin : class, IPlugin
    {
        /// <summary>
        ///     Load plugins across all plugin paths.
        /// </summary>
        /// <returns>Enumerable instance of all found plugins.</returns>
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

        private static IEnumerable<string> GetPluginsInDirectory(string dir)
        {
            List<string> files = Directory.GetFiles(dir, "*.dll", SearchOption.AllDirectories).ToList();
            Log.Debug("Searching in {@string}:", dir);
            foreach (string file in files) Log.Debug("\t{@string}", file);
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
            if (variables.ContainsKey("XDG_DATA_HOME"))
                directories.Add(Path.Join(variables["XDG_DATA_HOME"], "dung", "plugins"));

            if (variables.ContainsKey("XDG_DATA_DIRS"))
                directories.AddRange(variables["XDG_DATA_DIRS"].Split(":")
                    .Select(path => Path.Join(path, "dung", "plugins")));

            if (variables.ContainsKey("DUNG_CLI_PLUGIN_PATH"))
                directories.AddRange(variables["DUNG_CLI_PLUGIN_PATH"].Split(":"));

            return directories.Where(Directory.Exists);
        }

        private static Assembly LoadPlugin(string path)
        {
            var loadContext = new PluginLoadContext(path);
            var assemblyName = new AssemblyName(Path.GetFileNameWithoutExtension(path));
            Log.Debug("Loading plugin {@string}", assemblyName.FullName);
            return loadContext.LoadFromAssemblyName(assemblyName);
        }
    }
}