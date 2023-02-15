### Создание Razor-приложения вручную

Продолжаем наш сложный путь. Создаем пустое веб-приложение

```
dotnet new web
```

Получаем `Program.cs` следующего содержания

```csharp
var builder = WebApplication.CreateBuilder (args);
var app = builder.Build();
 
app.MapGet ("/", () => "Hello World!");
 
app.Run();
```

Это ещё не Razor-приложение. Как нам получить минимальное веб-приложение с Razor Pages? Элементарно! Надо подключить соответствующие сервисы плюс создать хотя бы одну Razor-страницу (и заодно выбросить ненужный `MapGet`):

```csharp
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorPages();
 
var app = builder.Build();
app.MapRazorPages();
 
app.Run();
```

Теперь создаем пустую Razor-страницу

```
dotnet new page --name Index --output Pages
```

и наполняем ее глубоким смыслом:

```html
@page
@model MyApp.Namespace.IndexModel
@{
}
 
<h3>Hello, Razor</h3>
<p>Now: @DateTime.Now</p>
```

Запускаем и любуемся:

```
dotnet run
```

Выглядит не очень, но вполне работоспособно. Улучшить внешний вид довольно легко: достаточно создать директорию wwwroor и наполнить ее полезными вещами вроде Bootstrap, JQuery и `favicon.ico`, а также не забыть вызвать `app.UseStaticFiles()`

```html
@page
@model MyApp.Namespace.IndexModel
@{
}
 
<link rel="stylesheet" href="~/lib/bootstrap/dist/css/bootstrap.min.css"/>
 
<div class="container">
  <h3>Hello, Razor</h3>
  <p>Now: @DateTime.Now</p>
</div>
```

Минимальная "красота" наведена.
