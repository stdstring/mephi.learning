using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SimplePythonPorter.Common;
using SimplePythonPorter.DestStorage;

namespace SimplePythonPorter.Expressions
{
    internal class UnaryExpressionConverter
    {
        public UnaryExpressionConverter(SemanticModel model, AppData appData, ExpressionConverterSettings settings)
        {
            _settings = settings;
            _expressionConverter = new ExpressionConverter(model, appData, settings.CreateChild());
        }

        public ConvertResult Convert(PrefixUnaryExpressionSyntax expression)
        {
            ImportData importData = new ImportData();
            ConvertResult operandResult = _expressionConverter.Convert(expression.Operand);
            importData.Append(operandResult.ImportData);
            switch (expression.Kind())
            {
                case SyntaxKind.UnaryPlusExpression:
                    return new ConvertResult($"+{operandResult.Result}", importData);
                case SyntaxKind.UnaryMinusExpression:
                    return new ConvertResult($"-{operandResult.Result}", importData);
                case SyntaxKind.LogicalNotExpression:
                    return new ConvertResult($"not {operandResult.Result}", importData);
                case SyntaxKind.PreIncrementExpression when _settings.AllowIncrementDecrement:
                return new ConvertResult($"{operandResult.Result} += 1", importData);
                case SyntaxKind.PreDecrementExpression when _settings.AllowIncrementDecrement:
                    return new ConvertResult($"{operandResult.Result} -= 1", importData);
                default:
                    throw new UnsupportedSyntaxException($"Unsupported PrefixUnaryExpressionSyntax expression: {expression.Kind()}");
            }
        }

        public ConvertResult Convert(PostfixUnaryExpressionSyntax expression)
        {
            ImportData importData = new ImportData();
            ConvertResult operandResult = _expressionConverter.Convert(expression.Operand);
            importData.Append(operandResult.ImportData);
            switch (expression.Kind())
            {
                case SyntaxKind.PostIncrementExpression when _settings.AllowIncrementDecrement:
                    return new ConvertResult($"{operandResult.Result} += 1", importData);
                case SyntaxKind.PostDecrementExpression when _settings.AllowIncrementDecrement:
                    return new ConvertResult($"{operandResult.Result} -= 1", importData);
                default:
                    throw new UnsupportedSyntaxException($"Unsupported PostfixUnaryExpressionSyntax expression: {expression.Kind()}");
            }
        }

        private readonly ExpressionConverterSettings _settings;
        private readonly ExpressionConverter _expressionConverter;
    }
}
