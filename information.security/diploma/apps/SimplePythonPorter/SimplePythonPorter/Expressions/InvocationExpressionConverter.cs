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
                case IdentifierNameSyntax identifierExpression:
                    return new ConvertResult($"self.{identifierExpression.Identifier.ValueText}", new ImportData());
                default:
                    throw new UnsupportedSyntaxException($"Unsupported invocation expression: {expression.Expression.Kind()}");
            }
        }

        private readonly SemanticModel _model;
        private readonly AppData _appData;
        private readonly ExpressionConverterSettings _settings;
    }
}
