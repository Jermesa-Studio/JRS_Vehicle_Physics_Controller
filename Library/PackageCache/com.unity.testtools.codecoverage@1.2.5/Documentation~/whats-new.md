# What's new in version 1.2

Summary of changes in Code Coverage package version 1.2

The main updates in this release include:

## Added

- Added `Pause Recording` and `Resume Recording` buttons in the toolbar in the [Code Coverage window](CodeCoverageWindow.md).
- Added `Test Runner References` coverage report option in the [Code Coverage window](CodeCoverageWindow.md). When you check this option, the generated coverage results include references to the triggering tests enabling the [Coverage by test methods](HowToInterpretResults.md#coverage-by-test-methods) section in the HTML report. This section allows you to see how each test contributes to the overall coverage. In [batchmode](CoverageBatchmode.md), you can generate test references by adding the `generateTestReferences` option in *-coverageOptions*.
- Added `Log Verbosity Level` setting in the [Code Coverage window](CodeCoverageWindow.md) which allows setting the verbosity level for the editor and console logs.
- Added `Additional Reports` option in the [Code Coverage window](CodeCoverageWindow.md) which if checked [SonarQube](https://docs.sonarqube.org/latest/analysis/generic-test), [Cobertura](https://cobertura.github.io/cobertura) and [LCOV](https://github.com/linux-test-project/lcov) reports will be generated. Added `generateAdditionalReports` in *-coverageOptions* for [batchmode](CoverageBatchmode.md).
- Added `filtersFromFile` in *-coverageOptions* for [batchmode](CoverageBatchmode.md). This allows you to specify an external Json file which contains path and assembly filtering rules.
- Added `dontClear` in *-coverageOptions* for [batchmode](https://docs.unity3d.com/Packages/com.unity.testtools.codecoverage@1.2/manual/CoverageBatchmode.html) which allows coverage results to be accumulated after every code coverage session. If not passed the results are cleared before a new session.

## Updated

- Updated the UI of the [Code Coverage window](CodeCoverageWindow.md) moving the action buttons into a toolbar at the top.
![Toolbar](images/toolbar.png) 
- Introduced new selection buttons under the *Included Assemblies* dropdown in the [Code Coverage window](CodeCoverageWindow.md).
- Renamed *assemblyFilters* aliases in [batchmode](CoverageBatchmode.md); `<user>` was renamed to `<assets>` and `<project>` was renamed to `<all>`.
- Replaced `pathStrippingPatterns` with `pathReplacePatterns` in [batchmode](CoverageBatchmode.md). The `pathReplacePatterns` option allows stripping and replacing specific sections from the paths that are stored in the coverage results xml files.
- The size of the coverage result files and the Code Coverage session duration have been optimized.
- Improved the editor and console logs.

## Fixed

- Ensure assemblies are removed from the Included Assemblies field if they no longer exist (case [1318668](https://issuetracker.unity3d.com/issues/code-coverage-the-included-assemblies-field-shows-assemblies-that-no-longer-exist)).
- Ensure hidden sequence points are ignored (case [1372305](https://issuetracker.unity3d.com/issues/class-which-derives-from-methodbase-causes-incorrect-sequence-points-to-be-generated-by-coverage-api)).

For a full list of changes and updates in this version, see the [Code Coverage package changelog](../changelog/CHANGELOG.html).
