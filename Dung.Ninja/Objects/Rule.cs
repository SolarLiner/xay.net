using System;
using System.Collections.Generic;
using System.Text;

namespace Dung.Ninja.Objects
{
    /// <summary>
    ///     Representation of a Ninja rule
    /// </summary>
    public struct Rule : IEquatable<Rule>
    {
        /// <summary>
        ///     Comparer class for Rule objects. Rules are equal if their names are equal.
        /// </summary>
        public class Comparer : IEqualityComparer<Rule>
        {
            /// <inheritdoc />
            public bool Equals(Rule x, Rule y)
            {
                return x.Equals(y);
            }

            /// <inheritdoc />
            public int GetHashCode(Rule obj)
            {
                return obj.GetHashCode();
            }
        }

        /// <summary>
        ///     Name of the rule.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     Command used to generate build artifacts.
        /// </summary>
        public string Command { get; set; }

        /// <summary>
        ///     Optional description shown during build.
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        ///     Makefile generated during the build for discovery of order dependencies.
        /// </summary>
        public string? Depfile { get; set; }

        /// <inheritdoc />
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"rule {Name}\n command = {Command}");
            if (Description != null)
                sb.AppendLine($" description = {Description}");
            return sb.ToString();
        }

        /// <inheritdoc />
        public bool Equals(Rule other)
        {
            return string.Equals(Name,
                other.Name,
                StringComparison.InvariantCulture);
        }

        /// <inheritdoc />
        public override bool Equals(object? obj)
        {
            return obj is Rule other && Equals(other);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return StringComparer.InvariantCulture.GetHashCode(Name);
        }
    }
}