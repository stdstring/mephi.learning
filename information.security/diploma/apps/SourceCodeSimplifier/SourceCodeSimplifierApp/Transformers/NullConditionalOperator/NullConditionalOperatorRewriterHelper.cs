using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace SourceCodeSimplifierApp.Transformers.NullConditionalOperator
{
    internal static class NullConditionalOperatorRewriterHelper
    {
        // TODO (std_string) : think about more elegance solution
        public static IList<ExpressionSyntax> SplitParts(this ConditionalAccessExpressionSyntax conditionalAccessExpression)
        {
            IList<ExpressionSyntax> parts = new List<ExpressionSyntax>();
            while (true)
            {
                parts.Add(conditionalAccessExpression.Expression);
                switch (conditionalAccessExpression.WhenNotNull)
                {
                    case ConditionalAccessExpressionSyntax childAccessExpression:
                        conditionalAccessExpression = childAccessExpression;
                        break;
                    default:
                        parts.Add(conditionalAccessExpression.WhenNotNull);
                        return parts;
                }
            }
        }

        public static Boolean CanProcess(this ConditionalAccessExpressionSyntax expression)
        {
            return !expression.DescendantNodes()
                .OfType<ParenthesizedExpressionSyntax>()
                .Select(expr => expr.Expression)
                .Any(HasExpressionForProcessing);
        }

        public static Boolean CanProcess(this BinaryExpressionSyntax expression)
        {
            if (!expression.IsKind(SyntaxKind.CoalesceExpression))
                return true;
            return !expression.DescendantNodes()
                .OfType<ParenthesizedExpressionSyntax>()
                .Select(expr => expr.Expression)
                .Any(HasExpressionForProcessing);
        }

        private static Boolean HasExpressionForProcessing(this ExpressionSyntax expression)
        {
            return expression.DescendantNodes().OfType<ConditionalAccessExpressionSyntax>().Any();
        }
    }
}