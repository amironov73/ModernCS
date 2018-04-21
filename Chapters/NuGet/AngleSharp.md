### Угловатый и пошарпаный: AngleSharp

AngleSharp – библиотека для парсинга и обработки веб-документов в форматах HTML, SVG, MathML и XML (последний, правда, без валидации схемы). Немаловажно, что CSS тоже поддерживается, т. е. нам не придётся мучительно придумывать способы для отбора элементов документа в соответствии с правилами CSS.

Важнейшие особенности:

* Библиотека полностью управляемая, портабельная, т. е. пригодная к применению на множестве платформ без доработок и условной компиляции
* Соответствует стандартам (воспроизводит поведение современных браузеров)
* Высокое быстродействие
* Расширяемая
* Содержит полезные абстракции
* Полностью функциональная DOM
* Поддерживается заполнение и отправка форм
* Навигация
* LINQ (в том числе LINQ по DOM!)

Библиотека проживает в NuGet: https://www.nuget.org/packages/AngleSharp/ и в GitHub https://github.com/AngleSharp/AngleSharp.

Авторы заявляют поддержку следующих платформ:

* .NET Framework 4.5 и 4.0 (с доустановкой Microsoft.Bcl.Async)
* Silverlight 5
* Windows 8/8.1/10
* Windows Phone 8.1 / Windows Phone Silverlight
* Xamarin.Android
* Xamarin.iOS

Пример кода:

```csharp
using AngleSharp;
using AngleSharp.Dom;
 
...
 
const string address 
    = "https://en.wikipedia.org/wiki/List_of_The_Big_Bang_Theory_episodes";
IConfiguration configuration = Configuration.Default.WithDefaultLoader();
IDocument document = await BrowsingContext.New(configuration)
    .OpenAsync(address);
const string cellSelector = "tr.vevent td:nth-child(3)";
IHtmlCollection<IElement> cells = document.QuerySelectorAll(cellSelector);
IEnumerable<string> titles = cells.Select(m => m.TextContent);
foreach (string title in titles)
{
    Console.WriteLine(title);
}
```

На экран будет выведено:

```
"Pilot"
"The Big Bran Hypothesis"
"The Fuzzy Boots Corollary"
...
"The Opening Night Excitation"
"The Sales Call Sublimation"[206]
```

##### Пример простых манипуляций с HTML-документом

```csharp
var parser = new HtmlParser();
var document = parser.Parse("<h1>Some example source</h1>"
    + "<p>This is a paragraph element");
//Do something with document like the following
 
Console.WriteLine("Serializing the (original) document:");
Console.WriteLine(document.DocumentElement.OuterHtml);
 
var p = document.CreateElement("p");
p.TextContent = "This is another paragraph.";
 
Console.WriteLine("Inserting another element in the body ...");
document.Body.AppendChild(p);
 
Console.WriteLine("Serializing the document again:");
Console.WriteLine(document.DocumentElement.OuterHtml);
```

Обратите внимание, библиотека корректно обрабатывает незакрытые теги.

##### Пример Linq to DOM и CSS-селекторов

```csharp
var parser = new HtmlParser();
var document = parser.Parse("<ul><li>First item"
    + "<li>Second item<li class='blue'>Third item!"
    + "<li class='blue red'>Last item!</ul>");
 
//Do something with LINQ
var blueListItemsLinq = document.All
    .Where(m => m.LocalName == "li"
    && m.ClassList.Contains("blue"));
 
//Or directly with CSS selectors
var blueListItemsCssSelector 
    = document.QuerySelectorAll("li.blue");
 
Console.WriteLine("Comparing both ways ...");
 
Console.WriteLine();
Console.WriteLine("LINQ:");
 
foreach (var item in blueListItemsLinq)
    Console.WriteLine(item.Text());
 
Console.WriteLine();
Console.WriteLine("CSS:");
 
foreach (var item in blueListItemsCssSelector)
    Console.WriteLine(item.Text());
```

Ещё пример:

```csharp
var parser = new HtmlParser();
var document = parser.Parse("<b><i>"
    + "This is some <em> bold <u>"
    + "and</u> italic </em> text!<"
    + "/i></b>");
var emphasize = document.QuerySelector("em");
 
Console.WriteLine("Difference between several "
    + "ways of getting text:");
Console.WriteLine();
Console.WriteLine("Only from C# / AngleSharp:");
Console.WriteLine();
Console.WriteLine(emphasize.ToHtml());  //<em> bold <u>and</u> italic </em>
Console.WriteLine(emphasize.Text());   //boldanditalic
 
Console.WriteLine();
Console.WriteLine("From the DOM:");
Console.WriteLine();
Console.WriteLine(emphasize.InnerHtml); // bold <u>and</u> italic
Console.WriteLine(emphasize.OuterHtml); //<em> bold <u>and</u> italic </em>
Console.WriteLine(emphasize.TextContent);// bold and italic 
```

Библиотека довольно активно развивается, есть надежда, что со временем она станет ещё лучше.

Статья с обзором: http://habrahabr.ru/post/273807/
