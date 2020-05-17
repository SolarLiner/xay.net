using System.Collections.Generic;

namespace Dung.Lib
{
    /// <summary>
    ///     Generic dependency interface. This serves to build the project dependency tree into a Ninja file.
    /// </summary>
    public interface IDependency
    {
        /// <summary>
        ///     Name of the dependency, which will be used as the name of the build in the ninja file
        /// </summary>
        public string Name { get; }

        /// <summary>
        ///     Dependency objects this dependency needs to be built.
        /// </summary>
        public IEnumerable<IDependency>? Dependencies { get; }

        /// <summary>
        ///     Flatten dependencies into a single enumerable
        /// </summary>
        /// <returns></returns>
        public IEnumerable<IDependency> FlattenDependencies();
    }
}