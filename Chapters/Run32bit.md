### ������ AnyCPU-���������� � 32-������ ������

������ ���, ��� � ��� �� ����� .NET-����������, ������������� ��� AnyCPU, �� �� �����-���� �������� ��� ���������� ��������� ��� � 32-������ ������ �� 64-������ ������. ��� ������?

������������ ����� � ���� ������ �����: �������� � �������� Windows SDK ������� corflags (��� �������, ��������� �� ���� �C:\Program Files (x86)\Microsoft SDKs\Windows\v7.0A\Bin\CorFlags.exe� ��� �C:\Program Files (x86)\Microsoft SDKs\Windows\v8.0A\bin\NETFX 4.0 Tools\CorFlags.exe� � ����������� �� ������ .NET Framework) � ��������� ��������� �����������:

```
corflags /32bit+  <C:\Path\To\program.exe>
```

��� ������� ��������� EXE-����� ����� �������, ��� ������ Windows ������ ��������� ��� ������ � 32-������ ������. �������� ������, ���� � ���� ����������� EXE-�����!

�� ���� � ����� ����������� ������ ������� ���� ��������. ���� �� ��� ������ �����: http://coolsoft.altervista.org/en/blog/2012/01/run-anycpu-net-applications-x86-mode. ���� �������, �� ������������ �������, ������� �������������� �������� ������� ������ ���������� � 32-������ ������ � �������� ���������� �� ������� �����.

� �� ��� �������� � ������� � ������� ����������� ���������, ����������� ��� WinForms-���������� (����� �������� ��������� ���� �������). ��� � �����:

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

���������� 1: ���� � ��������� ���� ���� ������������ program.exe.config, �� ��� ���������� ����������� � Run32bit.exe.config, ����� ������� �� ����� ������������ ����������.

���������� 2: ���������-���������, ������������������ ��� ���� ������ .NET (��������, 3.5), �� ������ ��������� ������, ���������������� ��� ������ ������ .NET (��������, 4.0).
