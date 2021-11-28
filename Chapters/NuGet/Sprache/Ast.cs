// ReSharper disable CheckNamespace
// ReSharper disable ConvertIfStatementToConditionalTernaryExpression
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable MemberCanBeProtected.Global
// ReSharper disable PropertyCanBeMadeInitOnly.Global
// ReSharper disable CompareOfFloatsByEqualityOperator

// Простой интерпретируемый язык с AST

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

using Sprache;

// контекст исполнения программы (проще говоря, значения переменных)
class Context
{
    public Dictionary<string, double> Variables { get; } = new ();

    public void Dump()
    {
        var keys = Variables.Keys.ToArray();
        Array.Sort (keys);
        foreach (var key in keys)
        {
            var value = Variables[key];
            Console.WriteLine ($"{key} = {value}");
        }
    }
}

// абстрактный узел AST
abstract class AstNode
{
    public virtual void Execute (Context context)
    {
    }
}

// логический узел
abstract class BooleanNode : AstNode
{
    public abstract bool Compute (Context context);
}

// сравнение двух чисел
sealed class ComparisonNode : BooleanNode
{
    private readonly AtomNode _left, _right;
    private readonly string _op;

    public ComparisonNode (AtomNode left, AtomNode right, string op)
    {
        _left = left;
        _right = right;
        _op = op;
    }

    public override bool Compute (Context context)
    {
        var left = _left.Compute (context);
        var right = _right.Compute (context);

        return _op switch
        {
            "<" => left < right,
            "<=" => left <= right,
            ">" => left > right,
            ">=" => left >= right,
            "==" => left == right,
            "!=" => left != right,
            _ => throw new Exception ($"Unknown operator '{_op}'")
        };
    }

    public override string ToString() => $"comparison ({_left} {_op} {_right})";
}

// условие, состоящее из серии сравнений
sealed class ConditionNode : BooleanNode
{
    private readonly BooleanNode _left, _right;
    private readonly string _op;

    public ConditionNode (BooleanNode left, BooleanNode right, string op)
    {
        _left = left;
        _right = right;
        _op = op;
    }

    public override bool Compute (Context context)
    {
        var left = _left.Compute (context);
        var right = _right.Compute (context);

        return _op switch
        {
            "and" => left && right,
            "or" => left || right,
            _ => throw new Exception ($"Unknown operator '{_op}'")
        };
    }

    public override string ToString() => $"condition ({_left} {_op} {_right})";
}

// узел, в котором происходят какие-то вычисления
abstract class AtomNode : AstNode
{
    public abstract double Compute (Context context);
}

// константное значение
sealed class ConstantNode : AtomNode
{
    private readonly double _value;

    public ConstantNode (double value) => _value = value;

    public override double Compute (Context context) => _value;

    public override string ToString() => $"constant '{_value}'";
}

// ссылка на переменную
sealed class VariableNode : AtomNode
{
    private readonly string _name;

    public VariableNode (string name) => _name = name;

    public override double Compute (Context context)
    {
        if (context.Variables.TryGetValue (_name, out var value))
        {
            return value;
        }

        Console.WriteLine ($"Variable '{_name}' not defined");

        return 0;
    }

    public override string ToString() => $"variable '{_name}'";
}

// изменение знака числа
sealed class NegationNode : AtomNode
{
    private readonly AtomNode _inner;

    public NegationNode (AtomNode inner) => _inner = inner;

    public override double Compute (Context context) =>
        -_inner.Compute (context);

    public override string ToString() => $"negation ({_inner})";
}

// выражение в скобках
sealed class ParenthesisNode : AtomNode
{
    private readonly AtomNode _inner;

    public ParenthesisNode (AtomNode inner)
    {
        _inner = inner;
    }

    public override double Compute (Context context) =>
        _inner.Compute (context);

    public override string ToString() => $"parenthesis ({_inner})";
}

// бинарная операция, например, сложение
sealed class BinaryNode : AtomNode
{
    private readonly AtomNode _left, _right;
    private readonly char _op;

    public BinaryNode (AtomNode left, AtomNode right, char op)
    {
        _left = left;
        _right = right;
        _op = op;
    }

    public override double Compute (Context context)
    {
        var left = _left.Compute (context);
        var right = _right.Compute (context);

        return _op switch
        {
            '+' => left + right,
            '-' => left - right,
            '*' => left * right,
            '/' => left / right,
            _ => throw new Exception ($"Unknown operation '{_op}'")
        };
    }

    public override string ToString() => $"binary ({_left} {_op} {_right})";
}

// базовый класс для стейтментов
abstract class StatementNode : AstNode
{
}

// присваивание переменной значения выражения
sealed class AssignmentNode : StatementNode
{
    public string VariableName { get; }
    public AtomNode Expression { get; }

    public AssignmentNode (string variableName, AtomNode expression)
    {
        VariableName = variableName;
        Expression = expression;
    }

    public override void Execute (Context context)
    {
        context.Variables[VariableName] = Expression.Compute (context);
    }

    public override string ToString() => $"Assignment: {VariableName} = {Expression};";
}

// распечатка значений переменных
sealed class PrintNode : StatementNode
{
    private readonly List<string> _variables;

    public PrintNode (IEnumerable<string> collection) => 
        _variables = new List<string> (collection);

    public override void Execute (Context context)
    {
        foreach (var variable in _variables)
        {
            if (variable[0] == '"')
            {
                Console.WriteLine (variable[1..^1]);
            }
            else
            {
                if (context.Variables.TryGetValue (variable, out var value))
                {
                    Console.WriteLine ($"{variable} = {value};");
                }
                else
                {
                    Console.WriteLine ($"Variable '{variable}' is not defined");
                }
            }
        }
    }

    public override string ToString()
    {
        return "Print: " + string.Join (',', _variables);
    }
}

// условный оператор if-then-else
sealed class IfNode : StatementNode
{
    private readonly BooleanNode _condition;
    private readonly List<StatementNode> _then, _else;

    public IfNode 
        (
            BooleanNode condition, 
            IEnumerable<StatementNode> thenCollection,
            IEnumerable<StatementNode> elseCollection
        )
    {
        _condition = condition;
        _then = new List<StatementNode> (thenCollection);
        _else = new List<StatementNode> (elseCollection);
    }

    public override void Execute (Context context)
    {
        if (_condition.Compute (context))
        {
            foreach (var statement in _then)
            {
                statement.Execute (context);
            }
        }
        else
        {
            foreach (var statement in _else)
            {
                statement.Execute (context);
            }
        }
    }

    public override string ToString()
    {
        var builder = new StringBuilder();
        builder.Append ($"If: {_condition}");
        builder.AppendLine();
        builder.Append ("Then: ");
        foreach (var statement in _then)
        {
            builder.AppendLine (statement.ToString());
        }

        builder.Append ("Else: ");
        foreach (var statement in _else)
        {
            builder.AppendLine (statement.ToString());
        }

        return builder.ToString();
    }
}

// цикл while
sealed class WhileNode : StatementNode
{
    private readonly BooleanNode _condition;
    private readonly List<StatementNode> _statements;

    public WhileNode (BooleanNode condition, IEnumerable<StatementNode> statements)
    {
        _condition = condition;
        _statements = new List<StatementNode> (statements);
    }

    public override void Execute (Context context)
    {
        while (_condition.Compute (context))
        {
            foreach (var statement in _statements)
            {
                statement.Execute (context);
            }
        }
    }

    public override string ToString()
    {
        var builder = new StringBuilder();
        builder.Append ($"While: {_condition}");
        builder.AppendLine();
        builder.Append ("Statements: ");
        foreach (var statement in _statements)
        {
            builder.AppendLine (statement.ToString());
        }

        return builder.ToString();
    }
}

// корень AST
sealed class ProgramNode : AstNode
{
    public List<StatementNode> Statements { get; }

    public ProgramNode (IEnumerable<StatementNode> collection)
    {
        Statements = new List<StatementNode> (collection);
    }

    public override void Execute (Context context)
    {
        foreach (var statement in Statements)
        {
            statement.Execute (context);
        }

        Console.WriteLine ("ALL DONE");
    }
}

// грамматика
static class Grammar
{
    private static readonly Parser<string> Text =
        from open in Parse.Char ('"')
        from text in Parse.CharExcept ('"').Many().Text()
        from close in Parse.Char ('"')
        select open + text + close;

    private static readonly Parser<string> Identifier =
        Parse.Identifier (Parse.Letter, Parse.LetterOrDigit);

    private static readonly Parser<AtomNode> Constant =
        from number in Parse.DecimalInvariant.Text()
        select new ConstantNode (double.Parse (number, CultureInfo.InvariantCulture));

    private static readonly Parser<AtomNode> Variable =
        from identifier in Identifier.Text()
        select new VariableNode (identifier);

    private static readonly Parser<AtomNode> Negation =
        from minus in Parse.Char ('-')
        from inner in Atom
        select new NegationNode (inner);

    private static readonly Parser<AtomNode> Parenthesis =
        from open in Parse.Char ('(').Token()
        from inner in Parse.Ref (() => ArithmeticExpression)
        from close in Parse.Char (')').Token()
        select new ParenthesisNode (inner);

    private static readonly Parser<AtomNode> Atom =
        Parenthesis.XOr (Negation).XOr (Constant).XOr (Variable);

    private static readonly Parser<AtomNode> Multiplication =
        from node in Parse.ChainOperator
            (
                Parse.Chars ('*', '/').Token(),
                Atom,
                (op, left, right) => new BinaryNode (left, right, op)
            )
        select node;

    private static readonly Parser<AtomNode> Addition =
        from node in Parse.ChainOperator
            (
                Parse.Chars ('+', '-').Token(),
                Multiplication,
                (op, left, right) =>
                    new BinaryNode (left, right, op)
            )
        select node;

    private static readonly Parser<AtomNode> ArithmeticExpression =
        from atom in Addition.XOr (Atom)
        select atom;

    private static readonly Parser<StatementNode> Assignment =
        from variable in Identifier.Text()
        from eq in Parse.Char ('=').Token()
        from expression in ArithmeticExpression
        select new AssignmentNode (variable, expression);

    private static readonly Parser<StatementNode> Print =
        from print in Parse.String ("print").Token()
        from variables in (Identifier.Or (Text)).DelimitedBy (Parse.Char (',').Token())
        select new PrintNode (variables);

    private static readonly Parser<string> Compare =
        Parse.String ("<=").Token()
            .Or (Parse.String ("<").Token())
            .Or (Parse.String (">=").Token())
            .Or (Parse.String (">").Token())
            .Or (Parse.String ("==").Token())
            .Or (Parse.String ("!=").Token()).Text();

    private static readonly Parser<string> AndOr =
        Parse.String ("and").Token().Or (Parse.String ("or").Token()).Text();

    private static readonly Parser<BooleanNode> Comparison =
        from left in Atom
        from op in Compare
        from right in Atom
        select new ComparisonNode (left, right, op);

    private static readonly Parser<BooleanNode> Condition =
        from condition in Parse.ChainOperator
            (
                AndOr.Token(),
                Comparison,
                (op, left, right) =>
                    new ConditionNode (left, right, op)
            )
        select condition;

    private static readonly Parser<IEnumerable<StatementNode>> Else =
        from _ in Parse.String ("else")
        from open in Parse.Char ('{').Token()
        from statements in Parse.Ref (() => Block)
        from close in Parse.Char ('}').Token()
        select statements;

    private static readonly Parser<StatementNode> If =
        from _ in Parse.String ("if")
        from open1 in Parse.Char ('(').Token()
        from condition in Condition
        from close1 in Parse.Char (')').Token()
        from open2 in Parse.Char ('{').Token()
        from thenCollection in Parse.Ref (() => Block)
        from close2 in Parse.Char ('}').Token()
        from elseCollection in Else.Optional()
        select new IfNode (condition, thenCollection,
            elseCollection.IsEmpty ? Array.Empty<StatementNode>() : elseCollection.Get());

    private static readonly Parser<StatementNode> While =
        from _ in Parse.String ("while")
        from open1 in Parse.Char ('(').Token()
        from condition in Condition
        from close1 in Parse.Char (')').Token()
        from open2 in Parse.Char ('{').Token()
        from statements in Block
        from close2 in Parse.Char ('}').Token()
        select new WhileNode (condition, statements);

    private static readonly Parser<StatementNode> DoesntRequireSemicolon =
        from statement in If.Or (While)
        select statement;

    private static readonly Parser<StatementNode> RequireSemicolon =
        from statement in Print.XOr (Assignment)
        from semicolon in Parse.Char (';').Token()
        select statement;

    private static readonly Parser<StatementNode> Statement =
        from statement in DoesntRequireSemicolon.Or (RequireSemicolon)
        select statement;

    private static readonly Parser<IEnumerable<StatementNode>> Block =
        from statements in Statement.Many()
        select statements;

    private static readonly Parser<ProgramNode> Program =
        from block in Block
        select new ProgramNode (block);

    public static ProgramNode ParseProgram (string sourceCode)
    {
        return Program.End().Parse (sourceCode);
    }
}

static class Program
{
    static void ExecuteProgram (string fileName)
    {
        try
        {
            var sourceCode = File.ReadAllText (fileName);
            var program = Grammar.ParseProgram (sourceCode);

            // дамп AST перед началом исполнения программы
            Console.WriteLine (new string ('-', 70));
            foreach (var statement in program.Statements)
            {
                Console.WriteLine (statement);
            }

            Console.WriteLine (new string ('-', 70));

            // исполнение программы
            var context = new Context();
            program.Execute (context);

            // дамп переменных по окончании работы программы
            Console.WriteLine (new string ('-', 70));
            context.Dump();
        }
        catch (Exception exception)
        {
            Console.WriteLine (exception);
        }
    }

    static void Main()
    {
        ExecuteProgram ("program.txt");
    }
}
