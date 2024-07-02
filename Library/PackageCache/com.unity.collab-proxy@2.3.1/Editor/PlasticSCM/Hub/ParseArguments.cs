using System.Collections.Generic;

namespace Unity.PlasticSCM.Editor.Hub
{
    internal static class ParseArguments
    {
        internal class Command
        {
            internal enum Operation
            {
                None,
                DownloadRepository,
                CreateWorkspace
            }

            internal readonly string ProjectPath;
            internal readonly string Organization;
            internal readonly string Repository;
            internal readonly Operation OperationType;

            internal Command(
                string projectPath,
                string organization,
                string repository,
                Operation operationType)
            {
                ProjectPath = projectPath;
                Organization = organization;
                Repository = repository;
                OperationType = operationType;
            }

            internal bool IsValid()
            {
                return !string.IsNullOrEmpty(ProjectPath)
                    && !string.IsNullOrEmpty(Organization)
                    && !string.IsNullOrEmpty(Repository)
                    && OperationType != Operation.None; 
            }
        }

        internal static Command GetCommand(Dictionary<string, string> args)
        {
            return new Command(
                GetProjectPath(args),
                GetOrganization(args),
                GetRepository(args),
                GetOperation(args));
        }

        internal static string GetProjectPath(Dictionary<string, string> args)
        {
            string data;

            if (args.TryGetValue(CREATE_PROJECT, out data))
                return data;

            if (args.TryGetValue(OPEN_PROJECT, out data))
                return data;

            return null;
        }

        internal static string GetOrganization(Dictionary<string, string> args)
        {
            string data;

            if (args.TryGetValue(UVCSArguments.ORGANIZATION_ARG, out data))
                return data;

            // -cloudOrganization 151d73c7-38cb-4eec-b11e-34764e707226-{plastic-org-name}
            if (args.TryGetValue(CloudArguments.ORGANIZATION_ARG, out data))
                return GetOrganizationNameFromData(
                    data, CloudArguments.PLASTIC_ORG_PREFIX_VALUE);

            return null;
        }

        internal static string GetRepository(Dictionary<string, string> args)
        {
            string data;

            if (args.TryGetValue(UVCSArguments.REPOSITORY_ARG, out data))
                return data;

            if (args.TryGetValue(CloudArguments.PROJECT_ARG, out data))
                return data;

            return null;
        }

        static Command.Operation GetOperation(Dictionary<string, string> args)
        {
            if (IsCreateWorkspaceCommand(args))
                return Command.Operation.CreateWorkspace;

            if (IsDownloadRepositoryCommand(args))
                return Command.Operation.DownloadRepository;

            return Command.Operation.None;
        }

        static string GetOrganizationNameFromData(string data, string plasticOrgPrefix)
        {
            if (data == null)
                return null;

            if (!data.StartsWith(plasticOrgPrefix))
                return null;

            if (data.Length <= plasticOrgPrefix.Length)
                return null;

            return data.Substring(plasticOrgPrefix.Length);
        }

        static bool IsCreateWorkspaceCommand(Dictionary<string, string> args)
        {
            // Connect UVCS to new project or existing project commands:
            // -createProject/-projectPath {project_path}
            //     -uvcsRepository {plastic_repo}
            //     -uvcsOrganization {plastic_org}
            //     -uvcsCreateWorkspace

            if (!args.ContainsKey(CREATE_PROJECT) &&
                !args.ContainsKey(OPEN_PROJECT))
                return false;

            return args.ContainsKey(UVCSArguments.ORGANIZATION_ARG)
                && args.ContainsKey(UVCSArguments.REPOSITORY_ARG)
                && args.ContainsKey(UVCSArguments.CREATE_WORKSPACE_FLAG);
        }

        static bool IsDownloadRepositoryCommand(Dictionary<string, string> args)
        {
            // Open remote project command:
            // -createProject {project_path}
            //     -cloudProject {plastic_repo}
            //     -cloudOrganization 151d73c7-38cb-4eec-b11e-34764e707226-{plastic_org}

            if (!args.ContainsKey(CREATE_PROJECT) ||
                !args.ContainsKey(CloudArguments.PROJECT_ARG))
                return false;

            string data;
            if (!args.TryGetValue(CloudArguments.ORGANIZATION_ARG, out data))
                return false;

            return data != null && data.StartsWith(
                CloudArguments.PLASTIC_ORG_PREFIX_VALUE);
        }

        static class UVCSArguments
        {
            internal const string ORGANIZATION_ARG = "-uvcsOrganization";
            internal const string REPOSITORY_ARG = "-uvcsRepository";
            internal const string CREATE_WORKSPACE_FLAG = "-uvcsCreateWorkspace";
        }

        static class CloudArguments
        {
            internal const string ORGANIZATION_ARG = "-cloudOrganization";
            internal const string PLASTIC_ORG_PREFIX_VALUE = "151d73c7-38cb-4eec-b11e-34764e707226-";

            internal const string PROJECT_ARG = "-cloudProject";
        }

        const string CREATE_PROJECT = "-createProject";
        const string OPEN_PROJECT = "-projectPath";
    }
}