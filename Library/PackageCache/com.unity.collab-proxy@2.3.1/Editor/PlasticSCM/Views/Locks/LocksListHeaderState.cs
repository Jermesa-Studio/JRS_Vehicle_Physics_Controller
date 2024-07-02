using System;
using System.Collections.Generic;

using UnityEditor.IMGUI.Controls;
using UnityEngine;

using PlasticGui;
using Unity.PlasticSCM.Editor.UI;
using Unity.PlasticSCM.Editor.UI.Tree;

namespace Unity.PlasticSCM.Editor.Views.Locks
{
    internal enum LocksListColumn
    {
        ItemPath,
        LockType,
        ModificationDate,
        Owner,
        Branch,
        DestinationBranch
    }

    [Serializable]
    internal sealed class LocksListHeaderState : MultiColumnHeaderState, ISerializationCallbackReceiver
    {
        internal static LocksListHeaderState GetDefault()
        {
            return new LocksListHeaderState(BuildColumns());
        }
        
        internal static List<string> GetColumnNames()
        {
            return new List<string>
            {
                PlasticLocalization.Name.ItemColumn.GetString(),
                PlasticLocalization.Name.StatusColumn.GetString(),
                PlasticLocalization.Name.DateModifiedColumn.GetString(),
                PlasticLocalization.Name.OwnerColumn.GetString(),
                PlasticLocalization.Name.BranchColumn.GetString(),
                PlasticLocalization.Name.DestinationBranchColumn.GetString()
            };
        }

        internal static string GetColumnName(LocksListColumn column)
        {
            switch (column)
            {
                case LocksListColumn.ItemPath:
                    return PlasticLocalization.Name.ItemColumn.GetString();
                case LocksListColumn.LockType:
                    return PlasticLocalization.Name.StatusColumn.GetString();
                case LocksListColumn.ModificationDate:
                    return PlasticLocalization.Name.DateModifiedColumn.GetString();
                case LocksListColumn.Owner:
                    return PlasticLocalization.Name.OwnerColumn.GetString();
                case LocksListColumn.Branch:
                    return PlasticLocalization.Name.BranchColumn.GetString();
                case LocksListColumn.DestinationBranch:
                    return PlasticLocalization.Name.DestinationBranchColumn.GetString();
                default:
                    return null;
            }
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            if (mHeaderTitles != null)
            {
                TreeHeaderColumns.SetTitles(columns, mHeaderTitles);
            }

            if (mColumnsAllowedToggleVisibility != null)
            {
                TreeHeaderColumns.SetVisibilities(columns, mColumnsAllowedToggleVisibility);
            }
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
        }

        static Column[] BuildColumns()
        {
            return new[]
            {
                new Column
                {
                    width = UnityConstants.LocksColumns.PATH_WIDTH,
                    minWidth = UnityConstants.LocksColumns.PATH_MIN_WIDTH,
                    headerContent = new GUIContent(GetColumnName(LocksListColumn.ItemPath)),
                    allowToggleVisibility = false,
                    sortingArrowAlignment = TextAlignment.Right
                },
                new Column
                {
                    width = UnityConstants.LocksColumns.LOCK_TYPE_WIDTH,
                    minWidth = UnityConstants.LocksColumns.LOCK_TYPE_MIN_WIDTH,
                    headerContent = new GUIContent(GetColumnName(LocksListColumn.LockType)),
                    sortingArrowAlignment = TextAlignment.Right
                },
                new Column
                {
                    width = UnityConstants.LocksColumns.MODIFICATION_DATE_WIDTH,
                    minWidth = UnityConstants.LocksColumns.MODIFICATION_DATE_MIN_WIDTH,
                    headerContent = new GUIContent(GetColumnName(LocksListColumn.ModificationDate)),
                    sortingArrowAlignment = TextAlignment.Right
                },
                new Column
                {
                    width = UnityConstants.LocksColumns.OWNER_WIDTH,
                    minWidth = UnityConstants.LocksColumns.OWNER_MIN_WIDTH,
                    headerContent = new GUIContent(GetColumnName(LocksListColumn.Owner)),
                    sortingArrowAlignment = TextAlignment.Right
                },
                new Column
                {
                    width = UnityConstants.LocksColumns.BRANCH_NAME_WIDTH,
                    minWidth = UnityConstants.LocksColumns.BRANCH_NAME_MIN_WIDTH,
                    headerContent = new GUIContent(GetColumnName(LocksListColumn.Branch)),
                    sortingArrowAlignment = TextAlignment.Right
                },
                new Column
                {
                    width = UnityConstants.LocksColumns.DESTINATION_BRANCH_NAME_WIDTH,
                    minWidth = UnityConstants.LocksColumns.DESTINATION_BRANCH_NAME_MIN_WIDTH,
                    headerContent = new GUIContent(GetColumnName(LocksListColumn.DestinationBranch)),
                    sortingArrowAlignment = TextAlignment.Right
                }
            };
        }

        LocksListHeaderState(Column[] columns)
            : base(columns)
        {
            if (mHeaderTitles == null)
            {
                mHeaderTitles = TreeHeaderColumns.GetTitles(columns);
            }

            if (mColumnsAllowedToggleVisibility == null)
            {
                mColumnsAllowedToggleVisibility = TreeHeaderColumns.GetVisibilities(columns);
            }
        }

        [SerializeField]
        string[] mHeaderTitles;

        [SerializeField]
        bool[] mColumnsAllowedToggleVisibility;
    }
}
