using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SimplePythonPorter.Common;
using SimplePythonPorter.DestStorage;
using SimplePythonPorter.Utils;

namespace SimplePythonPorter.Expressions
{
    internal class InterpolatedStringExpressionConverter
    {
        public InterpolatedStringExpressionConverter(SemanticModel model, AppData appData, ExpressionConverterSettings settings)
        {
            settings.QuoteMark = '\'';
            _expressionConverter = new ExpressionConverter(model, appData, settings);
        }

        public ConvertResult Convert(InterpolatedStringExpressionSyntax expression)
        {
            Boolean isVerbatimString = expression.GetFirstToken().IsKind(SyntaxKind.InterpolatedVerbatimStringStartToken);
            String quotes = isVerbatimString ? "\"\"\"" : "\"";
            StringBuilder dest = new StringBuilder();
            ImportData importData = new ImportData();
            List<String> afterResults = new List<String>();
            dest.Append("f");
            dest.Append(quotes);
            foreach (InterpolatedStringContentSyntax content in expression.Contents)
            {
                switch (content)
                {
                    case InterpolatedStringTextSyntax text:
                        dest.Append(text.TextToken.Text);
                        break;
                    case InterpolationSyntax interpolation:
                        CheckInterpolationExpression(interpolation);
                        ConvertResult result = _expressionConverter.Convert(interpolation.Expression);
                        dest.Append($"{{{result.Result}}}");
                        importData.Append(result.ImportData);
                        afterResults.AddRange(result.AfterResults);
                        break;
                    default:
                        throw new UnsupportedSyntaxException($"Unsupported type of interpolation string content: {content}");
                }
            }
            dest.Append(quotes);
            return new ConvertResult(dest.ToString(), importData, afterResults);
        }

        private void CheckInterpolationExpression(InterpolationSyntax interpolation)
        {
            if (interpolation.AlignmentClause != null)
                throw new UnsupportedSyntaxException($"Unsupported alignment clause in expression: {interpolation}");
            if (interpolation.FormatClause != null)
                throw new UnsupportedSyntaxException($"Unsupported format clause in expression: {interpolation}");
            interpolation.Expression.DescendantNodes().OfType<LiteralExpressionSyntax>().Foreach(CheckLiteralExpressionInInterpolation);
            if (interpolation.Expression.DescendantNodes().OfType<InterpolatedStringExpressionSyntax>().Any())
                throw new UnsupportedSyntaxException($"Unsupported usage of interpolated string in expression: {interpolation}");
        }

        private void CheckLiteralExpressionInInterpolation(LiteralExpressionSyntax literalExpression)
        {
            if (literalExpression.Kind() == SyntaxKind.StringLiteralExpression &&
                literalExpression.Token.ValueText.Contains('\\'))
                throw new UnsupportedSyntaxException($"Unsupported usage of backslash symbol in literal expression: {literalExpression}");
        }

        private readonly ExpressionConverter _expressionConverter;
    }
}
