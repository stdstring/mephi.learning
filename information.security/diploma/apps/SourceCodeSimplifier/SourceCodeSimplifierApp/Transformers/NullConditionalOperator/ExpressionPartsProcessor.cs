using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SourceCodeSimplifierApp.Utils;
using SourceCodeSimplifierApp.Variables;

namespace SourceCodeSimplifierApp.Transformers.NullConditionalOperator
{
    internal abstract class ExpressionPartsProcessor
    {
        public ExpressionPartsProcessor(SemanticModel model,
            VariableManager variableManager,
            StatementSyntax parentStatement,
            SyntaxTrivia leadingSpaceTrivia,
            SyntaxTrivia eolTrivia)
        {
            ParentStatement = parentStatement;
            _model = model;
            _variableManager = variableManager;
            _leadingSpaceTrivia = leadingSpaceTrivia;
            _leadingSpaceDelta = TriviaHelper.CalcLeadingSpaceDelta(parentStatement);
            _eolTrivia = eolTrivia;
        }

        public String[] ProcessParts(IList<ExpressionSyntax> conditionalExprParts)
        {
            return ProcessParts(conditionalExprParts, 0, "", _leadingSpaceTrivia);
        }

        protected abstract String ProcessLastPart(ExpressionSyntax lastPart, String conditionalExprPartPrefix, SyntaxTrivia leadingSpaceTrivia, SyntaxTrivia eolTrivia);

        private String[] ProcessParts(IList<ExpressionSyntax> conditionalExprParts, Int32 partIndex, String prevConditionalExprVariable, SyntaxTrivia leadingSpaceTrivia)
        {
            ExpressionSyntax conditionalExprPart = conditionalExprParts[partIndex];
            String conditionalExprPartPrefix = partIndex == 0 ? "" : $"{prevConditionalExprVariable}";
            Int32 lastPartIndex = conditionalExprParts.Count - 1;
            if (partIndex == lastPartIndex)
            {
                String lastPartStatement = ProcessLastPart(conditionalExprPart, conditionalExprPartPrefix, leadingSpaceTrivia, _eolTrivia);
                return new String[] {lastPartStatement};
            }
            String conditionalExprVariable = _variableManager.GenerateVariableName(ParentStatement, "condExpression");
            String conditionalExprType = _model.ResolveExpressionType(conditionalExprPart);
            String conditionalVariableDeclaration = $"{leadingSpaceTrivia}{conditionalExprType} {conditionalExprVariable} = " +
                                                    $"{conditionalExprPartPrefix}{conditionalExprPart};{_eolTrivia}";
            String ifExpression = $"{conditionalExprVariable} != null";
            SyntaxTrivia nextLeadingSpaceTrivia = TriviaHelper.ShiftRightLeadingSpaceTrivia(leadingSpaceTrivia, _leadingSpaceDelta);
            String[] bodyParts = ProcessParts(conditionalExprParts, partIndex + 1, conditionalExprVariable, nextLeadingSpaceTrivia);
            String body = String.Join("", bodyParts);
            String ifStatement = $"{leadingSpaceTrivia}if ({ifExpression}){_eolTrivia}{leadingSpaceTrivia}{{{_eolTrivia}{body}{leadingSpaceTrivia}}}{_eolTrivia}";
            return new String[] {conditionalVariableDeclaration, ifStatement};
        }

        protected readonly StatementSyntax ParentStatement;
        private readonly SemanticModel _model;
        private readonly VariableManager _variableManager;
        private readonly SyntaxTrivia _leadingSpaceTrivia;
        private readonly Int32 _leadingSpaceDelta;
        private readonly SyntaxTrivia _eolTrivia;
    }

    internal class AssignmentValuePartsProcessor : ExpressionPartsProcessor
    {
        public AssignmentValuePartsProcessor(SemanticModel model,
                                             VariableManager variableManager,
                                             StatementSyntax parentStatement,
                                             SyntaxTrivia leadingSpaceTrivia,
                                             SyntaxTrivia eolTrivia,
                                             String leftPartAssignment)
            : base(model, variableManager, parentStatement, leadingSpaceTrivia, eolTrivia)
        {
            _leftPartAssignment = leftPartAssignment;
        }

        public AssignmentValuePartsProcessor(SemanticModel model,
                                             VariableManager variableManager,
                                             StatementSyntax parentStatement,
                                             SyntaxTrivia leadingSpaceTrivia,
                                             SyntaxTrivia eolTrivia,
                                             SyntaxToken targetVariable)
            : this(model, variableManager, parentStatement, leadingSpaceTrivia, eolTrivia, targetVariable.Text)
        {
        }

        protected override String ProcessLastPart(ExpressionSyntax lastPart, String conditionalExprPartPrefix, SyntaxTrivia leadingSpaceTrivia, SyntaxTrivia eolTrivia)
        {
            SyntaxTriviaList leadingTrivia = TriviaHelper.ConstructLeadingTrivia(ParentStatement, leadingSpaceTrivia, eolTrivia);
            SyntaxTriviaList trailingTrivia = TriviaHelper.ConstructTrailingTrivia(ParentStatement, eolTrivia);
            return $"{leadingTrivia}{_leftPartAssignment} = {conditionalExprPartPrefix}{lastPart};{trailingTrivia}";
        }

        private readonly String _leftPartAssignment;
    }

    internal class SimpleStatementPartsProcessor : ExpressionPartsProcessor
    {
        public SimpleStatementPartsProcessor(SemanticModel model,
                                             VariableManager variableManager,
                                             StatementSyntax parentStatement,
                                             SyntaxTrivia leadingSpaceTrivia,
                                             SyntaxTrivia eolTrivia)
            : base(model, variableManager, parentStatement, leadingSpaceTrivia, eolTrivia)
        {
        }

        protected override string ProcessLastPart(ExpressionSyntax lastPart, String conditionalExprPartPrefix, SyntaxTrivia leadingSpaceTrivia, SyntaxTrivia eolTrivia)
        {
            SyntaxTriviaList leadingTrivia = TriviaHelper.ConstructLeadingTrivia(ParentStatement, leadingSpaceTrivia, eolTrivia);
            SyntaxTriviaList trailingTrivia = TriviaHelper.ConstructTrailingTrivia(ParentStatement, eolTrivia);
            return $"{leadingTrivia}{conditionalExprPartPrefix}{lastPart};{trailingTrivia}";
        }
    }
}