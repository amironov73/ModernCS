### TensorFlowSharp

GitHub: https://github.com/migueldeicaza/TensorFlowSharp, NuGet: https://www.nuget.org/packages/TensorFlowSharp/

```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TensorFlow;

class Program
{
    static void Main()
    {
        using (var session = new TFSession())
        {
            var graph = session.Graph;

            var a = graph.Const(2);
            var b = graph.Const(3);
            Console.WriteLine("a=2 b=3");

            var addingResults = session.GetRunner().Run(graph.Add(a, b));
            var addingResultValue = addingResults.GetValue();
            Console.WriteLine("a+b={0}", addingResultValue);

            // Multiply two constants
            var multiplyResults = session.GetRunner().Run(graph.Mul(a, b));
            var multiplyResultValue = multiplyResults.GetValue();
            Console.WriteLine("a*b={0}", multiplyResultValue);
        }
    }
}
```

Результат:

```
2018-05-27 13:09:57.998050: I tensorflow/core/platform/cpu_feature_guard.cc:140] Your CPU
supports instructions that this TensorFlow binary was not compiled to use: AVX2
a=2 b=3
a+b=5
a*b=6
```

