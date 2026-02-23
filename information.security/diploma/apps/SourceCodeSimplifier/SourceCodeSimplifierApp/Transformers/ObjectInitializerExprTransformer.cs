using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;
using SourceCodeSimplifierApp.Config;
using SourceCodeSimplifierApp.Output;
using SourceCodeSimplifierApp.Transformers.ObjectInitializerExpr;
using SourceCodeSimplifierApp.Utils;
using SourceCodeSimplifierApp.Variables;

namespace SourceCodeSimplifierApp.Transformers
{
    internal class ObjectInitializerExprTransformer : ITransformer
    {
        public const String Name = "SourceCodeSimplifierApp.Transformers.ObjectInitializerExprTransformer";

        public ObjectInitializerExprTransformer(IOutput output, TransformerState transformerState)
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
            String filename = source.FilePath ?? source.Name;
            DocumentEditor documentEditor = DocumentEditor.CreateAsync(source).Result;
            SyntaxNode? sourceRoot = source.GetSyntaxRootAsync().Result;
            if (sourceRoot == null)
                throw new InvalidOperationException("Bad source document (without syntax tree)");
            ObjectCreationExpressionSyntax[] objectCreationExpressions = sourceRoot.DescendantNodes()
                .OfType<ObjectCreationExpressionSyntax>()
                .Where(expr => expr.Initializer != null)
                .ToArray();
            if (objectCreationExpressions.IsEmpty())
                return source;
            StatementSyntax[] parentStatements = objectCreationExpressions
                .Select(expression => expression.GetParentStatement())
                .Distinct()
                .ToArray();
            VariableManager variableManager = new VariableManager();
            SemanticModel model = documentEditor.SemanticModel;
            foreach (StatementSyntax parentStatement in parentStatements)
            {
                IList<StatementSyntax> beforeStatements = new List<StatementSyntax>();
                IList<StatementSyntax> afterStatements = new List<StatementSyntax>();
                ObjectInitializerExprSyntaxRewriter rewriter = new ObjectInitializerExprSyntaxRewriter(model, variableManager, beforeStatements, afterStatements, _output, filename);
                StatementSyntax result = rewriter.Visit(parentStatement).MustCast<SyntaxNode, StatementSyntax>();
                IList<StatementSyntax> newStatements = new List<StatementSyntax>();
                newStatements.AddRange(beforeStatements);
                newStatements.Add(result);
                newStatements.AddRange(afterStatements);
                documentEditor.ReplaceStatement(parentStatement, newStatements);
            }
            Document destDocument = documentEditor.GetChangedDocument();
            return destDocument;
        }

        private readonly IOutput _output;
        private readonly TransformerState _transformerState;
    }
}
