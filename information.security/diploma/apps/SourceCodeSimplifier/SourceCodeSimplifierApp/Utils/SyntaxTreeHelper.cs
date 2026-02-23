using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace SourceCodeSimplifierApp.Utils
{
    internal static class SyntaxTreeHelper
    {
        public static StatementSyntax? FindParentStatement(this SyntaxNode node)
        {
            SyntaxNode? current = node.Parent;
            while (current != null)
            {
                if (current is StatementSyntax statement)
                    return statement;
                current = current.Parent;
            }
            return null;
        }

        public static StatementSyntax GetParentStatement(this SyntaxNode node)
        {
            return FindParentStatement(node) switch
            {
                null => throw new InvalidOperationException("Bad node - without parent statement"),
                var parent => parent
            };
        }

        public static MemberDeclarationSyntax? FindParentMember(this SyntaxNode node)
        {
            SyntaxNode? current = node.Parent;
            while (current != null)
            {
                if (current is MemberDeclarationSyntax memberDeclaration)
                    return memberDeclaration;
                current = current.Parent;
            }
            return null;
        }

        public static MemberDeclarationSyntax GetParentMember(this SyntaxNode node)
        {
            return FindParentMember(node) switch
            {
                null => throw new InvalidOperationException("Bad node - without parent member"),
                var parent => parent
            };
        }
    }
}
