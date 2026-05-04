using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SimplePythonPorter.Common;
using SimplePythonPorter.Converter;
using SimplePythonPorter.DestStorage;

namespace SimplePythonPorter.Expressions
{
    internal class BinaryExpressionConverter
    {
        public BinaryExpressionConverter(SemanticModel model, AppData appData)
        {
            _model = model;
            _expressionConverter = new ExpressionConverter(model, appData);
        }

        public ConvertResult Convert(BinaryExpressionSyntax expression)
        {
            ImportData importData = new ImportData();
            // TODO (std_string) : add check ability of applying binary expression for given arguments
            ConvertResult leftOperandResult = _expressionConverter.Convert(expression.Left);
            importData.Append(leftOperandResult.ImportData);
            ConvertResult rightOperandResult = _expressionConverter.Convert(expression.Right);
            importData.Append(rightOperandResult.ImportData);
            switch (expression.Kind())
            {
                case SyntaxKind.AddExpression:
                    return ProcessAddExpression(expression, leftOperandResult.Result, rightOperandResult.Result, importData);
                case SyntaxKind.SubtractExpression:
                    return new ConvertResult($"{leftOperandResult.Result} - {rightOperandResult.Result}", importData);
                case SyntaxKind.MultiplyExpression:
                    return new ConvertResult($"{leftOperandResult.Result} * {rightOperandResult.Result}", importData);
                case SyntaxKind.DivideExpression:
                    // TODO (std_string) : we must check type of arguments for choosing between float divide (/) and integer divide (//) operators
                    return new ConvertResult($"{leftOperandResult.Result} / {rightOperandResult.Result}", importData);
                case SyntaxKind.ModuloExpression:
                    return new ConvertResult($"{leftOperandResult.Result} % {rightOperandResult.Result}", importData);
                case SyntaxKind.EqualsExpression:
                    return new ConvertResult($"{leftOperandResult.Result} == {rightOperandResult.Result}", importData);
                case SyntaxKind.NotEqualsExpression:
                    return new ConvertResult($"{leftOperandResult.Result} != {rightOperandResult.Result}", importData);
                case SyntaxKind.LessThanExpression:
                    return new ConvertResult($"{leftOperandResult.Result} < {rightOperandResult.Result}", importData);
                case SyntaxKind.LessThanOrEqualExpression:
                    return new ConvertResult($"{leftOperandResult.Result} <= {rightOperandResult.Result}", importData);
                case SyntaxKind.GreaterThanExpression:
                    return new ConvertResult($"{leftOperandResult.Result} > {rightOperandResult.Result}", importData);
                case SyntaxKind.GreaterThanOrEqualExpression:
                    return new ConvertResult($"{leftOperandResult.Result} >= {rightOperandResult.Result}", importData);
                case SyntaxKind.BitwiseAndExpression:
                    return new ConvertResult($"{leftOperandResult.Result} & {rightOperandResult.Result}", importData);
                case SyntaxKind.BitwiseOrExpression:
                    return new ConvertResult($"{leftOperandResult.Result} | {rightOperandResult.Result}", importData);
                case SyntaxKind.LogicalAndExpression:
                    return new ConvertResult($"{leftOperandResult.Result} and {rightOperandResult.Result}", importData);
                case SyntaxKind.LogicalOrExpression:
                    return new ConvertResult($"{leftOperandResult.Result} or {rightOperandResult.Result}", importData);
                default:
                    throw new UnsupportedSyntaxException($"Unsupported binary expression: {expression.Kind()}");
            }
        }

        private ConvertResult ProcessAddExpression(BinaryExpressionSyntax node, String leftOperand, String rightOperand, ImportData importData)
        {
            IMethodSymbol methodSymbol = node.GetMethodSymbol(_model);
            switch (methodSymbol.ContainingType.GetTypeFullName())
            {
                case "System.String" when methodSymbol.Parameters.Length == 2:
                    String leftOperandType = methodSymbol.Parameters[0].Type.GetTypeFullName();
                    String rightOperandType = methodSymbol.Parameters[1].Type.GetTypeFullName();
                    switch (leftOperandType, rightOperandType)
                    {
                        case ("System.String", "System.String"):
                            return new ConvertResult($"{leftOperand} + {rightOperand}", importData);
                        case ("System.String", _):
                            return new ConvertResult($"{leftOperand} + str({rightOperand})", importData);
                        case (_, "System.String"):
                            return new ConvertResult($"str({leftOperand}) + {rightOperand}", importData);
                        default:
                            throw new UnsupportedSyntaxException($"Unsupported binary expression: node");
                    }
                default:
                    return new ConvertResult($"{leftOperand} + {rightOperand}", importData);
            }
        }

        private readonly SemanticModel _model;
        private readonly ExpressionConverter _expressionConverter;
    }
}
