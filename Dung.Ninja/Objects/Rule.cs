using System;
using System.Collections.Generic;
using System.Text;

namespace Dung.Ninja.Objects
{
    public struct Rule : IEquatable<Rule>
    {
        public class Comparer : IEqualityComparer<Rule>
        {
            public bool Equals(Rule x, Rule y)
            {
                return x.Equals(y);
            }

            public int GetHashCode(Rule obj)
            {
                return obj.GetHashCode();
            }
        }

        public string Name { get; set; }
        public string Command { get; set; }
        public string? Description { get; set; }
        public string? Depfile { get; set; }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"rule {Name}\n command = {Command}");
            if (Description != null)
                sb.AppendLine($" description = {Description}");
            return sb.ToString();
        }

        public bool Equals(Rule other)
        {
            return string.Equals(Name,
                other.Name,
                StringComparison.InvariantCulture);
        }

        public override bool Equals(object? obj)
        {
            return obj is Rule other && Equals(other);
        }

        public override int GetHashCode()
        {
            return StringComparer.InvariantCulture.GetHashCode(Name);
        }
    }
}