### RestSharp

Сайт: http://restsharp.org/, NuGet: https://www.nuget.org/packages/RestSharp/, GitHub: https://github.com/restsharp/RestSharp.

* Поддерживает .NET 3.5+, Silverlight 5, Windows Phone 8, Mono, MonoTouch, Mono for Android;
* Автоматическая десериализация для XML и JSON;
* Поддерживается кастомная сериализация и десериализация через интерфейсы ISerializer и IDeserializer;
* Нечеткая логика сопоставления полей класса и элементов XML/JSON (например, product_id в XML сопоставляется ProductId в классе);
* Автоматическое определение формата, в котором вернулся ответ сервера;
* Поддерживаются GET, POST, PUT, HEAD, OPTIONS и DELETE;
* Также поддерживаются нестандартные методы HTTP;
* Поддерживается аутентификация oAuth 1, oAuth 2, Basic, NTML и через параметр;
* Также поддерживается нестандартная аутентификация через интерфейс IAuthenticator;
* Загрузка форм/файлов в т. ч. через multi-part;
* Шаблон T4 для генерации классов C# по XML-документу.

Пример, который показывает почти всё, что нужно знать о PostSharp:

```csharp
var client = new RestClient("http://example.com");
// Если нужна аутентификация на сайте
// client.Authenticator = new HttpBasicAuthenticator(username, password);
 
var request = new RestRequest("resource/{id}", Method.POST);
// adds to POST or URL querystring based on Method
request.AddParameter("name", "value"); 
// replaces matching token in request.Resource
request.AddUrlSegment("id", "123"); 
 
// easily add HTTP Headers
request.AddHeader("header", "value");
 
// add files to upload (works with compatible verbs)
request.AddFile(path);
 
// execute the request
RestResponse response = client.Execute(request);
var content = response.Content; // raw content as string
 
// or automatically deserialize result
// return content type is sniffed but can be 
// explicitly set via RestClient.AddHandler();
RestResponse response2 = client.Execute(request);
var name = response2.Data.Name;
 
// easy async support
client.ExecuteAsync(request, response => {
    Console.WriteLine(response.Content);
});
 
// async with deserialization
var asyncHandle = client.ExecuteAsync(request, response => {
    Console.WriteLine(response.Data.Name);
});
 
// abort the request on demand
asyncHandle.Abort();
```
На примере Twitter

```csharp
using System;
 
using RestSharp;
using RestSharp.Authenticators;
 
namespace TwitterTest
{
    class Program
    {
        static void Main(string[] args)
        {
            RestClient client = new RestClient
            {
                BaseUrl = new Uri("http://twitter.com"),
                Authenticator = new HttpBasicAuthenticator
                    (
                        "amironov73",
                        "НЕ ДОЖДЁТЕСЬ"
                    )
            };
 
            RestRequest request = new RestRequest
            {
                Resource = "statuses/friends_timeline.xml"
            };
 
            IRestResponse response = client.Execute(request);
            Console.WriteLine(response.Content);
            Console.ReadLine();
        }
    }
}
```

При возникновении сетевых ошибок (например, отсутствие подключения, ошибка DNS и т. д.) исключений не возникает, а устанавливается `RestResponse.Status` в значение `ResponseStatus.Error`. При этом, если сервер вернёт 404, `ResponseStatus` будет `Completed` и надо проверять `RestResponse.StatusCode`.
