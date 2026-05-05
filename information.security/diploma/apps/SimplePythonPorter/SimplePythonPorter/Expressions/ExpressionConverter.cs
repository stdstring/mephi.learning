using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SimplePythonPorter.Common;
using SimplePythonPorter.DestStorage;
using SimplePythonPorter.Utils;
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
        public ExpressionConverter(SemanticModel model, AppData appData, ExpressionConverterSettings settings)
        {
            _model = model;
            _appData = appData;
            _settings = settings;
        }

        public ConvertResult Convert(ExpressionSyntax expression)
        {
            ExpressionConverterVisitor visitor = new ExpressionConverterVisitor(_model, _appData, _settings);
            visitor.VisitExpression(expression);
            return new ConvertResult(visitor.Buffer.ToString(), visitor.ImportData, visitor.AfterResults);
        }

        private readonly SemanticModel _model;
        private readonly AppData _appData;
        private readonly ExpressionConverterSettings _settings;
    }

    internal class ExpressionConverterVisitor : CSharpSyntaxWalker
    {
        public ExpressionConverterVisitor(SemanticModel model, AppData appData, ExpressionConverterSettings settings)
        {
            _model = model;
            _appData = appData;
            _settings = settings;
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
                case BinaryExpressionSyntax node:
                    VisitBinaryExpression(node);
                    break;
                case IdentifierNameSyntax node:
                    VisitIdentifierName(node);
                    break;
                case InterpolatedStringExpressionSyntax node:
                    VisitInterpolatedStringExpression(node);
                    break;
                case InvocationExpressionSyntax node:
                    VisitInvocationExpression(node);
                    break;
                case LiteralExpressionSyntax node:
                    VisitLiteralExpression(node);
                    break;
                case MemberAccessExpressionSyntax node:
                    VisitMemberAccessExpression(node);
                    break;
                case ObjectCreationExpressionSyntax node:
                    VisitObjectCreationExpression(node);
                    break;
                case ParenthesizedExpressionSyntax node:
                    VisitParenthesizedExpression(node);
                    break;
                case PrefixUnaryExpressionSyntax node:
                    VisitPrefixUnaryExpression(node);
                    break;
                case PostfixUnaryExpressionSyntax node:
                    VisitPostfixUnaryExpression(node);
                    break;
                default:
                    throw new UnsupportedSyntaxException($"Unsupported expression: {expression.Kind()}");
            }
        }

        public override void VisitAssignmentExpression(AssignmentExpressionSyntax node)
        {
            AssignmentExpressionConverter converter = new AssignmentExpressionConverter(_model, _appData, _settings.CreateChild());
            AppendResult(converter.Convert(node));
        }

        public override void VisitBinaryExpression(BinaryExpressionSyntax node)
        {
            BinaryExpressionConverter converter = new BinaryExpressionConverter(_model, _appData, _settings.CreateChild());
            AppendResult(converter.Convert(node));
        }

        public override void VisitIdentifierName(IdentifierNameSyntax node)
        {
            IdentifierExpressionConverter converter = new IdentifierExpressionConverter(_model, _appData, _settings);
            AppendResult(converter.Convert(node));
        }

        public override void VisitInterpolatedStringExpression(InterpolatedStringExpressionSyntax node)
        {
            InterpolatedStringExpressionConverter converter = new InterpolatedStringExpressionConverter(_model, _appData, _settings.CreateChild());
            AppendResult(converter.Convert(node));
        }

        public override void VisitInvocationExpression(InvocationExpressionSyntax node)
        {
            InvocationExpressionConverter converter = new InvocationExpressionConverter(_model, _appData, _settings);
            AppendResult(converter.Convert(node));
        }

        public override void VisitLiteralExpression(LiteralExpressionSyntax node)
        {
            LiteralExpressionConverter converter = new LiteralExpressionConverter(_settings.CreateChild());
            AppendResult(converter.Convert(node));
        }

        public override void VisitMemberAccessExpression(MemberAccessExpressionSyntax node)
        {
            MemberAccessExpressionConverter converter = new MemberAccessExpressionConverter(_model, _appData, _settings);
            AppendResult(converter.Convert(node));
        }

        public override void VisitObjectCreationExpression(ObjectCreationExpressionSyntax node)
        {
            ObjectCreationExpressionConverter converter = new ObjectCreationExpressionConverter(_model, _appData, _settings);
            AppendResult(converter.Convert(node));
        }

        public override void VisitParenthesizedExpression(ParenthesizedExpressionSyntax node)
        {
            ParenthesizedExpressionConverter converter = new ParenthesizedExpressionConverter(_model, _appData, _settings.CreateChild());
            AppendResult(converter.Convert(node));
        }

        public override void VisitPrefixUnaryExpression(PrefixUnaryExpressionSyntax node)
        {
            UnaryExpressionConverter converter = new UnaryExpressionConverter(_model, _appData, _settings);
            AppendResult(converter.Convert(node));
        }

        public override void VisitPostfixUnaryExpression(PostfixUnaryExpressionSyntax node)
        {
            UnaryExpressionConverter converter = new UnaryExpressionConverter(_model, _appData, _settings);
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
        private readonly ExpressionConverterSettings _settings;
    }
}
