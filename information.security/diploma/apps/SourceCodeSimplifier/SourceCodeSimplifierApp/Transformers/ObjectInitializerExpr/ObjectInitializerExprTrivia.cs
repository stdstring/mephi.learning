using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SourceCodeSimplifierApp.Utils;

namespace SourceCodeSimplifierApp.Transformers.ObjectInitializerExpr
{
    internal static class ObjectInitializerExprTrivia
    {
        public static SyntaxTrivia? ExtractTrailingTrivia(ObjectCreationExpressionSyntax objectCreationExpr)
        {
            SyntaxTriviaList trailingTrivia = objectCreationExpr.ArgumentList?.GetTrailingTrivia() ?? new SyntaxTriviaList();
            return trailingTrivia.Where(trivia => trivia.IsKind(SyntaxKind.SingleLineCommentTrivia)).ToArray() switch
            {
                [] => null,
                [var commentTrivia] => commentTrivia,
                _ => throw new InvalidOperationException("Bad (several) trailing trivia")
            };
        }

        public static SyntaxTriviaList ConstructTrailingTrivia(ObjectCreationExpressionSyntax objectCreationExpr, SyntaxTrivia eolTrivia)
        {
            ArgumentListSyntax arguments = objectCreationExpr.ArgumentList ?? SyntaxFactory.ArgumentList();
            return TriviaHelper.ConstructTrailingTrivia(arguments, eolTrivia);
        }

        public static SyntaxTriviaList ConstructTrailingTrivia(ExpressionSyntax expression, SyntaxTrivia eolTrivia)
        {
            SyntaxToken lastToken = expression.GetLastToken();
            SyntaxToken nextToken = lastToken.GetNextToken();
            SyntaxTriviaList trailingTrivia = nextToken switch
            {
                _ when nextToken.IsKind(SyntaxKind.CommaToken) => nextToken.TrailingTrivia,
                _ => lastToken.TrailingTrivia
            };
            IList<SyntaxTrivia> comments = TriviaHelper.ConstructSingleLineCommentsTrivia(trailingTrivia, 1, eolTrivia);
            return comments.IsEmpty() ? new SyntaxTriviaList(eolTrivia) : new SyntaxTriviaList(comments);
        }
    }
}