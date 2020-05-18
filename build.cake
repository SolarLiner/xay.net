#addin nuget:?package=Cake.MkDocs&version=2.1.1

using System.Linq;
using Path = Cake.Core.IO.Path;

var target = Argument("Target", "Build");
var slnFile = File("./dung.sln");
var version = "unreleased";

var docsDir = Directory("./docs");
var apiDocsDir = docsDir + Directory("api");

Task("Clean")
    .Does(() => {
        DeleteFile(File("./SolutionInfo.cs"));
        DotNetCoreClean(slnFile);
    });

var versiontask = Task("Version")
    .Does(() => {
        using (var process = StartAndReturnProcess("git", new ProcessSettings {
            Arguments = "describe --tags",
            RedirectStandardOutput = true
        })) {
            process.WaitForExit();
            version = process.GetStandardOutput().First();
            Information($"Git version: {version}");
        }
    });

var restoretask = Task("Restore")
    .Does(() => DotNetCoreRestore());

var buildtask = Task("Build")
    .IsDependentOn(versiontask)
    .IsDependentOn(restoretask)
    .Does(() => DotNetCoreBuild("dung.sln", new DotNetCoreBuildSettings { NoRestore = true }));

var apidocstask = Task("DocsApi")
    .IsDependentOn(buildtask)
    .IsDependentOn(Task("EnsureApiDocsDirExists")
        .Does(() => EnsureDirectoryExists(apiDocsDir))
    .IsDependentOn(Task("CleanApiDocsDir")
        .Does(() => CleanDirectory(apiDocsDir)))
    .DoesForEach(GetFiles("**/obj/*.md"), file => {
        var dest = apiDocsDir + file.GetFilename();
        Information($"Copying {file.FullPath} -> {(string)dest}");
        if(!IsDryRun()) CopyFile(file, dest);
    }));

var docstask = Task("Docs")
    .IsDependentOn(apidocstask)
    .Does(() => MkDocsBuild());

Task("DocsServe").IsDependentOn(apidocstask).Does(() => MkDocsServe());

Task("Rebuild").IsDependentOn("Clean").IsDependentOn(buildtask);

RunTarget(target);