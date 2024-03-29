﻿### Создание Razor-приложения с помощью dotnet cli

Как обычно, мы пойдём сложным путём. Во-первых, как создаются нынче приложения ASP.NET Core? Правильно, заклинанием dotnet new webapp. А какие возможности заложены в шаблон? Правильно, мы не помним! 🙂

```
Веб-приложение ASP.NET Core (C#)
Автор: Майкрософт
Описание: Шаблон проекта для создания приложения ASP.NET Core
с образцом содержимого ASP.NET Core Razor Pages
Этот шаблон содержит технологии сторонних производителей, кроме Майкрософт.
Дополнительные сведения см. в разделе
https://aka.ms/aspnetcore/8.0-third-party-notices.

Командная строка:
dotnet new webapp [опции] [параметры шаблона]
dotnet new razor [опции] [параметры шаблона]

Опции:
-n, --name <name>       Имя создаваемых выходных данных. Если имя не указано,
используется имя выходного каталога.
-o, --output <output>   Расположение для размещения созданных выходных данных.
--dry-run               Отображает сводку действий, которые могли бы произойти,
если бы данная командная строка была запущена,
если это приведет к созданию шаблона.
--force                 Заставляет создавать содержимое, даже если при этом изменяются
существующие файлы.
--no-update-check       Отключает проверку наличия обновлений пакета шаблона при создании
экземпляра шаблона.
--project <project>     Проект, который следует использовать для оценки контекста.
-lang, --language <C#>  Указывает язык шаблона для создания экземпляра.
--type <project>        Указывает тип шаблона для создания экземпляра.
```

Параметры шаблона:

* **-au, --auth \<None|Individual|…\>**
    Тип используемой проверки подлинности
    Тип: choice
    ```
    None           Без проверки подлинности
    Individual     Индивидуальная проверка подлинности
    IndividualB2C  Индивидуальная проверка подлинности в Azure AD B2C
    SingleOrg      Проверка подлинности в организации для одного клиента
    MultiOrg       Проверка подлинности в организации для нескольких клиентов
    Windows        Проверка подлинности Windows
    ```
    По умолчанию: `None`

* **--aad-b2c-instance \<aad-b2c-instance\>**

    Экземпляр Azure Active Directory B2C, к которому нужно подключиться (используется с проверкой подлинности `IndividualB2C`).
    Тип: `string`
    По умолчанию: https://login.microsoftonline.com/tfp/

* **-ssp, --susi-policy-id \<susi-policy-id\>**

    Идентификатор политики входа и регистрации для этого проекта (используется с проверкой подлинности `IndividualB2C`).
    Тип: `string`
    По умолчанию: `b2c_1_susi`

* **-socp, --signed-out-callback-path \<signed-out-callback-path\>**

    Глобальный обратный вызов для выхода (используется с проверкой подлинности `IndividualB2C`).
    Тип: `string`
    По умолчанию: `/signout/B2C_1_susi`

* **-rp, --reset-password-policy-id \<reset-password-policy-id\>**

    Идентификатор политики сброса паролей для этого проекта (используется с проверкой подлинности `IndividualB2C`).
    Тип: `string`
    По умолчанию: `b2c_1_reset`

* **-ep, --edit-profile-policy-id \<edit-profile-policy-id\>**

    Идентификатор политики изменения профиля для этого проекта (используется с проверкой подлинности `IndividualB2C`).
    Тип: `string`
    По умолчанию: `b2c_1_edit_profile`

* **--aad-instance \<aad-instance\>**

    Экземпляр Azure Active Directory, к которому нужно подключиться (используется с проверкой подлинности `SingleOrg` или `MultiOrg`).
    Тип: `string`
    По умолчанию: https://login.microsoftonline.com/

* **--client-id \<client-id\>**

    Идентификатор клиента для этого проекта (используется с проверкой подлинности `IndividualB2C`, `SingleOrg` или `MultiOrg`).
    Тип: `string`
    По умолчанию: `11111111-1111-1111-11111111111111111`

* **--domain \<domain\>**

    Домен для клиента каталога (используется с проверкой подлинности `SingleOrg` или `IndividualB2C`).
    Тип: `string`
    По умолчанию: `qualified.domain.name`

* **--tenant-id \<tenant-id\>**

    Идентификатор `TenantId` каталога, к которому нужно подключиться (используется с проверкой подлинности `SingleOrg`).
    Тип: `string`
    По умолчанию: `22222222-2222-2222-2222-222222222222`

* **--callback-path \<callback-path\>**

    Путь запроса в базовом пути приложения к URI перенаправления (используется с проверкой подлинности `SingleOrg` или `IndividualB2C`).
    Тип: `string`
    По умолчанию: `/signin-oidc`

* **-r, --org-read-access**

    Следует ли предоставлять этому приложению доступ на чтение к каталогу (применяется только к проверке подлинности `SingleOrg` или `MultiOrg`).
    Тип: `bool`
    По умолчанию: `false`

* **--exclude-launch-settings**

    Следует ли исключить `launchSettings.json` из созданного шаблона.
    Тип: `bool`
    По умолчанию: `false`

* **--no-restore**

    Если установлено, автоматическое восстановление проекта при создании пропускается.
    Тип: `bool`
    По умолчанию: `false`

* **--no-https**

    Следует ли отключить HTTPS. Этот параметр применяется, только если для `--auth` не используются `IndividualB2C`, `SingleOrg` или `MultiOrg`.
    Тип: `bool`
    По умолчанию: `false`

* **-uld, --use-local-db**

    Следует ли использовать LocalDB вместо SQLite. Этот параметр применяется, только если указывается `--auth Individual` или `--auth IndividualB2C`.
    Тип: `bool`
    По умолчанию: `false`

* **-f, --framework \<net8.0|net7.0|…\>**

    Целевая платформа для проекта.
    Тип: choice
    ```
    net8.0         Целевая net8.0
    net7.0         Целевая версия net7.0
    net6.0         Target net6.0
    net5.0         Target net5.0
    netcoreapp3.1  Target netcoreapp3.1
    ```
    По умолчанию: `net8.0`

* **--called-api-url \<called-api-url\>**

    URL-адрес API для вызова из веб-приложения. Этот параметр применяется, только если указывается `--auth SingleOrg`, `--auth MultiOrg` или `--auth IndividualB2C`.
    Тип: `string`
    По умолчанию: https://graph.microsoft.com/v1.0

* **–calls-graph**

    Указывает, вызывает ли веб-приложение Microsoft Graph. Этот параметр применяется, только если указывается `--auth SingleOrg` или `--auth MultiOrg`.
    Тип: `bool`
    По умолчанию: `false`

* **--called-api-scopes \<called-api-scopes\>**

    Области для запроса вызова API из веб-приложения. Этот параметр применяется, только если указывается
    `--auth SingleOrg`, `--auth MultiOrg` или `--auth IndividualB2C`.
    Тип: `string`
    По умолчанию: `user.read`

* **–use-program-main**

    Следует ли создавать явный класс `Program` и метод `Main` вместо операторов верхнего уровня.
    Тип: `bool`
    По умолчанию: `false`

* **-S, –SignedOutCallbackPath <SignedOutCallbackPath>**

    Глобальный обратный вызов для выхода (используется с проверкой подлинности `IndividualB2C`).
    Тип: `string`
    По умолчанию: `/signout/B2C_1_susi`

* **-rrc, --razor-runtime-compilation**

    Определяет, настроен ли проект на использование компиляции среды выполнения Razor в отладочных сборках.
    Тип: `bool`
    По умолчанию: `false`
