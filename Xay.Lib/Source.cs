using System.Collections.Generic;

namespace Xay.Lib
{
    /// <summary>
    ///     A source instance is a specialized dependency that exists on disk.
    /// </summary>
    public class Source : IDependency
    {
        /// <summary>
        ///     Create a new instance of a source file with the given filepath as argument.
        /// </summary>
        /// <param name="filePath">Path to the source file.</param>
        public Source(string filePath)
        {
            FilePath = filePath;
        }

        /// <summary>
        ///     Path to the source.
        /// </summary>
        public string FilePath { get; }

        /// <inheritdoc />
        public string Name => FilePath;

        /// <inheritdoc />
        public IEnumerable<IDependency>? Dependencies => null;

        /// <inheritdoc />
        public IEnumerable<IDependency> FlattenDependencies()
        {
            return new List<IDependency>();
        }
    }
}