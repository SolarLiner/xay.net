using Dung.Lib;

namespace Dung.Plugin
{
    /// <summary>
    ///     Build plugins are plugins that allow dựng to setup a project. Typically there is one build plugin per language
    ///     and/or framework.
    /// </summary>
    public interface IBuildPlugin : IPlugin
    {
        /// <summary>
        ///     Detect a project in the root directory, and given the source relative directory. Returns a
        ///     <see cref="Project" /> instance using the source and build relative paths to setup the source and build
        ///     directories if sources for the
        ///     project are found, otherwise <code>null</code>.
        /// </summary>
        /// <param name="root">Root directory.</param>
        /// <param name="sourceDirRelative">Relative source directory.</param>
        /// <param name="buildDirRelative">Relative build directory.</param>
        /// <returns>A <see cref="Project" /> instance if a project has been found.</returns>
        Project? DetectProject(string root, string sourceDirRelative, string buildDirRelative);
    }
}