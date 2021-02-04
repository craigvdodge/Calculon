var target = Argument("target", "Build");
if (HasArgument("full"))
{
    target = Argument("target", "Publish");
}
var configuration = Argument("config", "Release");
var projects = Argument("projects", "lib;cl;ut");
string[] cleanDirs = {"obj", "bin"};
string runtimes = Argument("rt", "win-x64;linux-x64;osx-x64");

// TASKS
Task("Clean")
    .WithCriteria(c => (HasArgument("rebuild") || HasArgument("full")))
    .DoesForEach(projects.Split(';'), (project) =>
    {
        foreach(string d in cleanDirs)
        {
            string dirstring = "./" + project + "/" + d + $"/{configuration}";
            Information($"Cleaning {dirstring}");
            CleanDirectory(dirstring);
        }
    });

// Only build ut on full because Publish builds again anyway
Task("Build")
    .IsDependentOn("Clean")
    .Does(() =>
    {
        if (!HasArgument("full"))
        {
            DotNetCoreBuild("./calculon.sln", 
                new DotNetCoreBuildSettings
            {
                Configuration = configuration,
            });
        }
        else
        {
            Information("Full build, only building UT in this target");
            DotNetCoreBuild("./ut/ut.csproj" , new DotNetCoreBuildSettings
            {
                Configuration = configuration,
            });
        }
    });

Task("Test")
    .IsDependentOn("Build")
    .WithCriteria(c => HasArgument("full"))
    .Does(() =>
{
    DotNetCoreTest("./calculon.sln", new DotNetCoreTestSettings
    {
        Configuration = configuration,
        NoBuild = true,
    });
});

Task("Publish")
    .IsDependentOn("Test")
    .WithCriteria(c => HasArgument("full"))
    .DoesForEach(runtimes.Split(';'), (target) =>
    {
        Information("Publishing " + target);
        // Ready to run only supported on Windows
        bool readyToRun = false;
        if (target.StartsWith("win"))
        {
            readyToRun = true;
        }
        DotNetCorePublish("./cl/calculoncl.csproj", new DotNetCorePublishSettings
        {
            Runtime = target,
            Configuration = configuration,
            SelfContained = true,
            PublishReadyToRun = readyToRun,
        });
});

// EXECUTION
RunTarget(target);