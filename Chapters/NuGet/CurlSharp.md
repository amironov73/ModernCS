### CurlSharp

[cURL](https://ru.wikipedia.org/wiki/CURL) - популярная утилита командной строки, позволяющая скачивать файлы с Сети по протоколам HTTP, FTP, TFTP, SCP и прочим (фактически, она позволяет отправлять произвольные запросы по произвольным URL плюс сохранять результат на диск). Эта утилита использует библиотеку [libcurl](http://curl.haxx.se/libcurl/). Естественно, возникает вопрос, нельзя ли как-нибудь нам использовать эту библиотеку из программы на C#?

Так вот, использовать можно - это позволяет замечательный пакет CurlSharp, исходные тексты которого размещены на GitHub: https://github.com/masroore/CurlSharp, а сам пакет на NuGet: https://www.nuget.org/packages/Shaman.CurlSharp/

Точный перечень поддерживаемых CurlSharp протоколов и возможностей:

* HTTP (GET / HEAD / PUT / POST / multi-part / form-data);
* FTP (upload / download / list / 3rd-party);
* HTTPS, FTPS, SSL, TLS (с помощью OpenSSL или GnuTLS);
* Прокси, туннелирование прокси, cookies, аутентификация по логину плюс паролю;
* Возобновление передачи после обрыва связи;
* Скачивание фрагмента файла;
* Множественная асинхронная передача.

Рабочей лошадкой является класс `CurlEasy` - обёртка над сессией `curl_easy`. Также имеется класс `CurlMulti`, содержащий несколько объектов `CurlEasy` и являющийся обёрткой над сессией `curl_multi`.

Класс `CurlShare` предоставляет инфраструктуру для сериализации доступа к данным с помощью общего объекта `CurlEasy`, включая cookie и хосты DNS.

Класс `CurlHttpMultiPartForm` позволяет легко конструировать multi-part forms.

Класс `CurlSlist` обертывает список строк, применяемый в cURL.

Библиотека поддерживает .NET Framework 4.6 и .NET Standard 1.3. Необходимо скачать и положить возле EXE-файла содержимое соответствующей папки из https://github.com/masroore/CurlSharp/tree/master/libs

Простой пример HTTP GET:

```csharp
using System;
using CurlSharp;

class Program
{
  public static void Main()
  {
    Curl.GlobalInit(CurlInitFlag.All);

    try
    {
      using (var easy = new CurlEasy())
      {
        easy.Url = "http://www.google.com/";
        easy.WriteFunction = OnWriteData;
        easy.Perform();
      }
    }
    finally
    {
      Curl.GlobalCleanup();
    }
  }

  public static int OnWriteData
      (
           byte[] buf,
           int size,
           int nmemb,
           object data
      )
  {
      Console.Write(Encoding.UTF8.GetString(buf));

      return size*nmemb;
  }
}
```

Пример HTTP POST:

```csharp
using (var easy = new CurlEasy())
{
    easy.Url = "http://hostname/testpost.php";
    easy.Post = true;
    var postData = "parm1=12345&parm2=Hello+world%21";
    easy.PostFields = postData;
    easy.PostFieldSize = postData.Length;
    easy.Perform();
}
```

Загрузка с помощью HTTP/2.0:

```csharp
using (var easy = new CurlEasy())
{
    easy.Url = "https://google.com/";
    easy.WriteFunction = OnWriteData;

    // HTTP/2 please
    easy.HttpVersion = CurlHttpVersion.Http2_0;

    // skip SSL verification during debugging
    easy.SslVerifyPeer = false;
    easy.SslVerifyhost = false;

    easy.Perform();
}
```

Множество примеров см. https://github.com/masroore/CurlSharp/tree/master/Samples
