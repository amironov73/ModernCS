### Встраиваемый язык OverScript

На Хабре появилась [статья](https://habr.com/ru/post/656035/) про язык OverScript — встраиваемый язык для платформы .NET 6:

1. Интерпретатор является полностью независимым, т.е. не использует имеющихся в .NET средств компиляции кода. Он также не использует никаких сторонних библиотек.
2. Это не транслятор, а хардкорный интерпретатор.
3. OverScript — C#-подобный язык с классическими принципами ООП. Он имеет статическую типизацию, поддерживает наследование, виртуальные методы, перегрузку операторов и многое другое. Автор ставил перед собой задачу была не придумывать новый синтаксис, а сделать язык, привычный для C#-программистов.
4. Скорость работы сопоставима с Python и ClearScript. OverScript пока чуть уступает, но ещё есть что оптимизировать. При этом он в разы быстрее других интерпретаторов, написанных на C#. Например, в 5 раз быстрее, чем MoonSharp (Lua).
5. OverScript предназначен для встраивания в .NET-программы.
6. В OverScript нет проблемы с отсутствием библиотек. Он может использовать типы стандартных .NET-библиотек. Можно импортировать функции из самописных библиотек.

Официальный сайт проекта: https://overscript.org, GitHub: https://github.com/overscript-lang/OverScript, NuGet-пакета нет. Системные требования: .NET 6 (автор не уверен, удастся ли откомпилировать под другие платформы).

Традиционный «Hello World»:

```c#
WriteLine ("Hello, world!"); // выводит "Hello, world!" с переводом строки
ReadKey(); // ожидание нажатия любой клавиши
```

К сожалению, никакой инфраструктуры для использования OverScript в чужих проектах автор не предусмотрел. Но при желании встроить OverScript в свою программу довольно легко. Почти вся магия сосредоточена в следующих строках:

```c#
internal static class Program
{
    static readonly System.Reflection.Assembly ExecutingAssembly = System.Reflection.Assembly.GetExecutingAssembly();
    public static readonly string OverScriptDir = Path.GetDirectoryName (ExecutingAssembly.Location);
    public static readonly string ModulesDir = Path.Combine (OverScriptDir, "modules");
    
    public static void Main 
        (
            string[] args
        )
    {
        if (args.Length < 1)
        {
            return;
        }

        try
        {
            var scriptFile = args[0];
            var scriptArgs = args.Skip (1).ToArray();
            var script = new Script (scriptFile, LoadingProgressChanged);
            var exec = new Executor (script);
            exec.Execute (scriptArgs);
        }
        catch (Exception exception)
        {
            Console.Error.WriteLine (exception);
        }
    }

    private static void LoadingProgressChanged 
        (
            object sender, 
            int step
        )
    {
        Console.WriteLine(step < 0 ? "completed." : $"[{new string('#', step).PadRight (Script.LoadingSteps, '-')}]");
    }
    
}
```

Проект [прилагаю](OverApp.zip). На всякий случай прилагаю также документацию с официального сайта: [основная часть](OverScriptDocs.pdf).
