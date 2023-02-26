### Аннотации данных

Аннотации данных предоставляют собой классы атрибутов, используемые для определения метаданных для ASP.NET MVC и элементов управления данными ASP.NET, Entity Framework и в других фреймворках.

#### AssociationAttribute

Указывает, что элемент сущности представляет связи данных, например связь по внешнему ключу. Атрибут объявлен устаревшим и больше не поддерживается.

#### CompareAttribute

Предоставляет атрибут, сравнивающий два свойства.

Свойства:

* **ErrorMessage**

Получает или задает сообщение об ошибке, которое необходимо связать с проверяющим элементом управления на случай сбоя во время проверки.

* **OtherProperty**

Получает свойство, с которым будет сравниваться текущее свойство.

* **OtherPropertyDisplayName**

Получает отображаемое имя другого свойства.

* **RequiresValidationContext**

* Получает значение, указывающее, требует ли атрибут контекста проверки.

#### ConcurrencyCheckAttribute

Указывает, что свойство участвует в проверках на оптимистичный параллелизм.

#### CreditCardAttribute

Указывает, что значение поля данных - это номер кредитной карты. Значение проверяется с помощью алгоритма обработки строк. Класс не проверяет, является ли номер кредитной карты допустимым для покупок, только правильно сформированным.

#### CustomValidationAttribute

Задает пользовательский метод проверки свойства или класса экземпляра.

Атрибут `CustomValidationAttribute` используется для выполнения пользовательской проверки при вызове метода `IsValid`. Метод `IsValid` перенаправляет вызов в метод, определяемый свойством `Method`, который, в свою очередь, выполняет фактическую проверку.

Атрибут `CustomValidationAttribute` можно применять к типам, свойствам, полям, методам и параметрам метода. При применении к свойству атрибут вызывается каждый раз, когда ему присваивается значение. При применении к методу атрибут вызывается всякий раз, когда программа вызывает этот метод. При применении к параметру метода атрибут вызывается перед вызовом метода.

```csharp
public CustomValidationAttribute (Type validatorType, string method);
```

Свойства:

* **ErrorMessage**

Получает или задает сообщение об ошибке, которое необходимо связать с проверяющим элементом управления на случай сбоя во время проверки.

* **Method**

Метод, осуществляющий проверку.

* **RequiresValidationContext**

Получает значение, указывающее, требует ли атрибут контекста проверки.

* **ValidatorType**

Тип, который выполняет пользовательскую проверку.

#### DataTypeAttribute

Задает имя дополнительного типа, который необходимо связать с полем данных.

Пример:

```csharp
using System;
using System.Web.DynamicData;
using System.ComponentModel.DataAnnotations;

[MetadataType (typeof(CustomerMetaData))]
public partial class Customer
{
}

public class CustomerMetaData
{

    // Add type information.
    [DataType (DataType.EmailAddress)]
    public object EmailAddress;
}
```

#### DisplayAttribute

Предоставляет атрибут общего назначения, который позволяет указать локализуемые строки для типов и членов разделяемых классов сущностей.

Свойства:

* **AutoGenerateField**

Нужно ли для отображения этого поля автоматически создавать пользовательский интерфейс.

* **AutoGenerateFilter**

Отображается ли пользовательский интерфейс фильтрации для данного поля автоматически.

* **Description**

Используется для отображения описания пользовательского интерфейса.

* **GroupName**

Группировка полей в пользовательском интерфейсе.

* **Name**

Отображение в элементе пользовательского интерфейса.

* **Order**

Порядковый номер столбца.

* **Prompt**

Подсказка в элементе пользовательского интерфейса.

* **ResourceType**

Тип, содержащий ресурсы для свойств `ShortName`, `Name`, `Prompt` и `Description`.

* **ShortName**

Используется в качестве метки столбца сетки.

#### DisplayColumnAttribute

Указывает столбец, который отображается в связанной таблице как столбец внешнего ключа.

Пример:

```csharp
using System;
using System.Web.DynamicData;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

[DisplayColumn ("City", "PostalCode", false)]
public partial class Address
{
}

[DisplayColumn ("LastName")]
public partial class Customer
{
}
```

#### DisplayFormatAttribute

Задает способ отображения и форматирования полей данных в платформе динамических данных ASP.NET.

Пример:

```csharp
using System;
using System.Web.DynamicData;
using System.ComponentModel.DataAnnotations;

[MetadataType (typeof (ProductMetaData))]
public partial class Product
{
}

public class ProductMetaData
{
    // Applying DisplayFormatAttribute
    // Display the text [Null] when the data field is empty.
    // Also, convert empty string to null for storing.
    [DisplayFormat (ConvertEmptyStringToNull = true, NullDisplayText = "[Null]")]
    public object Size;

    // Display currency data field in the format $1,345.50.
    [DisplayFormat (DataFormatString="{0:C}")]
    public object StandardCost;

    // Display date data field in the short format 11/12/08.
    // Also, apply format in edit mode.
    [DisplayFormat (ApplyFormatInEditMode=true, DataFormatString = "{0:d}")]
    public object SellStartDate;
}
```

#### EditableAttribute

Указывает, доступно ли поле данных для редактирования.

Наличие атрибута EditableAttribute в поле данных указывает, должен ли пользователь иметь возможность изменять значение поля.

Этот класс не применяет и не гарантирует возможность редактирования поля. Базовое хранилище данных может позволить изменить поле независимо от наличия этого атрибута.

#### EmailAddressAttribute

Проверяет адреса электронной почты.

#### EnumDataTypeAttribute

Включает перечисление .NET для сопоставления со столбцом данных.

В следующем примере показано, как заменить числовое значение перечисления соответствующим объявлением.

```csharp
public enum ReorderLevel
{
    Zero = 0,
    Five = 5,
    Ten = 10,
    Fifteen = 15,
    Twenty = 20,
    TwentyFive = 25,
    Thirty = 30
}

[MetadataType (typeof (ProductMD))]  
public partial class Product
{  
    public class ProductMD
    {  
        [EnumDataType (typeof (ReorderLevel))]
        public object ReorderLevel { get; set; }
    }
}
```

#### FileExtensionsAttribute

Проверяет расширения имени файла.

#### FilterUIHintAttribute

Представляет атрибут, позволяющий указать поведение фильтрации для столбца.

#### KeyAttribute

Обозначает одно или несколько свойств, которые однозначно определяют сущность.

#### MaxLengthAttribute

Задает максимально допустимый размер массива или длину строки для свойства.

#### MetadataTypeAttribute

Указывает, класс метаданных, который необходимо связать с классом модели данных.

#### MinLengthAttribute

Задает минимально допустимый размер массива или длину строки для свойства.

#### PhoneAttribute

Указывает, что значение поля данных — это номер телефона в правильном формате.

#### RangeAttribute

Задает ограничения числового диапазона для значения поля данных.

Пример:

```csharp
using System;
using System.Web.DynamicData;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

[MetadataType (typeof (ProductMetaData))]
public partial class Product
{
}

public class ProductMetaData
{
    
    [Range (10, 1000, 
        ErrorMessage = "Value for {0} must be between {1} and {2}.")]
    public object Weight;

    [Range (300, 3000)]
    public object ListPrice;

    [Range (typeof(DateTime), "1/2/2004", "3/4/2004",
        ErrorMessage = "Value for {0} must be between {1} and {2}")]
    public object SellEndDate;
}
```

#### RegularExpressionAttribute

Указывает, что значение поля данных в платформе динамических данных ASP.NET должно соответствовать заданному регулярному выражению.

Пример:

```csharp
using System;
using System.Web.DynamicData;
using System.ComponentModel.DataAnnotations;

[MetadataType (typeof (CustomerMetaData))]
public partial class Customer
{
}

public class CustomerMetaData
{
   
    // Allow up to 40 uppercase and lowercase 
    // characters. Use custom error.
    [RegularExpression (@"^[a-zA-Z''-'\s]{1,40}$", 
         ErrorMessage = "Characters are not allowed.")]
    public object FirstName;

    // Allow up to 40 uppercase and lowercase 
    // characters. Use standard error.
    [RegularExpression (@"^[a-zA-Z''-'\s]{1,40}$")]
    public object LastName;
}
```

#### RequiredAttribute

Указывает, что значение поля данных является обязательным.

Пример:

```csharp
using System;
using System.Web.DynamicData;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

[MetadataType (typeof (CustomerMetaData))]
public partial class Customer
{
}

public class CustomerMetaData
{
    // Require that the Title is not null.
    // Use custom validation error.
    [Required (ErrorMessage = "Title is required.")]
    public object Title;
    
    // Require that the MiddleName is not null.
    // Use standard validation error.
    [Required]
    public object MiddleName;
}
```

#### ScaffoldColumnAttribute

Указывает, использует ли класс или столбец данных формирования шаблонов.

#### StringLengthAttribute

Указывает минимальную и максимальную длину символов, разрешенных в поле данных.

#### TimestampAttribute

Указывает тип данных столбца как версию строки.

#### UIHintAttribute

Задает шаблон или пользовательский элемент управления, используемый платформой динамических данных для отображения поля данных.

#### UrlAttribute

Обеспечивает проверку URL-адреса.

#### ValidationAttribute

Служит базовым классом для всех атрибутов проверки.

Свойства:

* **ErrorMessage**

Сообщение об ошибке, которое необходимо связать с проверяющим элементом управления на случай сбоя во время проверки.

* **RequiresValidationContext**

Требует ли атрибут контекста проверки.

Методы:

* **FormatErrorMessage (String)**

Применяет к сообщению об ошибке форматирование на основе поля данных, в котором произошла ошибка.

* **GetValidationResult (Object, ValidationContext)**

Проверяет, является ли заданное значение допустимым относительно текущего атрибута проверки.

* **IsDefaultAttribute()**

При переопределении в производном классе указывает, является ли значение этого экземпляра значением по умолчанию для производного класса.

* **IsValid (Object)**

Определяет, является ли заданное значение объекта допустимым.

* **IsValid (Object, ValidationContext)**

Проверяет заданное значение относительно текущего атрибута проверки.

* **Validate (Object, String)**

Проверяет указанный объект.

* **Validate (Object, ValidationContext)**

Проверяет указанный объект.

#### ValidationContext

Контекст, в котором проводится проверка.

Этот класс описывает тип или член, для которого выполняется проверка. Она также позволяет добавлять пользовательскую проверку с помощью любой службы, реализующей интерфейс `IServiceProvider`.

#### ValidationException

Представляет исключение, которое происходит во время проверки поля данных при использовании класса `ValidationAttribute`.

Свойства:

* **Data**

Коллекция пар «ключ-значение», предоставляющая дополнительные сведения об исключении.

* **HelpLink**

Ссылка на файл справки, связанный с этим исключением.

* **HResult**

`HRESULT` — кодированное числовое значение, присвоенное определенному исключению.

* **InnerException**

Возвращает экземпляр класса `Exception`, который вызвал текущее исключение.

* **Message**

Сообщение, описывающее текущее исключение.

* **Source**

Имя приложения или объекта, вызывавшего ошибку.

* **StackTrace**

Строковое представление кадров в стеке вызова.

* **TargetSite**

Возвращает метод, создавший текущее исключение.

* **ValidationAttribute**

Экземпляр класса `ValidationAttribute`, который вызвал это исключение.

* **ValidationResult**

Экземпляр `ValidationResult`, описывающий ошибку проверки.

* **Value**

Значение объекта, при котором класс `ValidationAttribute` вызвал данное исключение.

#### ValidationResult

Представляет контейнер для результатов запроса на проверку.

Свойства:

* **Success**

Результат завершения проверки (`true`, если проверка прошла успешно).

* **ErrorMessage**

Сообщение об ошибке (если есть).

* **MemberNames**

Коллекция имен членов, указывающую поля, которые вызывают ошибки проверки.

#### Validator

Определяет вспомогательный класс, который может использоваться для проверки объектов, свойств и методов в случае его включения в связанные с ними атрибуты ValidationAttribute.

Методы:

* **TryValidateObject (Object, ValidationContext, ICollection\<ValidationResult\>)**

Определяет, является ли указанный объект допустимым, используя контекст проверки и коллекцию результатов проверки.

* **TryValidateObject (Object, ValidationContext, ICollection\<ValidationResult\>, Boolean)**

Определяет, является ли указанный объект допустимым, используя контекст проверки, коллекцию результатов проверки и значение, указывающее, следует ли проверять все свойства.

* **TryValidateProperty (Object, ValidationContext, ICollection\<ValidationResult\>)**

Проверяет свойство.

* **TryValidateValue (Object, ValidationContext, ICollection\<ValidationResult\>, IEnumerable\<ValidationAttribute\>)**

Возвращает значение, указывающее, является ли заданное значение допустимым относительно указанных атрибутов.

* **ValidateObject (Object, ValidationContext)**

Определяет, является ли указанный объект допустимым, используя контекст проверки.

* **ValidateObject (Object, ValidationContext, Boolean)**

Определяет, является ли указанный объект допустимым, используя контекст проверки и значение, указывающее, следует ли проверять все свойства.

* **ValidateProperty (Object, ValidationContext)**

Проверяет свойство.

* **ValidateValue (Object, ValidationContext, IEnumerable\<ValidationAttribute\>)**

Проверяет указанные атрибуты.
