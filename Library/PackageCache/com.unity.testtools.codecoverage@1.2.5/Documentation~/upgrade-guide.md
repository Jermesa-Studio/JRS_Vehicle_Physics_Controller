# Upgrading to Code Coverage package version 1.2

To upgrade to Code Coverage package version 1.2, you need to do the following:
- [Update assembly filtering aliases in batchmode](upgrade-guide.md#update-assembly-filtering-aliases-in-batchmode)
- [Rename `pathStrippingPatterns` to `pathReplacePatterns` in batchmode](upgrade-guide.md#rename-pathstrippingpatterns-to-pathreplacepatterns-in-batchmode)

**Note**: If you're upgrading from a version older than 1.1, follow the upgrade guide for version 1.1 first.

## Update assembly filtering aliases in batchmode
- Rename assembly filtering aliases when running in [batchmode](CoverageBatchmode.md). `<user>` alias was renamed to `<assets>` and `<project>` was renamed to `<all>`.

## Rename `pathStrippingPatterns` to `pathReplacePatterns` in batchmode
- Rename `pathStrippingPatterns` to `pathReplacePatterns` in [batchmode](CoverageBatchmode.md).<br/><br/>**Example:**<br/><br/>Change `pathStrippingPatterns:C:/MyProject/` to `pathReplacePatterns:C:/MyProject/,`.<br/>This is equivalent to stripping `C:/MyProject/` by replacing `C:/MyProject/` with an empty string.

# Upgrading to Code Coverage package version 1.1
To upgrade to Code Coverage package version 1.1, you need to do the following:
- [Update path filtering globbing rules](upgrade-guide.md#update-path-filtering-globbing-rules)

## Update path filtering globbing rules
- Update the path filtering globbing rules in your batchmode commands and code coverage window. To keep the current behavior when using [globbing](https://en.wikipedia.org/wiki/Glob_%28programming%29) to match any number of folders, the `*` character should be replaced with `**`. A single `*` character can be used to specify a single folder layer.<br/><br/>**Examples:**<br/><br/>`pathFilters:+C:/MyProject/Assets/Scripts/*` will include all files in the `C:/MyProject/Assets/Scripts` folder. Files in subfolders will not be included.<br/>`pathFilters:+C:/MyProject/Assets/Scripts/**` will include all files under the `C:/MyProject/Assets/Scripts` folder and any of its subfolders.

For a full list of changes and updates in this version, see the [Code Coverage package changelog](../changelog/CHANGELOG.html).
