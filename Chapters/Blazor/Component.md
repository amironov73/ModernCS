### Компоненты

**!!! Razor component file names require a capitalized first letter.**

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
