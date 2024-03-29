﻿### Tag helpers

Для упрощения создания интерфейса ASP.NET Core предоставляет специальный инструмент, который называется tag helpers.

```html
@page
@addTagHelper *, Microsoft.AspNetCore.Mvc.TagHelpers

<p>Это текст со страницы.</p>

<a asp-page="About">О проекте</a>
```

Первый параметр директивы `@addTagHelper` указывает на tag-хелперы, которые будут доступны на Razor-странице, а второй параметр определяет библиотеку хелперов. В данном случае директива использует синтаксис подстановок - знак звездочки ("*") означает, что подключаются все хелперы из библиотеки `Microsoft.AspNetCore.Mvc.TagHelpers`.

Вместо того, чтобы прописывать директиву `@addTagHelper` на каждой отдельной странице, мы можем подключить все хелперы разом. Для этого, как и для подключения различных пространств имен, применяется файл `_ViewImports.cshtml`.

Еще одна директива removeTagHelper удаляет ранее добавленные tag-хелперы. Ее применение аналогично:

```html
@removeTagHelper "*, Microsoft.AspNetCore.Mvc.TagHelpers"
```

Данная директива может быть полезной, если мы, например, захотим ограничить применение хелперов в какой-то одной странице или группе страниц Razor. Эту директиву также можно определять в файле `_ViewImports.cshtml`.

#### Создание ссылок

Для создания ссылок на странице Razor можно естественно применять стандартный html-элемент `<a>`. Однако для упрощения генерации ссылок ASP.NET Core также предоставляет такой tag-хелпер, как `AnchorTagHelper`. Он может принимать ряд специальных атрибутов:

* **asp-controller**: указывает на контроллер, которому предназначен запрос,

* **asp-action**: указывает на действие контроллера,

* **asp-area**: указывает на действие область, в которой расположен контроллер или страница RazorPage (если они находятся в отдельной области),

* **asp-page**: указывает на RazorPage, которая будет обрабатывать запрос,

* **asp-page-handler**: указывает на обработчик страницы RazorPage, которая будет применяться для обработки запроса,

* **asp-host**: указывает на домен сайта,

* **asp-protocol**: определяет протокол (`http` или `https`),

* **asp-route**: указывает на название маршрута,

* **asp-all-route-data**: устанавливает набор значений для параметров,

* **asp-route-\[название параметра\]**: определяет значение для определенного параметра,

* **asp-fragment**: определяет ту часть хэш-ссылки, которая идет после символа решетки `#.` Например, `"paragraph2"` в ссылке "http://mysite.com/#paragraph2"

Примеры:

```html
<a asp-page="About" asp-route-id="5">About Person 5</a>
<a asp-page="About" asp-page-handler="Site">О сайте</a>
```

#### Работа с формами

```html
@page 
@addTagHelper *, Microsoft.AspNetCore.Mvc.TagHelpers

<form method="post" asp-antiforgery="true">
    <p>
        <label asp-for="Product.Name">Название</label><br />
        <input type="text" asp-for="Product.Name" />
    </p>
    <p>
        <label asp-for="Product.Price">Цена</label><br />
        <input asp-for="Product.Price" />
    </p>
    <p>
        <label asp-for="Product.Company">Производитель</label><br />
        <input type="text" asp-for="Product.Company" />
    </p>
    <input type="submit" value="Отправить" />
</form>
```

Класс `FormTagHelper` может принимать следующие атрибуты:

* **asp-controller**: указывает на контроллер, которому предназначен запрос,

* **asp-action**: указывает на действие контроллера,

* **asp-area**: указывает на название области, в которой будет вызываться контроллер для обработки формы,
  
* **asp-antiforgery**: если имеет значение `true`, то для этой формы будет генерироваться `antiforgery token`,

* **asp-route**: указывает на название маршрута,

* **asp-all-route-data**: устанавливает набор значений для параметров,

* **asp-route-\[название параметра\]**: определяет значение для определенного параметра,

* **asp-page**: указывает на страницу RazorPage, которая будет обрабатывать запрос,

* **asp-page-handler**: указывает на обработчик страницы RazorPage, который применяется для обработки запроса,

* **asp-fragment**: указывает фрагмент, который добавляется к запрашиваемому адресу после символа `#`.

#### SelectTagHelper

`SelectTagHelper` создает элемент списка `<select>`. Он имеет специальный атрибут `asp-items`, который указывает на объект `IEnumerable<SelectListItem>` - набор элементов, используемых для создания списка.

