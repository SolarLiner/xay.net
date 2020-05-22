#addin nuget:?package=Cake.MkDocs&version=2.1.1
#addin nuget:?package=SharpZipLib
#addin nuget:?package=Cake.Compression
#addin nuget:?package=Cake.FileHelpers
using System.Linq;
using Path = Cake.Core.IO.Path;

var name = "xây";
var nameAscii = "xay";
var target = Argument("Target", "Build");
var _p = Argument("Prefix", "/usr");
var prefix = Directory(_p.StartsWith("/") ? _p.Substring(1) : _p);
var slnFile = File("./xay.sln");
var version = "unreleased";
var publishDir = Directory("./publish");

var docsDir = Directory("./docs");
var apiDocsDir = docsDir + Directory("api");

var cleantask = Task("Clean")
    .Does(() => {
        if(DirectoryExists(publishDir)) DeleteDirectory(publishDir, recursive: true);
        if(DirectoryExists("./site")) DeleteDirectory("./site", recursive: true);
        DotNetCoreClean(slnFile);
    });

var versiontask = Task("Version")
    .ContinueOnError()
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
    .Does(() => DotNetCoreBuild("xay.sln", new DotNetCoreBuildSettings {
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

var changelogTask = Task("Changelog")
    .IsDependentOn(Task("CreateChangelog")
        .WithCriteria(() => StartProcess("git", new ProcessSettings {
            Arguments = "describe --exact-match --tags"
        }) == 0)
        .Does(() => StartProcess("standard-changelog")))
    .ContinueOnError()
    .Does(() => {
        CopyFile(File("./CHANGELOG.md"), docsDir + File("changelog.md"));
    });

var docstask = Task("Docs")
    .IsDependentOn(apidocstask)
    .IsDependentOn(changelogTask)
    .Does(() => MkDocsBuild());

var publishPluginsTask = Task("PublishPlugins")
    .IsDependentOn(buildtask)
    .DoesForEach(GetFiles("Dung.Plugins.*/*.csproj"), proj => DotNetCorePublish(proj.FullPath, new DotNetCorePublishSettings {
        Configuration = "Release",
        OutputDirectory = IsRunningOnUnix() ? publishDir + prefix + Directory($"share/{nameAscii}/plugins") : publishDir + Directory("plugins"),
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
        if(IsRunningOnUnix()) {
            var publishPrefix = publishDir + prefix;
            var prefixShare = prefix + Directory($"share/{nameAscii}");
            var publishShare = publishDir + prefixShare;
            var publishBin = publishPrefix + Directory("bin");
            var binWrapperFile = publishBin + File(nameAscii);
            DotNetCorePublish("Dung.CLI/Dung.CLI.csproj", new DotNetCorePublishSettings {
                Configuration = "Release",
                OutputDirectory = publishShare,
                NoBuild = true,
                NoRestore = true,
            });
            EnsureDirectoryExists(publishDir + prefix + Directory("bin"));
            FileWriteLines(binWrapperFile, new string[] {
                "#!/usr/bin/env bash",
                $"cd /{prefixShare}",
                $"exec ./Dung.CLI $@"
            });
            StartProcess("chmod", new ProcessSettings { Arguments = $"+x {binWrapperFile}"});
        } else {
            DotNetCorePublish("Dung.CLI/Dung.CLI.csproj", new DotNetCorePublishSettings {
                Configuration = "Release",
                OutputDirectory = publishDir,
                NoBuild = true,
                NoRestore = true
            });
        }
    });

Task("DocsServe")
    .IsDependentOn(apidocstask)
    .IsDependentOn(changelogTask)
    .Does(() => MkDocsServe());

Task("Rebuild")
    .IsDependentOn(cleantask)
    .IsDependentOn(buildtask);

Task("Zip")
    .IsDependentOn(Task("ZipClean")
        .DoesForEach(GetFiles("*.zip"), file => DeleteFile(file)))
    .IsDependentOn(publishtask)
    .Does(() => {
        var suffix = IsRunningOnUnix() ? "unix" : "win";
        var filename = File($"{name}-{version}-{suffix}.zip");
        ZipCompress(publishDir, filename);
    });

RunTarget(target);
