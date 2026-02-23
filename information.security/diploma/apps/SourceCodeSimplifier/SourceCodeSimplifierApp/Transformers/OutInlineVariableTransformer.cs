using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;
using SourceCodeSimplifierApp.Config;
using SourceCodeSimplifierApp.Output;
using SourceCodeSimplifierApp.Utils;
using Microsoft.CodeAnalysis.Formatting;

namespace SourceCodeSimplifierApp.Transformers
{
    internal class OutInlineVariableTransformer : ITransformer
    {
        public const String Name = "SourceCodeSimplifierApp.Transformers.OutInlineVariableTransformer";

        public OutInlineVariableTransformer(IOutput output, TransformerState transformerState)
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
            DocumentEditor documentEditor = DocumentEditor.CreateAsync(source).Result;
            SyntaxNode? sourceRoot = source.GetSyntaxRootAsync().Result;
            if (sourceRoot == null)
                throw new InvalidOperationException("Bad source document (without syntax tree)");
            ArgumentListSyntax[] argumentLists = sourceRoot.DescendantNodes().OfType<ArgumentListSyntax>().ToArray();
            IDictionary<StatementSyntax, List<DeclarationExpressionSyntax>> statementStorage = new Dictionary<StatementSyntax, List<DeclarationExpressionSyntax>>();
            foreach (ArgumentListSyntax argumentList in argumentLists)
                CollectArgumentList(argumentList, statementStorage);
            foreach (KeyValuePair<StatementSyntax, List<DeclarationExpressionSyntax>> statementData in statementStorage)
                ProcessStatement(documentEditor, statementData.Key, statementData.Value);
            Document destDocument = documentEditor.GetChangedDocument();
            return destDocument;
        }

        private void CollectArgumentList(ArgumentListSyntax argumentList, IDictionary<StatementSyntax, List<DeclarationExpressionSyntax>> statementStorage)
        {
            IList<DeclarationExpressionSyntax> outDeclarationExpressions = new List<DeclarationExpressionSyntax>();
            foreach (ArgumentSyntax argument in argumentList.Arguments)
            {
                if (!argument.RefKindKeyword.IsKind(SyntaxKind.OutKeyword))
                    continue;
                switch (argument.Expression)
                {
                    case DeclarationExpressionSyntax declarationExpression:
                        outDeclarationExpressions.Add(declarationExpression);
                        break;
                }
            }
            if (outDeclarationExpressions.IsEmpty())
                return;
            StatementSyntax? parentStatement = argumentList.FindParentStatement();
            if (parentStatement == null)
                throw new InvalidOperationException("Unsupported case of out inline variable declaration");
            if (!statementStorage.ContainsKey(parentStatement))
                statementStorage.Add(parentStatement, new List<DeclarationExpressionSyntax>());
            statementStorage[parentStatement].AddRange(outDeclarationExpressions);
        }

        private void ProcessStatement(DocumentEditor documentEditor, StatementSyntax statement, IList<DeclarationExpressionSyntax> declarations)
        {
            SyntaxTrivia leadingSpaceTrivia = TriviaHelper.GetLeadingSpaceTrivia(statement);
            SyntaxTrivia eolTrivia = TriviaHelper.GetTrailingEndOfLineTrivia(statement);
            StatementSyntax newStatement = ProcessRewriterForStatement(statement)
                .WithLeadingTrivia(statement.GetLeadingTrivia())
                .WithTrailingTrivia(statement.GetTrailingTrivia());
            IList<StatementSyntax> newStatements = new List<StatementSyntax>();
            foreach (DeclarationExpressionSyntax declaration in declarations)
            {
                StatementSyntax declarationStatement = SyntaxFactory.ParseStatement($"{declaration};")
                    .WithAdditionalAnnotations(Formatter.Annotation)
                    .WithLeadingTrivia(leadingSpaceTrivia)
                    .WithTrailingTrivia(eolTrivia);
                newStatements.Add(declarationStatement);
            }
            newStatements.Add(newStatement);
            documentEditor.ReplaceStatement(statement, newStatements);
        }

        private StatementSyntax ProcessRewriterForStatement(StatementSyntax statement)
        {
            OutInlineVariableSyntaxRewriter rewriter = new OutInlineVariableSyntaxRewriter();
            switch (rewriter.Visit(statement))
            {
                case null:
                    throw new InvalidOperationException("Bad processing of statement");
                case StatementSyntax newStatement:
                    return newStatement;
                default:
                    throw new InvalidOperationException("Unexpected result of processing of statement");
            }
        }

        private readonly IOutput _output;
        private readonly TransformerState _transformerState;

        private class OutInlineVariableSyntaxRewriter : CSharpSyntaxRewriter
        {
            public override SyntaxNode? VisitArgument(ArgumentSyntax node)
            {
                if (!node.RefKindKeyword.IsKind(SyntaxKind.OutKeyword))
                    return base.VisitArgument(node);
                switch (node.Expression)
                {
                    case DeclarationExpressionSyntax{Designation: SingleVariableDesignationSyntax singleVariable}:
                        SyntaxToken outKeyword = SyntaxFactory.Token(SyntaxKind.OutKeyword);
                        IdentifierNameSyntax identifier = SyntaxFactory.IdentifierName(singleVariable.Identifier);
                        return SyntaxFactory.Argument(null, outKeyword, identifier)
                            .NormalizeWhitespace()
                            .WithLeadingTrivia(node.GetLeadingTrivia())
                            .WithTrailingTrivia(node.GetTrailingTrivia());
                    case DeclarationExpressionSyntax:
                        throw new NotSupportedException($"Unsupported variable declaration in argument: {node}");
                    default:
                        return base.VisitArgument(node);
                }
            }
        }
    }
}
