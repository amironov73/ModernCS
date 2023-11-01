using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace SampleGenerator
{

#pragma warning disable RS1035 // Do not use banned APIs for analyzers

    [Generator]
    public sealed class PropertyGenerator
        : ISourceGenerator
    {
        private const string AttributeName = "SampleGenerator.PropertyAttribute";

        private const string AttributeText = @"using System;

namespace SampleGenerator
{
    [AttributeUsage (AttributeTargets.Field)]
    internal sealed class PropertyAttribute: Attribute
    {
    }
}
";

        public void Initialize
            (
                GeneratorInitializationContext context
            )
        {
            context.RegisterForPostInitialization
                (
                    it => it.AddSource
                        (
                            "PropertyAttribute.g.cs",
                            AttributeText
                        )
                );

            context.RegisterForSyntaxNotifications (() => new SyntaxReceiver (AttributeName));
        }

        public void Execute
            (
                GeneratorExecutionContext context
            )
        {
            if (!(context.SyntaxContextReceiver is SyntaxReceiver receiver))
            {
                return;
            }

            var types = receiver.Collected.GroupBy<IFieldSymbol, INamedTypeSymbol>
                (
                    it => it.ContainingType, SymbolEqualityComparer.Default
                );
            foreach (var group in types)
            {
                var classSource = ProcessClass (group.Key, group.ToList());
                if (!string.IsNullOrEmpty (classSource))
                {
                    context.AddSource
                        (
                            $"{group.Key.Name}_properties.g.cs",
                            SourceText.From (classSource!, Encoding.UTF8)
                        );
                }
            }
        }

        private string? ProcessClass
            (
                INamedTypeSymbol classSymbol,
                IList<IFieldSymbol> fields
            )
        {
            if (!classSymbol.ContainingSymbol.Equals
                    (
                        classSymbol.ContainingNamespace,
                        SymbolEqualityComparer.Default
                    ))
            {
                // оказались вне пространства имен, это странно
                return null;
            }

            var namespaceName = classSymbol.ContainingNamespace.ToDisplayString();

            var source = new StringBuilder (
$@"
namespace {namespaceName}
{{
    partial class {classSymbol.Name}
    {{
");

            foreach (var fieldSymbol in fields)
            {
                ProcessField (source, fieldSymbol);
            }

            source.Append ("} }");
            return source.ToString();
        }

        private void ProcessField
            (
                StringBuilder source,
                IFieldSymbol fieldSymbol
            )
        {
            var fieldName = fieldSymbol.Name;
            var fieldType = fieldSymbol.Type;

            var propertyName = ChooseName (fieldName);
            if (propertyName.Length == 0 || propertyName == fieldName)
            {
                //TODO: issue a diagnostic that we can't process this field
                return;
            }

            source.Append
                (
    $@"

        public {fieldType} {propertyName}
        {{
            get => this.{fieldName};
            set => this.{fieldName} = value;
        }}
    "
                );
        }

        private static string ChooseName
            (
                string fieldName
            )
        {
            var result = fieldName.TrimStart ('_');
            return result.Length switch
            {
                0 => string.Empty,
                1 => result.ToUpper(),
                _ => result.Substring (0, 1).ToUpper()
                     + result.Substring (1)
            };
        }
   }

}
