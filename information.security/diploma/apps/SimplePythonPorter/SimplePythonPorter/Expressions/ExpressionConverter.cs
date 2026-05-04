using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SimplePythonPorter.Common;
using SimplePythonPorter.DestStorage;
using SimplePythonPorter.Utils;
using System.Runtime;
using System.Text;

namespace SimplePythonPorter.Expressions
{
    internal record ConvertResult(String Result, ImportData ImportData, IList<String> AfterResults)
    {
        public ConvertResult(String Result, ImportData ImportData) : this(Result, ImportData, Array.Empty<String>())
        {
        }
    }

    internal class ExpressionConverter
    {
        public ExpressionConverter(SemanticModel model, AppData appData)
        {
            _model = model;
            _appData = appData;
        }

        public ConvertResult Convert(ExpressionSyntax expression)
        {
            ExpressionConverterVisitor visitor = new ExpressionConverterVisitor(_model, _appData);
            visitor.VisitExpression(expression);
            return new ConvertResult(visitor.Buffer.ToString(), visitor.ImportData, visitor.AfterResults);
        }

        private readonly SemanticModel _model;
        private readonly AppData _appData;
    }

    internal class ExpressionConverterVisitor : CSharpSyntaxWalker
    {
        public ExpressionConverterVisitor(SemanticModel model, AppData appData)
        {
            _model = model;
            _appData = appData;
        }

        public StringBuilder Buffer { get; } = new StringBuilder();

        public ImportData ImportData { get; } = new ImportData();

        public IList<String> AfterResults { get; } = new List<String>();

        public void VisitExpression(ExpressionSyntax expression)
        {
            if (ProcessPredefinedExpressions(expression))
                return;
            switch (expression)
            {
                case AssignmentExpressionSyntax node:
                    VisitAssignmentExpression(node);
                    break;
                case IdentifierNameSyntax node:
                    VisitIdentifierName(node);
                    break;
                case BinaryExpressionSyntax node:
                    VisitBinaryExpression(node);
                    break;
                default:
                    throw new UnsupportedSyntaxException($"Unsupported expression: {expression.Kind()}");
            }
        }

        public override void VisitAssignmentExpression(AssignmentExpressionSyntax node)
        {
            AssignmentExpressionConverter converter = new AssignmentExpressionConverter(_model, _appData);
            AppendResult(converter.Convert(node));
        }

        public override void VisitIdentifierName(IdentifierNameSyntax node)
        {
            IdentifierExpressionConverter converter = new IdentifierExpressionConverter(_model, _appData);
            AppendResult(converter.Convert(node));
        }

        public override void VisitBinaryExpression(BinaryExpressionSyntax node)
        {
            BinaryExpressionConverter converter = new BinaryExpressionConverter(_model, _appData);
            AppendResult(converter.Convert(node));
        }

        private void AppendResult(ConvertResult result)
        {
            Buffer.Append(result.Result);
            ImportData.Append(result.ImportData);
            AfterResults.AddRange(result.AfterResults);
        }

        // TODO (std_string) : think about location
        private Boolean ProcessPredefinedExpressions(ExpressionSyntax expression)
        {
            String expressionRepresentation = expression.ToString();
            switch (expressionRepresentation)
            {
                case "double.NaN":
                    Buffer.Append("math.nan");
                    ImportData.AddImport("math");
                    return true;
                case "double.MaxValue":
                    Buffer.Append("1.7976931348623157E+308");
                    return true;
                case "double.MinValue":
                    Buffer.Append("-1.7976931348623157E+308");
                    return true;
                case "string.Empty":
                    Buffer.Append("\"\"");
                    return true;
                case "null":
                    Buffer.Append("None");
                    return true;
            }
            return false;
        }

        private readonly SemanticModel _model;
        private readonly AppData _appData;
    }
}
