### ���������� Polly

Polly � ��� ����������, ������� ��������� .NET-������������� �������� � ���� ���������� ������������ � ������� � �������� �� ��������� �� ������ ��������� � ������. ��� ������������� ����� ��������: Retry, Circuit Breaker, Timeout, Bulkhead Isolation � Fallback.

���� �������: http://www.thepollyproject.org, GitHub: https://github.com/App-vNext/Polly, NuGet: https://www.nuget.org/packages/Polly/. ����������� � ���������: https://github.com/App-vNext/Polly-Samples.

��������������: .NET 4.5, .NET Standard 1.1 � 2.0. ������ 4.x ������������ .NET 3.5 � 4.0.

```csharp
using System;
using System.Net;
 
using Polly;
 
class Program
{
    static void Main()
    {
        var policy = Policy.Handle<Exception>().Retry(3, (ex, attempt) =>
        {
            Console.WriteLine($"Got {ex.Message}");
            Console.WriteLine($"Attempt: {attempt}");
        });
 
        using (WebClient client = new WebClient())
        {
            policy.Execute
                (
                    () =>
                    {
                        string s = client.DownloadString("http://noogle.com");
                        Console.WriteLine($"Downloaded: {s}");
                    }
                );
        }
    }
}
```

�����

```
Attempt: 1
Got An error occurred while sending the request. �� ������� ���������� ���������� � ��������
Attempt: 2
Got An error occurred while sending the request. �� ������� ���������� ���������� � ��������
Attempt: 3
 
Unhandled Exception: System.Net.WebException: An error occurred while sending the request.
�� ������� ���������� ���������� � �������� ---> System.Net.Http.HttpRequestException: An error occurred while sending the request. ---> System.Net.Http.WinHttpException: �� ������� ���������� ���������� � ��������
   at System.Runtime.ExceptionServices.ExceptionDispatchInfo.Throw()
   at System.Threading.Tasks.RendezvousAwaitable`1.GetResult()
   at System.Net.Http.WinHttpHandler.d__105.MoveNext()
   --- End of inner exception stack trace ---
C:\projects\polly\src\Polly.Shared\Retry\RetrySyntax.cs:line 97
   at Polly.Policy.ExecuteInternal(Action`2 action, Context context, CancellationToken cancellationToken) in C:\projects\polly\src\Polly.Shared\Policy.cs:line 43
   at Polly.Policy.Execute(Action`2 action, Context context, CancellationToken cancellationToken) in C:\projects\polly\src\Polly.Shared\Policy.ExecuteOverloads.cs:line 83
   at Program.Main() in D:\Projects\Misc\ConsoleApp40\ConsoleApp40\Program.cs:line 18
```

|��������|�������|��-������|��� ����� �������?|
|--------|-------|---------|------------------|
|Retry (���������)|���� �������� ��������� � ����� �������� ����� ����� �������������� ��������������.|���������, ��� ��� ��������� ����|��� ��������� ����	����� ��������� �������������� ���������� �������.|
|Circuit Breaker(���������)|����� ������� ������� ���������, ����� ����� �������� ������, ��� ���������� ����������� �������� �����. ������ �� ���������� �������.|�����������, ���� ������ ����������, ����� ������� ���������|��������� ���������� �� ��������� �����, ���� ���������� ����� �������� �������� �����.|
|Timeout|��� ���������� ����-���� ������ ����� �� ����� ������.|��� ����� �����|�����������, ��� ����������� ����� ����� �� ����� ���������� �������.|
|Bulkhead Isolation|����� ���������� ����, ���������������� ������� ����� �������� ��� ��������� ������� �����. ������� ���������� ����� �������� � ����������� �������� ����������� ����������. ���� � ����� ���������� ����� ���������������� � �� ������.|����� ������ �� ������ �������� ���� ��������|������������ ����������� ���������� ����� �������� �������������� �������, �������� � �� ������.|
|Cache|��������� ������� ����� ���� �����������.|��� ��� ���������� ���|������������� ����� �� ����. �������� ���������� � ��� ��� ������ �������� ����������.|
|Fallback|����������, ��� ������ ��������, ����� �� ����� �����������.|��������� �����|����� ��������, ������� ����� ���������� � ������ ����.|
|PolicyWrap|������ ���� ������� ������ ��������; �������� �������� �������������� ���������.|���������������� ������|����� ����� ������������� ��������� �������.|
