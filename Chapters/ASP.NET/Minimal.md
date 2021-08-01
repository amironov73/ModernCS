### Минимальный пример Web API на ASP.NET Core

[Меньше 10 строк:](https://dotnetcoretutorials.com/2021/07/16/building-minimal-apis-in-net-6/)

```c#
using System;
using Microsoft.AspNetCore.Builder;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", (Func<string>)(() => "Hello World!"));

app.Run();
```
