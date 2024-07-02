using UnityEditor;
using UnityEngine;

using PlasticGui;
using PlasticGui.WorkspaceWindow.Locks;
using Unity.PlasticSCM.Editor.UI;

namespace Unity.PlasticSCM.Editor.Views.Locks
{
    internal class LocksViewMenu
    {
        internal GenericMenu Menu { get { return mMenu; } }

        internal LocksViewMenu(
            ILockMenuOperations lockMenuOperations)
        {
            mLockMenuOperations = lockMenuOperations;

            BuildComponents();
        }

        internal void Popup()
        {
            mMenu = new GenericMenu();

            UpdateMenuItems(mMenu);

            mMenu.ShowAsContext();
        }

        internal bool ProcessKeyActionIfNeeded(Event e)
        {
            LockMenuOperations operationToExecute = GetMenuOperations(e);

            if (operationToExecute == LockMenuOperations.None)
                return false;

            LockMenuOperations operations = LockMenuUpdater.GetAvailableMenuOperations(
                mLockMenuOperations.GetSelectedLocksStatus());

            if (!operations.HasFlag(operationToExecute))
                return false;

            ProcessMenuOperation(operationToExecute, mLockMenuOperations);
            return true;
        }

        internal void UpdateMenuItems(GenericMenu menu)
        {
            LockMenuOperations operations = LockMenuUpdater.GetAvailableMenuOperations(
                mLockMenuOperations.GetSelectedLocksStatus());

            if (operations == LockMenuOperations.None)
            {
                menu.AddDisabledItem(GetNoActionMenuItemContent(), false);
                return;
            }

            AddLockMenuItem(
                mReleaseLockMenuItemContent,
                menu,
                operations,
                LockMenuOperations.Release,
                mLockMenuOperations.ReleaseLocks);

            AddLockMenuItem(
                mRemoveLockMenuItemContent,
                menu,
                operations,
                LockMenuOperations.Remove,
                mLockMenuOperations.RemoveLocks);
        }

        static void AddLockMenuItem(
            GUIContent menuItemContent,
            GenericMenu menu,
            LockMenuOperations operations,
            LockMenuOperations operationsToCheck,
            GenericMenu.MenuFunction menuFunction)
        {
            if (operations.HasFlag(operationsToCheck))
            {
                menu.AddItem(
                    menuItemContent,
                    false,
                    menuFunction);
                return;
            }

            menu.AddDisabledItem(menuItemContent);
        }

        static void ProcessMenuOperation(
            LockMenuOperations operationToExecute,
            ILockMenuOperations lockMenuOperations)
        {
            if (operationToExecute == LockMenuOperations.Remove)
            {
                lockMenuOperations.RemoveLocks();
                return;
            }
        }

        static LockMenuOperations GetMenuOperations(Event e)
        {
            if (Keyboard.IsKeyPressed(e, KeyCode.Delete))
                return LockMenuOperations.Remove;

            return LockMenuOperations.None;
        }

        GUIContent GetNoActionMenuItemContent()
        {
            if (mNoActionMenuItemContent == null)
            {
                mNoActionMenuItemContent = new GUIContent(
                    PlasticLocalization.Name.NoActionMenuItem.GetString());
            }

            return mNoActionMenuItemContent;
        }

        void BuildComponents()
        {
            mReleaseLockMenuItemContent = new GUIContent(PlasticLocalization.
                Name.LockMenuItemReleaseLock.GetString());
            mRemoveLockMenuItemContent = new GUIContent(string.Format("{0} {1}", 
                PlasticLocalization.Name.LockMenuItemRemoveLock.GetString(),
                GetPlasticShortcut.ForDelete()));
        }

        GenericMenu mMenu;

        GUIContent mNoActionMenuItemContent;
        GUIContent mReleaseLockMenuItemContent;
        GUIContent mRemoveLockMenuItemContent;

        readonly ILockMenuOperations mLockMenuOperations;
    }
}
