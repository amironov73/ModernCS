using System;
using GeneratorDemo;

internal static class Program
{
    private static void Main()
    {

        var person = new Person
        {
            Name = "Alexey",
            Age = 50
        };

        Console.WriteLine (person);
    }
}
