using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SourceCodeSimplifierApp.Config;
using SourceCodeSimplifierApp.Output;

namespace SourceCodeSimplifierApp.Transformers
{
    internal class NameOfExprTransformer : ITransformer
    {
        public const String Name = "SourceCodeSimplifierApp.Transformers.NameOfExprTransformer";

        public NameOfExprTransformer(IOutput output, TransformerState transformerState)
        {
            _output = output;
            _transformerState = transformerState;
        }

        public Document Transform(Document source)
        {
            if (_transformerState == TransformerState.Off)
                return source;
            _output.WriteInfoLine($"Execution of {Name} started");
            Document dest = source;
            SyntaxTree? syntaxTree = dest.GetSyntaxTreeAsync().Result;
            if (syntaxTree == null)
                throw new InvalidOperationException("Bad syntax tree");
            SemanticModel? semanticModel = dest.GetSemanticModelAsync().Result;
            if (semanticModel == null)
                throw new InvalidOperationException("Bad semantic model");
            NameOfExprSyntaxRewriter rewriter = new NameOfExprSyntaxRewriter(semanticModel);
            SyntaxNode destRoot = rewriter.Visit(syntaxTree.GetRoot());
            dest = dest.WithSyntaxRoot(destRoot);
            _output.WriteInfoLine($"Execution of {Name} finished");
            return dest;
        }

        private readonly IOutput _output;
        private readonly TransformerState _transformerState;

        private class NameOfExprSyntaxRewriter : CSharpSyntaxRewriter
        {
            public NameOfExprSyntaxRewriter(SemanticModel model)
            {
                _model = model;
            }

            public override SyntaxNode VisitInvocationExpression(InvocationExpressionSyntax node)
            {
                switch (node.Expression)
                {
                    case IdentifierNameSyntax {Identifier.Text: "nameof"}:
                        SymbolInfo symbolInfo = ModelExtensions.GetSymbolInfo(_model, node);
                        if (symbolInfo.Symbol is null)
                        {
                            String name = node.ArgumentList.Arguments.First().Expression.ToString();
                            return SyntaxFactory.LiteralExpression(SyntaxKind.StringLiteralExpression, SyntaxFactory.Literal(name))
                                .WithLeadingTrivia(node.GetLeadingTrivia())
                                .WithTrailingTrivia(node.GetTrailingTrivia());
                        }
                        break;
                }
                return node;
            }

            private readonly SemanticModel _model;
        }
    }
}
