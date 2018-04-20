### Moq

GitHub: https://github.com/moq/moq4,  NuGet: https://www.nuget.org/packages/Moq, документация: http://www.nudoq.org/#!/Projects/Moq.

```csharp
using System;
using Moq;
 
public interface ICalculator
{
    int Add(int left, int right);
 
    int Sub(int left, int right);
}
 
class Program
{
    static void Main(string[] args)
    {
        var mock = new Mock<ICalculator>();
        mock.Setup(c => c.Add(1, 2)).Returns(3);
        mock.Setup(c => c.Sub(It.IsAny<int>(),
            It.IsAny<int>())).Returns(0);
 
        ICalculator calc = mock.Object;
        int result = calc.Add(1, 2);
        Console.WriteLine(result);
        result = calc.Add(2, 3);
        Console.WriteLine(result);
        result = calc.Sub(5, 10);
        Console.WriteLine(result);
        result = calc.Sub(50, 10);
        Console.WriteLine(result);
 
        mock.Verify(c => c.Add(1, 2), Times.Once);
        mock.Verify(c => c.Add(2, 3), Times.Once);
        mock.Verify(c => c.Add(It.IsAny<int>(),
            It.IsAny<int>()), Times.Exactly(2));
    }
}
```
