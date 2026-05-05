using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SimplePythonPorter.Common;

namespace SimplePythonPorter.Converter
{
    internal static class ConvertHelper
    {
        public static String GetTypeFullName(this ITypeSymbol type)
        {
            // TODO (std_string) : think about using SymbolDisplayFormat
            return type switch
            {
                IArrayTypeSymbol arrayType => $"{arrayType.ElementType.ContainingNamespace.ToDisplayString()}.{arrayType.ElementType.Name}[]",
                _ => $"{type.ContainingNamespace.ToDisplayString()}.{type.Name}"
            };
        }

        public static IMethodSymbol GetMethodSymbol(this ExpressionSyntax expression, SemanticModel model)
        {
            SymbolInfo nodeInfo = model.GetSymbolInfo(expression);
            return nodeInfo.Symbol switch
            {
                null => throw new UnsupportedSyntaxException($"Unrecognizable type of expression: {expression.Kind()}"),
                IMethodSymbol symbol => symbol,
                _ => throw new UnsupportedSyntaxException($"Unexpected type of expression: {expression.Kind()}")
            };
        }

        public static IReadOnlyList<ArgumentSyntax> GetArguments(this ArgumentListSyntax? argumentList)
        {
            return argumentList == null ? Array.Empty<ArgumentSyntax>() : argumentList.Arguments;
        }
    }
}
