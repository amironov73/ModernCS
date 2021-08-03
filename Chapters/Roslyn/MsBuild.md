### Настройки компилятора в файле проекта

#### Выходные файлы

```msbuild
<Project>
    <PropertyGroup>
        <OutputAssembly>folder</OutputAssembly>
        <PlatformTarget>net5.0</PlatformTarget>
        <PlatformTarget>anycpu</PlatformTarget>
        <ProduceReferenceAssembly>filepath</ProduceReferenceAssembly>
        <TargetType>library</TargetType>
        <DocumentationFile>bin\Debug\ManagedIrbis5.xml</DocumentationFile>
    </PropertyGroup>
</Project>
```

#### Входные файлы

```msbuild
<Project>
    <ItemGroup>
        <Compile Include="Source1.cs" />
        <Compile Include="Another*.cs" />
        <Compile Include="Sources\**\*.cs" />
        <Reference Include="first.dll" />
        <Reference Include="second.dll">
            <Aliases>SE</Aliases>
        </Reference>
        <AddModule Include="module1.netmodule" />
        <EmbedInteropTypes>file1;file2;file3</EmbedInteropTypes>        
    </ItemGroup>

    
</Project>
```

https://docs.microsoft.com/en-us/dotnet/core/project-sdk/msbuild-props
