using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Operations;
using SourceCodeSimplifierApp.Output;
using SourceCodeSimplifierApp.Utils;
using SourceCodeSimplifierApp.Variables;

namespace SourceCodeSimplifierApp.Transformers.ObjectInitializerExpr
{
    internal class ObjectInitializerExprSyntaxRewriter : CSharpSyntaxRewriter
    {
        public ObjectInitializerExprSyntaxRewriter(SemanticModel model,
                                                   VariableManager variableManager,
                                                   IList<StatementSyntax> beforeStatements,
                                                   IList<StatementSyntax> afterStatements,
                                                   IOutput output,
                                                   String filename)
        {
            _model = model;
            _variableManager = variableManager;
            _beforeStatements = beforeStatements;
            _afterStatements = afterStatements;
            _output = output;
            _filename = filename;
        }

        public override SyntaxNode VisitObjectCreationExpression(ObjectCreationExpressionSyntax node)
        {
            CheckTrailingTrivia(node);
            SyntaxNode? expression = base.VisitObjectCreationExpression(node);
            switch (expression)
            {
                case null:
                    throw new InvalidOperationException("Bad result of ObjectCreationExpression transformation");
                case ObjectCreationExpressionSyntax destExpression:
                {
                    switch (node)
                    {
                        case {Initializer: null}:
                            return destExpression;
                        case {Initializer: var initializer} when initializer.IsKind(SyntaxKind.CollectionInitializerExpression):
                            return destExpression;
                        case {Parent: ArgumentSyntax argument}:
                            return ProcessObjectCreationExpressionInArg(node, destExpression, argument);
                        case {Parent: ReturnStatementSyntax}:
                            return ProcessObjectCreationExpressionWithVar(node, destExpression, "returnValue");
                        case {Parent: InitializerExpressionSyntax}:
                            return ProcessObjectCreationExpressionWithVar(node, destExpression, "initValue");
                        case {Parent: AssignmentExpressionSyntax{Parent: InitializerExpressionSyntax}}:
                            return destExpression;
                        case {Parent: AssignmentExpressionSyntax{Parent: ExpressionStatementSyntax statement, Left: var assignmentLeft}}:
                            return ProcessObjectCreationExpressionForAssignment(destExpression, statement, assignmentLeft);
                        case {Parent: EqualsValueClauseSyntax{Parent: VariableDeclaratorSyntax{Parent: VariableDeclarationSyntax{Parent: LocalDeclarationStatementSyntax statement}}}}:
                            return ProcessObjectCreationExpressionForLocalDecl(destExpression, statement);
                        default:
                            return destExpression;
                    }
                }
                default:
                    return expression;
            }
        }

        private void CheckTrailingTrivia(ObjectCreationExpressionSyntax node)
        {
            if (node.Initializer == null)
                return;
            SyntaxTrivia? trailingTrivia = ObjectInitializerExprTrivia.ExtractTrailingTrivia(node);
            if (trailingTrivia == null)
                return;
            FileLinePositionSpan location = node.SyntaxTree.GetLineSpan(node.Span);
            _output.WriteWarningLine(_filename, location.StartLinePosition.Line, $"Unprocessed (lost) trailing comment: \"{trailingTrivia}\"");
        }

        private SyntaxNode ProcessObjectCreationExpressionInArg(ObjectCreationExpressionSyntax source,
                                                                ObjectCreationExpressionSyntax current,
                                                                ArgumentSyntax argument)
        {
            IOperation? operationInfo = _model.GetOperation(argument);
            switch (operationInfo)
            {
                case null:
                    throw new InvalidOperationException("Bad object initializer expression: absence type info");
                case IArgumentOperation {Parameter: null}:
                    throw new InvalidOperationException("Bad object initializer expression: absence parameter info");
                case IArgumentOperation {Parameter: var parameterInfo}:
                    return ProcessObjectCreationExpressionWithVar(source, current, parameterInfo.Name);
                default:
                    throw new InvalidOperationException("Bad object initializer expression: unknown operation info");
            }
        }

        private SyntaxNode ProcessObjectCreationExpressionWithVar(ObjectCreationExpressionSyntax source,
                                                                  ObjectCreationExpressionSyntax current,
                                                                  String prefixName)
        {
            String variableName = _variableManager.GenerateVariableName(source, prefixName);
            String typeName = current.Type.ToString();
            StatementSyntax parentStatement = source.GetParentStatement();
            SyntaxTrivia leadingSpaceTrivia = TriviaHelper.GetLeadingSpaceTrivia(parentStatement);
            SyntaxTrivia eolTrivia = TriviaHelper.GetTrailingEndOfLineTrivia(parentStatement);
            IList<StatementSyntax> dest = ObjectInitializerExprCollector.Collect(variableName, typeName, current, leadingSpaceTrivia, eolTrivia);
            _beforeStatements.AddRange(dest);
            return SyntaxFactory.IdentifierName(variableName);
        }

        private SyntaxNode ProcessObjectCreationExpressionForAssignment(ObjectCreationExpressionSyntax current,
                                                                        ExpressionStatementSyntax parentStatement,
                                                                        ExpressionSyntax assignmentLeft)
        {
            SyntaxTrivia leadingSpaceTrivia = TriviaHelper.GetLeadingSpaceTrivia(parentStatement);
            SyntaxTrivia eolTrivia = TriviaHelper.GetTrailingEndOfLineTrivia(parentStatement);
            IList<StatementSyntax> dest = new List<StatementSyntax>();
            ObjectInitializerExprCollector.Collect(assignmentLeft, current.Initializer!, leadingSpaceTrivia, eolTrivia, dest);
            _afterStatements.AddRange(dest);
            ArgumentListSyntax argList = SyntaxFactory.ArgumentList(current.ArgumentList?.Arguments ?? new SeparatedSyntaxList<ArgumentSyntax>());
            return SyntaxFactory.ObjectCreationExpression(current.Type, argList, null).NormalizeWhitespace();
        }

        private SyntaxNode ProcessObjectCreationExpressionForLocalDecl(ObjectCreationExpressionSyntax current, LocalDeclarationStatementSyntax parentStatement)
        {
            VariableDeclarationSyntax variableDeclaration = parentStatement.Declaration;
            if (variableDeclaration.Variables.Count > 1)
                throw new NotSupportedException("More than one variable declarations is not supported now");
            VariableDeclaratorSyntax variableDeclarator = variableDeclaration.Variables.First();
            IdentifierNameSyntax identifier = SyntaxFactory.IdentifierName(variableDeclarator.Identifier);
            SyntaxTrivia leadingSpaceTrivia = TriviaHelper.GetLeadingSpaceTrivia(parentStatement);
            SyntaxTrivia eolTrivia = TriviaHelper.GetTrailingEndOfLineTrivia(parentStatement);
            IList<StatementSyntax> dest = new List<StatementSyntax>();
            ObjectInitializerExprCollector.Collect(identifier, current.Initializer!, leadingSpaceTrivia, eolTrivia, dest);
            _afterStatements.AddRange(dest);
            ArgumentListSyntax argList = SyntaxFactory.ArgumentList(current.ArgumentList?.Arguments ?? new SeparatedSyntaxList<ArgumentSyntax>());
            return SyntaxFactory.ObjectCreationExpression(current.Type, argList, null)
                .NormalizeWhitespace();
        }

        private readonly SemanticModel _model;
        private readonly IList<StatementSyntax> _beforeStatements;
        private readonly IList<StatementSyntax> _afterStatements;
        private readonly VariableManager _variableManager;
        private readonly IOutput _output;
        private readonly String _filename;
    }
}