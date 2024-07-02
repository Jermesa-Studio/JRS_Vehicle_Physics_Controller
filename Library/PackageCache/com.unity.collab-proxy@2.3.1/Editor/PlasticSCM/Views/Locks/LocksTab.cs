using System.Collections.Generic;
using System.Linq;

using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

using Codice.CM.Common;
using PlasticGui;
using PlasticGui.Help.Actions;
using PlasticGui.WorkspaceWindow.Locks;
using Unity.PlasticSCM.Editor.AssetUtils;
using Unity.PlasticSCM.Editor.UI;
using Unity.PlasticSCM.Editor.UI.Progress;

namespace Unity.PlasticSCM.Editor.Views.Locks
{
    internal sealed class LocksTab :
        IRefreshableView,
        ILockMenuOperations
    {
        internal LocksListView Table { get { return mLocksListView; } }

        internal ILockMenuOperations Operations { get { return this; } }

        internal LocksTab(
            RepositorySpec repSpec,
            IRefreshView refreshView,
            EditorWindow parentWindow)
        {
            mRepSpec = repSpec;
            mRefreshView = refreshView;
            mParentWindow = parentWindow;
            mProgressControls = new ProgressControlsForViews();
            mFillLocksTable = new FillLocksTable();

            BuildComponents(mRepSpec);

            ((IRefreshableView) this).Refresh();
        }

        internal void DrawSearchFieldForLocksTab()
        {
            DrawSearchField.For(
                mSearchField,
                mLocksListView,
                UnityConstants.SEARCH_FIELD_WIDTH);
        }

        internal void OnDisable()
        {
            mSearchField.downOrUpArrowKeyPressed -= 
                SearchField_OnDownOrUpArrowKeyPressed;

            mLocksListView.OnDisable();
        }

        internal void Update()
        {
            mProgressControls.UpdateProgress(mParentWindow);
        }

        internal void OnGUI()
        {
            DoActionsToolbar(
                mRepSpec.Server,
                mProgressControls,
                this,
                mIsReleaseLocksButtonEnabled,
                mIsRemoveLocksButtonEnabled);

            DoLocksArea(
                mLocksListView,
                mProgressControls.IsOperationRunning());
        }

        void IRefreshableView.Refresh()
        {
            mFillLocksTable.FillTable(
                mRepSpec,
                null,
                mLocksListView,
                mLocksListView,
                mProgressControls);
        }

        List<LockInfo.LockStatus> ILockMenuOperations.GetSelectedLocksStatus()
        {
            return mLocksListView.GetSelectedLocks().
                Select(lockInfo => lockInfo.Status).ToList();
        }

        void ILockMenuOperations.ReleaseLocks()
        {
            LockOperations.ReleaseLocks(
                mRepSpec,
                mLocksListView.GetSelectedLocks(),
                this,
                mRefreshView,
                mProgressControls,
                RefreshAsset.VersionControlCache);
        }

        void ILockMenuOperations.RemoveLocks()
        {
            LockOperations.RemoveLocks(
                mRepSpec,
                mLocksListView.GetSelectedLocks(),
                this,
                mRefreshView,
                mProgressControls,
                RefreshAsset.VersionControlCache);
        }

        void SearchField_OnDownOrUpArrowKeyPressed()
        {
            mLocksListView.SetFocusAndEnsureSelectedItem();
        }

        void OnSelectionChanged()
        {
            LockMenuOperations operations = LockMenuUpdater.GetAvailableMenuOperations(
                ((ILockMenuOperations)this).GetSelectedLocksStatus());

            mIsReleaseLocksButtonEnabled = operations.HasFlag(
                LockMenuOperations.Release);
            mIsRemoveLocksButtonEnabled = operations.HasFlag(
                LockMenuOperations.Remove);
        }

        static void DoActionsToolbar(
            string server,
            ProgressControlsForViews progressControls,
            ILockMenuOperations lockMenuOperations,
            bool isReleaseLocksButtonEnabled,
            bool isRemoveLocksButtonEnabled)
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);

            DoReleaseLocksButton(
                lockMenuOperations,
                isReleaseLocksButtonEnabled);

            DoRemoveLocksButton(
                lockMenuOperations,
                isRemoveLocksButtonEnabled);

            if (progressControls.IsOperationRunning())
            {
                DrawProgressForViews.ForIndeterminateProgress(
                    progressControls.ProgressData);
            }

            GUILayout.FlexibleSpace();

            DoConfigureLockRulesButton(server);

            EditorGUILayout.EndHorizontal();
        }

        static void DoLocksArea(
            LocksListView locksListView,
            bool isOperationRunning)
        {
            GUI.enabled = !isOperationRunning;

            var rect = GUILayoutUtility.GetRect(0, 100000, 0, 100000);

            locksListView.OnGUI(rect);

            GUI.enabled = true;
        }

        static void DoReleaseLocksButton(
            ILockMenuOperations lockMenuOperations,
            bool isEnabled)
        {
            GUI.enabled = isEnabled;

            if (DrawActionButton.For(
                    PlasticLocalization.Name.ReleaseLocksButton.GetString(),
                    PlasticLocalization.Name.ReleaseLocksButtonTooltip.GetString()))
            {
                lockMenuOperations.ReleaseLocks();
            }

            GUI.enabled = true;
        }

        static void DoRemoveLocksButton(
            ILockMenuOperations lockMenuOperations,
            bool isEnabled)
        {
            GUI.enabled = isEnabled;

            if (DrawActionButton.For(
                    PlasticLocalization.Name.RemoveLocksButton.GetString(),
                    PlasticLocalization.Name.RemoveLocksButtonTooltip.GetString()))
            {
                lockMenuOperations.RemoveLocks();
            }

            GUI.enabled = true;
        }

        static void DoConfigureLockRulesButton(string server)
        {
            if (DrawActionButton.For(PlasticLocalization.Name.
                    ConfigureLockRules.GetString()))
            {
                OpenConfigureLockRulesPage.Run(server);
            }
        }

        void BuildComponents(RepositorySpec repSpec)
        {
            mSearchField = new SearchField();
            mSearchField.downOrUpArrowKeyPressed += 
                SearchField_OnDownOrUpArrowKeyPressed;

            mLocksListView = new LocksListView(
                repSpec,
                LocksListHeaderState.GetDefault(),
                LocksListHeaderState.GetColumnNames(),
                new LocksViewMenu(this),
                OnSelectionChanged);

            mLocksListView.Reload();
        }

        bool mIsReleaseLocksButtonEnabled;
        bool mIsRemoveLocksButtonEnabled;

        SearchField mSearchField;
        LocksListView mLocksListView;

        readonly ProgressControlsForViews mProgressControls;
        readonly FillLocksTable mFillLocksTable;
        readonly EditorWindow mParentWindow;
        readonly IRefreshView mRefreshView;
        readonly RepositorySpec mRepSpec;
    }
}
