# Technical details

## How it works

The package is a client of the coverage API. For more information, see the [coverage API's documentation](https://docs.unity3d.com/ScriptReference/TestTools.Coverage.html). The package uses a combination of this API and C# reflection to output the test coverage data in the OpenCover format. Optionally, a third-party report generator will then parse the OpenCover data and generate a report (HTML, SonarQube, Cobertura, LCOV).

## Requirements

This version of the Code Coverage package is compatible with the following versions of the Unity Editor:

* 2019.3 and later

## Third-party libraries used

* [ReportGenerator](https://github.com/danielpalme/ReportGenerator) - v5.0.4

## Known limitations

Code Coverage includes the following known limitations:

* Code Coverage currently only supports the [OpenCover](https://github.com/OpenCover/opencover) format.
* Code Coverage currently only supports code run in the Editor and not in Standalone/Player.
* NPath Complexity calculation and Branch Coverage are not implemented at present so they will always appear as zero in the coverage report.