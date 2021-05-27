### Повышение производительности FileStream на Windows

FileStream has been re-written in .NET 6, to have much higher performance and reliability on Windows.

The [re-write](https://github.com/dotnet/runtime/issues/40359) project has been phased over five PRs:

* [Introduce FileStreamStrategy as a first step of FileStream rewrite](https://github.com/dotnet/runtime/pull/47128)
* [FileStream rewrite part II](https://github.com/dotnet/runtime/pull/48813)
* [FileStream optimizations](https://github.com/dotnet/runtime/pull/49975)
* [FileStream rewrite: Use IValueTaskSource instead of TaskCompletionSource](https://github.com/dotnet/runtime/pull/50802)
* [FileStream rewrite: Caching the ValueTaskSource in AsyncWindowsFileStreamStrategy](https://github.com/dotnet/runtime/pull/51363)

The final result is that `FileStream` never blocks when created for async IO, on Windows. That’s a major improvement. You can observe that in the benchmarks, which we’ll look at shortly.

#### Configuration

The first PR enables FileStream to choose an implementation at runtime. The most obvious benefit of this pattern is enabling switching back to the old .NET 5 implementation, which you can do with the following setting, in `runtimeconfig.json`.

```json
{
    "configProperties": {
        "System.IO.UseNet5CompatFileStream": true
    }
}
```

We plan to add an io_uring strategy next, which takes advantage of a Linux feature by the same name in recent kernels.

#### Performance benchmark

Let’s measure the improvements using BenchmarkDotNet.

```c#
public class FileStreamPerf
{
private const int FileSize = 1_000_000; // 1 MB
private Memory<byte> _buffer = new byte[8_000]; // 8 kB

    [GlobalSetup(Target = nameof(ReadAsync))]
    public void SetupRead() => File.WriteAllBytes("file.txt", new byte[FileSize]);

    [Benchmark]
    public async ValueTask ReadAsync()
    {
        using FileStream fileStream = new FileStream("file.txt", FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize: 4096, useAsync: true);
        while (await fileStream.ReadAsync(_buffer) > 0)
        {
        }
    }

    [Benchmark]
    public async ValueTask WriteAsync()
    {
        using FileStream fileStream = new FileStream("file.txt", 
           FileMode.Create, FileAccess.Write, FileShare.Read, 
           bufferSize: 4096, useAsync: true);
        for (int i = 0; i < FileSize / _buffer.Length; i++)
        {
            await fileStream.WriteAsync(_buffer);
        }
    }

    [GlobalCleanup]
    public void Cleanup() => File.Delete("file.txt");
}
```

```
BenchmarkDotNet=v0.13.0, OS=Windows 10.0.18363.1500 (1909/November2019Update/19H2)
Intel Xeon CPU E5-1650 v4 3.60GHz, 1 CPU, 12 logical and 6 physical cores
.NET SDK=6.0.100-preview.5.21267.9
  [Host]     : .NET 5.0.6 (5.0.621.22011), X64 RyuJIT
  Job-OIMCTV : .NET 5.0.6 (5.0.621.22011), X64 RyuJIT
  Job-CHFNUY : .NET 6.0.0 (6.0.21.26311), X64 RyuJIT
```

Method     | Runtime  | Mean      | Ratio | Allocated
-----------|----------|-----------|-------|----------
ReadAsync  | .NET 5.0 | 3.785 ms  | 1.00  | 39 KB
ReadAsync  | .NET 6.0 | 1.762 ms  | 0.47  | 1 KB
WriteAsync | .NET 5.0 | 12.573 ms | 1.00  | 39 KB
WriteAsync | .NET 6.0 | 3.200 ms  | 0.25  | 1 KB

Environment: Windows 10 with SSD drive with BitLocker enabled

Results:

* Reading 1 MB file is now 2 times faster, while writing is 4 times faster.
* Memory allocations dropped from 39 kilobytes to 1 kilobyte! This is a 97.5% improvement!

These changes should provide a dramatic improvement for FileStream users on Windows. More details are available at dotnet/core #6098.
