using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;
using SourceCodeSimplifierApp.Config;
using SourceCodeSimplifierApp.Output;
using SourceCodeSimplifierApp.Transformers.NullConditionalOperator;
using SourceCodeSimplifierApp.Utils;
using SourceCodeSimplifierApp.Variables;

namespace SourceCodeSimplifierApp.Transformers
{
    internal class NullConditionalOperatorTransformer : ITransformer
    {
        public const String Name = "SourceCodeSimplifierApp.Transformers.NullConditionalOperatorTransformer";

        public NullConditionalOperatorTransformer(IOutput output, TransformerState transformerState)
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
            VariableManager variableManager = new VariableManager();
            // TODO (std_string) : think about ability work without searching statements for processing on each iteration
            Int32 iteration = 1;
            Document current = source;
            while (true)
            {
                DocumentEditor documentEditor = DocumentEditor.CreateAsync(current).Result;
                SyntaxNode root = current.GetSyntaxRootAsync().Result.Must();
                ConditionalAccessExpressionSyntax[] conditionalAccessExpressions = root
                    .DescendantNodes()
                    .OfType<ConditionalAccessExpressionSyntax>()
                    .ToArray();
                StatementSyntax[] parentStatements = conditionalAccessExpressions
                    .Select(expression => expression.GetParentStatement())
                    .Distinct()
                    .ToArray();
                if (parentStatements.Length == 0)
                    break;
                _output.WriteInfoLine($"Transformation iteration number {iteration} started");
                SemanticModel model = documentEditor.SemanticModel;
                StatementSyntax firstParent = parentStatements.First();
                NullConditionalOperatorRewriter rewriter = new NullConditionalOperatorRewriter(model, variableManager);
                NullConditionalOperatorRewriterResult result = rewriter.Process(firstParent);
                documentEditor.ReplaceStatement(firstParent, result.Statements);
                current = documentEditor.GetChangedDocument();
                _output.WriteInfoLine($"Transformation iteration number {iteration} finished");
                if (result.Last)
                    iteration = 1;
                else
                    ++iteration;
            }
            return current;
        }

        private readonly IOutput _output;
        private readonly TransformerState _transformerState;
    }
}
