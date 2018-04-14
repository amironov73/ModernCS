### Шаблон асинхронного кода в WinForms

```csharp
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
 
private static bool SomeLongOperationAsync
    (
        string arg1
    )
{
    // Делаем что-то очень сложное и длительное
    Thread.Sleep ( 3000 );
    return false;
}
 
private async void button1_Click 
    ( 
        object sender, 
        EventArgs e 
    )
{
    button1.Enabled = false;
    try
    {
      bool result = await Task.Run 
          (
              () => SomeLongOperationAsync (textBox1.Text) 
          );
      MessageBox.Show ( "Result = " + result );
    }
    finally
    {
      button1.Enabled = true;
    }
}
```
