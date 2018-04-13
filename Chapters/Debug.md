### Классы Debug и Trace

Классы Debug и Trace работают через так называемых «слушателей» – наследников абстрактного класса System.Diagnostics.TraceListener, которые, собственно, и занимаются выводом сообщений. У Debug и у Trace имеется коллекция Listeners, в которую можно добавить столько «слушателей», сколько нужно. Предусмотрены несколько готовых «слушателей»

* DefaultTraceListener – устанавливается по умолчанию;
* EventProviderTraceListener;
* EventLogTraceListener;
* TextWriterTraceListener;
* IisTraceListener;
* WebPageTraceListener;

плюс возможность самому создать наследника TraceListner (который, например, пишет сообщения в базу данных) и добавить его в коллекцию.

Классы Debug и Trace разделяют общую настройку в App.config, все необходимые настроечные элементы помещаются в элемент system.diagnostics/trace:
```xml
<configuration>
   <system.diagnostics>
      <trace useGlobalLock="false" autoflush="true" indentsize="1">
         <listeners>
            <add name="myListener"
                 type="System.Diagnostics.TextWriterTraceListener"
                 initializeData="c:\myListener.log" />
         </listeners>
      </trace>
   </system.diagnostics>
</configuration>
```
Таким образом, добавленный через App.config «слушатель» будет работать и в Debug и Trace, что в большинстве случаев правильно. Если вас это не устраивает, добавляйте «слушателей» вручную.

#### DefaultTraceListener
Этот «слушатель» по умолчанию добавляется в коллекции Debug.Listeneres и Trace.Listeners, причём для вывода сообщений он использует WIN32 API OutputDebugString. Однако, методы Fail и Assert приводят к тому, что сообщение дополнительно отображается в окне (message box), если приложение выполняется в интерактивном режиме (например, WinForms).

Удалить этого «слушателя» можно с помощью App.config:
```xml
<configuration>
    <system.diagnostics>
      <trace>
        <listeners>
          <remove name="Default" />
          <add name="myListener"
              type="System.Diagnostics.TextWriterTraceListener"
              initializeData="c:\myListener.log" />
        </listeners>
      </trace>
    </system.diagnostics>
</configuration>
```

#### Debug
Свойства:

* AutoFlush – нужно ли вызывать Flush после каждого вызова Write;
* IndentLevel – глубина вложенности отступов (по умолчанию 0);
* IndentSize – количество пробелов в отступе (по умолчанию 4);
* Listeners – коллекция «слушателей», выполняющих реальный вывод текста.

Настроить вышеперечисленные свойства `AutoFlush` и `IndentSize` можно в `App.config`:
```xml
<configuration>
  <system.diagnostics>
    <trace autoflush="true" indentsize="7" />
  </system.diagnostics>
</configuration>
```
Пример работы с отступами:
```csharp
Debug.WriteLine ("Список возможных причин сбоя:");
Debug.Indent ();
Debug.WriteLine ("Отсутствие администраторских прав");
Debug.WriteLine ("Отсутствие подключения к сети");
Debug.WriteLine ("Недостаточно памяти для проведения операции");
Debug.Unindent ();
Debug.WriteLine ("Сообщите администратору о сбое");
```
Будет выведено следующее:
```
Список возможных причин сбоя:
    Отсутствие администраторских прав
    Отсутствие подключения к сети
    Недостаточно памяти для проведения операции
Сообщите администратору о сбое
```

##### Метод Assert (все перегрузки помечены ConditionalAttribute(«DEBUG»)):
```csharp
public static void Assert (bool condition);
public static void Assert (bool condition, string message);
public static void Assert (bool condition, string message, 
    string detailMessage);
public static void Assert (bool condition, string message, 
    string detailMessageFormat, params object[] args);
```

при невыполнении указанного условия выводит сообщение об ошибке. По умолчанию, при наличии в коллекции `Listeners` экземпляра `DefaultTraceListener`, показывает окно (message box) с этим сообщением и тремя кнопками: `Abort`, `Retry` и `Ignore`. Нажатие на `Abort` немедленно завершает приложение. Нажатие на `Retry` приводит к переходу в отладчик или к предложению запустить отладчик, если он ещё не запущен. Нажатие на `Ignore` должно продолжить выполнение программы.

Метод `Close` «выталкивает» выходной буфер и вызывает `Close` у всех подключенных «слушателей».

#### Метод Fail (все перегрузки помечены ConditionalAttribute(«DEBUG»)):
```csharp
public static void Fail (string message);
public static void Fail (string message, string detailMessage);
```
Метод `Flush` «выталкивает» выходной буфер у всех подключенных «слушателей».

Метод `Indent` увеличивает вложенность отступов на единицу (см. `IndentLevel`).

#### Метод Print (все перегрузки помечены ConditionalAttribute(«DEBUG»)):
```csharp
public static void Print (string message);
public static void Print (string format, params object[] args);
```
выводит указанное сообщение плюс перевод строки.

Метод `Unindent` уменьшает вложенность отступов на единицу (см. `IndentLevel`).

#### Методы Write, WriteIf, WriteLine и WriteIf (все помечены ConditionalAttribute(«DEBUG»)):
```csharp
public static void Write (object value);
public static void Write (string message);
public static void Write (object value, string category);
public static void Write (string message, string category);
public static void WriteIf (bool condition, object value);
public static void WriteIf (bool condition, string message);
public static void WriteIf (bool condition, string message,
    string category);
public static void WriteIf (bool condition, string message,
    string category);
public static void WriteLine (object value);
public static void WriteLine (string message);
public static void WriteLine (object value, string category);
public static void WriteLine (string format, params object[] args);
public static void WriteLine (string message, string category);
public static void WriteLineIf (bool condition, object value);
public static void WriteLineIf (bool condition, string message);
public static void WriteLineIf (bool condition, object value,
    string category);
public static void WriteLineIf (bool condition, string message,
    string category);
```
выводит указанное сообщение с помощью «слушателей» из коллекции Listeners. Параметр category задаёт (произвольную) категорию сообщения и применяется, главным образом, для группировки однотипных сообщений при просмотре отладочных логов.

Манипуляции с коллекцией Listeners. Очистить коллекцию (и таким образом прекратить вывод отладочных сообщений) можно в коде:
```csharp
System.Diagnostics.Debug.Listeners.Clear();
```
или с помощью App.config:
```xml
<configuration>
  <system.diagnostics>
    <trace>
      <listeners>
        <clear/>
        <add name="console" 
          type="System.Diagnostics.ConsoleTraceListener" >
          <filter type="System.Diagnostics.EventTypeFilter" 
            initializeData="Error" />
        </add>
      </listeners>
    </trace>
  </system.diagnostics>
</configuration>
```
Добавить «слушателя» также можно в коде:
```xml
var consoleListener = new ConsoleTraceListener ();
Debug.Listeners.Add (consoleListener);
 
var textListener = new TextWriterTraceListener ("logfile.txt");
Debug.Listeners.Add (textListener);
 
var delimitedListener 
   = new DelimitedListTraceListener ("logfile.csv")
   {
       Delimiter = ",",
       TraceOutputOptions = TraceOptions.DateTime 
           | TraceOptions.ThreadId
   };
Debug.Listeners.Add (delimitedListener);
```
и с помощью App.config:
```xml
<configuration>
  <system.diagnostics>
    <trace autoflush="false" indentsize="4">
      <listeners>
        <add name="configConsoleListener"  
          type="System.Diagnostics.ConsoleTraceListener" />
        <add name="myListener" 
          type="System.Diagnostics.TextWriterTraceListener" 
          initializeData="TextWriterOutput.log" />
        <add name="delimitedListener" 
          type="System.Diagnostics.DelimitedListTraceListener" 
          delimiter="," 
          initializeData="delimitedOutput.csv" 
          traceOutputOptions="ProcessId, DateTime" />
        <remove name="Default" />
      </listeners>
    </trace>
  </system.diagnostics>
</configuration>
```

#### Trace
Класс Trace устроен во многом аналогично классу Debug, однако, у него есть небольшие отличия, главным из которых является то, что методы, отмеченные у Debug атрибутом ConditionalAttribute(«DEBUG»), у него отмечены атрибутом ConditionalAttribute(«TRACE»). Таким образом, легко добиться такой ситуации, когда отладочная печать через Debug.WriteLine отсутствует, но работает трассировка через Trace.WriteLine, и наоборот.

Вторым важным отличием является ориентация на так называемые «переключатели» (TraceSwitch), которые могут включаться и выключаться по ходу программы, в то время как отладочная печать через Debug.WriteLine ориентирована на статический сценарий.


#### Switch
```xml
<configuration>
  <system.diagnostics>
    <switches>
      <add name="mySwitch" value="true" />
    </switches>
  </system.diagnostics>
</configuration>
```
...
```csharp
static BooleanSwitch boolSwitch = new BooleanSwitch
    (
        "mySwitch",
        "Очень важный переключатель"
    );
...
Console.WriteLine
   (
        "Переключатель '{0}' установлен в {1}",
        mySwitch.DisplayName,
        mySwitch.Enabled
   );
...
if (mySwitch.Enabled)
{
    // Некие действия
}
```

