using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SimplePythonPorter.Common;
using SimplePythonPorter.DestStorage;

namespace SimplePythonPorter.Converter
{
    internal class FileConverterVisitor : CSharpSyntaxWalker
    {
        public FileConverterVisitor(SemanticModel model, FileStorage currentFile, AppData appData)
        {
            _model = model;
            _currentFile = currentFile;
            _appData = appData;
        }

        public override void VisitClassDeclaration(ClassDeclarationSyntax node)
        {
            TypeConverterVisitor typeConverter = new TypeConverterVisitor(_model, _currentFile, _appData);
            typeConverter.Visit(node);
        }

        public override void VisitInterfaceDeclaration(InterfaceDeclarationSyntax node)
        {
            TypeConverterVisitor typeConverter = new TypeConverterVisitor(_model, _currentFile, _appData);
            typeConverter.Visit(node);
        }

        private readonly SemanticModel _model;
        private readonly FileStorage _currentFile;
        private readonly AppData _appData;
    }
}
