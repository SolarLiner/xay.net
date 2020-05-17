using System.Collections.Generic;
using System.Linq;
using Dung.Ninja.Objects;

namespace Dung.Lib
{
    /// <summary>
    ///     A buildable instance is a specialized dependency that uses a rule to be built.
    /// </summary>
    public interface IBuildable : IDependency
    {
        /// <summary>
        ///     The rule this buildable object needs in order to be built.
        /// </summary>
        public Rule Rule { get; }

        /// <summary>
        ///     Flattened list of rules used to build dependencies of this buildable object
        /// </summary>
        public HashSet<Rule> RulesUsed =>
            Dependencies.Where(d => d is IBuildable)
                .SelectMany(b => ((IBuildable) b).RulesUsed)
                .ToHashSet(new Rule.Comparer());

        /// <summary>
        ///     Get the Ninja build object needed to build this instance.
        /// </summary>
        /// <returns></returns>
        public Build GetBuild()
        {
            return new Build
            {
                Inputs = Dependencies.Select(d => d.Name),
                Outputs = new[] {Name},
                Rule = Rule
            };
        }
    }
}