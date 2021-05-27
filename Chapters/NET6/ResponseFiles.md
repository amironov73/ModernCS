### Response files

A response file is a file that contains a set of command-line arguments for a tool. Response files satisfy two primary use cases: enables a command-line invocation to extend past the character limit of the terminal, and it is a convenience over typing the same commands repeatedly. Response file support has been added for the .NET CLI. The syntax is @file.rsp. The format of the response file is a single line of text, just as it would be structured in a terminal.

The following animation illustrates using a response file with dotnet build, demonstrated as: `dotnet build @demo.rsp`

#### Directives

Directives are a System.CommandLine experience for interacting with commands directly. Within the System.CommandLine model, commands are a set of objects that can be invoked or explored, as data with associated behavior.

The suggest directive enables you to search for commands if you don’t know the exact command. This directive exists to enable the dotnet-suggest global tool

```
> dotnet [suggest] buil
build
build-server
msbuild
```

The parse directive demonstrates the manner in which the CLI parses your commands. It can be useful to understand why a command doesn’t work or has different results than you expect.

```
dotnet [parse] build -f net5.0
[ dotnet [ build [ -f <net5.0> ] ] ]
```
