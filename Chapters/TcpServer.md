### TCP-сервер на тасках

```csharp
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
 
namespace AsyncServer
{
    class Program
    {
        static void Main()
        {
            DoAsyncListener();
        }
 
        static void HandleClient
           (
                TcpClient client,
                CancellationToken token
           )
        {
            if (token.IsCancellationRequested)
            {
                client.Close();
                return;
            }
 
            Encoding encoding = Encoding.UTF8;
            NetworkStream stream = client.GetStream();
            byte[] buffer = new byte[4000];
            int size = stream.Read(buffer, 0, buffer.Length);
            string text = encoding.GetString(buffer, 0, size);
            Console.WriteLine("Received: {0}", text);
            text = "ACK " + text;
            buffer = encoding.GetBytes(text);
            stream.Write(buffer, 0, buffer.Length);
            client.Close();
        }
 
        static void DoAsyncListener()
        {
            CancellationTokenSource cts = new CancellationTokenSource();
            TcpListener listener = new TcpListener
                (
                    IPAddress.Loopback,
                    12345
                );
            listener.Start();
            while (!cts.IsCancellationRequested)
            {
                try
                {
                    Task<TcpClient> task = listener.AcceptTcpClientAsync();
                    task.ConfigureAwait(false);
                    task.Wait(cts.Token);
                    if (!cts.IsCancellationRequested)
                    {
                        Task.Factory.StartNew
                           (
                               () => HandleClient(task.Result, cts.Token)
                           );
                    }
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                }
            }
        }
    }
}
```

