using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace SourceCodeSimplifierApp.Utils
{
    internal static class ExpressionResolverHelper
    {
        public static String ResolveExpressionType(this SemanticModel model, ExpressionSyntax expression)
        {
            TypeInfo typeInfo = model.GetTypeInfo(expression);
            return typeInfo.Type switch
            {
                null => throw new InvalidOperationException("Unknown semantic info"),
                // TODO (std_string) : think about specifying of format
                var typeSymbol => typeSymbol.ToDisplayString()
            };
        }
    }
}