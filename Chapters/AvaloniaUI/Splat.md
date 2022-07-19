### Библиотека Splat

На дворе XXI век, а некоторые вещи написать кроссплатформенно всё ещё нетривиально. Например, если вы пишете контрол, который отображает картинки в заданной папке, ждите `#ifdef` в коде. Это довольно утомительно и чревато ошибками. Поэтому была создана небольшая, но полезная библиотечка [Splat](https://github.com/reactiveui/splat/).

Что же умеет Splat?

* Кроссплатформенная загрузка/сохранение изображений.
* `System.Drawing.Color` для портативных библиотек.
* Кроссплатформенные геометрические примитивы (`PointF`, `SizeF`, `RectangleF`), а также куча дополнительных методов расширения, облегчающих их использование.
* Способ определить, находитесь ли вы в режиме запуска модульных тестов/режиме разработки.
* Некая абстракция журналирования (проще говоря, ещё один абстрактный логгер).
* Простой, но гибкий сервис-локатор.

NuGet: https://www.nuget.org/packages/Splat/

Поддерживаются:

* .NET Framework 4.6.2, .NET Framework 4.7.2, .NET Standard 2.0 and .NET 6.0
* WPF, Windows Forms, UWP
* MAUI
* Xamarin (Android, iOS and Mac)
* Tizen

* Переходим к вкусностям. 🙂

Как определить, что код выполняется под Unit-тестером?

```csharp
if (ModeDetector.InUnitTestRunner())
{
// If true, we are running unit tests
...
}
```

Как понять, что мы находимся в режиме дизайна?

```csharp
if (PlatformModeDetector.InDesignMode())
{
// If true, we are running inside Blend, so don't do anything
...
}
```

**Сервис-локатор.** Splat обеспечивает простую реализацию сервис-локатора, которая оптимизирована для настольных и мобильных приложений, но при этом остается достаточно гибкой. В конструкции локатора есть 2 части:

* **Свойство `Locator.Current`**, используемое для получения сервисов. `Locator.Current` — это статическая переменная, которую можно установить при запуске, чтобы адаптировать Splat к другим платформам DI/IoC.
* **Свойство `Locator.CurrentMutable`**, используемое для регистрации сервисов.

Чтобы получить сервис:

```csharp
// Чтобы получить единую регистрацию сервиса
var toaster = Locator.Current.GetService<IToaster>();

// Чтобы получить все сервисные регистрации
var allToasterImpls = Locator.Current.GetServices<IToaster>();
```

Реализация сервис-локатора по умолчанию также позволяет регистрировать новые типы во время выполнения.

```csharp
// Создавать новый тостер каждый раз, когда кто-то просит
Locator.CurrentMutable.Register
  (
    () => new Toaster(),
    typeof (IToaster)
  );

// Зарегистрировать экземпляр синглтона
Locator.CurrentMutable.RegisterConstant
  (
    new ExtraGoodToaster(),
    typeof (IToaster)
  );

// Зарегистрировать синглтон, который не будет создан,
// пока первый пользователь не запросит к нему доступ
Locator.CurrentMutable.RegisterLazySingleton
  (
    () => new LazyToaster(),
    typeof (IToaster)
  );
```

**Адаптеры для разрешения зависимостей.** Для каждого из предоставленных адаптеров разрешения зависимостей существует определенный пакет, который позволяет реализовать локатор служб другим контейнером IOC. Штатно имеются адаптеры для Autofac, DryIoc, Microsoft.Extensions.DependencyInjection, Ninject и для SimpleInjector.

**Ведение логов.** Splat предоставляет простую абстракцию для ведения журналов для библиотек и приложений. По умолчанию это ведение журнала не настроено (т. е. оно регистрируется в нулевом регистраторе). Чтобы настроить ведение журнала:

1. Зарегистрируйте реализацию `ILogger`, используя сервис-локатор (см. выше).
2. В классе, в котором вы хотите регистрировать данные, «реализуйте» интерфейс `IEnableLogger` (это маркерный интерфейс, на самом деле реализация не требуется).
3. Вызовите метод `Log` для записи записей в журнал:

```csharp
this.Log().Warn ("Произошло что-то плохое: {0}", errorMessage);
this.Log().ErrorException ("Пытался что-то сделать и не удалось", exception);
```

Для статических методов можно использовать `LogHost.Default` как объект для записи в журнал. Статический регистратор использует интерфейс, отличный от основного регистратора, чтобы обеспечить захват дополнительного контекста вызывающей стороны, поскольку он не имеет сведений об экземпляре класса и т. д. по сравнению с обычным регистратором. Чтобы воспользоваться ими, вам не нужно много делать, поскольку они являются необязательными параметрами в конце методов, которые используются компилятором\фреймворком. В настоящее время фиксируется только `CallerMemberName`.

Имеются адаптеры для: Console (простой вывод в консоль), Debug (вывод в окно отладчика), Log4Net, Microsoft.Extensions.Logging, NLog и Serilog.

Как подключить Microsoft.Extensions.Logging:

```csharp
using Splat.Microsoft.Extensions.Logging;

// note: this is different from the other adapter extension methods
// as it needs knowledge of the logger factory
// also the "container" is how you configured
// the Microsoft.Logging.Extensions
var loggerFactory = container.Resolve<ILoggerFactor>();

// in theory it could also be
// var loggerFactory = new LoggerFactory();

// then in your service locator initialisation
Locator.CurrentMutable
.UseMicrosoftExtensionsLoggingWithWrappingFullLogger (loggerFactory);
```

Как подключить NLog:

```csharp
using Splat.NLog;

//  then in your service locator initialisation
Locator.CurrentMutable.UseNLogWithWrappingFullLogger();
```

Как подключить Serilog:

```csharp
using Splat.Serilog;

// Then in your service locator initialisation
Locator.CurrentMutable.UseSerilogFullLogger()
```

**Кроссплатформенное рисование.** Надо подключить пакет `Splat.Drawing` и дать волю фантазии 🙂

```csharp
// This System.Drawing class works, even on WinRT
// where it's not supposed to exist
// Also, this works in a Portable Library, in your ViewModel

ProfileBackgroundAccentColor = Color.FromArgb(255, 255, 255, 255);

// Later, in the view, we can use it

ImageView.Background = ViewModel.ProfileBackgroundAccentColor.ToNativeBrush();
```

**Кроссплатформенная загрузка картинок.** Сначала надо зарегистрировать соответствующий сервис:

```csharp
Locator.CurrentMutable.RegisterPlatformBitmapLoader();
```

После этого можно кроссплатформенно загружать картинки

```csharp
// Load an Image
// This code even works in a Portable Library

var wc = new WebClient();
Stream imageStream = wc.OpenRead ("http://octodex.github.com/images/Professortocat_v2.png");

// IBitmap is a type that provides basic image information such as dimensions
IBitmap profileImage = await BitmapLoader.Current.Load
  (
    imageStream,
    null /* Use original width */,
    null /* Use original height */
  );

// ToNative always converts an IBitmap into the type that the platform
// uses, such as UIBitmap on iOS or BitmapSource in WPF
ImageView.Source = ViewModel.ProfileImage.ToNative();
```

До кучи в Splat предусмотрены ништяки вроде отслеживания падений приложения, мониторинг использования фич и прочее. Но нам в России всё это недоступно, так что не будем о грустном. 🙁
