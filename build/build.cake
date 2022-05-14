#tool "nuget:?package=GitVersion.CommandLine&version=5.10.1"
#tool "nuget:?package=xunit.runner.console&version=2.4.1"

var gitInfo = GitVersion();
var target = Argument("target", "Push");
var configuration = Argument("configuration", "Release");

var isAllowedToPushArtifact = configuration == "Release" && gitInfo.BranchName == "master" && "true" == EnvironmentVariable("ALLOW_NUGET_PUSH");

Task("Clean")
    .WithCriteria(c => HasArgument("rebuild"))
    .Does(() =>
{
    CleanDirectory($"../src/OpenTracing.Contrib.Mongo/bin/{configuration}");
});

Task("Build")
    .IsDependentOn("Clean")
    .Does(() =>
{
    DotNetCoreBuild("../OpenTracing.Contrib.sln", new DotNetCoreBuildSettings
    {
        Configuration = configuration,
    });
});

Task("Test")
    .IsDependentOn("Build")
    .Does(() =>
{
    DotNetCoreTest("../OpenTracing.Contrib.sln", new DotNetCoreTestSettings
    {
        Configuration = configuration,
        NoBuild = true,
    });
});

Task("Package")
    .WithCriteria(() => isAllowedToPushArtifact)
    .IsDependentOn("Test")
    .Does(() => {
        Information("Packaging version: " + gitInfo.SemVer);
        var nuGetPackSettings = new NuGetPackSettings {
            Version = gitInfo.SemVer,
            OutputDirectory = "../out/artifacts/"
        };

        var nuspecFile = File("../src/OpenTracing.Contrib.Mongo/OpenTracing.Contrib.Mongo.csproj.nuspec");
        NuGetPack(nuspecFile, nuGetPackSettings);
    });

Task("Push")
    .WithCriteria(() => isAllowedToPushArtifact)
    .IsDependentOn("Package")
    .Does(() => {
        var settings = new DotNetCoreNuGetPushSettings
        {
            Source = EnvironmentVariable("NUGET_API_SERVER"),
            ApiKey = EnvironmentVariable("NUGET_API_KEY")
        };

        var file = File("../out/artifacts/OpenTracing.Contrib.Mongo." + gitInfo.SemVer +".nupkg");
        Information("Pushing to Nuget" + file);
        DotNetCoreNuGetPush(file.Path.FullPath, settings);
    });

RunTarget(target);
