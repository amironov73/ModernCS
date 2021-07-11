### Модификация исходного кода с помощью Roslyn

Допустим, у нас есть некий исходный код, в который мы хотим вносить изменения. Вот как это делается с помощью Roslyn:

```c#
using System;
using System.Linq;
 
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
 
#nullable enable
 
class Program
{
    // программа, подлежащая модификации
    private const string SourceCode = @"using System;
class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine(""Hello World!"");
    }
}";
     
    static void Main()
    {
        // синтаксическое дерево
        var tree = CSharpSyntaxTree.ParseText(SourceCode);
         
        // корневой узел
        var root = (CompilationUnitSyntax)tree.GetRoot();
         
        // добавляем using
        root = root.AddUsings
            (
                SyntaxFactory.UsingDirective
                    (
                        SyntaxFactory.ParseName("System.IO")
                    )
            );
         
        // находим метод Main
        var main = 
            (
                from method 
                in root.DescendantNodes()
                .OfType<MethodDeclarationSyntax>()
                where method.Identifier.ValueText == "Main"
                select method
            )
            .First();
 
        // конструируем Console.WriteLine("Здравствуй, мир!")
        var console = SyntaxFactory.IdentifierName("Console");
        var writeLine = SyntaxFactory.IdentifierName("WriteLine");
        var memberAccess = SyntaxFactory.MemberAccessExpression
            (
                SyntaxKind.SimpleMemberAccessExpression, 
                console,
                writeLine
            );
        var argument = SyntaxFactory.Argument
            (
                SyntaxFactory.LiteralExpression
                    (
                        SyntaxKind.StringLiteralExpression,
                        SyntaxFactory.Literal("Здравствуй, мир!")
                    )
            );
        var argumentList = SyntaxFactory.SeparatedList
            (
                new[] { argument }
            );
        var writeLineCall = SyntaxFactory.ExpressionStatement
            (
                SyntaxFactory.InvocationExpression
                    (
                        memberAccess,
                        SyntaxFactory.ArgumentList(argumentList)
                    )
            );
 
        // добавляем сконструированный вызов
        var newMain = main.AddBodyStatements(writeLineCall);
         
        // заменяем тело функции
        root = root.ReplaceNode(main, newMain)
            .NormalizeWhitespace();
         
        // распечатываем результат
        Console.WriteLine(root.ToFullString());
 
        // Вот и всё!
    }
}
```

А вот как можно «сшить» два исходника: все стейтменты из второго добавляются в конец Main из первого:

```c#
using System;
using System.Linq;
 
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
 
const string SourceCodeOne = @"using System;
class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine(""Hello World!"");
    }
}";
 
const string SourceCodeTwo = @"
var counter = 0;
 
for (var i = 0; i < 3; i++)
{
  counter++;
  ConsoleWriteLine($""{i}"");
}
 
Console.WriteLine(counter);
";
 
// синтаксические деревья
var treeOne = CSharpSyntaxTree.ParseText(SourceCodeOne);
var treeTwo = CSharpSyntaxTree.ParseText(SourceCodeTwo);
          
// корневые узлы
var rootOne = treeOne.GetRoot();
var rootTwo = treeTwo.GetRoot();
 
// находим метод Main
var main = 
    (
        from method in rootOne.DescendantNodes()
                .OfType<MethodDeclarationSyntax>()
        where method.Identifier.ValueText == "Main"
        select method
    )
    .First();
 
var statements = rootTwo.ChildNodes();
var newMain = main;
foreach (var node in statements)
{
    newMain = newMain.AddBodyStatements(((GlobalStatementSyntax)node).Statement);
}
 
var resultRoot = rootOne.ReplaceNode(main, newMain)
    .NormalizeWhitespace();
 
Console.WriteLine(resultRoot.ToFullString());
```
