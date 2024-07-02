using System;
using System.Collections.Generic;

using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

using Codice.CM.Common;
using PlasticGui;
using PlasticGui.WorkspaceWindow.Locks;
using Unity.PlasticSCM.Editor.UI;
using Unity.PlasticSCM.Editor.UI.Tree;

namespace Unity.PlasticSCM.Editor.Views.Locks
{
    internal sealed class LocksListView :
        TreeView,
        FillLocksTable.IShowContentView,
        FillLocksTable.ILocksList
    {
        internal GenericMenu Menu { get { return mMenu.Menu; } }
        internal string EmptyStateMessage { get { return mEmptyStateMessage; } }

        internal LocksListView(
            RepositorySpec repSpec,
            LocksListHeaderState headerState,
            List<string> columnNames,
            LocksViewMenu menu,
            Action selectionChangedAction)
            : base(new TreeViewState())
        {
            mRepSpec = repSpec;
            mColumnNames = columnNames;
            mMenu = menu;
            mSelectionChangedAction = selectionChangedAction;

            mLocksSelector = new LocksSelector(this, mListViewItemIds);

            mCooldownFilterAction = new CooldownWindowDelayer(
                DelayedSearchChanged, UnityConstants.SEARCH_DELAYED_INPUT_ACTION_INTERVAL);

            mCooldownSelectionAction = new CooldownWindowDelayer(
                DelayedSelectionChanged, UnityConstants.SELECTION_DELAYED_INPUT_ACTION_INTERVAL);

            SetupTreeView(headerState);
        }

        public override IList<TreeViewItem> GetRows()
        {
            return mRows;
        }

        public override void OnGUI(Rect rect)
        {
            base.OnGUI(rect);

            if (mRows.Count == 0)
                DrawTreeViewEmptyState.For(rect, mEmptyStateMessage);

            Event e = Event.current;

            if (e.type != EventType.KeyDown)
                return;

            bool isProcessed = mMenu.ProcessKeyActionIfNeeded(e);

            if (isProcessed)
                e.Use();
        }

        protected override TreeViewItem BuildRoot()
        {
            return new TreeViewItem(0, -1, string.Empty);
        }

        protected override IList<TreeViewItem> BuildRows(
            TreeViewItem rootItem)
        {
            if (mLocksList == null)
            {
                ClearRows(rootItem, mRows);

                return mRows;
            }

            RegenerateRows(
                mListViewItemIds,
                mLocksList,
                rootItem,
                mRows);

            return mRows;
        }

        protected override void SearchChanged(string newSearch)
        {
            mCooldownFilterAction.Ping();
        }

        protected override void SelectionChanged(IList<int> selectedIds)
        {
            mCooldownSelectionAction.Ping();
        }

        protected override void ContextClickedItem(int id)
        {
            mMenu.Popup();
            Repaint();
        }

        protected override void BeforeRowsGUI()
        {
            int firstRowVisible;
            int lastRowVisible;
            GetFirstAndLastVisibleRows(out firstRowVisible, out lastRowVisible);

            GUI.DrawTexture(new Rect(0,
                firstRowVisible * rowHeight,
                GetRowRect(0).width,
                (lastRowVisible * rowHeight) + 1000),
                Images.GetTreeviewBackgroundTexture());

            DrawTreeViewItem.InitializeStyles();
            base.BeforeRowsGUI();
        }

        protected override void RowGUI(RowGUIArgs args)
        {
            if (args.item is LocksListViewItem)
            {
                LocksListViewItemGUI(
                    mRepSpec,
                    rowHeight,
                    ((LocksListViewItem)args.item).LockInfo,
                    args,
                    Repaint);
                return;
            }

            base.RowGUI(args);
        }

        internal void OnDisable()
        {
            TreeHeaderSettings.Save(
                multiColumnHeader.state,
                UnityConstants.LOCKS_TABLE_SETTINGS_NAME);
        }

        internal List<LockInfo> GetSelectedLocks()
        {
            return mLocksSelector.GetSelectedLocks();
        }

        void FillLocksTable.IShowContentView.ShowContentPanel()
        {
            mEmptyStateMessage = null;

            Reload();

            mLocksSelector.RestoreSelectedLocks();
        }

        void FillLocksTable.IShowContentView.ShowEmptyStatePanel(string explanationText)
        {
            mEmptyStateMessage = explanationText;

            Reload();
        }

        void FillLocksTable.IShowContentView.ShowErrorPanel(string errorText)
        {
            Debug.LogErrorFormat(
                PlasticLocalization.Name.LoadLocksErrorExplanation.GetString(),
                errorText);

            mEmptyStateMessage = PlasticLocalization.Name.LoadLocksError.GetString();

            mLocksList = null;
            mListViewItemIds.Clear();

            Reload();
        }

        void FillLocksTable.ILocksList.Fill(LockInfoList lockInfoList, Filter filter)
        {
            mLocksSelector.SaveSelectedLocks();

            mListViewItemIds.Clear();

            mLocksList = lockInfoList;

            Filter();
            Sort();
        }

        void Filter()
        {
            if (mLocksList == null)
                return;

            mLocksList.Filter(new Filter(searchString));
        }

        void Sort()
        {
            if (mLocksList == null)
                return;

            int sortedColumnIdx = multiColumnHeader.state.sortedColumnIndex;
            bool sortAscending = multiColumnHeader.IsSortedAscending(sortedColumnIdx);

            mLocksList.Sort(mColumnNames[sortedColumnIdx], sortAscending);
        }

        void DelayedSearchChanged()
        {
            Filter();

            Reload();

            TableViewOperations.ScrollToSelection(this);
        }

        void DelayedSelectionChanged()
        {
            mSelectionChangedAction();
        }

        void SortingChanged(MultiColumnHeader header)
        {
            Sort();

            Reload();
        }

        void SetupTreeView(LocksListHeaderState headerState)
        {
            TreeHeaderSettings.Load(
                headerState,
                UnityConstants.LOCKS_TABLE_SETTINGS_NAME,
                (int)LocksListColumn.ModificationDate,
                false);

            multiColumnHeader = new MultiColumnHeader(headerState);
            multiColumnHeader.canSort = true;
            multiColumnHeader.sortingChanged += SortingChanged;

            rowHeight = UnityConstants.TREEVIEW_ROW_HEIGHT;
            showAlternatingRowBackgrounds = false;
        }

        static void RegenerateRows(
            ListViewItemIds<LockInfo> listViewItemIds,
            LockInfoList locksList,
            TreeViewItem rootItem,
            List<TreeViewItem> rows)
        {
            ClearRows(rootItem, rows);

            if (locksList == null)
                return;

            foreach (LockInfo lockInfo in locksList.GetLocks())
            {
                int objectId;
                if (!listViewItemIds.TryGetInfoItemId(lockInfo, out objectId))
                    objectId = listViewItemIds.AddInfoItem(lockInfo);

                LocksListViewItem lockListViewItem =
                    new LocksListViewItem(objectId, lockInfo);

                rootItem.AddChild(lockListViewItem);
                rows.Add(lockListViewItem);
            }
        }

        static void ClearRows(
            TreeViewItem rootItem,
            List<TreeViewItem> rows)
        {
            if (rootItem.hasChildren)
                rootItem.children.Clear();

            rows.Clear();
        }

        static void LocksListViewItemGUI(
            RepositorySpec repSpec,
            float rowHeight,
            LockInfo item,
            RowGUIArgs args,
            Action avatarLoadedAction)
        {
            for (var visibleColumnIdx = 0; visibleColumnIdx < args.GetNumVisibleColumns(); visibleColumnIdx++)
            {
                var cellRect = args.GetCellRect(visibleColumnIdx);

                if (visibleColumnIdx == 0)
                {
                    cellRect.x += UnityConstants.FIRST_COLUMN_WITHOUT_ICON_INDENT;
                    cellRect.width -= UnityConstants.FIRST_COLUMN_WITHOUT_ICON_INDENT;
                }

                var column = (LocksListColumn) args.GetColumn(visibleColumnIdx);
                
                DrawLocksListViewItem.ForCell(
                    repSpec,
                    cellRect,
                    rowHeight,
                    item,
                    column,
                    avatarLoadedAction,
                    args.selected,
                    args.focused);
            }
        }


        ListViewItemIds<LockInfo> mListViewItemIds = new ListViewItemIds<LockInfo>();
        List<TreeViewItem> mRows = new List<TreeViewItem>();

        LockInfoList mLocksList;
        string mEmptyStateMessage;

        readonly CooldownWindowDelayer mCooldownFilterAction;
        readonly CooldownWindowDelayer mCooldownSelectionAction;
        readonly LocksSelector mLocksSelector;
        readonly Action mSelectionChangedAction;
        readonly LocksViewMenu mMenu;
        readonly List<string> mColumnNames;
        readonly RepositorySpec mRepSpec;
    }
}
