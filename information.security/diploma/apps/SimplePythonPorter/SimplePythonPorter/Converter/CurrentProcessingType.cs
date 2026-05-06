using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SimplePythonPorter.Common;

namespace SimplePythonPorter.Converter
{
    internal record CurrentProcessingType(TypeDeclarationSyntax Type, String ContainedNamespace, String ModuleName)
    {
        public static CurrentProcessingType Create(TypeDeclarationSyntax type, SemanticModel model, AppData appData)
        {
            ITypeSymbol typeSymbol = model.GetDeclaredSymbol(type)!;
            String containedNamespace = typeSymbol.ContainingNamespace.ToString()!;
            String moduleName = appData.NameTransformer.TransformNamespaceName(containedNamespace);
            return new CurrentProcessingType(type, containedNamespace, moduleName);
        }
    }
}
