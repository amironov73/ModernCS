### Генераторы исходного кода

Генераторы исходного кода — это функция компилятора C#, позволяющая разработчикам C# анализировать код пользователя по мере его компиляции и создавать "на лету" новые исходные файлы C#, которые добавляются в пользовательскую компиляцию. Таким образом появляется возможность создать код, который выполняется во время компиляции и может анализировать программу для создания дополнительных исходных файлов, компилируемых вместе с остальной частью кода.

Генератор исходного кода — это новый тип компонента, который разработчики C# могут использовать, чтобы выполнять два основных действия:

* Получение объекта, представляющий весь компилируемый пользовательский код. Этот объект можно анализировать, написать код, который работает с синтаксисом и семантическими моделями компилируемого кода, как и с анализаторами.

* Создать исходные файлы C#, которые можно добавить в объект компиляции во время компиляции. Иными словами, во время компиляции кода можно указать дополнительный исходный код в качестве входных данных для компиляции.

![Схема работы генераторов кода](img/source-generator-visualization.png)

### Документация и примеры

* [docs.microsoft.com](https://docs.microsoft.com/en-us/dotnet/csharp/roslyn-sdk/source-generators-overview) официальная документация;

* [dotnet/roslyn feature design document](https://github.com/dotnet/roslyn/blob/main/docs/features/source-generators.md) описание фичи компилятора;

* [dotnet/roslyn cookbook](https://github.com/dotnet/roslyn/blob/main/docs/features/source-generators.cookbook.md) книга рецептов для Roslyn;

* [dotnet/roslyn-sdk samples](https://github.com/dotnet/roslyn-sdk/tree/main/samples/CSharp/SourceGenerators) show how to implement a source generator and use features like external package references (*inside* generators). Includes AutoNotify, Csv, Maths, Mustache, and SettingsXml;

* [SourceGeneratorPlayground](https://wengier.com/SourceGeneratorPlayground) - онлайн-песочница для генераторов. [Source repo](https://github.com/davidwengier/SourceGeneratorPlayground).

* [davidwengier/SourceGeneratorTemplate](https://github.com/davidwengier/SourceGeneratorTemplate)  - ![stars](https://img.shields.io/github/stars/davidwengier/SourceGeneratorTemplate?style=flat-square&cacheSeconds=604800) ![last commit](https://img.shields.io/github/last-commit/davidwengier/SourceGeneratorTemplate?style=flat-square&cacheSeconds=86400) Базовый шаблон для генератора на C# от разработчика Roslyn.

### Статьи

* [Mastering at Source Generators](https://medium.com/c-sharp-progarmming/mastering-at-source-generators-18125a5f3fca) (2022-01-15) Generating CRUD controller from DTO model using text template.

* [Using C# Source Generators to create an external DSL](https://devblogs.microsoft.com/dotnet/using-c-source-generators-to-create-an-external-dsl/) (2021-01-27) that shows how to implement a simple DSL.

* [4 ways to generate code in C# — Including Source Generators in .NET 5](https://levelup.gitconnected.com/four-ways-to-generate-code-in-c-including-source-generators-in-net-5-9e6817db425) (2021-01-19) demonstrates the comparison between Source Generators, T4 template and Reflection, etc.

* [.NET 5 Source Generators - MediatR - CQRS - OMG!](https://www.edument.se/en/blog/post/net-5-source-generators-mediatr-cqrs) (2020-12-16) explores how source generators can be used to automatically generate an API for a system using the MediatR library and the CQRS pattern.

* [Source Generators in .NET 5 with ReSharper](https://blog.jetbrains.com/dotnet/2020/11/12/source-generators-in-net-5-with-resharper/) (2020-11-20) introduces source generators and briefly mentions how they are being worked into the ReSharper product.

* [Source Generators - real world example](https://dominikjeske.github.io/source-generators) (2020-11-09) contains a rich and deep dive into a real world generator with lots of useful tips.

* [How to profile C# 9.0 Source Generators](https://jaylee.org/archive/2020/10/10/profiling-csharp-9-source-generators.html) (2020-10-10) demonstrates how to profile your source generator using the [performance profiling tools built into Visual Studio](https://docs.microsoft.com/en-us/visualstudio/profiling/?view=vs-2019).

* [How to Debug C# 9 Source Code Generators](https://nicksnettravels.builttoroam.com/debug-code-gen/) (2020-10-09) contains debugging tips.

* [How to generate code using Roslyn source generators in real world scenarios](https://www.cazzulino.com/source-generators.html) (2020-09-17) rich story of how ThisAssembly generator was written using Scriban templates.

* [.NET Blog 'New C# Source Generator Samples' post](https://devblogs.microsoft.com/dotnet/new-c-source-generator-samples/) (2020-08-25) that shows some simple samples.

* [.NET Blog 'Introducing C# Source Generators' post](https://devblogs.microsoft.com/dotnet/introducing-c-source-generators/) (2020-04-29) that announces the feature.
