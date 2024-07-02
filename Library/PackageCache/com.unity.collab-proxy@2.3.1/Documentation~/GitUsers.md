# Unity version control for Git users

| **GIT**| **Unity VCS**| **Explanation**|
|:--|:--|:--|
| To Commit| To Check in| To Check in is to submit changes to the repo.|
| Commit| Changeset| Each new change on the history of the repo, grouping several individual file and directory changes.|
| Master| Main| When you create a repo in Unity VCS, there's always an "empty" branch. Unity VCS calls it Main.|
| To checkout  | To update| Downloading content to the workspace (working copy). This is called "update" because in Unity VCS, "checkout" has a different meaning.|
|| Checkout| When you checkout a file in Unity VCS, you're saying you are going to modify the file.|
|| Exclusive checkout or lock | This is locking a file so nobody can touch it. It’s only useful for non-mergeable files, like binaries, images, or art in a video game.|
| Rebase|| Unity VCS handles branching differently than Git. In Unity VCS, a rebase is just a merge operation.|
| Repository   | Repository| Where the entire history of the project is stored.
| Working copy | Workspace| In Git, you have the working copy and the repository in the exact location. You have a working copy and a .git hidden dir with the repository. In Unity VCS, this is slightly different since repositories and workspaces are separated. You can have several workspaces working with the same local repository.
