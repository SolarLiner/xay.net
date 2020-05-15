using System;
using Dung.Lib;

namespace Dung.Plugin
{
    public interface IBuildPlugin : IPlugin
    {
        Project? DetectProject(string root, string sourceDirRelative, string buildDirRelative);
    }
}
