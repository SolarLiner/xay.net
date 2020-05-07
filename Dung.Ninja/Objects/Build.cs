using System;
using System.Collections.Generic;

namespace Dung.Ninja.Objects
{
    public struct Build : IEquatable<Build>
    {
        public class Comparer : IEqualityComparer<Build>
        {
            public bool Equals(Build x, Build y)
            {
                return x.Equals(y);
            }

            public int GetHashCode(Build obj)
            {
                return obj.GetHashCode();
            }
        }

        public Rule Rule { get; set; }

        public IEnumerable<string> Inputs { get; set; }

        public string[] Outputs { get; set; }

        public override string ToString()
        {
            return $"build {string.Join(' ', Outputs)}: {Rule.Name} {string.Join(' ', Inputs)}";
        }

        public bool Equals(Build other)
        {
            return Rule.Equals(other.Rule) && Outputs.Equals(other.Outputs);
        }

        public override bool Equals(object? obj)
        {
            return obj is Build other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Rule.GetHashCode() * 397) ^ Outputs.GetHashCode();
            }
        }
    }
}