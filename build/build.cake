#tool "nuget:?package=GitVersion.CommandLine"
#tool "nuget:?package=xunit.runner.console"

//Arguments
var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");

var gitInfo = GitVersion();
var srcProjFiles = GetFiles("../src/**/*.csproj");
var testProjFiles = GetFiles("../test/**/*.csproj");
var isAllowedToPushArtifact = configuration == "Release" && gitInfo.BranchName == "master" && "true" == EnvironmentVariable("ALLOW_NUGET_PUSH");

Task("Restore")
    .DoesForEach(srcProjFiles, (file) => DotNetCoreRestore(file.FullPath))
    .DoesForEach(testProjFiles, (file) => DotNetCoreRestore(file.FullPath));

Task("Build")
    .DoesForEach(srcProjFiles, (file) => {
        var buildConfiguration = new DotNetCoreBuildSettings
        {
            Framework = "netstandard2.0",
            Configuration = configuration
        };

        DotNetCoreBuild(file.FullPath, buildConfiguration);
    });

Task("Tests")
    .DoesForEach(testProjFiles, (file) => DotNetCoreTest(file.FullPath));

Task("Package")
    .WithCriteria(() => isAllowedToPushArtifact)
    .Does(() =>
{
    Information("Building version number: " + gitInfo.SemVer);    

    var nuGetPackSettings = new NuGetPackSettings {
        Version = gitInfo.SemVer,
        OutputDirectory = "../out/artifacts/"
    };

    var nuspecFile = File("../src/OpenTracing.Contrib.Mongo/OpenTracing.Contrib.Mongo.csproj.nuspec");
    NuGetPack(nuspecFile, nuGetPackSettings);
});

Task("Push")
    .WithCriteria(() => isAllowedToPushArtifact)
    .Does(() =>
{
    var settings = new DotNetCoreNuGetPushSettings
    {
        Source = EnvironmentVariable("NUGET_API_SERVER"),
        ApiKey = EnvironmentVariable("NUGET_API_KEY")
    };

    var file = File("../out/artifacts/OpenTracing.Contrib.Mongo." + gitInfo.SemVer +".nupkg");
    DotNetCoreNuGetPush(file.Path.FullPath, settings);
});


Task("BuildAndTest")
    .IsDependentOn("Restore")
    .IsDependentOn("Build")
    .IsDependentOn("Tests");

Task("PackageAndPush")
    .IsDependentOn("Package")
    .IsDependentOn("Push");

Task("Default")
    .IsDependentOn("BuildAndTest")
    .IsDependentOn("PackageAndPush");

RunTarget(target);