using System;

using UnityEngine;

using Codice.CM.Common;
using PlasticGui.WorkspaceWindow.Locks;
using Unity.PlasticSCM.Editor.UI.Avatar;
using Unity.PlasticSCM.Editor.UI.Tree;

namespace Unity.PlasticSCM.Editor.Views.Locks
{
    internal static class DrawLocksListViewItem
    {
        internal static void ForCell(
            RepositorySpec mRepSpec,
            Rect rect,
            float rowHeight,
            LockInfo lockInfo,
            LocksListColumn column,
            Action avatarLoadedAction,
            bool isSelected,
            bool isFocused)
        {
            var columnText = LockInfoView.GetColumnText(
                mRepSpec,
                lockInfo,
                LocksListHeaderState.GetColumnName(column));
            
            if (column == LocksListColumn.Owner)
            {
                DrawTreeViewItem.ForItemCell(
                    rect,
                    rowHeight,
                    -1,
                    GetAvatar.ForEmail(columnText, avatarLoadedAction),
                    null,
                    columnText,
                    isSelected,
                    isFocused,
                    false,
                    false);

                return;
            }

            DrawTreeViewItem.ForLabel(
                rect,
                columnText,
                isSelected,
                isFocused,
                false);
        }
    }
}
