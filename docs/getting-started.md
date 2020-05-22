# Getting started

xây doesn't require any dependencies to execute, but needs `ninja` to actually perform the build.

## New project

1. In an empty directory, create a `src` folder. Write your source code in it. In general, make sure to adhere
 to [xây's conventions].
1. If you have source code dependencies, add them to the `lib` folder.
1. If you have system dependencies, add them to `project.yml` as follows:
```yaml
dependencies:
  - <dependency>
  - <dependency>
  - ...
```
1. When ready to build, execute the command `xay` (or `Dung.CLI` if compiling from source). A `build` folder
 has been created with a Ninja build definition file.
1. Execute `ninja` in the build directory to build your files.

## Existing project

1. Make sure to adhere to [xây's conventions]. This might require a restructure of the
 project.
1. When ready to build, execute the command `xay` (or `Dung.CLI` if compiling from source). A `build` folder
 has been created with a Ninja build definition file.
1. Execute `ninja` in the build directory to build your files.

[xây's conventions]: /conventions