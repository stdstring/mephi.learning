using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SimplePythonPorter.Checker;
using SimplePythonPorter.Common;
using SimplePythonPorter.Converter;
using SimplePythonPorter.DestStorage;

namespace SimplePythonPorter.Expressions
{
    internal class InvocationExpressionConverter
    {
        public InvocationExpressionConverter(SemanticModel model, AppData appData, ExpressionConverterSettings settings)
        {
            _model = model;
            _appData = appData;
            _settings = settings;
        }

        public ConvertResult Convert(InvocationExpressionSyntax expression)
        {
            expression.ArgumentList.GetArguments().CheckForMethod().MustSuccess();
            switch (expression.Expression)
            {
                case MemberAccessExpressionSyntax memberAccessExpression:
                    MemberAccessExpressionConverter converter = new MemberAccessExpressionConverter(_model, _appData, _settings);
                    return converter.Convert(memberAccessExpression, expression.ArgumentList);
                case IdentifierNameSyntax:
                    return ConvertForIdentifierNameSyntax(expression);
                default:
                    throw new UnsupportedSyntaxException($"Unsupported invocation expression: {expression.Expression.Kind()}");
            }
        }

        private ConvertResult ConvertForIdentifierNameSyntax(InvocationExpressionSyntax expression)
        {
            ImportData importData = new ImportData();
            ArgumentListConverter argumentListConverter = new ArgumentListConverter(_model, _appData, _settings.CreateChild());
            IReadOnlyList<ArgumentSyntax> arguments = expression.ArgumentList.GetArguments();
            ConvertArgumentsResult convertedArguments = argumentListConverter.Convert(expression, arguments);
            importData.Append(convertedArguments.ImportData);
            SymbolInfo symbolInfo = _model.GetSymbolInfo(expression);
            switch (symbolInfo.Symbol)
            {
                case null:
                    throw new UnsupportedSyntaxException($"Bad symbol info for expression: {expression.Expression.Kind()}");
                case IMethodSymbol methodSymbol:
                    ITypeSymbol containedType = methodSymbol.ContainingType;
                    String containedTypeName = containedType.GetTypeFullName();
                    MemberModifier modifier = methodSymbol.DeclaredAccessibility.ToMemberModifier();
                    String methodName = _appData.NameTransformer.TransformMethodName(containedTypeName, methodSymbol.Name, modifier);
                    String invocation = $"self.{methodName}({String.Join(", ", convertedArguments.Result.Values)})";
                    return new ConvertResult(invocation, importData);
                default:
                    throw new UnsupportedSyntaxException($"Unsupported invocation expression: {expression.Expression.Kind()}");
            }
        }

        private readonly SemanticModel _model;
        private readonly AppData _appData;
        private readonly ExpressionConverterSettings _settings;
    }
}
