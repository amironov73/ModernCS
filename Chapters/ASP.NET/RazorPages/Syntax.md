### Синтаксис Razor

Добавим в наше приложение страницу `About`:

```
dotnet new page --name About --output Pages --no-pagemodel
```

Модель для данной страницы не нужна. Содержимое страницы таково:

```html
@page

<p>Некие сведения об организации.</p>
```

Razor-страницы состоят из HTML-разметки, дополненной символом "собака", который

1. позволяет задавать директивы вроде вышеприведенной `@page`,
2. позволяет вставлять произвольный C#-код.

По умолчанию Razor импортирует в каждую страницу следующие пространства имен (содержимое служебного файла `WebApplication1.GlobalUsings.g.cs`):

```csharp
global using global::Microsoft.AspNetCore.Builder;
global using global::Microsoft.AspNetCore.Hosting;
global using global::Microsoft.AspNetCore.Http;
global using global::Microsoft.AspNetCore.Routing;
global using global::Microsoft.Extensions.Configuration;
global using global::Microsoft.Extensions.DependencyInjection;
global using global::Microsoft.Extensions.Hosting;
global using global::Microsoft.Extensions.Logging;
global using global::System;
global using global::System.Collections.Generic;
global using global::System.IO;
global using global::System.Linq;
global using global::System.Net.Http;
global using global::System.Net.Http.Json;
global using global::System.Threading;
global using global::System.Threading.Tasks;
```

Редактировать этот файл бессмысленно, лучше завести собственный файл `GlobalUsings.cs` и дописать в него нужные директивы, либо дополнить файл проекта соответствующими директивами:

```xml
<Project>
    <ItemGroup>
        <Using Include="System.Math" Static="True" />
        <Using Include="Spectre.Console.AnsiConsole" Alias="Console" />
    </ItemGroup>

    <ItemGroup>
        <Using Remove="System.Collections" />
    </ItemGroup>
</Project>
```

Кроме того, в файле `_ViewImports.cshtml` имеются следующие строки

```html
@using Microsoft.AspNetCore.Identity
@using WebApplication1
@using WebApplication1.Data
@namespace WebApplication1.Pages
@addTagHelper *, Microsoft.AspNetCore.Mvc.TagHelpers
```

и вот этот файл можно редактировать сколько душе угодно.

Конструкции Razor бывают двух типов: однострочные выражения и блоки кода. Однострочники очень просты и удобны для использования

```html
<p>Сейчас в Иркутске @Model.Temperature градусов Цельсия</p>
```

Если в однострочнике имеются пробелы или другие символы, могущие нарушить совместимость с HTML, то достаточно заключить его в круглые скобки:

```html
<p>Результат вычислений равен @(1 + 2 * 3)</p>
```

Чтобы экранировать символ `@`, достаточно удвоить его:

```html
<p>Директива @@using используется для подключения пространства имен.</p>
```

Блок кода начинается `@{`, заканчивается `}` и может иметь произвольную сложность. В принципе, ничто не мешает нам запихать в блок кода почти всю программу, но лучше так не делать. Сложную логику лучше выносить в отдельные cs-файлы. Тем не менее, внутри блока кода могут помещаться переменные, методы, классы. Использовать их можно за пределами блока, в соответствии с правилами видимости имен C#.

```html
@{
    var person = new Person { Name = "Алексей", Age = 50 };

    class Person 
    {
        public string Name { get; set; }
        public int Age { get; set; }
    }
}

<p>Имя: @person.Name</p>
<p>Возраст: @person.Age</p>
```

В виде блоков оформляются конструкции `if/else`, `for`, `foreach` и аналогичные

```html
<ul>
@foreach (var person in staff)
{
    <li>Имя: @person.Name, возраст: @person.Age</li>
}
</ul>

@for (var i = 1; i <= 10; i++)
{
    <p>Причина номер @i: ...</p>
}
```

Если нам нужно вывести простой текст (не в тегах), то требуется небольшое ухищрение

```html
@if (i > 0)
{
    @: больше нуля
}
```

Директива `@functions` позволяет определить функции, доступные в представлении

```html
@functions
{
    public double CircleArea (double radius) => 3.14 * radius * radius;
}

<p>Площадь круга с радиусом, равным 1, составляет @CircleArea(1).</p>
<p>А с радиусом, равным 2, @CircleArea(2).</p>
```

Наконец, нам доступны Razor-комментарии `@* ... *@`:

```html
@using SomeNamespace @* очень важное пространство имен! *@
```
