### Span&lt;T&gt;

`System.Span<T>` � ����� ��� ������, ����������� � .NET Core (�, � ���������, ����������� � ������������ .NET). �� ������������ ��� (������������!) ������������� �������� ������ � ������������ ��������, �������� �������� ������ � �������� `T[]`.

��� ��������� � ������ `System.Memory`, ����� ������� ����� �� dotnet.myget.org.

������� � ���, ��� �������� dotnet.myget.org � ������������ NuGet. ��-�����, ����� ������� ������ � �������� � ����� ������� ���� NuGet.Config ���������� ����������:
```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
<packageSources>
<add key="dotnet-core"
   value="https://dotnet.myget.org/F/dotnet-core/api/v3/index.json"
   protocolVersion="3" />
</packageSources>
</configuration>
```
� ���� ������� ��������� ������ �� ����� `System.Memory`:
```xml
<Project Sdk="Microsoft.NET.Sdk">
 
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>netcoreapp1.1</TargetFramework>
  </PropertyGroup>
 
  <ItemGroup>
    <PackageReference Include="System.Memory"
      Version="4.4.0-preview1-25205-01" />
  </ItemGroup>
 
</Project>
```
�������, ��� ����� ���������:
```csharp
using System;
 
class Program
{
    static void Main()
    {
        int[] array = { 1, 2, 3 };
        // ������ "����" �������
        var span = new Span<int>(array, start: 1, length: 2);
 
        Console.WriteLine(span[1]); // ���������� "3"
    }
}
```
������ ��������� ���������� �� �� � ������������� �������. (�� �������� �������� � ������ ���������� ������� �`<AllowUnsafeBlocks>True</AllowUnsafeBlocks>`�)
```csharp
using System;
using System.Runtime.InteropServices;
 
class Program
{
    static void Main(string[] args)
    {
        var nativeMemory = Marshal.AllocHGlobal(100);
        Span<byte> span;
        unsafe
        {
            span = new Span<byte>(nativeMemory.ToPointer(), 100);
        }
        for ( int i = 0; i < 100; i++ )
        {
            // ��������� ������ ���-������
            span[i] = (byte)i;
        }
        Console.WriteLine(span[1]); // ���������� "1"
        Marshal.FreeHGlobal(nativeMemory);
    }
}
```
����� ����� ��������� `Span` � �������� ������:
```csharp
// stack memory
Span<byte> span;
unsafe
{
    byte* stackMemory = stackalloc byte[100];
    span = new Span<byte>(stackMemory, 100);
}
// ������ ���-������ �� span
```
��� �� ���� ������������ `Span<T>`:
```csharp
public struct Span<T> 
{
    public Span(T[] array)
    public Span(T[] array, int index)
    public Span(T[] array, int index, int length)
    public unsafe Span(void* memory, int length)
 
    public static implicit operator Span<T> (ArraySegment<T> arraySegment);
    public static implicit operator Span<T> (T[] array);
 
    public int Length { get; }
    public ref T this[int index] { get; }
 
    public Span<T> Slice(int index);
    public Span<T> Slice(int index, int length);
    public bool TryCopyTo(T[] destination);
    public bool TryCopyTo(Span<T> destination);
 
    public T[] ToArray();
}
```
������� ����� `ReadOnlySpan<T>`, ������� ����� �� �� ��, ����� ������ � ���� ������.

������ �����������: `Span<T>` ���� ������ �� �����, �. �. �� ����� ���� ����� ��������� ��� ������!

* * *

� ������� .NET FW 4.7.1 �� ��� ���� ��� ���������� ����� �������� � ������������ ����������.
