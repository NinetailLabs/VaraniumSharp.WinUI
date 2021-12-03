/*
 * Execute NUnit tests.
 * Script uses AltCover to calculate test coverage.
 * To work correctly the AltCover and Appveyor.TestLogger nuget packages must be added to the test project.
 * The test output is geared entirely towards AppVeyor, however there are packages available that can also output NUnit3 xml
 */

#region Variables

// Indicate if the unit tests passed
var testPassed = false;
// Directory where coverage results should be generated
var coverDirectory = "./coverage/";
// Name of the coverage file for the final result
var coverFilename = "coverage.xml";
// Path to the final coverage file
var coverPath = $"{coverDirectory}{coverFilename}";
// The name of the temporary file where coverage results will be combined
var sharedCoverageFile = "coverage.json";
// Filter used to locate unit test dlls
var unitTestFilter = "./*Tests/bin/Release/net*/*.Tests.dll";
// Collection of namespaces to exclude
var excludedNamespaces = new List<string> { "[Microsoft.*]*" };
// Collection of namespaces to include
var includedNamespaces = new List<string>();

#endregion

#region Tasks

// Execute unit tests
Task ("UnitTests")
    .Does (() =>
    {
        var blockText = "Unit Tests";
        StartBlock(blockText);

        RemoveCoverageResults();
        ExecuteUnitTests();

        EndBlock(blockText);
    });

Task ("FailBuildIfTestFailed")
    .Does(() => {
        var blockText = "Build Success Check";
        StartBlock(blockText);

        if(!testPassed)
        {
            throw new CakeException("Unit test have failed - Failing the build");
        }

        EndBlock(blockText);
    });

#endregion

#region Public Methods

// Add namespaces to the exclusion list
public void AddNamespaceExclusion(string namespaceToExclude)
{
    excludedNamespaces.Add(namespaceToExclude);
}

#endregion

#region Private Methods

// Delete the coverage results if it already exists
private void RemoveCoverageResults()
{
    CleanDirectories(coverDirectory);
}

// Execute NUnit tests
private void ExecuteUnitTests()
{
    testPassed = true;

    var testAssemblies = GetFiles(unitTestFilter);
    var totalAssemblies = testAssemblies.Count;
    var currentAssembly = 0;

    foreach(var assembly in testAssemblies)
    {
        try
        {
            var assemblyFilename = assembly.GetFilenameWithoutExtension();

            Information($"Testing: {assembly}");
            
            var toExclude = string.Join(",", excludedNamespaces);
            var toInclude = string.Join(",", includedNamespaces);

            var args = $"coverlet {assembly} --target \"dotnet\" --targetargs \"test {assembly}\" --format opencover --output {coverPath} --exclude \"{toExclude}\" --include \"{toInclude}\"";           
            DotNetTool(args);

        }
        catch(Exception ex)
        {
            testPassed = false;
            Error(ex.Message);
            Error("There was an error while executing tests");
        }
    }
}

#endregion