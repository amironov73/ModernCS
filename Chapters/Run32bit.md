### Запуск AnyCPU-приложения в 32-битном режиме

Бывает так, что у нас на руках .NET-приложение, маркированное как AnyCPU, но по каким-либо причинам нам необходимо запустить его в 32-битном режиме на 64-битной машине. Что делать?

Классический совет в этом случае таков: отыскать в закромах Windows SDK утилиту corflags (как правило, находится по пути «C:\Program Files (x86)\Microsoft SDKs\Windows\v7.0A\Bin\CorFlags.exe» или «C:\Program Files (x86)\Microsoft SDKs\Windows\v8.0A\bin\NETFX 4.0 Tools\CorFlags.exe» в зависимости от версии .NET Framework) и проделать следующую манипуляцию:

```
corflags /32bit+  <C:\Path\To\program.exe>
```

Это изменит заголовок EXE-файла таким образом, что отныне Windows станет запускать его только в 32-битном режиме. Проблема решена, хоть и путём модификации EXE-файла!

Но есть и менее интрузивные методы решения этой проблемы. Один из них описан здесь: http://coolsoft.altervista.org/en/blog/2012/01/run-anycpu-net-applications-x86-mode. Если коротко, то предлагается утилита, которая самостоятельно загрузит главную сборку приложения в 32-битном режиме и передаст управление на входную точку.

Я не мог остаться в стороне и написал аналогичную программу, оформленную как WinForms-приложение (чтобы избежать появления окна консоли). Вот её текст:

```csharp
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
 
namespace Run32bit
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static int Main(string[] args)
        {
            if (args.Length < 1)
            {
                MessageBox.Show("Usage: Run32bit <program.exe>");
                return 1;
            }
 
            try
            {
                string fileName = Path.GetFullPath(args[0]);
                Assembly assembly = Assembly.LoadFile(fileName);
                MethodInfo entryPoint = assembly.EntryPoint;
                ParameterInfo[] parameters = entryPoint.GetParameters();
 
                if (parameters.Length == 0)
                {
                    entryPoint.Invoke(null, null);
                }
                else
                {
                    string[] args2 = args.Skip(1).ToArray();
 
                    entryPoint.Invoke(null, new object[] {args2});
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return 1;
            }
 
            return 0;
        }
    }
}
```

Примечание 1: если у программы есть файл конфигурации program.exe.config, то его необходимо скопировать в Run32bit.exe.config, иначе рантайм не найдёт конфигурацию приложения.

Примечание 2: программа-загрузчик, скомпилилированная под одну версию .NET (например, 3.5), не сможет загрузить сборку, скомпилированную под другую версию .NET (например, 4.0).
