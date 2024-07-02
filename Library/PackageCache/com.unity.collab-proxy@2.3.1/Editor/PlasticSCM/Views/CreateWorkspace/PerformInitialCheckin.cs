using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using Codice.Client.BaseCommands;
using Codice.Client.Commands;
using Codice.Client.Commands.CheckIn;
using Codice.Client.Common;
using Codice.Client.Common.Threading;
using Codice.CM.Common;
using Codice.Client.GameUI.Checkin;
using PlasticGui;
using PlasticGui.Help.Conditions;

namespace Unity.PlasticSCM.Editor.Views.CreateWorkspace
{
    internal static class PerformInitialCheckin
    {
        internal static void IfRepositoryIsEmpty(
            WorkspaceInfo wkInfo,
            string repository,
            bool isGluonWorkspace,
            IPlasticAPI plasticApi,
            IProgressControls progressControls,
            CreateWorkspaceView.ICreateWorkspaceListener createWorkspaceListener,
            PlasticWindow plasticWindow)
        {
            RepositoryInfo repInfo = null;
            bool isEmptyRepository = false;

            progressControls.ShowProgress(string.Empty);

            IThreadWaiter waiter = ThreadWaiter.GetWaiter(10);
            waiter.Execute(
            /*threadOperationDelegate*/ delegate
            {
                RepositorySpec repSpec = new SpecGenerator().
                    GenRepositorySpec(false, repository);

                repInfo = plasticApi.GetRepositoryInfo(repSpec);

                isEmptyRepository = IsEmptyRepositoryCondition.
                    Evaluate(wkInfo, repSpec, plasticApi);
            },
            /*afterOperationDelegate*/ delegate
            {
                progressControls.HideProgress();

                if (waiter.Exception != null)
                {
                    DisplayException(progressControls, waiter.Exception);
                    return;
                }

                if (!isEmptyRepository)
                {
                    plasticWindow.RefreshWorkspaceUI();
                    return;
                }

                CheckinPackagesAndProjectSettingsFolders(
                    wkInfo, isGluonWorkspace, plasticApi,
                    progressControls, createWorkspaceListener);
            });
        }

        static void CheckinPackagesAndProjectSettingsFolders(
            WorkspaceInfo wkInfo,
            bool isGluonWorkspace,
            IPlasticAPI plasticApi,
            IProgressControls progressControls,
            CreateWorkspaceView.ICreateWorkspaceListener createWorkspaceListener)
        {
            progressControls.ShowProgress(PlasticLocalization.GetString(
                PlasticLocalization.Name.UnityInitialCheckinProgress));

            IThreadWaiter waiter = ThreadWaiter.GetWaiter(10);
            waiter.Execute(
            /*threadOperationDelegate*/ delegate
            {
                PerformCheckinPackagesAndProjectSettingsFolders(
                    wkInfo, isGluonWorkspace, plasticApi);
            },
            /*afterOperationDelegate*/ delegate
            {
                progressControls.HideProgress();

                if (waiter.Exception != null &&
                    !IsMergeNeededException(waiter.Exception))
                {
                    DisplayException(progressControls, waiter.Exception);
                    return;
                }

                createWorkspaceListener.OnWorkspaceCreated(wkInfo, isGluonWorkspace);
            });
        }

        internal static void PerformCheckinPackagesAndProjectSettingsFolders(
            WorkspaceInfo wkInfo,
            bool isGluonWorkspace,
            IPlasticAPI plasticApi)
        {
            List<string> paths = new List<string> {
                    Path.Combine(wkInfo.ClientPath, "Packages"),
                    Path.Combine(wkInfo.ClientPath, "ProjectSettings")
                };

            string comment = PlasticLocalization.GetString(
                PlasticLocalization.Name.UnityInitialCheckinComment);

            PerformAdd(paths, plasticApi);

            PerformCheckinForMode(wkInfo, paths, comment, isGluonWorkspace);
        }

        static void PerformAdd(
            List<string> paths,
            IPlasticAPI plasticApi)
        {
            AddOptions options = new AddOptions();
            options.AddPrivateParents = true;
            options.CheckoutParent = true;
            options.Recurse = true;
            options.SearchForPrivatePaths = true;
            options.SkipIgnored = true;

            IList checkouts;
            plasticApi.Add(paths.ToArray(), options, out checkouts);
        }

        static void PerformCheckinForMode(
            WorkspaceInfo wkInfo,
            List<string> paths,
            string comment,
            bool isGluonWorkspace)
        {
            if (isGluonWorkspace)
            {
                new BaseCommandsImpl().PartialCheckin(wkInfo, paths, comment);
                return;
            }

            CheckinParams ciParams = new CheckinParams();
            ciParams.paths = paths.ToArray();
            ciParams.comment = comment;
            ciParams.time = DateTime.MinValue;
            ciParams.flags = CheckinFlags.Recurse | CheckinFlags.ProcessSymlinks;

            new BaseCommandsImpl().CheckIn(ciParams);
        }

        static bool IsMergeNeededException(Exception exception)
        {
            if (exception == null)
                return false;

            // Check the check-in exception for gluon
            if (exception is CheckinConflictsException)
                return true;

            // Check the check-in exceptions for plastic
            return exception is CmClientMergeNeededException;
        }

        static void DisplayException(
            IProgressControls progressControls,
            Exception ex)
        {
            ExceptionsHandler.LogException(
                "PerformInitialCheckin", ex);

            progressControls.ShowError(
                ExceptionsHandler.GetCorrectExceptionMessage(ex));
        }
    }
}
