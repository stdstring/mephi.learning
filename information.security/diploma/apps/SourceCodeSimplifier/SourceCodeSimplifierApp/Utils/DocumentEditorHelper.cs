using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;

namespace SourceCodeSimplifierApp.Utils
{
    internal static class DocumentEditorHelper
    {
        public static void ReplaceStatement(this DocumentEditor documentEditor, StatementSyntax oldStatement, IList<StatementSyntax> newStatements)
        {
            switch (oldStatement.Parent)
            {
                case null:
                    throw new NotSupportedException("Root (without parent) statements isn't support now");
                case SwitchSectionSyntax:
                case BlockSyntax:
                    documentEditor.InsertAfter(oldStatement, newStatements);
                    documentEditor.RemoveNode(oldStatement);
                    break;
                default:
                    SyntaxTrivia leadingTrivia = TriviaHelper.GetLeadingSpaceTrivia(oldStatement.Parent);
                    SyntaxTrivia trailingTrivia = TriviaHelper.GetTrailingEndOfLineTrivia(oldStatement.Parent);
                    BlockSyntax block = SyntaxFactory.Block(newStatements)
                        .WithOpenBraceToken(SyntaxFactory.Token(SyntaxKind.OpenBraceToken).WithTrailingTrivia(trailingTrivia))
                        .WithCloseBraceToken(SyntaxFactory.Token(SyntaxKind.CloseBraceToken).WithLeadingTrivia(leadingTrivia))
                        .WithLeadingTrivia(leadingTrivia)
                        .WithTrailingTrivia(trailingTrivia);
                    documentEditor.ReplaceNode(oldStatement, block);
                    break;
            }
        }
    }
}