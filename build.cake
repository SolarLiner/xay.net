#addin nuget:?package=Cake.MkDocs&version=2.1.1
#tool "nuget:?package=GitVersion.CommandLine"

var target = Argument("Target", "Build");

Task("Clean").Does(() => DotNetCoreClean("dung.sln"));

var versiontask = Task("UpdateAssemblyInfo").Does(() => Information("Stub about version tracking"));

var docstask = Task("Docs").IsDependentOn(versiontask).Does(() => MkDocsBuild());

var restoretask = Task("Restore").Does(() => DotNetCoreRestore());

var buildTask = Task("Build")
    .IsDependentOn(versiontask)
    .IsDependentOn(restoretask)
    .Does(() => DotNetCoreBuild("dung.sln"));

RunTarget(target);