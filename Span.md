### Span&lt;T&gt;

`System.Span<T>` — новый тип данных, появившийся в .NET Core (и, к сожалению, недоступный в классическом .NET). Он предназначен для (управляемого!) представления регионов памяти с произвольным доступом, скорость которого близка к обычному `T[]`.

Тип проживает в сборке `System.Memory`, взять которую можно на dotnet.myget.org.

Сначала о том, как добавить dotnet.myget.org к репозиториям NuGet. По-моему, самый простой способ — положить в папку проекта файл NuGet.Config следующего содержания:
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
В файл проекта добавляем ссылку на пакет `System.Memory`:
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
Наконец, сам текст программы:
```csharp
using System;
 
class Program
{
    static void Main()
    {
        int[] array = { 1, 2, 3 };
        // делаем "срез" массива
        var span = new Span<int>(array, start: 1, length: 2);
 
        Console.WriteLine(span[1]); // напечатает "3"
    }
}
```
Теперь попробуем провернуть то же с неуправляемой памятью. (Не забываем добавить в проект магическую строчку «`<AllowUnsafeBlocks>True</AllowUnsafeBlocks>`»)
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
            // заполняем массив чем-нибудь
            span[i] = (byte)i;
        }
        Console.WriteLine(span[1]); // напечатает "1"
        Marshal.FreeHGlobal(nativeMemory);
    }
}
```
Также можно создавать `Span` в стековой памяти:
```csharp
// stack memory
Span<byte> span;
unsafe
{
    byte* stackMemory = stackalloc byte[100];
    span = new Span<byte>(stackMemory, 100);
}
// делаем что-нибудь со span
```
Что из себя представляет `Span<T>`:
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
Имеется также `ReadOnlySpan<T>`, который умеет всё то же, кроме записи в него данных.

Важное ограничение: `Span<T>` живёт только на стеке, т. е. не может быть полем структуры или класса!

* * *

С выходом .NET FW 4.7.1 всё это дело без приседаний стало работать в классическом фреймворке.
