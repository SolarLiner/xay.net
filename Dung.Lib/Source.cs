using System.Collections.Generic;

namespace Dung.Lib
{
    public class Source : IDependency
    {
        public Source(string filePath)
        {
            FilePath = filePath;
        }

        public string FilePath { get; }
        public string Name => FilePath;
        public IEnumerable<IDependency>? Dependencies => null;

        public IEnumerable<IDependency> FlattenDependencies()
        {
            return new List<IDependency>();
        }
    }
}