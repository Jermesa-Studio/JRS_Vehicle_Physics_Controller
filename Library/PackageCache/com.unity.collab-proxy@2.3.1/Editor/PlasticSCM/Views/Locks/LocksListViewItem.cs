using Codice.CM.Common;
using UnityEditor.IMGUI.Controls;

namespace Unity.PlasticSCM.Editor.Views.Locks
{
    internal sealed class LocksListViewItem : TreeViewItem
    {
        internal LockInfo LockInfo { get; private set; }

        internal LocksListViewItem(int id, LockInfo lockInfo)
            : base(id, 1)
        {
            LockInfo = lockInfo;

            displayName = id.ToString();
        }
    }
}
