using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Dung.Ninja.Objects;

namespace Dung.Ninja
{
    public class NinjaSyntax
    {
        private readonly StreamWriter _writer;

        public NinjaSyntax(string filename) : this(File.Create(filename))
        {
        }

        public NinjaSyntax(Stream stream) : this(new StreamWriter(stream) {AutoFlush = true})
        {
        }

        public NinjaSyntax(StreamWriter writer)
        {
            _writer = writer;
        }

        public void Newline()
        {
            _writer.WriteLine();
        }

        public void Comment(string comment)
        {
            foreach (string line in comment.Split('\n').SelectMany(s => WrapLine(s, 78)))
                _writer.WriteLine($"# {line}");
        }

        public void Variable(string name, string contents, uint indent = 0)
        {
            for (var i = 0; i < indent; i++) _writer.Write(' ');

            _writer.WriteLine($"{name} = {contents}");
        }

        public void Variable(string name, IEnumerable<string> contents, uint indent = 0)
        {
            Variable(name, string.Join(' ', contents), indent);
        }

        public void Pool(string name, uint depth)
        {
            _writer.WriteLine($"pool {name}");
            Variable("depth", $"{depth}", 1);
        }

        public void Rule(Rule rule)
        {
            _writer.WriteLine($"rule {rule.Name}");
            Variable("command", rule.Command, 1);
            if (rule.Description != null)
                Variable("description", rule.Description, 1);
            if (rule.Depfile != null)
                Variable("depfile", rule.Depfile, 1);
        }

        public void Build(Build build)
        {
            string outputs = string.Join(' ', build.Outputs.Select(EscapePath));
            string inputs = string.Join(' ', build.Inputs.Select(EscapePath));
            _writer.WriteLine($"build {outputs}: {build.Rule.Name} {inputs}");
        }

        public void Include(string path)
        {
            _writer.WriteLine($"include {path}");
        }

        public void Subninja(string path)
        {
            _writer.WriteLine($"subninja {path}");
        }

        public void Default(Build b)
        {
            _writer.WriteLine($"default {b.Outputs[0]}");
        }

        private static int CountDollarsBeforeIndex(string s, int i)
        {
            var dollarCount = 0;
            int dollarIndex = i - 1;
            while (dollarIndex > 0 && s[dollarIndex] == '$')
            {
                dollarCount++;
                dollarIndex--;
            }

            return dollarCount;
        }

        private static string Escape(string input)
        {
            return input.Replace("$", "$$");
        }

        private static string EscapePath(string path)
        {
            return path.Replace("$ ",
                    "$$ ")
                .Replace(" ",
                    "$ ")
                .Replace(":",
                    "$:");
        }

        private static IEnumerable<string> WrapLine(string line, uint width)
        {
            var lines = new List<string>();
            var sb = new StringBuilder();
            foreach (string word in line.Split(' ', StringSplitOptions.RemoveEmptyEntries))
            {
                if (sb.Length + word.Length + 1 > width)
                {
                    lines.Add(sb.ToString());
                    sb.Clear();
                    continue;
                }

                if (sb.Length > 0) sb.Append(" ");
                sb.Append(word);
            }

            lines.Add(sb.ToString());
            return lines;
        }
    }
}