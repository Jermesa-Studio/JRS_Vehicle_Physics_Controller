using System;
using System.Threading;

using UnityEditor;

using Codice.Client.Common.EventTracking;
using Codice.CM.Common;
using Codice.LogWrapper;
using PlasticGui;
using PlasticGui.Help.Conditions;
using PlasticGui.WebApi.Responses;
using Unity.PlasticSCM.Editor.AssetUtils;
using Unity.PlasticSCM.Editor.Configuration;
using Unity.PlasticSCM.Editor.UI;
using Unity.PlasticSCM.Editor.Views.CreateWorkspace;
using Unity.PlasticSCM.Editor.WebApi;

namespace Unity.PlasticSCM.Editor.Hub.Operations
{
    internal class CreateWorkspace
    {
        internal static void LaunchOperation(
            OperationParams parameters)
        {
            PlasticApp.InitializeIfNeeded();

            CreateWorkspace createWorkspaceOperation =
                new CreateWorkspace();

            createWorkspaceOperation.CreateWorkspaceOperation(
                parameters);
        }

        internal static WorkspaceInfo CreateWorkspaceForRepSpec(
            RepositorySpec repositorySpec,
            string projectPath,
            ILog log)
        {
            CreateWorkspaceDialogUserAssistant assistant =
                new CreateWorkspaceDialogUserAssistant(
                    PlasticGuiConfig.Get().Configuration.DefaultWorkspaceRoot,
                    PlasticGui.Plastic.API.GetAllWorkspacesArray());

            assistant.RepositoryChanged(
                repositorySpec.ToString(),
                string.Empty,
                string.Empty);

            WorkspaceInfo wkInfo = PlasticGui.Plastic.API.CreateWorkspace(
                projectPath,
                assistant.GetProposedWorkspaceName(),
                repositorySpec.ToString());

            log.DebugFormat("Created workspace {0} on {1}",
                wkInfo.Name,
                wkInfo.ClientPath);

            return wkInfo;
        }

        void CreateWorkspaceOperation(
            OperationParams parameters)
        {
            RefreshAsset.BeforeLongAssetOperation();

            try
            {
                ThreadPool.QueueUserWorkItem(
                    CreateWorkspaceIfNeeded,
                    parameters);

                while (mStatus != Status.Finished)
                {
                    if (mDisplayProgress)
                    {
                        DisplayProgress(
                            mStatus, parameters.Repository);
                    }

                    Thread.Sleep(150);
                }
            }
            finally
            {
                EditorUtility.ClearProgressBar();

                RefreshAsset.AfterLongAssetOperation();

                if (!mOperationFailed)
                {
                    PlasticPlugin.Enable();
                    ShowWindow.PlasticDisablingCollab();
                }
            }
        }

        void CreateWorkspaceIfNeeded(object state)
        {
            OperationParams parameters = (OperationParams)state;

            try
            {
                if (FindWorkspace.HasWorkspace(parameters.ProjectFullPath))
                {
                    // each domain reload, the package is reloaded.
                    // way need to check if we already created it
                    return;
                }

                mDisplayProgress = true;

                mStatus = Status.ConfiguringCredentials;

                TokenExchangeResponse tokenExchangeResponse =
                    AutoConfig.PlasticCredentials(
                        parameters.AccessToken,
                        parameters.RepositorySpec.Server,
                        parameters.ProjectFullPath);

                if (tokenExchangeResponse.Error != null)
                {
                    mOperationFailed = true;

                    LogTokenExchangeErrorInConsole(
                        tokenExchangeResponse.Error);
                    return;
                }

                mStatus = Status.CreatingWorkspace;

                TrackFeatureUseEvent.For(
                    parameters.RepositorySpec, TrackFeatureUseEvent.
                        Features.UnityPackage.CreateWorkspaceFromHub);

                WorkspaceInfo wkInfo = CreateWorkspaceForRepSpec(
                    parameters.RepositorySpec,
                    parameters.ProjectFullPath,
                    mLog);

                mStatus = Status.PerformingInitialCheckin;

                PerformInitialCheckinIfRepositoryIsEmpty(
                    wkInfo, parameters.RepositorySpec,
                    PlasticGui.Plastic.API, mLog);
            }
            catch (Exception ex)
            {
                LogException(ex);
                LogExceptionErrorInConsole(ex);

                mOperationFailed = true;
            }
            finally
            {
                mStatus = Status.Finished;
            }
        }

        static void PerformInitialCheckinIfRepositoryIsEmpty(
            WorkspaceInfo wkInfo,
            RepositorySpec repositorySpec,
            IPlasticAPI plasticApi,
            ILog log)
        {
            try
            {
                bool isEmptyRepository = IsEmptyRepositoryCondition.
                    Evaluate(wkInfo, repositorySpec, plasticApi);

                if (!isEmptyRepository)
                    return;

                PerformInitialCheckin.PerformCheckinPackagesAndProjectSettingsFolders(
                    wkInfo, false, plasticApi);

                log.DebugFormat("Created initial checkin on repository '{0}'",
                    repositorySpec.ToString());
            }
            catch (Exception ex)
            {
                // create the initial checkin if it's possible, otherwise
                // just log the exception (no error shown for the user)
                LogException(ex);
            }
        }

        static void DisplayProgress(
            Status status,
            string repository)
        {
            string progressMessage = GetProgressString(status);

            float progressPercent = (int)status / (float)Status.Finished;

            EditorUtility.DisplayProgressBar(
                string.Format("{0} {1}",
                    PlasticLocalization.GetString(
                        PlasticLocalization.Name.CreatingWorkspaceProgress),
                    repository),
                progressMessage, progressPercent);
        }

        static void LogTokenExchangeErrorInConsole(ErrorResponse.ErrorFields error)
        {
            UnityEngine.Debug.LogErrorFormat(
                PlasticLocalization.GetString(
                    PlasticLocalization.Name.ErrorCreatingWorkspaceForProject),
                string.Format("Unable to get TokenExchangeResponse: {0} [code {1}]",
                    error.Message, error.ErrorCode));
        }

        static void LogExceptionErrorInConsole(Exception ex)
        {
            UnityEngine.Debug.LogErrorFormat(
                PlasticLocalization.GetString(
                    PlasticLocalization.Name.ErrorCreatingWorkspaceForProject),
                ex.Message);
        }

        static void LogException(Exception ex)
        {
            mLog.WarnFormat("Message: {0}", ex.Message);

            mLog.DebugFormat(
                "StackTrace:{0}{1}",
                Environment.NewLine, ex.StackTrace);
        }

        static string GetProgressString(Status status)
        {
            switch (status)
            {
                case Status.Starting:
                    return PlasticLocalization.GetString(
                        PlasticLocalization.Name.CreateWorkspaceProgressStarting);
                case Status.ConfiguringCredentials:
                    return PlasticLocalization.GetString(
                        PlasticLocalization.Name.CreateWorkspaceProgressConfiguringCredentials);
                case Status.CreatingWorkspace:
                    return PlasticLocalization.GetString(
                        PlasticLocalization.Name.CreateWorkspaceProgressCreatingWorkspace);
                case Status.PerformingInitialCheckin:
                    return PlasticLocalization.GetString(
                        PlasticLocalization.Name.CreateWorkspaceProgressPerformingInitialCheckin);
                case Status.Finished:
                    return PlasticLocalization.GetString(
                        PlasticLocalization.Name.CreateWorkspaceProgressFinished);
            }

            return string.Empty;
        }

        CreateWorkspace()
        {
        }

        enum Status : int
        {
            Starting = 1,
            ConfiguringCredentials = 2,
            CreatingWorkspace = 3,
            PerformingInitialCheckin = 4,
            Finished = 5
        };

        volatile Status mStatus = Status.Starting;
        volatile bool mOperationFailed = false;
        volatile bool mDisplayProgress;

        static readonly ILog mLog = LogManager.GetLogger("CreateWorkspace");
    }
}
