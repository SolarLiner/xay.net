using System;
using Dung.Lib.Lang.C;

namespace Dung.CLI
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            string cwd = Environment.CurrentDirectory;
            CProject? project = CProject.DetectProject(cwd);
            if (project == null)
            {
                Console.WriteLine($"Couldn't detect a project in the directory {cwd}.");
                Console.WriteLine("Make sure you are in the right directory to call this command.");
            }
            else
            {
                project.WriteNinja();
            }
        }
    }
}