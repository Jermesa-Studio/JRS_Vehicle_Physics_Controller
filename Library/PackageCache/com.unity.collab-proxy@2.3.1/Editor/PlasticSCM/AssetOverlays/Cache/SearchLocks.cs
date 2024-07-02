using System.Collections.Generic;

using Codice.Client.Commands;
using Codice.Client.Commands.WkTree;
using Codice.Client.Common.Locks;
using Codice.Client.Common.WkTree;
using Codice.CM.Common;
using Codice.CM.WorkspaceServer;

namespace Unity.PlasticSCM.Editor.AssetsOverlays.Cache
{
    internal static class SearchLocks
    {
        internal static Dictionary<WorkspaceTreeNode, LockInfo> GetLocksInfo(
            WorkspaceInfo wkInfo,
            Dictionary<MountPointWithPath, List<WorkspaceTreeNode>> locksCandidates)
        {
            Dictionary<WorkspaceTreeNode, LockInfo> result =
                new Dictionary<WorkspaceTreeNode, LockInfo>();

            ServerLocks.ForWorkingBranchOnRepoByItem locksForWorkingBranchOnRepoByItem =
                new ServerLocks.ForWorkingBranchOnRepoByItem();

            foreach (KeyValuePair<MountPointWithPath, List<WorkspaceTreeNode>> each in locksCandidates)
            {
                FillRepositoryLocks(
                    wkInfo, each.Key, each.Value,
                    locksForWorkingBranchOnRepoByItem, result);
            }

            return result;
        }

        static void FillRepositoryLocks(
            WorkspaceInfo wkInfo,
            MountPointWithPath mount,
            List<WorkspaceTreeNode> candidates,
            ServerLocks.ForWorkingBranchOnRepoByItem locksForWorkingBranchOnRepoByItem,
            Dictionary<WorkspaceTreeNode, LockInfo> locks)
        {
            if (candidates.Count == 0)
                return;

            LockRule lockRule = ServerLocks.GetLockRule(mount.RepSpec);

            if (lockRule == null)
                return;

            candidates = GetLockableCandidates(candidates, lockRule);

            if (candidates.Count == 0)
                return;

            BranchInfo workingBranch = CheckoutBranchSolver.Get(wkInfo).
                GetWorkingBranchWithoutBranchExpansionByMount(mount);

            if (workingBranch == null)
                return;

            ServerLocks.GetLocksForRepoByItemId(
                mount.RepSpec, workingBranch.Id, locksForWorkingBranchOnRepoByItem);

            Dictionary<long, LockInfo> lockByItemCache;
            if (!locksForWorkingBranchOnRepoByItem.TryGetLocks(
                    mount.RepSpec, workingBranch.Id, out lockByItemCache))
                return;

            if (lockByItemCache.Count == 0)
                return;

            foreach (WorkspaceTreeNode candidate in candidates)
            {
                LockInfo serverLock;
                if (!lockByItemCache.TryGetValue(
                        candidate.RevInfo.ItemId, out serverLock))
                    continue;

                locks[candidate] = serverLock;
            }
        }

        static List<WorkspaceTreeNode> GetLockableCandidates(
            List<WorkspaceTreeNode> candidates,
            LockRule lockRule)
        {
            List<WorkspaceTreeNode> result = new List<WorkspaceTreeNode>();

            LockedFilesFilter filter = new LockedFilesFilter(lockRule.Rules);

            foreach (WorkspaceTreeNode candidate in candidates)
            {
                string cmPath = WorkspaceNodeOperations.GetCmPath(candidate);

                if (cmPath == null)
                {
                    //The node could not be on the head tree (like copied items) so when we
                    //cannot calculate the path we assume that it's lockable.
                    result.Add(candidate);
                    continue;
                }

                if (filter.IsLockable(cmPath))
                    result.Add(candidate);
            }

            return result;
        }
    }
}
