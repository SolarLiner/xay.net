using System.Collections.Generic;

namespace Dung.Lib
{
    public interface IDependency
    {
        public string Name { get; }
        public IEnumerable<IDependency>? Dependencies { get; }

        public IEnumerable<IDependency> FlattenDependencies();
    }
}