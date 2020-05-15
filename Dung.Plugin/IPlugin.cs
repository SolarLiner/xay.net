using System;
namespace Dung.Plugin
{
    public interface IPlugin
    {
        string Name { get; }
        string Version { get; }
        string Author { get; }
        bool Enabled => true;
    }
}
