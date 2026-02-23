using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace SourceCodeSimplifierApp.Utils
{
    internal static class TypeDefaultValueHelper
    {
        public static String CreateDefaultValueForType(SemanticModel model, TypeSyntax type)
        {
            return $"default({type})";
        }

        public static String CreateDefaultValueForExpression(SemanticModel model, ExpressionSyntax expression)
        {
            return CreateDefaultValue(model.GetTypeInfo(expression));
        }

        private static String CreateDefaultValue(TypeInfo typeInfo)
        {
            return typeInfo.Type switch
            {
                null => throw new InvalidOperationException("Unknown semantic info"),
                // TODO (std_string) : think about specifying of format
                var typeSymbol => $"default({typeSymbol})"
            };
        }
    }
}