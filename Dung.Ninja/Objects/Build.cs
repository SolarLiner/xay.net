using System;
using System.Collections.Generic;

namespace Dung.Ninja.Objects
{
    /// <summary>
    ///     Representation of a Ninja build artifact.
    /// </summary>
    public struct Build : IEquatable<Build>
    {
        /// <summary>
        ///     Comparer class for <see cref="Build" /> objects. Build objects are equal if their rule and inputs are equal.
        /// </summary>
        public class Comparer : IEqualityComparer<Build>
        {
            /// <inheritdoc />
            public bool Equals(Build x, Build y)
            {
                return x.Equals(y);
            }

            /// <inheritdoc />
            public int GetHashCode(Build obj)
            {
                return obj.GetHashCode();
            }
        }

        /// <summary>
        ///     Rule used to build this artifact.
        /// </summary>
        public Rule Rule { get; set; }

        /// <summary>
        ///     Build inputs of this artifact.
        /// </summary>
        public IEnumerable<string> Inputs { get; set; }

        /// <summary>
        ///     Ouputs generated from this build artifact.
        /// </summary>
        public string[] Outputs { get; set; }

        /// <summary>
        ///     Ninja representation of this build artifact.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"build {string.Join(' ', Outputs)}: {Rule.Name} {string.Join(' ', Inputs)}";
        }


        /// <inheritdoc />
        public bool Equals(Build other)
        {
            return Rule.Equals(other.Rule) && Outputs.Equals(other.Outputs);
        }

        /// <inheritdoc />
        public override bool Equals(object? obj)
        {
            return obj is Build other && Equals(other);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            unchecked
            {
                return (Rule.GetHashCode() * 397) ^ Outputs.GetHashCode();
            }
        }
    }
}