using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;
using Dung.Ninja.Objects;

namespace Dung.Ninja
{
    public struct NinjaFile
    {
        public Dictionary<string, string>? Variables { get; set; }
        public HashSet<Rule> Rules { get; set; }
        public HashSet<Build> Builds { get; set; }
        public string? Default { get; set; }

        public void Write([NotNull] StreamWriter writer)
        {
            writer.Write(ToString());
        }

        public void Write(string filename)
        {
            using var f = File.CreateText(filename);
            Write(f);
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine("# This file was generated automatically.\n# Do not edit, changes will be discarded.");
            sb.AppendLine();
            sb.AppendLine("# Global variables");
            if (Variables != null)
                foreach (KeyValuePair<string, string> pair in Variables)
                    sb.AppendLine($"${pair.Key} = {pair.Value}");

            sb.AppendLine();
            sb.AppendLine("# Rules");
            foreach (Rule rule in Rules) sb.AppendLine(rule.ToString());

            sb.AppendLine();
            sb.AppendLine("# Build artifacts");
            foreach (Build build in Builds) sb.AppendLine(build.ToString());

            sb.AppendLine();
            if (Default != null) sb.AppendLine($"default {Default}");

            return sb.ToString();
        }
    }
}