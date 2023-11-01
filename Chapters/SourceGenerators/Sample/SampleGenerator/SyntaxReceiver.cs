using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace SampleGenerator
{
    internal sealed class SyntaxReceiver
        : ISyntaxContextReceiver
    {
        public List<IFieldSymbol> Collected { get; } = new List<IFieldSymbol>();

        public SyntaxReceiver (string attributeName)
            => _attributeName = attributeName;

        private readonly string _attributeName;

        private bool ContainsAttribute 
            (
                AttributeData attributeData
            )
        {
            var attributeClass = attributeData.AttributeClass;
            if (attributeClass is null)
            {
                return false;
            }

            var attributeName = attributeClass.ToDisplayString();
            return StringComparer.Ordinal.Equals (attributeName, _attributeName );
        }

        /// <inheritdoc cref="ISyntaxContextReceiver.OnVisitSyntaxNode"/>
        public void OnVisitSyntaxNode
            (
                GeneratorSyntaxContext context
            )
        {
            if (context.Node is FieldDeclarationSyntax node
                && node.AttributeLists.Count > 0)
            {
                foreach (var variable in node.Declaration.Variables)
                {
                    if (context.SemanticModel.GetDeclaredSymbol (variable) is IFieldSymbol symbol
                        && symbol.GetAttributes().Any (ContainsAttribute))
                    {
                        Collected.Add (symbol);
                    }
                }
            }
        }
    }
}
