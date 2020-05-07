using System.Collections.Generic;
using Dung.Ninja.Objects;

namespace Dung.Ninja
{
    public interface IObject
    {
        public IEnumerable<Build> Builds(string buildDir);
    }
}