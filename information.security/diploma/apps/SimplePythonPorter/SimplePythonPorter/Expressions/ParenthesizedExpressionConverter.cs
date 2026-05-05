using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SimplePythonPorter.Common;

namespace SimplePythonPorter.Expressions
{
    internal class ParenthesizedExpressionConverter
    {
        public ParenthesizedExpressionConverter(SemanticModel model, AppData appData, ExpressionConverterSettings settings)
        {
            _expressionConverter = new ExpressionConverter(model, appData, settings);
        }

        public ConvertResult Convert(ParenthesizedExpressionSyntax expression)
        {
            ConvertResult innerResult = _expressionConverter.Convert(expression.Expression);
            return new ConvertResult($"({innerResult.Result})", innerResult.ImportData);
        }

        private readonly ExpressionConverter _expressionConverter;
    }
}
