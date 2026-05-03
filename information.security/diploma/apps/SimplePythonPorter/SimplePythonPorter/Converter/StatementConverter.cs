using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using SimplePythonPorter.Common;
using SimplePythonPorter.DestStorage;

namespace SimplePythonPorter.Converter
{
    internal class StatementConverterVisitor : CSharpSyntaxWalker
    {
        private readonly SemanticModel _model;
        private readonly MethodStorage _currentMethod;
        private readonly AppData _appData;
    }
}
