using SampleGenerator;

namespace GeneratorDemo
{
    partial class Person
    {
        [Property] private string? _name;
        [Property] private int _age;

        public override string ToString() => $"{Name} of {Age}";
    }
}
