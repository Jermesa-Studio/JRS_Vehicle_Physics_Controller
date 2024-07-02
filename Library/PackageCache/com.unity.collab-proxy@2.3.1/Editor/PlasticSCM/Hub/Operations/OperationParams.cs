using System.IO;

using Codice.CM.Common;
using PlasticGui.WebApi;

namespace Unity.PlasticSCM.Editor.Hub.Operations
{
    internal class OperationParams
    {
        internal readonly string ProjectFullPath;
        internal readonly string Organization;
        internal readonly string Repository;
        internal readonly RepositorySpec RepositorySpec;
        internal readonly string AccessToken;

        internal static OperationParams BuildFromCommand(
            ParseArguments.Command command,
            string unityAccessToken)
        {
            return new OperationParams(
                Path.GetFullPath(command.ProjectPath),
                command.Organization,
                command.Repository,
                BuildRepositorySpec(
                    command.Organization, command.Repository),
                unityAccessToken);
        }

        static RepositorySpec BuildRepositorySpec(
            string organization,
            string repository)
        {
            string defaultCloudAlias = new PlasticWebRestApi()
                .GetDefaultCloudAlias();

            return new RepositorySpec()
            {
                Name = repository,
                Server = CloudServer.BuildFullyQualifiedName(
                    organization, defaultCloudAlias)
            };
        }

        OperationParams(
            string projectFullPath,
            string organization,
            string repository,
            RepositorySpec repositorySpec,
            string accessToken)
        {
            ProjectFullPath = projectFullPath;
            Organization = organization;
            Repository = repository;
            RepositorySpec = repositorySpec;
            AccessToken = accessToken;
        }
    }
}
