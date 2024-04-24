# sqlproj-issue-557
This repository is intended to reproduce the current problem with [MSBuild.Sdk.SqlProj #557](https://github.com/rr-wfm/MSBuild.Sdk.SqlProj/issues/557). The smallest possible setup was chosen for the reproduction.

By default `<Project Sdk="MSBuild.Sdk.SqlProj/2.7.2">` is active, and the generated file `foo_Create.sql` has no tables. After turning back to version `2.6.1` and rebuild everything, the table `[dbo].[mainCode]` is part of the file.