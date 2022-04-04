### ������������ ���� OverScript

�� ����� ��������� [������](https://habr.com/ru/post/656035/) ��� ���� OverScript � ������������ ���� ��� ��������� .NET 6:

1. ������������� �������� ��������� �����������, �.�. �� ���������� ��������� � .NET ������� ���������� ����. �� ����� �� ���������� ������� ��������� ���������.
2. ��� �� ����������, � ���������� �������������.
3. OverScript � C#-�������� ���� � ������������� ���������� ���. �� ����� ����������� ���������, ������������ ������������, ����������� ������, ���������� ���������� � ������ ������. ����� ������ ����� ����� ������ ���� �� ����������� ����� ���������, � ������� ����, ��������� ��� C#-�������������.
4. �������� ������ ����������� � Python � ClearScript. OverScript ���� ���� ��������, �� ��� ���� ��� ��������������. ��� ���� �� � ���� ������� ������ ���������������, ���������� �� C#. ��������, � 5 ��� �������, ��� MoonSharp (Lua).
5. OverScript ������������ ��� ����������� � .NET-���������.
6. � OverScript ��� �������� � ����������� ���������. �� ����� ������������ ���� ����������� .NET-���������. ����� ������������� ������� �� ���������� ���������.

����������� ���� �������: https://overscript.org, GitHub: https://github.com/overscript-lang/OverScript, NuGet-������ ���. ��������� ����������: .NET 6 (����� �� ������, ������� �� ��������������� ��� ������ ���������).

������������ �Hello World�:

```c#
WriteLine ("Hello, world!"); // ������� "Hello, world!" � ��������� ������
ReadKey(); // �������� ������� ����� �������
```

� ���������, ������� �������������� ��� ������������� OverScript � ����� �������� ����� �� ������������. �� ��� ������� �������� OverScript � ���� ��������� �������� �����. ����� ��� ����� ������������� � ��������� �������:

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

������ [��������](OverApp.zip). �� ������ ������ �������� ����� ������������ � ������������ �����: [�������� �����](OverScriptDocs.pdf).
