/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Unity Technologies.
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using System.Text;
using UnityEditor.Compilation;

namespace Microsoft.Unity.VisualStudio.Editor
{

	internal class LegacyStyleProjectGeneration : ProjectGeneration
	{
		internal override string StyleName => "Legacy";

		public LegacyStyleProjectGeneration(string tempDirectory, IAssemblyNameProvider assemblyNameProvider, IFileIO fileIoProvider, IGUIDGenerator guidGenerator) : base(tempDirectory, assemblyNameProvider, fileIoProvider, guidGenerator)
		{
		}

		public LegacyStyleProjectGeneration(string tempDirectory) : base(tempDirectory)
		{
		}

		public LegacyStyleProjectGeneration()
		{
		}

		internal override void GetProjectHeader(ProjectProperties properties, out StringBuilder headerBuilder)
		{
			headerBuilder = new StringBuilder();

			//Header
			headerBuilder.Append(@"<?xml version=""1.0"" encoding=""utf-8""?>").Append(k_WindowsNewline);
			headerBuilder.Append($@"<Project ToolsVersion=""4.0"" DefaultTargets=""Build"" xmlns=""{MSBuildNamespaceUri}"">").Append(k_WindowsNewline);
			headerBuilder.Append(@"  <!-- Generated file, do not modify, your changes will be overwritten (use AssetPostprocessor.OnGeneratedCSProject) -->").Append(k_WindowsNewline);
			headerBuilder.Append(@"  <PropertyGroup>").Append(k_WindowsNewline);
			headerBuilder.Append(@"    <LangVersion>").Append(properties.LangVersion).Append(@"</LangVersion>").Append(k_WindowsNewline);
			headerBuilder.Append(@"  </PropertyGroup>").Append(k_WindowsNewline);
			headerBuilder.Append(@"  <PropertyGroup>").Append(k_WindowsNewline);
			headerBuilder.Append(@"    <Configuration Condition="" '$(Configuration)' == '' "">Debug</Configuration>").Append(k_WindowsNewline);
			headerBuilder.Append(@"    <Platform Condition="" '$(Platform)' == '' "">AnyCPU</Platform>").Append(k_WindowsNewline);
			headerBuilder.Append(@"    <ProductVersion>10.0.20506</ProductVersion>").Append(k_WindowsNewline);
			headerBuilder.Append(@"    <SchemaVersion>2.0</SchemaVersion>").Append(k_WindowsNewline);
			headerBuilder.Append(@"    <RootNamespace>").Append(properties.RootNamespace).Append(@"</RootNamespace>").Append(k_WindowsNewline);
			headerBuilder.Append(@"    <ProjectGuid>{").Append(properties.ProjectGuid).Append(@"}</ProjectGuid>").Append(k_WindowsNewline);
			headerBuilder.Append(@"    <OutputType>Library</OutputType>").Append(k_WindowsNewline);
			headerBuilder.Append(@"    <AppDesignerFolder>Properties</AppDesignerFolder>").Append(k_WindowsNewline);
			headerBuilder.Append(@"    <AssemblyName>").Append(properties.AssemblyName).Append(@"</AssemblyName>").Append(k_WindowsNewline);
			// In the end, given we use NoConfig/NoStdLib (see below), hardcoding the target framework version with the legacy format will have no impact, even when targeting netstandard/net48 from Unity.
			// And VSTU/Unity Game workload has a dependency towards net471 reference assemblies, so IDE will not complain that this specific SDK is not available.
			// Unity already selected proper API surface through referenced DLLs for us.
			headerBuilder.Append(@"    <TargetFrameworkVersion>v4.7.1</TargetFrameworkVersion>").Append(k_WindowsNewline);
			headerBuilder.Append(@"    <FileAlignment>512</FileAlignment>").Append(k_WindowsNewline);
			headerBuilder.Append(@"    <BaseDirectory>.</BaseDirectory>").Append(k_WindowsNewline);
			headerBuilder.Append(@"  </PropertyGroup>").Append(k_WindowsNewline);

			GetProjectHeaderConfigurations(properties, headerBuilder);

			// Explicit references
			headerBuilder.Append(@"  <PropertyGroup>").Append(k_WindowsNewline);
			headerBuilder.Append(@"    <NoConfig>true</NoConfig>").Append(k_WindowsNewline);
			headerBuilder.Append(@"    <NoStdLib>true</NoStdLib>").Append(k_WindowsNewline);
			headerBuilder.Append(@"    <AddAdditionalExplicitAssemblyReferences>false</AddAdditionalExplicitAssemblyReferences>").Append(k_WindowsNewline);
			headerBuilder.Append(@"    <ImplicitlyExpandNETStandardFacades>false</ImplicitlyExpandNETStandardFacades>").Append(k_WindowsNewline);
			headerBuilder.Append(@"    <ImplicitlyExpandDesignTimeFacades>false</ImplicitlyExpandDesignTimeFacades>").Append(k_WindowsNewline);
			headerBuilder.Append(@"  </PropertyGroup>").Append(k_WindowsNewline);

			GetProjectHeaderVstuFlavoring(properties, headerBuilder);
			GetProjectHeaderAnalyzers(properties, headerBuilder);
		}

		internal override void AppendProjectReference(Assembly assembly, Assembly reference, StringBuilder projectBuilder)
		{
			// If the current assembly is a Player project, we want to project-reference the corresponding Player project
			var referenceName = m_AssemblyNameProvider.GetAssemblyName(assembly.outputPath, reference.name);

			projectBuilder.Append(@"    <ProjectReference Include=""").Append(referenceName).Append(GetProjectExtension()).Append(@""">").Append(k_WindowsNewline);
			projectBuilder.Append("      <Project>{").Append(ProjectGuid(referenceName)).Append("}</Project>").Append(k_WindowsNewline);
			projectBuilder.Append("      <Name>").Append(referenceName).Append("</Name>").Append(k_WindowsNewline);
			projectBuilder.Append("    </ProjectReference>").Append(k_WindowsNewline);
		}

		internal override void GetProjectFooter(StringBuilder footerBuilder)
		{
			footerBuilder.Append(string.Join(k_WindowsNewline,
				$"  <Import Project=\"{@"$(MSBuildToolsPath)\Microsoft.CSharp.targets".NormalizePathSeparators()}\" />",
				@"  <Target Name=""GenerateTargetFrameworkMonikerAttribute"" />",
				@"  <!-- To modify your build process, add your task inside one of the targets below and uncomment it.",
				@"       Other similar extension points exist, see Microsoft.Common.targets.",
				@"  <Target Name=""BeforeBuild"">",
				@"  </Target>",
				@"  <Target Name=""AfterBuild"">",
				@"  </Target>",
				@"  -->",
				@"</Project>",
				@""));
		}
	}
}
