### Модель страницы

Как правило, Razor-страницы создаются в паре с моделью, причем имя модели это имя страницы плюс суффикс `Model`. Но при необходимости мы можем использовать любое другое валидное имя класса, либо же вообще обойтись без модели.

Класс модели должен наследовать от абстрактному `PageModel`:

```csharp
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApplication1.Pages
{
    public class IndexModel: PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
        }
    }
}
```

По умолчанию класс содержит пустой метод `OnGet`, который вызывается рантаймом в момент загрузки страницыы методом `GET`. Мы можем добавить в этот метод свой код, если хотим задать реакцию модели на указанную ситуацию, либо просто удалить этот метод, если не испытываем необходимости в нем.

Мы можем дополнять модель по своему усмотрению

```csharp
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApplication1.Pages
{
    public class IndexModel: PageModel
    {
        public string Place { get; set; }

        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
            Place = "Иркутск";
        }

        public string Now() => DateTime.Now.ToShortTimeString();
    }
}
```

и потом использовать наши доработки на Razor-странице

```html
<p>В городе @Model.Place сейчас @Model.Now()</p>
```

Методы `OnGet`/`OnPost` могут принимать параметры, которые будут автоматически подставлены рантаймом

```csharp
public class IndexModel: PageModel
{
    public string? Place { get; set; }

    public void OnGet (string? place)
    {
        Place = place ?? "Иркутск";
    }
}
```

Если теперь загрузить страницу по адресу `http://localhost:1234/?place=Братск`, то на странице будет ведено `В городе Братск сейчас...`.

Само собой, метод `OnGet`/`OnPost` может принимать столько параметров, сколько понадобится.

Попробуем передать в модель данные формы. Сначала определим модель

```csharp
public class IndexModel: PageModel
{
    public string? Name { get; set; }
    public string? Place { get; set; }

    public void OnPost (string? name, string? place)
    {
        Name = name;
        Place = place;
    }
}
```

потом страницу

```html
@page
@model IndexModel

<form method="post">
    <label>Имя: </label>
    <input type="text" name="name"/>
    <br/>
    <label>Город: </label>
    <input type="text" name="place" />
    <br/>
    <input type="submit"/>
</form>

<p>Было введено имя "@Model.Name" и место "@Model.Place".</p>
```

Вместо метода `OnPost` можно использовать атрибут `BindProperty`, который заставляет рантайм находить и присваивать значения свойств.

```csharp
public class IndexModel: PageModel
{
    [BindProperty]
    public string? Name { get; set; }

    [BindProperty]
    public string? Place { get; set; }
}
```

Атрибут `BindProperty` можно довольно гибко настраивать:

```csharp
public class IndexModel: PageModel
{
    [BindProperty (SupportsGet=true, Name="id")]
    public string? Name { get; set; }
}
```

Обратите внимание, что атрибут `BindProperty` не отменяет методы `OnGet`/`OnPost`, их вполне можно использовать совместно.

Важно понимать, что - класс модели Razor для валидации полученных форм использует специальный токен `AntiforgeryToken`. Это поведение можно отключить с помощью атрибута `IgnoreAntiforgeryToken`, примененнного к классу объекта или сконфигурировать при добавлении Razor-сервисов:

```csharp
builder.Services.AddRazorPages(options =>
{
    // отключаем глобально Antiforgery-токен
    options.Conventions.ConfigureFilter (new IgnoreAntiforgeryTokenAttribute());
});
```

Модели поддерживают сложные объекты, оформленные в виде класса:

```csharp
public class IndexModel: PageModel
{
    public Person? Who { get; set; }

    public void OnGet (Person? who)
    {
        Who = who;
    }
}

public record class Person (string Name, int Age);
```

Метод `OnGet`/`OnPost` можно описать непосредственно на странице, а не в модели

```html
@page

<h2>@Message</h2>

@functions {
    public string? Message { get; set; };

    public void OnGet (string name, int age)
    {
        Message = $"Name: {name}  Age: {age}";
    }
}
```

Данные в модель могут также попадать из данных о роутинге http://localhost:1234/Index/1234:

```html
@page "{id?}"
@model IndexModel

<p>Идентификатор: "@Model.Id"</p>
```

```csharp
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApplication1.Pages;

public class IndexModel: PageModel
{
    public string? Id { get; set; }

    public void OnGet()
    {
        Id = (string?) RouteData.Values["Id"];
    }
}
```

или так:

```csharp
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApplication1.Pages;

public class IndexModel: PageModel
{
    public string? Id { get; set; }

    public void OnGet (string? id)
    {
        Id = id;
    }
}
```

или так:

```csharp
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApplication1.Pages;

public class IndexModel: PageModel
{
    [BindProperty (SupportsGet=true)]
    public string? Id { get; set; }
}
```

Результат, как вы понимаете, будет одинаковый.

На параметры, передаваемые через роутинг, можно накладывать ограничения:

```html
@page "{name:alpha:minlength(3)}/{age:int}"
@model PersonModel
 
<p>Name = @Model.Name</p>
<p>Age = @Model.Age</p>
```

#### Обработчкии

```csharp
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApplication1.Pages;

public class IndexModel: PageModel
{
    public string? Message { get; set; }

    public void OnGet()
    {
        Message = "OnGet()";
    }

    public void OnGetByName (string name)
    {
        Message = $"OnGetByName({name})";
    }

    public void OnGetById (string id)
    {
        Message = $"OnGetById({id})";
    }
}
```

```html
@page
@model IndexModel

<p>Сообщение: "@Model.Message"</p>
```

Теперь при вызове страницы http://localhost:1234/?handler=byname&name=Alexey будет отображено `Сообщение: "OnGetByName(Alexey)"`

Обработчик можно передать как элемент роутинга:

```html
@page "{handler?}"
@model IndexModel

<p>Сообщение: "@Model.Message"</p>
```

Модель остается прежней, а вызов страницы трансформируется в http://localhost:123/byname/?name=Alexey.

#### Возврат результата

Выше мы использовали метод `void OnGet()`, возвращающий `void`, что с точки зрения рантайма эквивалентно `return Page()`, т. е. простому отображению страницы, ассоциированной с моделью. Чаще всего это именно то, что нужно. Но бывает, что нужно нестандартное поведение, например, перенаправить пользователя на другую страницу. В этом случае необходимо возвращать `IActionResult`. На этот случай в классе `PageModel` предусмотрены методы

* **Content()** возвращает объект `ContentResult`, то есть фактически некоторое текстовое содержимое,

* **File()** возвращает с помощью различных перегруженных версий объекты `FileContentResult`/`FileStreamResult`/`VirtualFileResult`, то есть отправляет клиенту файл,

* **Forbid()** возвращает статусный код 403,

* **LocalRedirect()**/**LocalRedirectPermanent()** выполняет переадресацию по определенному локальному адресу,

* **NotFound()** возвращает статусный код 404,

* **PhysicalFile()** возвращает файл по физическому пути,

* **Page()** возвращает объект `PageResult` или фактически текущую страницу Razor,

* **Redirect()**/**RedirectPermanent()** выполняет переадресацию по определенному адресу,

* **RedirectToAction()**/**RedirectToActionPermanent()** выполняет переадресацию на определенное действие контроллера,

* **RedirectToPage()**/**RedirectToPagePermanent()** выполняет переадресацию на определенную страницу Razor,

* **RedirectToRoute()**/**RedirectToRoutePermanent()** выполняет переадресацию по определенному маршруту,

* **StatusCode()** возвращает объект `StatusCodeResult`, то есть посылает статусный код,

* **Unauthorized()** возвращает объект `UnauthorizedResult`, то есть статусный код ошибки 401.

Для отправки json нет определенного метода, но определен специальный тип `JsonResult`, в конструктор которого можно передавать сериализованный в json объект.

### Переадресация

Для переадресации на страницу Razor применяются методы `RedirectToPage()` и `RedirectToPagePermanent()`. Метод `RedirectToPage()` выполняет временную переадресацию, отправляя статусный код 302. А метод `RedirectToPagePermanent()` выполняет постоянную переадресацию и отправляет статусный код 301.

Рассмотрим варианты метода `RedirectToPage()`:

* **RedirectToPage()((: выполняет переадресацию на текущую страницу, отправляя статусный код 302

* **RedirectToPage(object routeValues)**: выполняет переадресацию на текущую страницу, отправляя статусный код 302 и объект с параметрами для маршрутов

* **RedirectToPage(string pageName)**: выполняет переадресацию на определенную страницу

* **RedirectToPage(string pageName, object routeValues)**: выполняет переадресацию на определенную страницу, отправляя объект с параметрами маршрута

* **RedirectToPage(string pageName, string pageHandler)**: выполняет переадресацию на определенную страницу pageName и обращается к ее обработчику pageHandler

* **RedirectToPage(string pageName, string pageHandler, string fragment)**: выполняет переадресацию на определенную страницу pageName и обращается к ее обработчику pageHandler, добавляя к адресу переадресации фрагмент fragment

* **RedirectToPage(string pageName, string pageHandler, object routeValues, string fragment)**: выполняет переадресацию на определенную страницу pageName и обращается к ее обработчику pageHandler, добавляя к адресу переадресации фрагмент fragment и отправляя объект с параметрами маршрута

Метод `RedirectToPagePermanent()` имеет аналогичные версии.

Пример передачи параметров роутинга:

```csharp
public IActionResult OnGet()
{
    return RedirectToPage ("/New", new { name="Sam", age=28 });
}
```
