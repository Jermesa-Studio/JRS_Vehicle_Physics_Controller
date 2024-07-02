# Using Code Coverage in batchmode

You can pass the following arguments in batchmode:

**-enableCodeCoverage**, to enable code coverage.  
**-coverageResultsPath** (optional), to set the location where the coverage results and report are saved to. The default location is the project's path.  
**-coverageHistoryPath** (optional), to set the location where the coverage report history is saved to. The default location is the project's path.  
**-coverageOptions** (optional), to pass extra options. Options are separated by semicolon. Some shells use semicolons to separate commands. Therefore, to ensure that coverage options are parsed correctly, enclose them in quotation marks.

|Coverage Option|Description|
|:---|:---|
|`generateHtmlReport`|Add this to generate a coverage [HTML report](HowToInterpretResults.md).|
|`generateHtmlReportHistory`|Add this to generate and include the coverage history in the HTML report.|
|`generateAdditionalReports`|Add this to generate [SonarQube](https://docs.sonarqube.org/latest/analysis/generic-test), [Cobertura](https://cobertura.github.io/cobertura) and [LCOV](https://github.com/linux-test-project/lcov) reports.|
|`generateBadgeReport`|Add this to generate coverage summary badges in SVG and PNG format.|
|`generateAdditionalMetrics`|Add this to generate and include additional metrics in the HTML report. These currently include Cyclomatic Complexity and Crap Score calculations for each method. See the [Risk Hotspots](HowToInterpretResults.md#risk-hotspots) section for more information.|
|`generateTestReferences`|Add this to include test references to the generated coverage results and enable the [Coverage by test methods](HowToInterpretResults.md#coverage-by-test-methods) section in the HTML report. This shows how each test contributes to the overall coverage.|
|`verbosity`|Add this to set the verbosity level for the editor and console logs. The default value is `info`.<br/>**Values:** `verbose`, `info`, `warning`, `error`, `off`|
|`useProjectSettings`|Add this to use the settings specified in `ProjectSettings/Packages/com.unity.testtools.codecoverage/Settings.json` instead. Any options passed in the command line will override this. This option can only be used in batchmode and it does not take effect when running the editor from the command line in non-batchmode.|
|`dontClear`|Add this to allow coverage results to be accumulated after every code coverage session. If not passed the results are cleared before a new session. For more information see [Generate combined report from EditMode and PlayMode tests](#generate-combined-report-from-editmode-and-playmode-tests).|
|`sourcePaths`|Add this to specify the source directories which contain the corresponding source code. The source directories are used by the report generator when the path information of classes cannot be determined. This is a comma separated string. Globbing is not supported.<br/><br/>**Example:** See [Generate combined report from separate projects](#generate-combined-report-from-separate-projects).|
|`assemblyFilters`|Add this to specify the assemblies to include or exclude in the coverage calculation and/or report. This is a comma-separated string. Prefix assemblies with `+` to include them or with `-` to exclude them. Globbing can be used to filter the assemblies.<br/><br/>**Available aliases:**<br/><br/>`<all>` maps to all the assemblies in the project.<br/>`<assets>` maps to the assemblies under the `Assets` folder.<br/>`<packages>` maps to the Packages' assemblies in the project, including the built-in packages.<br/><br/>**By default, if there are no included assemblies specified, only the assemblies under the `Assets` folder will be included.**<br/><br/>**Examples:**<br/><br/>`assemblyFilters:+<all>` will include code from all the assemblies in the project.<br/>`assemblyFilters:+my.assembly` will only include code from the assembly called `my.assembly`.<br/>`assemblyFilters:+unity.*` will include code from any assembly whose name starts with `unity.`<br/>`assemblyFilters:-*unity*` will exclude code from all assemblies that contain the word `unity` in their names.<br/>`assemblyFilters:+my.assembly.*,-my.assembly.tests` will include code from any assembly whose name starts with `my.assembly.`, but will explicitly exclude code from the assembly called `my.assembly.tests`.<br/>`assemblyFilters:+my.locale.??` will only include code from assemblies whose names match this format, e.g. `my.locale.en`, `my.locale.99`, etc.<br/>`assemblyFilters:+my.assembly.[a-z][0-9]` will only include code from assemblies whose names match this format, e.g. `my.assembly.a1`, `my.assembly.q7`, etc.|
|`pathFilters`|Add this to specify the paths that should be included or excluded in the coverage calculation and/or report. This is a comma-separated string. Prefix paths with `+` to include them and with `-` to exclude them. Globbing can be used to filter the paths.<br/><br/>Both absolute and relative paths are supported. Absolute paths can be shortened using globbing e.g. `**/Assets/Scripts/`. Relative paths require the `sourcePaths` option to be set. See [Using relative paths in path filters](#using-relative-paths-in-path-filters).<br/><br/>**Note:** If `pathFilters` are specified and there are no included assemblies specified in `assemblyFilters`, then all the assemblies in the project are included in order for `path filtering` to take precedence over `assembly filtering`.<br/><br/><br/>**Examples:**<br/><br/>`pathFilters:+C:/MyProject/Assets/MyClass.cs` will only include the `MyClass.cs` file.<br/>`pathFilters:+C:/MyProject/Assets/Scripts/*` will include all files in the `C:/MyProject/Assets/Scripts` folder. Files in subfolders will not be included.<br/>`pathFilters:-C:/MyProject/Assets/AutoGenerated/**` will exclude all files under the `C:/MyProject/Assets/AutoGenerated` folder and any of its subfolders.<br/>`pathFilters:+**/Assets/Editor/**` will include just the files that have `/Assets/Editor/` in their path.<br/>`pathFilters:+C:/MyProject/Assets/**/MyClass.cs` will include any file named `MyClass.cs` that is under the `C:/MyProject/Assets` folder and any of its subfolders.<br/>`pathFilters:+C:/MyProject/**,-**/Packages/**` will only include files under `C:/MyProject/` folder and exclude all files under any `Packages` folder.<br/>`pathFilters:+**/MyGeneratedClass_??.cs` will include only files with filenames that match this format, i.e. `MyGeneratedClass_01.cs`, `MyGeneratedClass_AB.cs`, etc.<br/>`pathFilters:+**/MyClass_[A-Z][0-9].cs` will include only files with filenames that match this format, i.e. `MyClass_A1.cs`, `MyClass_Q7.cs`, etc.|
|`pathFiltersFromFile`|Add this to specify the file to read path filtering rules from. Instead of defining all path filtering rules directly in the command line, as you would with the `pathFilters` option, this allows you to store them in a separate file, making your commands clearer and easier to manage.<br/><br/>Like with the `pathFilters` option, `pathFiltersFromFile` also supports relative paths. See [Using relative paths in path filters](#using-relative-paths-in-path-filters).<br/><br/>**Examples:**<br/><br/>`pathFiltersFromFile:C:/MyProject/FilteringRules.txt` will read rules from a file located in `C:/MyProject/FilteringRules.txt`<br/>`pathFiltersFromFile:FilteringRules.txt` will read rules from `FilteringRules.txt` located in the root of your project.<br/><br/>Syntax of the rules is the same as with the `pathFilters` option, however, rules should be listed in separate lines in the file.<br/><br/>**File example:**<br/><br/>This will include all the files in the `Scripts` folder and exclude all the files in the `Scripts/Generated` folder<pre><code>+**/Scripts/**<br/>-**/Scripts/Generated/**</code></pre><br/>**Note:** The `pathFiltersFromFile` option will be deprecated in the next package major release. Please use the `filtersFromFile` option instead.|
|`filtersFromFile`|Add this to specify the json file to read path and assembly filtering rules from. Instead of defining all filtering rules directly in the command line, as you would with `pathFilters` and `assemblyFilters` options, this allows you to store them in a separate file, making your commands clearer and easier to manage.<br/><br/>Like with the `pathFilters` option, `filtersFromFile` also supports relative paths. See [Using relative paths in path filters](#using-relative-paths-in-path-filters).<br/><br/>**Examples:**<br/><br/>`filtersFromfile:C:/MyProject/FilteringRules.json` will read rules from a file located in `C:/MyProject/FilteringRules.json`.<br/>`filtersFromFile:FilteringRules.json` will read rules from `FilteringRules.json` located in the root of your project.<br/><br/>**File example:**<br/><br/>This will include the `my.included.assembly`, exclude `my.excluded.assembly` and all assemblies with `unity` in their name. It will also include all files in the `Scripts` folder, and exclude all files in the `Scripts/Generated` folder<pre><code>{<br/>    "assembliesInclude": [<br/>        "my.included.assembly"<br/>    ],<br/>    "assembliesExclude": [<br/>        "my.excluded.assembly",<br/>        "*unity*"<br/>    ],<br/>    "pathsInclude": [<br/>        "**/Scripts/**"<br/>    ],<br/>    "pathsExclude": [<br/>        "**/Scripts/Generated/**"<br/>    ]</br>}</pre></code><br/>**Note:** The `pathFiltersFromFile` option will be deprecated in the next package major release. Please use the `filtersFromFile` option instead.|
|`pathReplacePatterns`|Add this to replace specific sections from the paths that are stored in the coverage results xml files. This is a comma separated string and requires elements to be passed in pairs i.e. `pathReplacePatterns:from,to,from,to`. Globbing is supported.<br/><br/>You can change the file paths in the coverage results xml to relative paths so that coverage data generated on different machines can be merged into a single report. Use the `pathReplacePatterns` option in conjunction with the `sourcePaths` option to specify the source directories which contain the corresponding source code. For more information see [Generate combined report from separate projects](#generate-combined-report-from-separate-projects).<br/><br/>**Note:** The [OpenCover](https://github.com/OpenCover/opencover) results xml format specifies file paths as absolute paths (`fullPath`). Changing these paths to relative paths will invalidate the OpenCover standard format. When the results xml files are fed into other tools, these may not work as expected if the paths are relative.<br/><br/>**Examples:**<br/><br/>`pathReplacePatterns:C:/MyProject,C:/MyOtherProject` will store the path as `C:/MyOtherProject/Assets/Scripts/MyScript.cs`, when the original path is `C:/MyProject/Assets/Scripts/MyScript.cs`<br/>`pathReplacePatterns:@*,,**/PackageCache/,Packages/` will store the path as `Packages/com.unity.my.package/Editor/MyScript.cs`, when the original path is `C:/Project/Library/PackageCache/com.unity.my.package@12345/Editor/MyScript.cs`<br/>`pathReplacePatterns:C:/MyProject/,` will store the path as `Assets/Scripts/MyScript.cs`, when the original path is `C:/MyProject/Assets/Scripts/MyScript.cs`<br/>`pathReplacePatterns:**Assets/,` will store the path as `Scripts/MyScript.cs`, when the original path is `C:/MyProject/Assets/Scripts/MyScript.cs`<br/>`pathReplacePatterns:C:/*/Assets/,` will store the path as `Scripts/MyScript.cs`, when the original path is `C:/MyProject/Assets/Scripts/MyScript.cs`<br/>`pathReplacePatterns:C:/MyProject??/,` will store the path as `Assets/Scripts/MyScript.cs`, when the original path is `C:/MyProject01/Assets/Scripts/MyScript.cs`<br/>`pathReplacePatterns:**/MyProject[A-Z][0-9]/,` will store the path as `Assets/Scripts/MyScript.cs`, when the original path is `C:/MyProjectA1/Assets/Scripts/MyScript.cs`|

## Example

```
Unity.exe -projectPath <path-to-project> -batchmode -testPlatform editmode -runTests -testResults
<path-to-results-xml> -debugCodeOptimization 
-enableCodeCoverage
-coverageResultsPath <path-to-coverage-results>
-coverageHistoryPath <path-to-coverage-history>
-coverageOptions "generateAdditionalMetrics;generateHtmlReport;generateHtmlReportHistory;generateBadgeReport;
assemblyFilters:+my.assembly.*,+<packages>;
pathFilters:-**/Tests/**,-**/BuiltInPackages/**"
```
The example above opens the project at `\<path-to-project\>`, runs the `EditMode` tests and produces an HTML coverage report and coverage summary badges in `\<path-to-coverage-results\>`. The report includes the coverage history, Cyclomatic Complexity and Crap Score calculations. The coverage history files are saved in `\<path-to-coverage-history\>`.

Additionally, the report includes code from any assembly whose name starts with `my.assembly.`, and includes code from all the Packages' assemblies. It excludes files that have `/Tests/` in their path (i.e. all the files under the Tests folder) and also excludes files that have `/BuiltInPackages/` in their path (i.e. all the built-in packages).

**Note:** `-debugCodeOptimization` is passed above to ensure Code optimization is set to Debug mode. See [Using Code Coverage with Code Optimization](UsingCodeCoverage.md#using-code-coverage-with-code-optimization).

## Generate combined report from EditMode and PlayMode tests

To get coverage information for both EditMode and PlayMode tests, run the editor three times as shown in the example below:
```
Unity.exe -projectPath <path-to-project> -batchmode -testPlatform editmode -runTests -debugCodeOptimization -enableCodeCoverage -coverageResultsPath <path-to-coverage-results>
-coverageOptions "generateAdditionalMetrics;assemblyFilters:+my.assembly.*;dontClear"

Unity.exe -projectPath <path-to-project> -batchmode -testPlatform playmode -runTests -debugCodeOptimization -enableCodeCoverage -coverageResultsPath <path-to-coverage-results>
-coverageOptions "generateAdditionalMetrics;assemblyFilters:+my.assembly.*;dontClear"

Unity.exe -projectPath <path-to-project> -batchmode -debugCodeOptimization -enableCodeCoverage -coverageResultsPath <path-to-coverage-results>
-coverageOptions "generateHtmlReport;generateBadgeReport;assemblyFilters:+my.assembly.*" -quit
```
The first generates the coverage results for the EditMode tests, the second generates the coverage results for the PlayMode tests and the third generates the coverage report and summary badges based on both coverage results.<br/><br/>**Note:** In [Unity Test Framework 2.0](https://docs.unity3d.com/Packages/com.unity.test-framework@2.0) and above the coverage results from both the EditMode and PlayMode test runs are stored in the `Automated` folder. In this example, passing the `dontClear` coverage option ensures that the results from the EditMode test run are not cleared during the PlayMode test run.

## Generate combined report from separate projects

To get a coverage report for your shared code which is used on separate projects, run the tests for each project making sure the `-coverageResultsPath` points to a separate location inside a shared root folder as shown in the example below:
```
Unity.exe -projectPath C:/MyProject -batchmode -testPlatform playmode -runTests -debugCodeOptimization -enableCodeCoverage -coverageResultsPath C:/CoverageResults/MyProject
-coverageOptions "generateAdditionalMetrics;assemblyFilters:+my.assembly.*;pathReplacePatterns:C:/MyProject/,"

Unity.exe -projectPath C:/MyOtherProject -batchmode -testPlatform playmode -runTests -debugCodeOptimization -enableCodeCoverage -coverageResultsPath C:/CoverageResults/MyOtherProject
-coverageOptions "generateAdditionalMetrics;assemblyFilters:+my.assembly.*;pathReplacePatterns:C:/MyOtherProject/,"

Unity.exe -projectPath C:/MyProject -batchmode -debugCodeOptimization -enableCodeCoverage -coverageResultsPath C:/CoverageResults
-coverageOptions "generateHtmlReport;generateBadgeReport;assemblyFilters:+my.assembly.*;sourcePaths:C:/MyProject" -quit
```
The first run generates the coverage results for the PlayMode tests for `MyProject` and stores these in `C:/CoverageResults/MyProject`. The second run generates the coverage results for the PlayMode tests for `MyOtherProject` and stores these in `C:/CoverageResults/MyOtherProject`. The third run generates the coverage report and summary badges based on the results found under the common `C:/CoverageResults` folder.

## Using relative paths in path filters ##

When the `sourcePaths` option is specified, the path filtering rules set by the `pathFilters`, `pathFiltersFromFile` and `filtersFromFile` options can be defined as relative paths.

**Example:**
 
```
Unity.exe -projectPath C:/MyProject -batchmode -testPlatform playmode -runTests -debugCodeOptimization -enableCodeCoverage -coverageResultsPath C:/CoverageResults/MyProject
-coverageOptions "generateHtmlReport;generateAdditionalMetrics;assemblyFilters:+<all>;pathFiltersFromFile:FilteringRules.txt;sourcePaths:C:/MyProject/Assets"
```

`FilteringRules.txt`
```
+Scripts/Animation/**
-**/Generated/**
+C:/MyPackages/com.my.company.mypackage/**
```

This example contains three rules:
* `+Scripts/Animation/**` - because the `sourcePaths` option was set and this is a relative path, this rule will include all the scripts in the `C:/MyProject/Assets/Scripts/Animation` folder and its subfolders.
* `-**/Generated/**` - excludes all the files that have `/Generated/` in their path. This is not a relative path so the `sourcePaths` option has no effect.
* `+C:/MyPackages/com.my.company.mypackage/**` - includes all the scripts located in the package outside of the project. This is an absolute path so the `sourcePaths` option has no effect. 
