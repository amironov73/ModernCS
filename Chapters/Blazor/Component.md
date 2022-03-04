### Компоненты

**!!! Имя компонента должно начинаться с заглавной буквы.** Расширение должно быть `.razor`.

Пример Razor-компонента:

```html
@if (BusyState)
{
    <div class="row my-3">
        <div class="col col-md-8 offset-md-2 text-center">
            <img src="@ImageSource" class="w-100" alt="@AltText"/>
        </div>
    </div>
}

@code
{
    /// <summary>
    /// Подпись к картинке.
    /// </summary>
    [Parameter]
    public string? AltText { get; set; }

    /// <summary>
    /// Источник картинки.
    /// </summary>
    [Parameter]
    public string? ImageSource { get; set; }

    /// <summary>
    /// Состояние "занято".
    /// </summary>
    [Parameter]
    public bool BusyState { get; set; }

    /// <summary>
    /// Установка состояния.
    /// </summary>
    public void SetState
        (
            bool busyState
        )
    {
        BusyState = busyState;
        StateHasChanged();
    }

}
```

Пример богатого внутреннего содержимого компонента:

```html
<h2 class="text-center my-3">@ChildContent</h2>

@code
{
    /// <summary>
    /// Текст заголовка
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

}
```

Асинхронные методы в компонентах не поддерживают возврата `void`. Следует обязательно использовать `Task` или `ValueTask`.

Как правило, пространство имен компонента является производным от корневого пространства имен приложения и расположения компонента (папки) в приложении. Если пространством имен корня приложения является `BlazorSample`, а компонент `Counter` находится в папке `Pages`:

* Пространством имен компонента `Counter` является `BlazorSample.Pages`.
* Полным именем компонента является `BlazorSample.Pages.Counter`.

Директивы `@using` в файле `_Imports.razor` применяются только к файлам Razor (.razor), но не к файлам C# (.cs).

На компоненты также можно ссылаться с помощью полных имен, для чего не требуется директива @using. В следующем примере папка `Components` приложения напрямую ссылается на компонент `ProductDetail`:

```html
<BlazorSample.Components.ProductDetail />
```

Пространство имен компонента, созданного с помощью Razor, основано на следующем (в порядке приоритета).

* Директива `@namespace` в разметке файла Razor (например, `@namespace BlazorSample.CustomNamespace`).
* `RootNamespace` проекта в файле проекта (например, `<RootNamespace>BlazorSample</RootNamespace>`).
* Имя проекта, полученное из имени файла проекта (.csproj), и путь из корневого каталога проекта к компоненту. Например, платформа разрешает {PROJECT ROOT}/Pages/Index.razor с помощью пространства имен проекта `BlazorSample` (`BlazorSample.csproj`) в пространство имен `BlazorSample.Pages` для компонента `Index`. {PROJECT ROOT} — корневой путь к проекту. Компоненты соответствуют правилам привязки имен C#. Для компонента `Index` в этом примере компонентами в области действия являются все компоненты:
 * в этой же папке `Pages`;
 * в корневой папке проекта, которая не задает другое пространство имен явным образом.

Следующие службы не поддерживаются:

* Квалификация `global::`.
* Импорт компонентов с инструкциями `using`, содержащими псевдонимы. Например, `@using Foo = Bar` не поддерживается.
* Частичные имена. Например, нельзя добавить `@using BlazorSample` в компонент, а затем сослаться на компонент NavMenu в папке `Shared` приложения (`Shared/NavMenu.razor`) с помощью `<Shared.NavMenu></Shared.NavMenu>`.

Компоненты формируются как разделяемые классы C#. Создать их можно одним из следующих способов:

* Один файл содержит код C#, определенный в одном или нескольких блоках @code, разметке HTML и разметке Razor. Шаблоны проекта Blazor определяют свои компоненты с помощью этого однофайлового подхода.
* HTML и разметка Razor помещаются в файл Razor (.razor). Код C# помещается в файл кода программной части, определенный как разделяемый класс (.cs).

