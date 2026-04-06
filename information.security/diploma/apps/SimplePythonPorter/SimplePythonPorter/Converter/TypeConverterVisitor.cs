using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SimplePythonPorter.Common;
using SimplePythonPorter.DestStorage;

namespace SimplePythonPorter.Converter
{
    internal class TypeConverterVisitor : CSharpSyntaxWalker
    {
        public TypeConverterVisitor(SemanticModel model, FileStorage currentFile, AppData appData)
        {
            _model = model;
            _currentFile = currentFile;
            _appData = appData;
        }

        public override void VisitClassDeclaration(ClassDeclarationSyntax node)
        {
            INamedTypeSymbol? currentType = _model.GetDeclaredSymbol(node);
            CheckTypeDeclaration(node, currentType);
            String destClassName = _appData.NameTransformer.TransformTypeName(node.Identifier.Text);
            ClassStorage classStorage = _currentFile.CreateClassStorage(destClassName);
            Boolean isAbstract = node.Modifiers.Any(m => m.IsKind(SyntaxKind.AbstractKeyword));
            if (isAbstract)
            {
                classStorage.AddBaseClass("ABC");
                classStorage.ImportStorage.AddEntity("abc", "ABC");
            }
            IReadOnlyList<BaseTypeSyntax> baseTypes = node.BaseList == null ? Array.Empty<BaseTypeSyntax>() : node.BaseList.Types;
            foreach (BaseTypeSyntax baseType in baseTypes)
            {
                classStorage.AddBaseClass(baseType.Type.ToString());
            }
            base.VisitClassDeclaration(node);
        }

        public override void VisitInterfaceDeclaration(InterfaceDeclarationSyntax node)
        {
            INamedTypeSymbol? currentType = _model.GetDeclaredSymbol(node);
            CheckTypeDeclaration(node, currentType);
            String destClassName = _appData.NameTransformer.TransformTypeName(node.Identifier.Text);
            ClassStorage classStorage = _currentFile.CreateClassStorage(destClassName);
            classStorage.AddBaseClass("ABC");
            classStorage.ImportStorage.AddEntity("abc", "ABC");
            IReadOnlyList<BaseTypeSyntax> baseTypes = node.BaseList == null ? Array.Empty<BaseTypeSyntax>() : node.BaseList.Types;
            foreach (BaseTypeSyntax baseType in baseTypes)
            {
                classStorage.AddBaseClass(baseType.Type.ToString());
            }
            base.VisitInterfaceDeclaration(node);
        }

        private void CheckTypeDeclaration(TypeDeclarationSyntax node, INamedTypeSymbol? currentType)
        {
            if (currentType == null)
                throw new InvalidOperationException("Absence of semantic info");
            SyntaxNode? parentDecl = node.Parent;
            if (parentDecl == null)
                throw new InvalidOperationException("Absence of parent");
            if (!parentDecl.IsKind(SyntaxKind.NamespaceDeclaration))
                throw new UnsupportedSyntaxException("Nested types are not supported");
        }

        private readonly SemanticModel _model;
        private readonly FileStorage _currentFile;
        private readonly AppData _appData;
    }
}
