/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

#if UNITY_EDITOR_WIN

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.Win32;
using Unity.CodeEditor;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;
using IOPath = System.IO.Path;

namespace Microsoft.Unity.VisualStudio.Editor
{
	internal class VisualStudioForWindowsInstallation : VisualStudioInstallation
	{
		// C# language version support for Visual Studio
		private static readonly VersionPair[] WindowsVersionTable =
		{
			// VisualStudio 2022
			new VersionPair(17,4, /* => */ 11,0),
			new VersionPair(17,0, /* => */ 10,0),

			// VisualStudio 2019
			new VersionPair(16,8, /* => */ 9,0),
			new VersionPair(16,0, /* => */ 8,0),
			
			// VisualStudio 2017
			new VersionPair(15,7, /* => */ 7,3),
			new VersionPair(15,5, /* => */ 7,2),
			new VersionPair(15,3, /* => */ 7,1),
			new VersionPair(15,0, /* => */ 7,0),
		};

		private static string _vsWherePath = null;
		private static readonly IGenerator _generator = new LegacyStyleProjectGeneration();

		public override bool SupportsAnalyzers
		{
			get
			{
				return Version >= new Version(16, 3);
			}
		}

		public override Version LatestLanguageVersionSupported
		{
			get
			{
				return GetLatestLanguageVersionSupported(WindowsVersionTable);
			}
		}

		private static string ReadRegistry(RegistryKey hive, string keyName, string valueName)
		{
			try
			{
				var unitykey = hive.OpenSubKey(keyName);

				var result = (string)unitykey?.GetValue(valueName);
				return result;
			}
			catch (Exception)
			{
				return null;
			}
		}

		private string GetWindowsBridgeFromRegistry()
		{
			var keyName = $"Software\\Microsoft\\Microsoft Visual Studio {Version.Major}.0 Tools for Unity";
			const string valueName = "UnityExtensionPath";

			var bridge = ReadRegistry(Registry.CurrentUser, keyName, valueName);
			if (string.IsNullOrEmpty(bridge))
				bridge = ReadRegistry(Registry.LocalMachine, keyName, valueName);

			return bridge;
		}

		private string GetExtensionPath()
		{
			const string extensionName = "Visual Studio Tools for Unity";
			const string extensionAssembly = "SyntaxTree.VisualStudio.Unity.dll";

			var vsDirectory = IOPath.GetDirectoryName(Path);
			var vstuDirectory = IOPath.Combine(vsDirectory, "Extensions", "Microsoft", extensionName);

			if (File.Exists(IOPath.Combine(vstuDirectory, extensionAssembly)))
				return vstuDirectory;

			return null;
		}

		public override string[] GetAnalyzers()
		{
			var vstuPath = GetExtensionPath();
			if (string.IsNullOrEmpty(vstuPath))
				return Array.Empty<string>();

			var analyzers = GetAnalyzers(vstuPath);
			if (analyzers?.Length > 0)
				return analyzers;

			var bridge = GetWindowsBridgeFromRegistry();
			if (File.Exists(bridge))
				return GetAnalyzers(IOPath.Combine(IOPath.GetDirectoryName(bridge), ".."));

			return Array.Empty<string>();
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
			return File.Exists(path) && Regex.IsMatch(path, "devenv.exe$", RegexOptions.IgnoreCase);
		}

		public static bool TryDiscoverInstallation(string editorPath, out IVisualStudioInstallation installation)
		{
			installation = null;

			if (string.IsNullOrEmpty(editorPath))
				return false;

			if (!IsCandidateForDiscovery(editorPath))
				return false;

			// On windows we use the executable directly, so we can query extra information
			if (!File.Exists(editorPath))
				return false;

			// VS preview are not using the isPrerelease flag so far
			// On Windows FileDescription contains "Preview", but not on Mac
			var vi = FileVersionInfo.GetVersionInfo(editorPath);
			var version = new Version(vi.ProductVersion);
			var isPrerelease = vi.IsPreRelease || string.Concat(editorPath, "/" + vi.FileDescription).ToLower().Contains("preview");

			installation = new VisualStudioForWindowsInstallation()
			{
				IsPrerelease = isPrerelease,
				Name = $"{FormatProductName(vi.FileDescription)} [{version.ToString(3)}]",
				Path = editorPath,
				Version = version
			};
			return true;
		}

		public static string FormatProductName(string productName)
		{
			if (string.IsNullOrEmpty(productName))
				return string.Empty;

			return productName.Replace("Microsoft ", string.Empty);
		}

		public static IEnumerable<IVisualStudioInstallation> GetVisualStudioInstallations()
		{
			foreach (var installation in QueryVsWhere())
				yield return installation;
		}

		#region VsWhere Json Schema
#pragma warning disable CS0649
		[Serializable]
		internal class VsWhereResult
		{
			public VsWhereEntry[] entries;

			public static VsWhereResult FromJson(string json)
			{
				return JsonUtility.FromJson<VsWhereResult>("{ \"" + nameof(VsWhereResult.entries) + "\": " + json + " }");
			}

			public IEnumerable<VisualStudioInstallation> ToVisualStudioInstallations()
			{
				foreach (var entry in entries)
				{
					yield return new VisualStudioForWindowsInstallation
					{
						Name = $"{FormatProductName(entry.displayName)} [{entry.catalog.productDisplayVersion}]",
						Path = entry.productPath,
						IsPrerelease = entry.isPrerelease,
						Version = Version.Parse(entry.catalog.buildVersion)
					};
				}
			}
		}

		[Serializable]
		internal class VsWhereEntry
		{
			public string displayName;
			public bool isPrerelease;
			public string productPath;
			public VsWhereCatalog catalog;
		}

		[Serializable]
		internal class VsWhereCatalog
		{
			public string productDisplayVersion; // non parseable like "16.3.0 Preview 3.0"
			public string buildVersion;
		}
#pragma warning restore CS3021
		#endregion

		private static IEnumerable<VisualStudioInstallation> QueryVsWhere()
		{
			var progpath = _vsWherePath;

			if (string.IsNullOrWhiteSpace(progpath))
				return Enumerable.Empty<VisualStudioInstallation>();

			const string arguments = "-prerelease -format json";

			// We've seen issues with json parsing in utf8 mode and with specific non-UTF code pages like 949 (Korea)
			// So try with utf8 first, then fallback to non utf8 in case of an issue
			// See https://github.com/microsoft/vswhere/issues/264
			try
			{
				return QueryVsWhere(progpath, $"{arguments} -utf8");
			}
			catch
			{
				return QueryVsWhere(progpath, $"{arguments}");
			}
		}

		private static IEnumerable<VisualStudioInstallation> QueryVsWhere(string progpath, string arguments)
		{
			var result = ProcessRunner.StartAndWaitForExit(progpath, arguments);

			if (!result.Success)
				throw new Exception($"Failure while running vswhere: {result.Error}");

			// Do not catch any JsonException here, this will be handled by the caller
			return VsWhereResult
				.FromJson(result.Output)
				.ToVisualStudioInstallations();
		}

		private enum COMIntegrationState
		{
			Running,
			DisplayProgressBar,
			ClearProgressBar,
			Exited
		}

		public override void CreateExtraFiles(string projectDirectory)
		{
			// See https://devblogs.microsoft.com/setup/configure-visual-studio-across-your-organization-with-vsconfig/
			// We create a .vsconfig file to make sure our ManagedGame workload is installed
			try
			{
				var vsConfigFile = IOPath.Combine(projectDirectory.NormalizePathSeparators(), ".vsconfig");
				if (File.Exists(vsConfigFile))
					return;

				const string content = @"{
  ""version"": ""1.0"",
  ""components"": [
    ""Microsoft.VisualStudio.Workload.ManagedGame""
  ]
}
";
				File.WriteAllText(vsConfigFile, content);
			}
			catch (IOException)
			{
			}			
		}

		public override bool Open(string path, int line, int column, string solution)
		{
			var progpath = FileUtility.GetPackageAssetFullPath("Editor", "COMIntegration", "Release", "COMIntegration.exe");

			if (string.IsNullOrWhiteSpace(progpath))
				return false;

			string absolutePath = "";
			if (!string.IsNullOrWhiteSpace(path))
			{
				absolutePath = IOPath.GetFullPath(path);
			}

			// We remove all invalid chars from the solution filename, but we cannot prevent the user from using a specific path for the Unity project
			// So process the fullpath to make it compatible with VS
			if (!string.IsNullOrWhiteSpace(solution))
			{
				solution = $"\"{solution}\"";
				solution = solution.Replace("^", "^^");
			}
			
			var psi = ProcessRunner.ProcessStartInfoFor(progpath, $"\"{CodeEditor.CurrentEditorInstallation}\" {solution} \"{absolutePath}\" {line}");
			psi.StandardOutputEncoding = System.Text.Encoding.Unicode;
			psi.StandardErrorEncoding = System.Text.Encoding.Unicode;

			// inter thread communication
			var messages = new BlockingCollection<COMIntegrationState>();

			var asyncStart = AsyncOperation<ProcessRunnerResult>.Run(
				() => ProcessRunner.StartAndWaitForExit(psi, onOutputReceived: data => OnOutputReceived(data, messages)),
				e => new ProcessRunnerResult {Success = false, Error = e.Message, Output = string.Empty},
				() => messages.Add(COMIntegrationState.Exited)
			);

			MonitorCOMIntegration(messages);

			var result = asyncStart.Result;

			if (!result.Success && !string.IsNullOrWhiteSpace(result.Error))
				Debug.LogError($"Error while starting Visual Studio: {result.Error}");

			return result.Success;
		}

		private static void MonitorCOMIntegration(BlockingCollection<COMIntegrationState> messages)
		{
			var displayingProgress = false;
			COMIntegrationState state;
			
			do
			{
				state = messages.Take();
				switch (state)
				{
					case COMIntegrationState.ClearProgressBar:
						EditorUtility.ClearProgressBar();
						displayingProgress = false;
						break;
					case COMIntegrationState.DisplayProgressBar:
						EditorUtility.DisplayProgressBar("Opening Visual Studio", "Starting up Visual Studio, this might take some time.", .5f);
						displayingProgress = true;
						break;
				}
			} while (state != COMIntegrationState.Exited);

			// Make sure the progress bar is properly cleared in case of COMIntegration failure
			if (displayingProgress)
				EditorUtility.ClearProgressBar();
		}
		
		private static readonly COMIntegrationState[] ProgressBarCommands = {COMIntegrationState.DisplayProgressBar, COMIntegrationState.ClearProgressBar};
		private static void OnOutputReceived(string data, BlockingCollection<COMIntegrationState> messages)
		{
			if (data == null)
				return;

			foreach (var cmd in ProgressBarCommands)
			{
				if (data.IndexOf(cmd.ToString(), StringComparison.OrdinalIgnoreCase) >= 0)
					messages.Add(cmd);
			}
		}

		public static void Initialize()
		{
			_vsWherePath = FileUtility.GetPackageAssetFullPath("Editor", "VSWhere", "vswhere.exe");
		}
	}
}

#endif
