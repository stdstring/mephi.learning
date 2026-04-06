using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using SimplePythonPorter.Common;
using SimplePythonPorter.DestStorage;

namespace SimplePythonPorter.Converter
{
    internal class MethodConverterVisitor : CSharpSyntaxWalker
    {
        public MethodConverterVisitor(SemanticModel model, ClassStorage currentClass, AppData appData)
        {
            _model = model;
            _currentClass = currentClass;
            _appData = appData;
        }

        private readonly SemanticModel _model;
        private readonly ClassStorage _currentClass;
        private readonly AppData _appData;
    }
}
