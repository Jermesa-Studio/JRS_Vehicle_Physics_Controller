/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Unity Technologies.
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using System.Collections.Generic;
using System.IO;

namespace Microsoft.Unity.VisualStudio.Editor
{
	internal static class Discovery
	{
		public static IEnumerable<IVisualStudioInstallation> GetVisualStudioInstallations()
		{
#if UNITY_EDITOR_WIN
			foreach (var installation in VisualStudioForWindowsInstallation.GetVisualStudioInstallations())
				yield return installation;
#elif UNITY_EDITOR_OSX
			foreach (var installation in VisualStudioForMacInstallation.GetVisualStudioInstallations())
				yield return installation;
#endif

			foreach (var installation in VisualStudioCodeInstallation.GetVisualStudioInstallations())
				yield return installation;
		}

		public static bool TryDiscoverInstallation(string editorPath, out IVisualStudioInstallation installation)
		{
			try
			{
#if UNITY_EDITOR_WIN
				if (VisualStudioForWindowsInstallation.TryDiscoverInstallation(editorPath, out installation))
					return true;
#elif UNITY_EDITOR_OSX
				if (VisualStudioForMacInstallation.TryDiscoverInstallation(editorPath, out installation))
					return true;
#endif
				if (VisualStudioCodeInstallation.TryDiscoverInstallation(editorPath, out installation))
					return true;
			}
			catch (IOException)
			{
				installation = null;
			}

			return false;
		}

		public static void Initialize()
		{
#if UNITY_EDITOR_WIN
			VisualStudioForWindowsInstallation.Initialize();
#elif UNITY_EDITOR_OSX
			VisualStudioForMacInstallation.Initialize();
#endif
			VisualStudioCodeInstallation.Initialize();
		}
	}
}
