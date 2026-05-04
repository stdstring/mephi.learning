using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using SimplePythonPorter.Common;
using SimplePythonPorter.DestStorage;

namespace SimplePythonPorter.Converter
{
    internal class StatementConverterVisitor : CSharpSyntaxWalker
    {
        public StatementConverterVisitor(SemanticModel model, MethodStorage currentMethod, AppData appData)
        {
            _model = model;
            _currentMethod = currentMethod;
            _appData = appData;
        }

        public override void Visit(SyntaxNode? node)
        {
            if (node == null)
                return;
            throw new UnsupportedSyntaxException($"Unsupported node with kind = {node.Kind()}");
        }

        private readonly SemanticModel _model;
        private readonly MethodStorage _currentMethod;
        private readonly AppData _appData;
    }
}
