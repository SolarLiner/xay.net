using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Xay.Ninja.Objects;

namespace Xay.Ninja
{
    /// <summary>
    ///     Helper class for generating `build.ninja` files.
    /// </summary>
    public class NinjaSyntax
    {
        private readonly StreamWriter _writer;

        /// <summary>
        ///     Instantiate the class, writing into the file at the given path.
        /// </summary>
        /// <param name="filename">Path to `build.ninja`.</param>
        public NinjaSyntax(string filename) : this(File.Create(filename))
        {
        }

        /// <summary>
        ///     Instantiate the class, writing into the given <see cref="Stream" />.
        /// </summary>
        /// <param name="stream">Stream on which to write the file to.</param>
        public NinjaSyntax(Stream stream) : this(new StreamWriter(stream) {AutoFlush = true})
        {
        }

        /// <summary>
        ///     Instantiate the class, writing into the given <see cref="StreamWriter" />.
        /// </summary>
        /// <param name="writer">Writer class to write the file contents into.</param>
        public NinjaSyntax(StreamWriter writer)
        {
            _writer = writer;
        }

        /// <summary>
        ///     Writes a new-line character into the stream.
        /// </summary>
        public void Newline()
        {
            _writer.WriteLine();
        }

        /// <summary>
        ///     Adds a line-wrapped comment into the stream.
        /// </summary>
        /// <param name="comment">Contents of the comment.</param>
        public void Comment(string comment)
        {
            foreach (string line in comment.Split('\n').SelectMany(s => WrapLine(s, 78)))
                _writer.WriteLine($"# {line}");
        }

        /// <summary>
        ///     Adds an optionally space-indented variable into the stream.
        /// </summary>
        /// <param name="name">Variable identifier.</param>
        /// <param name="contents">Value of the variable.</param>
        /// <param name="indent">Indentation level (default: 0)</param>
        public void Variable(string name, string contents, uint indent = 0)
        {
            for (var i = 0; i < indent; i++) _writer.Write(' ');

            _writer.WriteLine($"{name} = {contents}");
        }

        /// <summary>
        ///     Adds an optionally space-indented variable into the stream.
        /// </summary>
        /// <param name="name">Variable identifier.</param>
        /// <param name="contents">Variable values.</param>
        /// <param name="indent">Indentation level (default: 0)</param>
        public void Variable(string name, IEnumerable<string> contents, uint indent = 0)
        {
            Variable(name, string.Join(' ', contents), indent);
        }

        /// <summary>
        ///     Adds a stdio pool of the given depth. into the stream.
        /// </summary>
        /// <param name="name">Name of the pool.</param>
        /// <param name="depth">Depth of the pool.</param>
        public void Pool(string name, uint depth)
        {
            _writer.WriteLine($"pool {name}");
            Variable("depth", $"{depth}", 1);
        }

        /// <summary>
        ///     Adds a build rule into the stream.
        /// </summary>
        /// <param name="rule">Rule to add.</param>
        public void Rule(Rule rule)
        {
            _writer.WriteLine($"rule {rule.Name}");
            Variable("command", rule.Command, 1);
            if (rule.Description != null)
                Variable("description", rule.Description, 1);
            if (rule.Depfile != null)
                Variable("depfile", rule.Depfile, 1);
        }

        /// <summary>
        ///     Adds a build artifact into the stream.
        /// </summary>
        /// <param name="build">Build artifact to add.</param>
        public void Build(Build build)
        {
            string outputs = string.Join(' ', build.Outputs.Select(EscapePath));
            string inputs = string.Join(' ', build.Inputs.Select(EscapePath));
            _writer.WriteLine($"build {outputs}: {build.Rule.Name} {inputs}");
        }

        /// <summary>
        ///     Adds an include instruction into the stream.
        /// </summary>
        /// <param name="path">Path to the included `build.ninja`</param>
        public void Include(string path)
        {
            _writer.WriteLine($"include {path}");
        }

        /// <summary>
        ///     Adds a subninja instruction into the stream.
        /// </summary>
        /// <param name="path">Path to the subninja file.</param>
        public void Subninja(string path)
        {
            _writer.WriteLine($"subninja {path}");
        }

        /// <summary>
        ///     Adds a default artifact to build into the stream.
        /// </summary>
        /// <param name="b">Build artifact to build by default.</param>
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