### Microsoft.Extensions.Logging compile-time source generator

.NET 6 introduces the LoggerMessageAttribute type. This attribute is part of the `Microsoft.Extensions.Logging` namespace, and when used, it source-generates performant logging APIs. The source-generation logging support is designed to deliver a highly usable and highly performant logging solution for modern .NET applications. The auto-generated source code relies on the `ILogger` interface in conjunction with `LoggerMessage.Define` functionality.

The source generator is triggered when `LoggerMessageAttribute` is used on `partial` logging methods. When triggered, it is either able to autogenerate the implementation of the `partial` methods it’s decorating, or produce compile-time diagnostics with hints about proper usage. The compile-time logging solution is typically considerably faster at run time than existing logging approaches. It achieves this by eliminating boxing, temporary allocations, and copies to the maximum extent possible.

There are benefits over manually using `LoggerMessage.Define` APIs directly:

* Shorter and simpler syntax: Declarative attribute usage rather than coding boilerplate.
* Guided developer experience: The generator gives warnings to help developers do the right thing.
* Support for an arbitrary number of logging parameters. `LoggerMessage.Define` supports a maximum of six.
* Support for dynamic log level. This is not possible with `LoggerMessage.Define` alone.

If you would like to keep track of improvements and known issues, see dotnet/runtime#52549.

#### Basic usage

To use the `LoggerMessageAttribute`, the consuming class and method need to be `partial`. The code generator is triggered at compile time, and generates an implementation of the `partial` method.

```c#
public static partial class Log
{
  [LoggerMessage(EventId = 0, Level = LogLevel.Critical, Message = "Could not open socket to `{hostName}`")]
  public static partial void CouldNotOpenSocket(ILogger logger, string hostName);
}
```

In the preceding example, the logging method is static and the log level is specified in the attribute definition. When using the attribute in a static context, the ILogger instance is required as a parameter. You may choose to use the attribute in a non-static context as well. For more examples and usage scenarios visit the docs for the compile-time logging source generator.
