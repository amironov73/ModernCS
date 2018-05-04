### TextTemplate как замена T4

До сих пор я считал, что T4 намертво привязан к Vusual Studio, однако вчера узнал, что существует реализация движка для Mono, похожая на (но, к сожалению, не совпадающая с) T4, которую можно использовать в своей программе для корыстных целей. 🙂 Вот она: http://texttemplate.codeplex.com/, пакет NuGet: http://nuget.org/List/Packages/TextTemplate. Главное отличие — использование разделителей <% %> вместо стандартных <# #>.

Вот простейшая программа с использованием TextTemplate:

```csharp
using System;
using System.IO;
using TextTemplate;
 
class Program
{
    static void Main()
    {
        Template template = new Template
        {
            Content = File.ReadAllText("Template1.txt")
        };
        string result = template.Execute();
        Console.WriteLine(result);
    }
}
```

Вот более сложный пример: передача параметров в шаблон:

```csharp
using (var template = new Template<List<string>>("Names"))
{
    template.Content = @"
<%@ language C#v3.5 %>
<%@ using System.Collections.Generic %>
<%
   foreach (var name in Names)
   {
%>
Hello, <%= name %>.
<%
   }
%>
";
    var output = template.Execute(new List<string>
       {
           "Lasse",
           "Mads",
           "Anders"
       });
 
    Console.WriteLine(output);
}
```
