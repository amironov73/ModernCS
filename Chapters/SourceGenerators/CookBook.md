# Книга рецептов

https://github.com/dotnet/roslyn/blob/main/docs/features/source-generators.cookbook.md

### Обработка дополнительных файлов

```csharp
[Generator]
public class FileTransformGenerator : ISourceGenerator
{
    public void Initialize (GeneratorInitializationContext context) {}

    public void Execute (GeneratorExecutionContext context)
    {
        // find anything that matches our files
        var myFiles = context.AdditionalFiles.Where (at => at.Path.EndsWith(".xml"));
        foreach (var file in myFiles)
        {
            var content = file.GetText (context.CancellationToken);

            // do some transforms based on the file context
            string output = MyXmlToCSharpCompiler.Compile (content);

            var sourceText = SourceText.From (output, Encoding.UTF8);

            context.AddSource ($"{file.Name}_generated.cs", sourceText);
        }
    }
}
```

### Вывод диагностики

```csharp
[Generator]
public class MyXmlGenerator : ISourceGenerator
{

    private static readonly DiagnosticDescriptor InvalidXmlWarning = new DiagnosticDescriptor
        (
            id: "MYXMLGEN001",
            title: "Couldn't parse XML file",
            messageFormat: "Couldn't parse XML file '{0}'.",
            category: "MyXmlGenerator",
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true
        );

    public void Execute (GeneratorExecutionContext context)
    {
        // Using the context, get any additional files that end in .xml
        IEnumerable<AdditionalText> xmlFiles = context.AdditionalFiles.Where
            (
                at => at.Path.EndsWith (".xml", StringComparison.OrdinalIgnoreCase)
            );
        foreach (AdditionalText xmlFile in xmlFiles)
        {
            XmlDocument xmlDoc = new XmlDocument();
            string text = xmlFile.GetText (context.CancellationToken).ToString();
            try
            {
                xmlDoc.LoadXml (text);
            }
            catch (XmlException)
            {
                // issue warning MYXMLGEN001: Couldn't parse XML file '<path>'
                context.ReportDiagnostic (Diagnostic.Create (InvalidXmlWarning, Location.None, xmlFile.Path));
                continue;
            }

            // continue generation...
        }
    }

    public void Initialize (GeneratorInitializationContext context)
    {
    }
}
```

### Как включить генератор в NuGet-пакет

```msbuild
<Project>
    
  <PropertyGroup>
    <GeneratePackageOnBuild>true</GeneratePackageOnBuild> <!-- Generates a package at build -->
    <IncludeBuildOutput>false</IncludeBuildOutput> <!-- Do not include the generator as a lib dependency -->
  </PropertyGroup>

  <ItemGroup>
    <!-- Package the generator in the analyzer directory of the nuget package -->
    <None Include="$(OutputPath)\$(AssemblyName).dll" Pack="true" PackagePath="analyzers/dotnet/cs" Visible="false" />
  </ItemGroup>
    
</Project>
```

### Как использовать генератор из NuGet-пакета


```msbuild
<Project>
    
  <PropertyGroup>
    <GeneratePackageOnBuild>true</GeneratePackageOnBuild> <!-- Generates a package at build -->
    <IncludeBuildOutput>false</IncludeBuildOutput> <!-- Do not include the generator as a lib dependency -->
  </PropertyGroup>

  <ItemGroup>
    <!-- Take a public dependency on Json.Net. Consumers of this generator will get a reference to this package -->
    <PackageReference Include="Newtonsoft.Json" Version="12.0.1" />

    <!-- Package the generator in the analyzer directory of the nuget package -->
    <None Include="$(OutputPath)\$(AssemblyName).dll" Pack="true" PackagePath="analyzers/dotnet/cs" Visible="false" />
  </ItemGroup>

</Project>
```

### Как проверить, что подключен нужный NuGet-пакет

```csharp
using System.Linq;

[Generator]
public class SerializingGenerator : ISourceGenerator
{
    public void Execute (GeneratorExecutionContext context)
    {
        // check that the users compilation references the expected library 
        if (!context.Compilation.ReferencedAssemblyNames.Any
            (
                ai => ai.Name.Equals ("Newtonsoft.Json", StringComparison.OrdinalIgnoreCase))
            )
        {
            context.ReportDiagnostic (/*error or warning*/);
        }
    }

    public void Initialize(GeneratorInitializationContext context)
    {
    }
}
```
