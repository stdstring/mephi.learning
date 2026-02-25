using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;
using SourceCodeSimplifierApp.Config;
using SourceCodeSimplifierApp.Output;
using SourceCodeSimplifierApp.Utils;

namespace SourceCodeSimplifierApp.Transformers
{
    internal class ForLoopTransformer : ITransformer
    {
        public const String Name = "SourceCodeSimplifierApp.Transformers.ForLoopTransformer";

        public ForLoopTransformer(IOutput output, TransformerState transformerState)
        {
            _output = output;
            _transformerState = transformerState;
        }

        public Document Transform(Document source)
        {
            if (_transformerState == TransformerState.Off)
                return source;
            _output.WriteInfoLine($"Execution of {Name} started");
            Document dest = TransformImpl(source);
            _output.WriteInfoLine($"Execution of {Name} finished");
            return dest;
        }

        private Document TransformImpl(Document source)
        {
            Document destDocument = source;
            while (true)
            {
                SyntaxTree? tree = destDocument.GetSyntaxTreeAsync().Result;
                if (tree == null)
                    throw new InvalidOperationException("Bad syntax tree");
                ForLoopCollector forLoopCollector = new ForLoopCollector();
                forLoopCollector.Visit(tree.GetRoot());
                if (forLoopCollector.ReadyLoops.IsEmpty() && forLoopCollector.HasUnreadyLoops)
                    throw new InvalidOperationException("Bad definition of \"for\" loops");
                if (!forLoopCollector.ReadyLoops.IsEmpty())
                    destDocument = TransformImpl(destDocument, forLoopCollector.ReadyLoops.ToArray());
                if (!forLoopCollector.HasUnreadyLoops)
                    break;
            }
            return destDocument;
        }

        private Document TransformImpl(Document source, ForStatementSyntax[] forLoops)
        {
            DocumentEditor documentEditor = DocumentEditor.CreateAsync(source).Result;
            foreach (ForStatementSyntax forLoop in forLoops)
            {
                SyntaxNode? parent = forLoop.Parent;
                if (parent == null)
                    throw new InvalidOperationException("Bad for loop");
                IList<StatementSyntax> replacement = CreateReplacements(forLoop);
                switch (parent)
                {
                    case BlockSyntax:
                        documentEditor.ReplaceStatement(forLoop, replacement);
                        break;
                    case StatementSyntax statement:
                        SyntaxTrivia leadingTrivia = TriviaHelper.GetLeadingSpaceTrivia(statement);
                        SyntaxTrivia eolTrivia = TriviaHelper.GetTrailingEndOfLineTrivia(statement);
                        SyntaxToken openBraceToken = SyntaxFactory.Token(SyntaxKind.OpenBraceToken)
                            .WithLeadingTrivia(leadingTrivia)
                            .WithTrailingTrivia(eolTrivia);
                        SyntaxToken closeBraceToken = SyntaxFactory.Token(SyntaxKind.CloseBraceToken)
                            .WithLeadingTrivia(leadingTrivia)
                            .WithTrailingTrivia(eolTrivia);
                        BlockSyntax containerBlock = SyntaxFactory.Block(openBraceToken, new SyntaxList<StatementSyntax>(replacement), closeBraceToken);
                        documentEditor.ReplaceNode(forLoop, containerBlock);
                        break;
                    default:
                        throw new InvalidOperationException("Bad for loop");
                }
            }
            Document destDocument = documentEditor.GetChangedDocument();
            return destDocument;
        }

        private IList<StatementSyntax> CreateReplacements(ForStatementSyntax forLoop)
        {
            VariableDeclarationSyntax? declarationSection = forLoop.Declaration;
            ExpressionSyntax? conditionSection = forLoop.Condition;
            SeparatedSyntaxList<ExpressionSyntax> incrementors = forLoop.Incrementors;
            StatementSyntax body = forLoop.Statement;
            IList<StatementSyntax> newStatements = new List<StatementSyntax>();
            ProcessDeclarationStatement(forLoop, declarationSection, newStatements);
            ProcessLoopHeader(forLoop, conditionSection, newStatements);
            ProcessBodyBlock(forLoop, body, incrementors, newStatements);
            return newStatements;
        }

        private void ProcessDeclarationStatement(ForStatementSyntax forLoop, VariableDeclarationSyntax? declarationSection, IList<StatementSyntax> dest)
        {
            if (declarationSection == null)
                return;
            SyntaxTrivia leadingTrivia = TriviaHelper.GetLeadingSpaceTrivia(forLoop);
            SyntaxTrivia eolTrivia = TriviaHelper.GetTrailingEndOfLineTrivia(forLoop);
            String declarationStatement = $"{leadingTrivia}{declarationSection.ToString()};{eolTrivia}";
            dest.Add(SyntaxFactory.ParseStatement(declarationStatement));
        }

        private void ProcessLoopHeader(ForStatementSyntax forLoop, ExpressionSyntax? conditionSection, IList<StatementSyntax> dest)
        {
            SyntaxTrivia leadingTrivia = TriviaHelper.GetLeadingSpaceTrivia(forLoop);
            SyntaxTrivia eolTrivia = TriviaHelper.GetTrailingEndOfLineTrivia(forLoop);
            String loopHeader = $"{leadingTrivia}while ({(conditionSection == null ? "true" : conditionSection.ToString())}){eolTrivia}";
            dest.Add(SyntaxFactory.ParseStatement(loopHeader));
        }

        private void ProcessBodyBlock(ForStatementSyntax forLoop, StatementSyntax sourceBody, IReadOnlyList<ExpressionSyntax> incrementors, IList<StatementSyntax> dest)
        {
            SyntaxTrivia leadingTrivia = TriviaHelper.GetLeadingSpaceTrivia(forLoop);
            SyntaxTrivia eolTrivia = TriviaHelper.GetTrailingEndOfLineTrivia(forLoop);
            Int32 leadingSpaceDelta = TriviaHelper.CalcLeadingSpaceDelta(forLoop);
            SyntaxTrivia bodyLeadingTrivia = TriviaHelper.ShiftRightLeadingSpaceTrivia(leadingTrivia, leadingSpaceDelta);
            IList<StatementSyntax> incrementStatements = CreateIncrementorStatements(incrementors, bodyLeadingTrivia, eolTrivia);
            switch (sourceBody)
            {
                case BlockSyntax block:
                    dest.Add(block.AddStatements(incrementStatements.ToArray()));
                    return;
                default:
                    SyntaxToken openBraceToken = SyntaxFactory.Token(SyntaxKind.OpenBraceToken)
                        .WithLeadingTrivia(leadingTrivia)
                        .WithTrailingTrivia(eolTrivia);
                    SyntaxToken closeBraceToken = SyntaxFactory.Token(SyntaxKind.CloseBraceToken)
                        .WithLeadingTrivia(leadingTrivia)
                        .WithTrailingTrivia(eolTrivia);
                    SyntaxList<StatementSyntax> bodyStatements = new SyntaxList<StatementSyntax>(new[] { sourceBody }.Concat(incrementStatements));
                    dest.Add(SyntaxFactory.Block(openBraceToken, bodyStatements, closeBraceToken));
                    return;
            }
        }

        private IList<StatementSyntax> CreateIncrementorStatements(IReadOnlyList<ExpressionSyntax> incrementors, SyntaxTrivia leadingTrivia, SyntaxTrivia eolTrivia)
        {
            IList<StatementSyntax> result = new List<StatementSyntax>();
            foreach (ExpressionSyntax incrementor in incrementors)
                result.Add(CreateIncrementorStatement(incrementor, leadingTrivia, eolTrivia));
            return result;
        }

        private StatementSyntax CreateIncrementorStatement(ExpressionSyntax incrementor, SyntaxTrivia leadingTrivia, SyntaxTrivia eolTrivia)
        {
            switch (incrementor)
            {
                case PrefixUnaryExpressionSyntax prefixUnaryExpression when prefixUnaryExpression.IsKind(SyntaxKind.PreIncrementExpression):
                    return SyntaxFactory.ParseStatement($"{leadingTrivia}{prefixUnaryExpression.Operand.ToString()} += 1;{eolTrivia}");
                case PrefixUnaryExpressionSyntax prefixUnaryExpression when prefixUnaryExpression.IsKind(SyntaxKind.PreDecrementExpression):
                    return SyntaxFactory.ParseStatement($"{leadingTrivia}{prefixUnaryExpression.Operand.ToString()} -= 1;{eolTrivia}");
                case PostfixUnaryExpressionSyntax prefixUnaryExpression when prefixUnaryExpression.IsKind(SyntaxKind.PostIncrementExpression):
                    return SyntaxFactory.ParseStatement($"{leadingTrivia}{prefixUnaryExpression.Operand.ToString()} += 1;{eolTrivia}");
                case PostfixUnaryExpressionSyntax prefixUnaryExpression when prefixUnaryExpression.IsKind(SyntaxKind.PostDecrementExpression):
                    return SyntaxFactory.ParseStatement($"{leadingTrivia}{prefixUnaryExpression.Operand.ToString()} -= 1;{eolTrivia}");
                default:
                    return SyntaxFactory.ParseStatement($"{leadingTrivia}{incrementor.ToString()};{eolTrivia}");
            }
        }

        private readonly IOutput _output;
        private readonly TransformerState _transformerState;

        private class ForLoopCollector : CSharpSyntaxWalker
        {
            public override void VisitForStatement(ForStatementSyntax node)
            {
                _currentLoop = node;
                Visit(node.Statement);
                if (_currentLoop == node)
                    ReadyLoops.Add(node);
                else
                    HasUnreadyLoops = true;
            }

            public IList<ForStatementSyntax> ReadyLoops { get; } = new List<ForStatementSyntax>();

            public Boolean HasUnreadyLoops { get; set; }

            private ForStatementSyntax? _currentLoop;
        }
    }
}
