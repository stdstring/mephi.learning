using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;
using SourceCodeSimplifierApp.Config;
using SourceCodeSimplifierApp.Output;
using SourceCodeSimplifierApp.Utils;

namespace SourceCodeSimplifierApp.Transformers
{
    internal class StringInterpolationExprTransformer : ITransformer
    {
        public const String Name = "SourceCodeSimplifierApp.Transformers.StringInterpolationExprTransformer";

        public StringInterpolationExprTransformer(IOutput output, TransformerState transformerState)
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
            StringInterpolationExprSyntaxRewriter rewriter = new StringInterpolationExprSyntaxRewriter();
            SyntaxNode destRoot = rewriter.Visit(syntaxTree.GetRoot());
            dest = dest.WithSyntaxRoot(destRoot);
            _output.WriteInfoLine($"Execution of {Name} finished");
            return dest;
        }

        private readonly IOutput _output;
        private readonly TransformerState _transformerState;

        private class StringInterpolationExprSyntaxRewriter : CSharpSyntaxRewriter
        {
            public override SyntaxNode VisitInterpolatedStringExpression(InterpolatedStringExpressionSyntax node)
            {
                FileLinePositionSpan location = node.SyntaxTree.GetLineSpan(node.Span);
                Boolean isVerbatimString = node.GetFirstToken().IsKind(SyntaxKind.InterpolatedVerbatimStringStartToken);
                StringBuilder dest = new StringBuilder("string.Format(");
                if (isVerbatimString)
                    dest.Append('@');
                dest.Append('"');
                IList<ExpressionSyntax> expressions = new List<ExpressionSyntax>();
                foreach (InterpolatedStringContentSyntax content in node.Contents)
                {
                    switch (content)
                    {
                        case InterpolatedStringTextSyntax text:
                            dest.Append(text.TextToken.Text);
                            break;
                        case InterpolationSyntax interpolation:
                            dest.Append($"{{{expressions.Count}");
                            expressions.Add(interpolation.Expression);
                            if (interpolation.AlignmentClause != null)
                                dest.Append(interpolation.AlignmentClause);
                            if (interpolation.FormatClause != null)
                                dest.Append(interpolation.FormatClause);
                            dest.Append('}');
                            break;
                        default:
                            throw new InvalidOperationException($"Unsupported type of interpolation string contents (location is {location})");
                    }
                }
                dest.Append('"');
                expressions.ForEach(expression => dest.Append($", {expression}"));
                dest.Append(')');
                return SyntaxFactory.ParseExpression(dest.ToString())
                    .WithAdditionalAnnotations(Formatter.Annotation)
                    .WithLeadingTrivia(node.GetLeadingTrivia())
                    .WithTrailingTrivia(node.GetTrailingTrivia());
            }
        }
    }
}
