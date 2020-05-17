#load nuget:?package=Cake.MkDocs&version=2.1.1

var target = Argument("Target", "Build");

Task("Clean").Does(() => DotNetCoreClean());

var restoretask = Task("Restore").Does(() => DotNetCoreRestore());

var buildTask = Task("Build")
    .IsDependentOn(restoretask)
    .Does(() => DotNetCoreBuild("dung.sln"));

RunTarget(target);