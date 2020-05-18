# CLI usage

In the following section, replace `dung` by `Dung.CLI` if using binaries compiled from source.

## Global options

    -C           Change directory before running the executable (unimplemented)

## `dung`

Generate Ninja build files for compilation.

## `dung build`

!!! warning "Unimplemented feature"
    This feature is not yet implemented, but is planned and will be available on the first stable version.

Generate Ninja build files and compiles the project at once.

## `dung foreach`

!!! warning "Unimplemented feature"
    This feature is not yet implemented, but is planned and will be available on the first stable version.

Executes a command for each source file in the project. Use `{}` as a placeholder for the filename, `;` at the
end to execute one command per filename, or `+` to execute the command once with the list of filenames.

!!! example ""
    **Example**: `dung foreach --stdio prettier`

```
--stdin     Pass the file content to stdin instead (runs one command per file)
--pipe      Pass the file content to stdin instead, and writes stdout back into the file (runs one command
            per file)
```