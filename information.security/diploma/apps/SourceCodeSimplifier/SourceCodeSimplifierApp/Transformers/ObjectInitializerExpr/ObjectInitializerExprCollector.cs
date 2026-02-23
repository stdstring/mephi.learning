using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;
using SourceCodeSimplifierApp.Utils;

namespace SourceCodeSimplifierApp.Transformers.ObjectInitializerExpr
{
    internal static class ObjectInitializerExprCollector
    {
        public static IList<StatementSyntax> Collect(String identifierName,
                                                     String typeName,
                                                     ObjectCreationExpressionSyntax objectCreationExpr,
                                                     SyntaxTrivia leadingSpaceTrivia,
                                                     SyntaxTrivia eolTrivia)
        {
            SyntaxTriviaList leadingTrivia = new SyntaxTriviaList(leadingSpaceTrivia);
            SyntaxTriviaList trailingTrivia = new SyntaxTriviaList(eolTrivia);
            return Collect(identifierName, typeName, objectCreationExpr, leadingSpaceTrivia, eolTrivia, leadingTrivia, trailingTrivia);
        }

        public static void Collect(ExpressionSyntax baseLeftAssignment,
                                   InitializerExpressionSyntax initializerExpression,
                                   SyntaxTrivia leadingSpaceTrivia,
                                   SyntaxTrivia eolTrivia,
                                   IList<StatementSyntax> newStatements)
        {
            baseLeftAssignment = baseLeftAssignment.WithLeadingTrivia().WithTrailingTrivia();
            foreach (ExpressionSyntax expression in initializerExpression.Expressions)
            {
                FileLinePositionSpan location = expression.SyntaxTree.GetLineSpan(expression.Span);
                SyntaxTriviaList leadingTrivia = TriviaHelper.ConstructLeadingTrivia(expression.GetLeadingTrivia(), leadingSpaceTrivia, eolTrivia);
                switch (expression)
                {
                    case AssignmentExpressionSyntax {Left: SimpleNameSyntax name, Right: ObjectCreationExpressionSyntax objectCreationExpr}
                        when objectCreationExpr.Initializer.IsKind(SyntaxKind.ObjectInitializerExpression):
                    {
                        SyntaxTriviaList trailingTrivia = ObjectInitializerExprTrivia.ConstructTrailingTrivia(objectCreationExpr, eolTrivia);
                        SimpleNameSyntax memberName = SyntaxFactory.IdentifierName(name.Identifier.Text);
                        MemberAccessExpressionSyntax leftAssignment = SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, baseLeftAssignment, memberName);
                        ArgumentListSyntax arguments = SyntaxFactory.ArgumentList(objectCreationExpr.ArgumentList?.Arguments ?? new SeparatedSyntaxList<ArgumentSyntax>());
                        ObjectCreationExpressionSyntax rightAssignment = SyntaxFactory.ObjectCreationExpression(objectCreationExpr.Type, arguments, null)
                            .NormalizeWhitespace();
                        ProcessAssignmentExpression(leftAssignment, rightAssignment, leadingTrivia, trailingTrivia, newStatements);
                        if (objectCreationExpr.Initializer != null)
                            Collect(leftAssignment, objectCreationExpr.Initializer, leadingSpaceTrivia, eolTrivia, newStatements);
                        break;
                    }
                    case AssignmentExpressionSyntax {Left: SimpleNameSyntax name, Right: InitializerExpressionSyntax innerInitializerExpression}:
                    {
                        SimpleNameSyntax memberName = SyntaxFactory.IdentifierName(name.Identifier.Text);
                        MemberAccessExpressionSyntax leftAssignment = SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, baseLeftAssignment, memberName);
                        Collect(leftAssignment, innerInitializerExpression, leadingSpaceTrivia, eolTrivia, newStatements);
                        break;
                    }
                    case AssignmentExpressionSyntax {Left: SimpleNameSyntax name, Right: var rightAssignmentExpr}:
                    {
                        SyntaxTriviaList trailingTrivia = ObjectInitializerExprTrivia.ConstructTrailingTrivia(expression, eolTrivia);
                        ProcessAssignmentExpression($"{baseLeftAssignment}.{name}", rightAssignmentExpr, leadingTrivia, trailingTrivia, newStatements);
                        break;
                    }
                    default:
                        throw new InvalidOperationException($"Unexpected syntax node rather than AssignmentExpressionSyntax (location is {location})");
                }
            }
        }

        private static IList<StatementSyntax> Collect(String identifierName,
                                                      String typeName,
                                                      ObjectCreationExpressionSyntax objectCreationExpr,
                                                      SyntaxTrivia leadingSpaceTrivia,
                                                      SyntaxTrivia eolTrivia,
                                                      SyntaxTriviaList leadingTrivia,
                                                      SyntaxTriviaList trailingTrivia)
        {
            IdentifierNameSyntax identifier = SyntaxFactory.IdentifierName(identifierName);
            ArgumentListSyntax argList = SyntaxFactory.ArgumentList(objectCreationExpr.ArgumentList?.Arguments ?? new SeparatedSyntaxList<ArgumentSyntax>());
            ObjectCreationExpressionSyntax newObjectCreationExpr = SyntaxFactory.ObjectCreationExpression(objectCreationExpr.Type, argList, null)
                .NormalizeWhitespace();
            StatementSyntax newLocalDeclarationStatement = SyntaxFactory.ParseStatement($"{typeName} {identifierName} = {newObjectCreationExpr};")
                .WithLeadingTrivia(leadingTrivia)
                .WithTrailingTrivia(trailingTrivia);
            IList<StatementSyntax> objectInitializerExprStatements = new List<StatementSyntax> { newLocalDeclarationStatement };
            Collect(identifier, objectCreationExpr.Initializer!, leadingSpaceTrivia, eolTrivia, objectInitializerExprStatements);
            return objectInitializerExprStatements;
        }

        private static void ProcessAssignmentExpression(ExpressionSyntax assignmentLeftPart,
                                                        ExpressionSyntax assignmentRightPart,
                                                        SyntaxTriviaList leadingTrivia,
                                                        SyntaxTriviaList trailingTrivia,
                                                        IList<StatementSyntax> statementDest)
        {
            ProcessAssignmentExpression(assignmentLeftPart.ToString(), assignmentRightPart, leadingTrivia, trailingTrivia, statementDest);
        }

        private static void ProcessAssignmentExpression(String assignmentLeftPart,
                                                        ExpressionSyntax assignmentRightPart,
                                                        SyntaxTriviaList leadingTrivia,
                                                        SyntaxTriviaList trailingTrivia,
                                                        IList<StatementSyntax> statementDest)
        {
            StatementSyntax statement = SyntaxFactory.ParseStatement($"{assignmentLeftPart} = {assignmentRightPart};")
                .WithAdditionalAnnotations(Formatter.Annotation)
                .WithLeadingTrivia(leadingTrivia)
                .WithTrailingTrivia(trailingTrivia);
            statementDest.Add(statement);
        }
    }
}