using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace SourceCodeSimplifierApp.Utils
{
    internal static class TriviaHelper
    {
        public static SyntaxTrivia GetLeadingSpaceTrivia(SyntaxNode node)
        {
            return SyntaxFactory.Whitespace(new String(' ', CalcLeadingSpaceTriviaSize(node)));
        }

        public static Int32 CalcLeadingSpaceDelta(StatementSyntax statement)
        {
            Int32 currentSize = CalcLeadingSpaceTriviaSize(statement);
            Int32 parentSize = CalcLeadingSpaceTriviaSize(statement.Parent.Must());
            return currentSize - parentSize;
        }

        public static SyntaxTrivia ShiftRightLeadingSpaceTrivia(SyntaxTrivia sourceTrivia, Int32 delta)
        {
            if (!sourceTrivia.IsKind(SyntaxKind.WhitespaceTrivia))
                throw new NotImplementedException($"Bad kind of trivia: {sourceTrivia.Kind()}");
            return SyntaxFactory.Whitespace(new String(' ', sourceTrivia.Span.Length + delta));
        }

        public static SyntaxTrivia GetTrailingEndOfLineTrivia(SyntaxNode node)
        {
            SyntaxTrivia endOfLineTrivia = node.GetTrailingTrivia().FirstOrDefault(trivia => trivia.IsKind(SyntaxKind.EndOfLineTrivia));
            return endOfLineTrivia.IsKind(SyntaxKind.EndOfLineTrivia) ? endOfLineTrivia : SyntaxFactory.EndOfLine(Environment.NewLine);
        }

        public static IList<SyntaxTrivia> ConstructSingleLineCommentsTrivia(SyntaxTriviaList source, SyntaxTrivia prefixTrivia, SyntaxTrivia eolTrivia)
        {
            IList<SyntaxTrivia> destTrivia = new List<SyntaxTrivia>();
            source.Where(trivia => trivia.IsKind(SyntaxKind.SingleLineCommentTrivia)).ForEach(comment =>
            {
                destTrivia.Add(prefixTrivia);
                destTrivia.Add(comment);
                destTrivia.Add(eolTrivia);
            });
            return destTrivia;
        }

        public static IList<SyntaxTrivia> ConstructSingleLineCommentsTrivia(SyntaxTriviaList source, Int32 prefixLength, SyntaxTrivia eolTrivia)
        {
            SyntaxTrivia prefixTrivia = SyntaxFactory.Whitespace(new String(' ', prefixLength));
            return ConstructSingleLineCommentsTrivia(source, prefixTrivia, eolTrivia);
        }

        public static SyntaxTriviaList ConstructLeadingTrivia(SyntaxTriviaList sourceTrivia, SyntaxTrivia leadingSpaceTrivia, SyntaxTrivia eolTrivia)
        {
            IList<SyntaxTrivia> comments = ConstructSingleLineCommentsTrivia(sourceTrivia, leadingSpaceTrivia, eolTrivia);
            comments.Add(leadingSpaceTrivia);
            return new SyntaxTriviaList(comments);
        }

        public static SyntaxTriviaList ConstructLeadingTrivia(SyntaxNode node, SyntaxTrivia leadingSpaceTrivia, SyntaxTrivia eolTrivia)
        {
            SyntaxTriviaList sourceTrivia = node.GetLeadingTrivia();
            return ConstructLeadingTrivia(sourceTrivia, leadingSpaceTrivia, eolTrivia);
        }

        public static SyntaxTriviaList ConstructTrailingTrivia(SyntaxNode node, SyntaxTrivia eolTrivia)
        {
            SyntaxTriviaList trailingTrivia = node.GetTrailingTrivia();
            IList<SyntaxTrivia> comments = ConstructSingleLineCommentsTrivia(trailingTrivia, 1, eolTrivia);
            return comments.IsEmpty() ? new SyntaxTriviaList(eolTrivia) : new SyntaxTriviaList(comments);
        }

        private static Int32 CalcLeadingSpaceTriviaSize(SyntaxNode node)
        {
            IReadOnlyList<SyntaxTrivia> leadingTrivia = node.GetLeadingTrivia();
            Int32 lastEndOfLineIndex = leadingTrivia.Count - 1;
            while (lastEndOfLineIndex >= 0)
            {
                if (leadingTrivia[lastEndOfLineIndex].IsKind(SyntaxKind.EndOfLineTrivia))
                    break;
                --lastEndOfLineIndex;
            }
            Int32 totalSpaceSize = 0;
            for (Int32 index = lastEndOfLineIndex + 1; index < leadingTrivia.Count; ++index)
                totalSpaceSize += leadingTrivia[index].Span.Length;
            return totalSpaceSize;
        }
    }
}