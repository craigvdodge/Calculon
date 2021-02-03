var configuration = Argument("configuration", "Release");
string[] proj = {"lib", "cl", "ut"};
string[] cleanDirs = {"obj", "bin"};

//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////

Task("Clean")
    .WithCriteria(c => (HasArgument("rebuild") || HasArgument("full")))
    .Does(() =>
{
    foreach (string p in proj)
    {
        foreach(string d in cleanDirs)
        {
            string dirstring = "./" + p + "/" + d + $"/{configuration}";
            CleanDirectory(dirstring);
        }
    }
});

Task("Build")
    .IsDependentOn("Clean")
    .Does(() =>
{
    DotNetCoreBuild("./calculon.sln", new DotNetCoreBuildSettings
    {
        Configuration = configuration,
    });
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
    .Does(() =>
{
    DotNetCorePublish("./calculon.sln");
});
//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

if (HasArgument("full")) 
{
    RunTarget("Publish");
}
else
{
    RunTarget("Build");
}