### Crossgen2

https://devblogs.microsoft.com/dotnet/conversation-about-crossgen2/

Crossgen2 is a replacement of the crossgen tool. It is intended to satisfy two outcomes: make crossgen development more efficient and enable a set of capabilities that are not currently possible with crossgen. This transition is somewhat similar to the native code csc.exe to managed-code Roslyn-based compiler (if you recall the Roslyn project). Crossgen2 is written in C#, however, it doesn’t expose a fancy API like Roslyn does.

The PGO plans we have are contingent on crossgen2, as are other plans that affect ready-to-run code generation. There are perhaps a half-dozen projects we have planned for .NET 6 that are dependent on crossgen2. I’ll be describing those in later previews. Suffice to say, crossgen2 is a very important project.

Crossgen2 will enable cross-compilation (hence the name “crossgen”) across operating system and architecture dimensions. That means that you will be able to use a single build machine to generate native code for all targets, at least as it relates to ready-to-run code. Running and testing that code is a different story, however, and you’ll need appropriate hardware and operating systems for that.

The first step (and test) for us is to compile the platform itself with crossgen2. The System.Private.Corelib assembly is compiled by crossgen2 in Preview 1. In subsequent previews, we will target progressively broader parts of the product until the entire .NET SDK is compiled with crossgen2 for all platforms and architectures. That’s our goal for the release and a necessary pre-condition for retiring the existing crossgen. Note that crossgen2 only applies to CoreCLR and not to Mono-based applications (which have a separation set of native code tools).

This project — at least at first — is not oriented on performance. The goal is to enable a much better architecture for hosting the RyuJIT (or any other) compiler to generate code in an offline manner (not requiring or starting the runtime). In our early testing, we haven’t observed any performance differences, which is exactly what is expected.

You might say “hey … don’t you have to start the runtime to run crossgen2 if it is written in C#?” Yes, but that’s not what is meant by “offline” in this context. When crossgen2 runs, we’re not using the JIT that comes with the runtime that crossgen2 is running on to generate ready-to-run (R2R) code. That won’t work, at least not with the goals we have. Imagine crossgen2 is running on an x64 machine, and we need to generate code for Arm64. Crossgen2 loads the Arm64 RyuJIT — compiled for x64 — as a native plugin, and then uses it to generate Arm64 R2R code. The machine instructions are just a stream of bytes that are saved to a file. It can also work in the opposite direction. On Arm64, crossgen2 can generate x64 code using the x64 RyuJIT compiled to Arm64. We use the same approach to target x64 code on x64 machines. Crossgen2 loads a RyuJIT built for that configuration. That may seem complicated, but it’s the sort of system you need in place if you want to enable seamless user experiences, and that’s exactly what we want.

For now, the SDK still defaults to using the existing crossgen. The default will change to crossgen2 in an upcoming preview.

We hope to use the term “crossgen2” for just one release, after which it will replace the existing crossgen, and then we’ll go back to using the “crossgen” term for “crossgen2”. In the rare case we need to, we’ll say “the old crossgen” or “crossgen1” to refer to the old one.
