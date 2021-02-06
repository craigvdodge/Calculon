string target = Argument("target", "Build");
if (HasArgument("full"))
{
    target = Argument("target", "Zip");
}
string configuration = Argument("config", "Release");
string projects = Argument("projects", "lib;cl;ut");
string[] cleanDirs = {"obj", "bin"};
string runtimes = Argument("rt", "win-x64;linux-x64;osx-x64");
string framework = Argument("framework", "net5.0");

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
        DotNetCorePublish("./cl/calculoncl.csproj", new DotNetCorePublishSettings
        {
            Runtime = target,
            Configuration = configuration,
            SelfContained = true,
        });
});

Task("Zip")
    .IsDependentOn("Publish")
    .WithCriteria(c => HasArgument("full"))
    .DoesForEach(runtimes.Split(';'), (target) =>
    {
        string src = $"./cl/bin/{configuration}/{framework}/{target}/publish";
        string zipfile = $"./cl/bin/{configuration}/calculon_{target}.zip";
        Information("Creating " + zipfile);
        System.IO.Compression.ZipFile.CreateFromDirectory(src, zipfile); 
});

// EXECUTION
RunTarget(target);