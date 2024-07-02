# Unity Version Control Package
![ReleaseBadge](https://badges.cds.internal.unity3d.com/packages/com.unity.collab-proxy/release-badge.svg)

This package provides an in-editor interface for teams to work with [Unity Version Control](https://unity.com/solutions/version-control) (Unity VCS), our leading version control solution, directly in Unity.

_Note this project is the natural evolution of the old Collaborate package, hence its name._

[Documentation](https://docs.unity.com/ugs/en-us/manual/devops/manual/vcs-plugins/unityeditor-plugin/unity-version-control-package) - [Changelog](https://docs.unity3d.com/Packages/com.unity.collab-proxy@2.1/changelog/CHANGELOG.html) - [Yamato](https://unity-ci.cds.internal.unity3d.com/project/259)

## Compatibility
The minimum supported version of the Unity Editor is 2021.3 LTS.

Windows and macOS are officially supported.

The solution is exclusively targeting .NetStandard 2.0, and will not work with the legacy Mono runtime.

## Maintenance

This project is currently maintained by the VCS Ecosystem team (@vcs-ecosystem-team), part of UGS DevOps.

All suggestions and issues are very welcome in the Slack channel [#devs-unity-version-control](https://unity.slack.com/archives/C017V8W6BJ7).

## Development

### For developers
Option 1: clone this repository out into the `packages/` directory in a project.

Option 2: clone elsewhere and link with the `packages/manifest.json` file in the project:
```
"com.unity.collab-proxy": "file:/some/path/to/package"
```
To add testing support also add the testibles section to the manifest. Your manifest should look like this:
```json
{
  "dependencies": {
    "com.unity.collab-proxy": "file:/some/path/to/package",
    ...
  },
  "testables": [
    "com.unity.collab-proxy",
    ...
  ]
}
```

### For internal testers
Simply add the git url into the `packages/manifest.json` file:
```
"com.unity.collab-proxy": "git://git@github.cds.internal.unity3d.com:unity/com.unity.cloud.collaborate.git"
```
If you need a specific revisision:
```
"com.unity.collab-proxy": "git://git@github.cds.internal.unity3d.com:unity/com.unity.cloud.collaborate.git#<rev>"
```
If you need more information, read the [Documentation](https://docs.unity3d.com/Manual/upm-dependencies.html#Git) for package dependencies from git.

Code style is as dictated in [Unity Meta](https://github.cds.internal.unity3d.com/unity/unity-meta).
