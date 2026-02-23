using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Operations;
using SourceCodeSimplifierApp.Utils;
using SourceCodeSimplifierApp.Variables;

namespace SourceCodeSimplifierApp.Transformers.NullConditionalOperator
{
    internal record NullConditionalOperatorRewriterResult(Boolean Last, IList<StatementSyntax> Statements);

    internal class NullConditionalOperatorRewriter : CSharpSyntaxRewriter
    {
        public NullConditionalOperatorRewriter(SemanticModel model, VariableManager variableManager)
        {
            _model = model;
            _variableManager = variableManager;
        }

        public NullConditionalOperatorRewriterResult Process(StatementSyntax sourceStatement)
        {
            StatementSyntax destStatement = Visit(sourceStatement).MustCast<SyntaxNode, StatementSyntax>();
            IList<StatementSyntax> result = _destStatements.Select(statement => SyntaxFactory.ParseStatement(statement)).ToList();
            if (_includeTransformedSource || !_lastTransformations)
                result.Add(destStatement);
            return new NullConditionalOperatorRewriterResult(_lastTransformations, result);
        }

        public override SyntaxNode VisitConditionalAccessExpression(ConditionalAccessExpressionSyntax node)
        {
            ConditionalAccessExpressionSyntax destExpression = base.VisitConditionalAccessExpression(node).MustCast<SyntaxNode, ConditionalAccessExpressionSyntax>();
            if (!node.CanProcess())
                return destExpression;
            switch (node.Parent)
            {
                case BinaryExpressionSyntax{Left: var leftExpr, Parent: var parent} binaryExpr
                    when binaryExpr.IsKind(SyntaxKind.CoalesceExpression) && leftExpr == node:
                    return ProcessConditionalAccessExpression(destExpression, parent.Must());
                case BinaryExpressionSyntax{Right: var rightExpr} binaryExpr
                    when binaryExpr.IsKind(SyntaxKind.CoalesceExpression) && rightExpr == node:
                    throw new InvalidOperationException("Unsupported null conditional operator in right part of coalesce expression");
                case var parent:
                    return ProcessConditionalAccessExpression(destExpression, parent.Must());
            }
        }

        public override SyntaxNode VisitBinaryExpression(BinaryExpressionSyntax node)
        {
            if (!node.IsKind(SyntaxKind.CoalesceExpression))
                return node;
            Boolean hasLeftConditionalAccessExpression = (node.Left is ConditionalAccessExpressionSyntax) ||
                                                         (node.Left.DescendantNodes().OfType<ConditionalAccessExpressionSyntax>().Any());
            Boolean hasRightConditionalAccessExpression = (node.Right is ConditionalAccessExpressionSyntax) ||
                                                          (node.Right.DescendantNodes().OfType<ConditionalAccessExpressionSyntax>().Any());
            if (hasRightConditionalAccessExpression)
                throw new InvalidOperationException("Unsupported null conditional operator in right part of coalesce expression");
            if (!hasLeftConditionalAccessExpression)
                return node;
            return ProcessCoalesceExpression(node);
        }

        public override SyntaxNode VisitParenthesizedExpression(ParenthesizedExpressionSyntax node)
        {
            ParenthesizedExpressionSyntax destExpression = base.VisitParenthesizedExpression(node).MustCast<SyntaxNode, ParenthesizedExpressionSyntax>();
            switch (destExpression.Expression)
            {
                case IdentifierNameSyntax identifier:
                    _lastTransformations = false;
                    return identifier.WithLeadingTrivia(destExpression.GetLeadingTrivia()).WithTrailingTrivia(destExpression.GetTrailingTrivia());
                default:
                    return destExpression;
            }
        }

        private SyntaxNode ProcessCoalesceExpression(BinaryExpressionSyntax node)
        {
            BinaryExpressionSyntax destExpression = base.VisitBinaryExpression(node).MustCast<SyntaxNode, BinaryExpressionSyntax>();
            if (!node.CanProcess())
                return destExpression;
            switch (node.Parent)
            {
                case null:
                    throw new InvalidOperationException("Bad syntax tree: BinaryExpressionSyntax node without parent");
                case AssignmentExpressionSyntax assignmentExpression:
                {
                    _includeTransformedSource = false;
                    StatementSyntax statement = assignmentExpression.Parent.MustCast<SyntaxNode, StatementSyntax>();
                    ProcessCoalescePart(assignmentExpression.Left, destExpression.Right, statement);
                    return destExpression;
                }
                case EqualsValueClauseSyntax {Parent: VariableDeclaratorSyntax {Parent: VariableDeclarationSyntax {Parent: LocalDeclarationStatementSyntax statement}}}:
                    _includeTransformedSource = false;
                    ProcessCoalescePartForLocalDeclarationStatement(statement, destExpression);
                    return destExpression;
                default:
                    IdentifierNameSyntax identifier = destExpression.Left.MustCast<ExpressionSyntax, IdentifierNameSyntax>();
                    ProcessCoalescePart(identifier, destExpression.Right, node.FindParentStatement().Must());
                    return identifier;
            }
        }

        private void ProcessCoalescePartForLocalDeclarationStatement(LocalDeclarationStatementSyntax statement, BinaryExpressionSyntax expression)
        {
            VariableDeclarationSyntax variableDeclaration = statement.Declaration;
            if (variableDeclaration.Variables.Count > 1)
                throw new NotSupportedException("More than one variable declarations is not supported now");
            VariableDeclaratorSyntax variableDeclarator = variableDeclaration.Variables.First();
            IdentifierNameSyntax identifier = SyntaxFactory.IdentifierName(variableDeclarator.Identifier);
            ProcessCoalescePart(identifier, expression.Right, statement);
        }

        private void ProcessCoalescePart(ExpressionSyntax namePart, ExpressionSyntax rightPart, StatementSyntax baseStatement)
        {
            SyntaxTrivia outerLeadingTrivia = TriviaHelper.GetLeadingSpaceTrivia(baseStatement);
            SyntaxTrivia eolTrivia = TriviaHelper.GetTrailingEndOfLineTrivia(baseStatement);
            Int32 leadingSpaceDelta = TriviaHelper.CalcLeadingSpaceDelta(baseStatement);
            SyntaxTrivia innerLeadingTrivia = TriviaHelper.ShiftRightLeadingSpaceTrivia(outerLeadingTrivia, leadingSpaceDelta);
            String coalesceStatement = $"{outerLeadingTrivia}if ({namePart} == null){eolTrivia}" +
                                       $"{outerLeadingTrivia}{{{eolTrivia}" +
                                       $"{innerLeadingTrivia}{namePart} = {rightPart};{eolTrivia}" +
                                       $"{outerLeadingTrivia}}}{eolTrivia}";
            _destStatements.Add(coalesceStatement);
        }

        private SyntaxNode ProcessConditionalAccessExpression(ConditionalAccessExpressionSyntax expr, SyntaxNode parent)
        {
            StatementSyntax parentStatement = parent switch
            {
                StatementSyntax statement => statement,
                _ => parent.FindParentStatement().Must()
            };
            switch (parent)
            {
                case ConditionalAccessExpressionSyntax:
                    return expr;
                case AssignmentExpressionSyntax assignmentExpression:
                    _includeTransformedSource = false;
                    ProcessAssignmentStatement(assignmentExpression, expr);
                    return expr;
                case EqualsValueClauseSyntax{Parent: VariableDeclaratorSyntax{Parent: VariableDeclarationSyntax{Parent: LocalDeclarationStatementSyntax statement}}}:
                    _includeTransformedSource = false;
                    ProcessLocalDeclarationStatement(statement, expr);
                    return expr;
                case ExpressionStatementSyntax statement:
                    _includeTransformedSource = false;
                    ProcessSimpleStatement(statement, expr);
                    return expr;
                case ArgumentSyntax argument:
                    return ProcessArgument(argument, expr, parentStatement);
                case ReturnStatementSyntax:
                    return ProcessWithLocalVariableCreation("returnExpression", expr, parentStatement);
                default:
                    return ProcessWithLocalVariableCreation("expression", expr, parentStatement);
            }
        }

        private void ProcessLocalDeclarationStatement(LocalDeclarationStatementSyntax statement, ConditionalAccessExpressionSyntax conditionalExpr)
        {
            VariableDeclarationSyntax variableDeclaration = statement.Declaration;
            if (variableDeclaration.Variables.Count > 1)
                throw new NotSupportedException("More than one variable declarations is not supported now");
            VariableDeclaratorSyntax variableDeclarator = variableDeclaration.Variables.First();
            TypeSyntax variableType = variableDeclaration.Type;
            SyntaxToken identifier = variableDeclarator.Identifier;
            SyntaxTrivia leadingSpaceTrivia = TriviaHelper.GetLeadingSpaceTrivia(statement);
            SyntaxTrivia eolTrivia = TriviaHelper.GetTrailingEndOfLineTrivia(statement);
            SyntaxTriviaList leadingTrivia = TriviaHelper.ConstructLeadingTrivia(statement, leadingSpaceTrivia, eolTrivia);
            SyntaxTriviaList trailingTrivia = TriviaHelper.ConstructTrailingTrivia(statement, eolTrivia);
            String defaultValue = TypeDefaultValueHelper.CreateDefaultValueForType(_model, variableType);
            String destLocalDeclaration = $"{leadingTrivia}{variableType} {identifier} = {defaultValue};{trailingTrivia}";
            _destStatements.Add(destLocalDeclaration);
            IList<ExpressionSyntax> conditionalExprParts = conditionalExpr.SplitParts();
            AssignmentValuePartsProcessor partsProcessor = new AssignmentValuePartsProcessor(_model, _variableManager, statement, leadingSpaceTrivia, eolTrivia, identifier);
            _destStatements.AddRange(partsProcessor.ProcessParts(conditionalExprParts));
        }

        private void ProcessAssignmentStatement(AssignmentExpressionSyntax assignmentExpr, ConditionalAccessExpressionSyntax conditionalExpr)
        {
            StatementSyntax statement = assignmentExpr.Parent.MustCast<SyntaxNode, StatementSyntax>();
            SyntaxTrivia leadingSpaceTrivia = TriviaHelper.GetLeadingSpaceTrivia(statement);
            SyntaxTrivia eolTrivia = TriviaHelper.GetTrailingEndOfLineTrivia(statement);
            String leftPartAssignment = $"{assignmentExpr.Left}";
            IList<ExpressionSyntax> conditionalExprParts = conditionalExpr.SplitParts();
            AssignmentValuePartsProcessor partsProcessor = new AssignmentValuePartsProcessor(_model, _variableManager, statement, leadingSpaceTrivia, eolTrivia, leftPartAssignment);
            _destStatements.AddRange(partsProcessor.ProcessParts(conditionalExprParts));
        }

        private void ProcessSimpleStatement(ExpressionStatementSyntax statement, ConditionalAccessExpressionSyntax conditionalExpr)
        {
            SyntaxTrivia leadingSpaceTrivia = TriviaHelper.GetLeadingSpaceTrivia(statement);
            SyntaxTrivia eolTrivia = TriviaHelper.GetTrailingEndOfLineTrivia(statement);
            IList<ExpressionSyntax> conditionalExprParts = conditionalExpr.SplitParts();
            SimpleStatementPartsProcessor partsProcessor = new SimpleStatementPartsProcessor(_model, _variableManager, statement, leadingSpaceTrivia, eolTrivia);
            _destStatements.AddRange(partsProcessor.ProcessParts(conditionalExprParts));
        }

        private SyntaxNode ProcessArgument(ArgumentSyntax argument, ConditionalAccessExpressionSyntax conditionalExpr, StatementSyntax parentStatement)
        {
            IOperation? operationInfo = _model.GetOperation(argument);
            switch (operationInfo)
            {
                case null:
                    throw new InvalidOperationException("Bad object initializer expression: absence type info");
                case IArgumentOperation {Parameter: null}:
                    throw new InvalidOperationException("Bad object initializer expression: absence parameter info");
                case IArgumentOperation {Parameter: var parameterInfo}:
                    return ProcessWithLocalVariableCreation(parameterInfo.Name, conditionalExpr, parentStatement);
                default:
                    throw new InvalidOperationException("Bad object initializer expression: unknown operation info");
            }
        }

        private SyntaxNode ProcessWithLocalVariableCreation(String variableNamePrefix, ConditionalAccessExpressionSyntax conditionalExpr, StatementSyntax parentStatement)
        {
            String variableName = _variableManager.GenerateVariableName(parentStatement, variableNamePrefix);
            String variableType = _model.ResolveExpressionType(conditionalExpr);
            SyntaxTrivia leadingSpaceTrivia = TriviaHelper.GetLeadingSpaceTrivia(parentStatement);
            SyntaxTrivia eolTrivia = TriviaHelper.GetTrailingEndOfLineTrivia(parentStatement);
            String defaultValue = TypeDefaultValueHelper.CreateDefaultValueForExpression(_model, conditionalExpr);
            String destLocalDeclaration = $"{leadingSpaceTrivia}{variableType} {variableName} = {defaultValue};{eolTrivia}";
            _destStatements.Add(destLocalDeclaration);
            IList<ExpressionSyntax> conditionalExprParts = conditionalExpr.SplitParts();
            AssignmentValuePartsProcessor partsProcessor = new AssignmentValuePartsProcessor(_model, _variableManager, parentStatement, leadingSpaceTrivia, eolTrivia, variableName);
            _destStatements.AddRange(partsProcessor.ProcessParts(conditionalExprParts));
            return SyntaxFactory.IdentifierName(variableName);
        }

        private readonly SemanticModel _model;
        private readonly VariableManager _variableManager;
        private readonly IList<String> _destStatements = new List<String>();
        private Boolean _includeTransformedSource = true;
        private Boolean _lastTransformations = true;
    }
}