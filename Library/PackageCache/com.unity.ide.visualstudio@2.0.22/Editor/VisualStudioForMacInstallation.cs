/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

#if UNITY_EDITOR_OSX

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using Unity.CodeEditor;
using IOPath = System.IO.Path;

namespace Microsoft.Unity.VisualStudio.Editor
{
	internal class VisualStudioForMacInstallation : VisualStudioInstallation
	{
		// C# language version support for Visual Studio for Mac
		private static readonly VersionPair[] OSXVersionTable =
		{
			// VisualStudio for Mac 2022
			new VersionPair(17,4, /* => */ 11,0),
			new VersionPair(17,0, /* => */ 10,0),

			// VisualStudio for Mac 8.x
			new VersionPair(8,8, /* => */ 9,0),
			new VersionPair(8,3, /* => */ 8,0),
			new VersionPair(8,0, /* => */ 7,3),
		};

		private static readonly IGenerator _generator = new LegacyStyleProjectGeneration();

		public override bool SupportsAnalyzers
		{
			get
			{
				return Version >= new Version(8, 3);
			}
		}

		public override Version LatestLanguageVersionSupported
		{
			get
			{
				return GetLatestLanguageVersionSupported(OSXVersionTable);
			}
		}

		private string GetExtensionPath()
		{
			const string addinName = "MonoDevelop.Unity";
			const string addinAssembly = addinName + ".dll";

			// user addins repository
			var localAddins = IOPath.Combine(
				Environment.GetFolderPath(Environment.SpecialFolder.Personal),
				$"Library/Application Support/VisualStudio/${Version.Major}.0" + "/LocalInstall/Addins");

			// In the user addins repository, the addins are suffixed by their versions, like `MonoDevelop.Unity.1.0`
			// When installing another local user addin, MD will remove files inside the folder
			// So we browse all VSTUM addins, and return the one with an addin assembly
			if (Directory.Exists(localAddins))
			{
				foreach (var folder in Directory.GetDirectories(localAddins, addinName + "*", SearchOption.TopDirectoryOnly))
				{
					if (File.Exists(IOPath.Combine(folder, addinAssembly)))
						return folder;
				}
			}

			// Check in Visual Studio.app/
			// In that case the name of the addin is used
			var addinPath = IOPath.Combine(Path, $"Contents/Resources/lib/monodevelop/AddIns/{addinName}");
			if (File.Exists(IOPath.Combine(addinPath, addinAssembly)))
				return addinPath;

			addinPath = IOPath.Combine(Path, $"Contents/MonoBundle/Addins/{addinName}");
			if (File.Exists(IOPath.Combine(addinPath, addinAssembly)))
				return addinPath;

			return null;
		}

		public override string[] GetAnalyzers()
		{
			var vstuPath = GetExtensionPath();
			if (string.IsNullOrEmpty(vstuPath))
				return Array.Empty<string>();

			return GetAnalyzers(vstuPath);
		}

		public override IGenerator ProjectGenerator
		{
			get
			{
				return _generator;
			}
		}

		private static bool IsCandidateForDiscovery(string path)
		{
			return Directory.Exists(path) && Regex.IsMatch(path, "Visual\\s?Studio(?!.*Code.*).*.app$", RegexOptions.IgnoreCase);
		}

		public static bool TryDiscoverInstallation(string editorPath, out IVisualStudioInstallation installation)
		{
			installation = null;

			if (string.IsNullOrEmpty(editorPath))
				return false;

			if (!IsCandidateForDiscovery(editorPath))
				return false;

			// On Mac we use the .app folder, so we need to access to main assembly
			var fvi = IOPath.Combine(editorPath, "Contents/Resources/lib/monodevelop/bin/VisualStudio.exe");

			if (!File.Exists(fvi))
				fvi = IOPath.Combine(editorPath, "Contents/MonoBundle/VisualStudio.exe");

			if (!File.Exists(fvi))
				fvi = IOPath.Combine(editorPath, "Contents/MonoBundle/VisualStudio.dll");				

			if (!File.Exists(fvi))
				return false;

			// VS preview are not using the isPrerelease flag so far
			// On Windows FileDescription contains "Preview", but not on Mac
			var vi = FileVersionInfo.GetVersionInfo(fvi);
			var version = new Version(vi.ProductVersion);
			var isPrerelease = vi.IsPreRelease || string.Concat(editorPath, "/" + vi.FileDescription).ToLower().Contains("preview");

			installation = new VisualStudioForMacInstallation()
			{
				IsPrerelease = isPrerelease,
				Name = $"{vi.FileDescription}{(isPrerelease ? " Preview" : string.Empty)} [{version.ToString(3)}]",
				Path = editorPath,
				Version = version
			};
			return true;
		}

		public static IEnumerable<IVisualStudioInstallation> GetVisualStudioInstallations()
		{
			var candidates = Directory.EnumerateDirectories("/Applications", "*.app");
			foreach (var candidate in candidates)
			{
				if (TryDiscoverInstallation(candidate, out var installation))
					yield return installation;
			}
		}

		[DllImport("AppleEventIntegration")]
		private static extern bool OpenVisualStudio(string appPath, string solutionPath, string filePath, int line);

		public override void CreateExtraFiles(string projectDirectory)
		{
		}

		public override bool Open(string path, int line, int column, string solution)
		{
			string absolutePath = "";
			if (!string.IsNullOrWhiteSpace(path))
			{
				absolutePath = IOPath.GetFullPath(path);
			}

			return OpenVisualStudio(CodeEditor.CurrentEditorInstallation, solution, absolutePath, line);
		}

		public static void Initialize()
		{
		}
	}
}

#endif
