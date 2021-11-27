### CORS

Cross-origin resource sharing (CORS; с англ. — «совместное использование ресурсов между разными источниками») — технология современных браузеров, которая позволяет предоставить веб-страницам доступ к ресурсам другого домена.

Если сервер `www.b.com` разрешает получение данных с `www.a.com`, то в ответе сервера будет присутствовать строка:

```
Access-Control-Allow-Origin: http://www.a.com
```

Если в ответе сервера отсутствует данная строка, то браузер, поддерживающий технологию CORS, вернёт код ошибки вместо данных.

В случае, если сервер хочет разрешить доступ для страниц с любого домена, он может указать в ответе:

```
Access-Control-Allow-Origin: *
```

Если сервер хочет разрешить доступ более чем одному домену, то в ответе сервера должно быть по одной строчке `Access-Control-Allow-Origin` для каждого домена.

```
Access-Control-Allow-Origin: http://www.a.com
Access-Control-Allow-Origin: http://www.b.com
Access-Control-Allow-Origin: http://www.c.com
```

На практике чаще используется запись из нескольких доменов, разделенных пробелом:

```
Access-Control-Allow-Origin: http://www.a.com http://www.b.com http://www.c.com
```

### В ASP.NET Core

https://docs.microsoft.com/en-us/aspnet/core/security/cors?view=aspnetcore-5.0

В ASP.NET Core можно настроить CORS следующими методами:

* In middleware using a named policy or default policy.
* Using endpoint routing.
* With the `[EnableCors]` attribute.

#### CORS with named policy and middleware

```c#
public class Startup
{
    readonly string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy(name: MyAllowSpecificOrigins,
                              builder =>
                              {
                                  builder.WithOrigins("http://example.com",
                                                      "http://www.contoso.com");
                              });
        });

        // services.AddResponseCaching();
        services.AddControllers();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseRouting();

        app.UseCors(MyAllowSpecificOrigins);

        // app.UseResponseCaching();

        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
}
```

#### CORS with default policy and middleware

```c#
public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddDefaultPolicy(
                builder =>
                {
                    builder.WithOrigins("http://example.com",
                                        "http://www.contoso.com");
                });
        });

        services.AddControllers();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseRouting();

        app.UseCors();

        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
}
```

#### Enable CORS with attributes

```c#
[Route("api/[controller]")]
[ApiController]
public class WidgetController : ControllerBase
{
    // GET api/values
    [EnableCors("AnotherPolicy")]
    [HttpGet]
    public ActionResult<IEnumerable<string>> Get()
    {
        return new string[] { "green widget", "red widget" };
    }

    // GET api/values/5
    [EnableCors("Policy1")]
    [HttpGet("{id}")]
    public ActionResult<string> Get(int id)
    {
        return id switch
        {
            1 => "green widget",
            2 => "red widget",
            _ => NotFound(),
        };
    }
}
```



