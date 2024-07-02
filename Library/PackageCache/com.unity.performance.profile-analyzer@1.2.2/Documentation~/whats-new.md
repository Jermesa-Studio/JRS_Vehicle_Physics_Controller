# What's new in 1.2.2

Summary of changes in Profile Analyzer version 1.2.2.

The main updates in this release include:

## Added 

* Added support for removing vsync time, so we can see actual CPU duration over multiple frames. A drop down has been added to 'remove' a marker from the analysis and has entries for "FPS Wait", "Present Wait" and "Custom" where you can select any marker from the table using the right click context menu to "Remove Marker".
* Added optional standard deviation (SD) column into marker table (on single view) for sorting data based on variation of marker timings in over the frames.
* Added export of the comparison table as CSV.

For a full list of changes and updates in this version, see the [Profile Analyzer package changelog](https://docs.unity3d.com/Packages/com.unity.performance.profile-analyzer@latest/index.html?subfolder=/changelog/CHANGELOG.html).
