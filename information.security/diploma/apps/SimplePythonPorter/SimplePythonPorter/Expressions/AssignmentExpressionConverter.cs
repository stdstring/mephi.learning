using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SimplePythonPorter.Common;
using SimplePythonPorter.DestStorage;

namespace SimplePythonPorter.Expressions
{
    internal class AssignmentExpressionConverter
    {
        public AssignmentExpressionConverter(SemanticModel model, AppData appData, ExpressionConverterSettings settings)
        {
            _expressionConverter = new ExpressionConverter(model, appData, settings);
        }

        public ConvertResult Convert(AssignmentExpressionSyntax expression)
        {
            ImportData importData = new ImportData();
            switch (expression.Kind())
            {
                case SyntaxKind.SimpleAssignmentExpression:
                {
                    ConvertResult leftAssignmentResult = _expressionConverter.Convert(expression.Left);
                    importData.Append(leftAssignmentResult.ImportData);
                    ConvertResult rightAssignmentResult = _expressionConverter.Convert(expression.Right);
                    importData.Append(rightAssignmentResult.ImportData);
                    return new ConvertResult($"{leftAssignmentResult.Result} = {rightAssignmentResult.Result}", importData);
                }
                default:
                    throw new UnsupportedSyntaxException($"Unsupported assignment expression: {expression.Kind()}");
            }
        }

        private readonly ExpressionConverter _expressionConverter;
    }
}
