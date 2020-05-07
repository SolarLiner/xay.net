using System.Collections.Generic;
using System.Linq;
using Dung.Ninja.Objects;

namespace Dung.Lib
{
    public interface IBuildable : IDependency
    {
        public Rule Rule { get; }

        public HashSet<Rule> RulesUsed =>
            Dependencies.Where(d => d is IBuildable)
                .SelectMany(b => ((IBuildable) b).RulesUsed)
                .ToHashSet(new Rule.Comparer());

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