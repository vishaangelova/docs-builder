// Licensed to Elasticsearch B.V under one or more agreements.
// Elasticsearch B.V licenses this file to you under the Apache 2.0 License.
// See the LICENSE file in the project root for more information

using System.Diagnostics.CodeAnalysis;
using System.IO.Abstractions;

namespace Elastic.Documentation.Configuration;

public static class Paths
{
	public static readonly DirectoryInfo WorkingDirectoryRoot = DetermineWorkingDirectoryRoot();

	public static readonly DirectoryInfo ApplicationData = GetApplicationFolder();

	private static DirectoryInfo DetermineWorkingDirectoryRoot()
	{
		var directory = new DirectoryInfo(Directory.GetCurrentDirectory());
		while (directory != null &&
			   (directory.GetFiles("*.sln").Length == 0 || directory.GetDirectories(".git").Length == 0))
			directory = directory.Parent;
		return directory ?? new DirectoryInfo(Directory.GetCurrentDirectory());
	}

	public static IDirectoryInfo? DetermineSourceDirectoryRoot(IDirectoryInfo sourceDirectory)
	{
		IDirectoryInfo? sourceRoot = null;
		var directory = sourceDirectory;
		while (directory != null && directory.GetDirectories(".git").Length == 0)
			directory = directory.Parent;
		sourceRoot ??= directory;
		return sourceRoot;
	}

	/// Used in debug to locate static folder so we can change js/css files while the server is running
	public static DirectoryInfo? GetSolutionDirectory()
	{
		var directory = new DirectoryInfo(Directory.GetCurrentDirectory());
		while (directory != null && directory.GetFiles("*.sln").Length == 0)
			directory = directory.Parent;
		return directory;
	}

	// ~/Library/Application\ Support/ on osx
	// XDG_DATA_HOME or home/.local/share on linux
	// %LOCAL_APPLICATION_DATA% windows
	private static DirectoryInfo GetApplicationFolder()
	{
		var localPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
		var elasticPath = Path.Combine(localPath, "elastic", "docs-builder");
		return new DirectoryInfo(elasticPath);
	}

	public static (IDirectoryInfo, IFileInfo) FindDocsFolderFromRoot(IFileSystem readFileSystem, IDirectoryInfo rootPath)
	{
		string[] files = ["docset.yml", "_docset.yml"];
		string[] knownFolders = [rootPath.FullName, Path.Combine(rootPath.FullName, "docs")];
		var mostLikelyTargets =
			from file in files
			from folder in knownFolders
			select Path.Combine(folder, file);

		var knownConfigPath = mostLikelyTargets.FirstOrDefault(readFileSystem.File.Exists);
		var configurationPath = knownConfigPath is null ? null : readFileSystem.FileInfo.New(knownConfigPath);
		if (configurationPath is not null)
			return (configurationPath.Directory!, configurationPath);

		configurationPath = rootPath
			.EnumerateFiles("*docset.yml", SearchOption.AllDirectories)
			.FirstOrDefault()
		?? throw new Exception($"Can not locate docset.yml file in '{rootPath}'");

		var docsFolder = configurationPath.Directory ?? throw new Exception($"Can not locate docset.yml file in '{rootPath}'");

		return (docsFolder, configurationPath);
	}

	public static bool TryFindDocsFolderFromRoot(
		IFileSystem readFileSystem,
		IDirectoryInfo rootPath,
		[NotNullWhen(true)] out IDirectoryInfo? docDirectory,
		[NotNullWhen(true)] out IFileInfo? configurationPath
	)
	{
		docDirectory = null;
		configurationPath = null;
		try
		{
			(docDirectory, configurationPath) = FindDocsFolderFromRoot(readFileSystem, rootPath);
			return true;
		}
		catch
		{
			return false;
		}
	}
}
