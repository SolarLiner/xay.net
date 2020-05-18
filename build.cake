#addin nuget:?package=Cake.MkDocs&version=2.1.1
#addin nuget:?package=SharpZipLib
#addin nuget:?package=Cake.Compression
using System.Linq;
using Path = Cake.Core.IO.Path;

var name = "dựng";
var target = Argument("Target", "Build");
var slnFile = File("./dung.sln");
var version = "unreleased";
var publishDir = Directory("./publish");

var docsDir = Directory("./docs");
var apiDocsDir = docsDir + Directory("api");

var cleantask = Task("Clean")
    .Does(() => {
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
    .Does(() => DotNetCoreBuild("dung.sln", new DotNetCoreBuildSettings {
        Configuration = "Release",
        Runtime = EnvironmentVariable("RID"),
        NoIncremental = true,
        NoRestore = true
    }));

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

var publishPluginsTask = Task("PublishPlugins")
    .IsDependentOn(buildtask)
    .DoesForEach(GetFiles("Dung.Plugins.*/*.csproj"), proj => DotNetCorePublish(proj.FullPath, new DotNetCorePublishSettings {
        Configuration = "Release",
        OutputDirectory = publishDir + Directory("plugins"),
        NoBuild = true,
        NoRestore = true,
    }));

var publishtask = Task("Publish")
    .IsDependentOn(Task("PublishClean").Does(() => {
        if(DirectoryExists(publishDir)) DeleteDirectory(publishDir, recursive: true);
    }))
    .IsDependentOn(buildtask)
    .IsDependentOn(publishPluginsTask)
    .Does(() => {
        DotNetCorePublish("Dung.CLI/Dung.CLI.csproj", new DotNetCorePublishSettings {
            Configuration = "Release",
            OutputDirectory = publishDir,
            NoBuild = true,
            NoRestore = true,
        });
    });

var shelltask = Task("ShellWrapper")
    .IsDependeeOf("Publish")
    .Does(() => Information("Stub for shell wrapper"));

Task("DocsServe").IsDependentOn(apidocstask).Does(() => MkDocsServe());

Task("Rebuild")
    .IsDependentOn(cleantask)
    .IsDependentOn(buildtask);

Task("Zip")
    .IsDependentOn(Task("ZipClean")
        .DoesForEach(GetFiles("*.zip"), file => DeleteFile(file)))
    .IsDependentOn(publishtask)
    .Does(() => ZipCompress(publishDir, $"{name}-{version}.zip"));

RunTarget(target);