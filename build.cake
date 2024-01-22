#region ScriptImports

// Scripts
#load "CakeScripts/base/base.buildsystem.cake"
#load "CakeScripts/base/base.variables.cake"
#load "CakeScripts/base/base.setup.cake"
#load "CakeScripts/base/base.nuget.restore.cake"
#load "CakeScripts/base/base.msbuild.cake"
#load "CakeScripts/base/base.coverlet.tool.cake"
#load "CakeScripts/base/base.coveralls.upload.cake"
#load "CakeScripts/base/base.gitreleasenotes.cake"
#load "custom.nuget.pack.cake"
#load "CakeScripts/base/base.nuget.push.cake"
#load "CakeScripts/base/base.docfx.cake"
#load "CakeScripts/base/base.sonarqube.cake"

#endregion

#region Tasks

// Set up variables specific for the project
Task ("VariableSetup")
	.Does(() => {
		projectName = "VaraniumSharp.WinUI";
		releaseFolderString = "./{0}/bin/{1}/net8.0-windows10.0.19041.0";
		releaseBinaryType = "dll";
		repoOwner = "NinetailLabs";
		botName = "NinetailLabsBot";
		botEmail = "gitbot@ninetaillabs.com";
		botToken = EnvironmentVariable("BotToken");
		gitRepo = string.Format("https://github.com/{0}/{1}.git", repoOwner, projectName);
		sonarQubeKey = "NinetailLabs_VaraniumSharp.WinUI";
		sonarBranch = branch;
		sonarOrganization = "ninetaillabs";
		sonarQubeServerUrl = "https://sonarcloud.io";
		sonarLogin = EnvironmentVariable("SonarToken");
		excludedFiles = new List<string> { "**/System.Text.Json.SourceGeneration/**/*.g.cs" };
		toolVersion = MSBuildToolVersion.VS2022;
	});

Task ("Default")
	.IsDependentOn ("DiscoverBuildDetails")
	.IsDependentOn ("OutputVariables")
	.IsDependentOn ("LocateFiles")
	.IsDependentOn ("VariableSetup")
	.IsDependentOn ("NugetRestore")
	.IsDependentOn ("SonarQubeStartup")
	.IsDependentOn ("Build")
	.IsDependentOn ("UnitTests")
	.IsDependentOn ("SonarQubeShutdown")
	.IsDependentOn ("CoverageUpload")
	// .IsDependentOn ("GenerateReleaseNotes")
	.IsDependentOn ("NugetPack")
	.IsDependentOn ("NugetPush")
	//.IsDependentOn ("Documentation")
	.IsDependentOn ("FailBuildIfTestFailed");

#endregion

#region RunTarget

RunTarget (target);

#endregion