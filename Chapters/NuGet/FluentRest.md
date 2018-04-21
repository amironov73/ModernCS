### FluentRest — HttpClient стал удобным

* Fluent request building
* Fluent form building
* Automatic deserialization of response
* Plugin different serialization
* Fake HTTP responses

Водится на NuGet: https://nuget.org/packages/FluentRest, GitHub: http://github.com/loresoft/FluentRest, страничка автора: http://www.loresoft.com/, используется так:

```csharp
using System;
 
using FluentRest;
using Newtonsoft.Json.Linq;
 
class Program
{
    static async void Test1()
    {
        var client = new FluentClient
        {
            BaseUri = new Uri
              ( 
                "http://md5.jsontest.com/", 
                 UriKind.Absolute
              )
        };

        var result = await client.GetAsync<JObject>
            (
                b => b.QueryString("text", "sample text")
            );
        Console.WriteLine(result);
    }

    static void Main(string[] args)
    {
        Test1();
        Console.ReadLine();
    }
}
```

Напечатает следующее:

```javascript
{
  "md5": "70ee1738b6b21e2c8a43f3a5ab0eee71",
  "original": "sample text"
}
```
