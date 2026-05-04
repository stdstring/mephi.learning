using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SimplePythonPorter.Common;
using SimplePythonPorter.Converter;
using SimplePythonPorter.DestStorage;
using SimplePythonPorter.ExternalEntities;

namespace SimplePythonPorter.Expressions
{
    internal class IdentifierExpressionConverter
    {
        public IdentifierExpressionConverter(SemanticModel model, AppData appData)
        {
            _model = model;
            _appData = appData;
            _externalEntityResolver = new ExternalEntityResolver(model, appData);
        }

        public ConvertResult Convert(IdentifierNameSyntax identifier)
        {
            ImportData importData = new ImportData();
            SymbolInfo symbolInfo = _model.GetSymbolInfo(identifier);
            switch (symbolInfo.Symbol)
            {
                case null:
                    throw new UnsupportedSyntaxException($"Unrecognizable identifier: {identifier.Identifier}");
                case ILocalSymbol localSymbol:
                    return new ConvertResult(_appData.NameTransformer.TransformLocalVariableName(localSymbol.Name), importData);
                case IParameterSymbol parameterSymbol:
                    return new ConvertResult(_appData.NameTransformer.TransformLocalVariableName(parameterSymbol.Name), importData);
                case INamedTypeSymbol typeSymbol:
                    TypeResolveData resolveData = _externalEntityResolver.ResolveType(typeSymbol);
                    return ProcessTypeResolveData(resolveData, importData);
                case IFieldSymbol {IsStatic: false} fieldSymbol:
                    ITypeSymbol containedType = fieldSymbol.Type;
                    String containedTypeName = containedType.ToString()!;
                    MemberModifier modifier = fieldSymbol.DeclaredAccessibility.ToMemberModifier();
                    String fieldName = _appData.NameTransformer.TransformFieldName(containedTypeName, fieldSymbol.Name, modifier);
                    return new ConvertResult($"self.{fieldName}", importData);
                default:
                    throw new UnsupportedSyntaxException($"Unsupported identifier with name = {identifier.Identifier} and kind = {identifier.Kind()}");
            }
        }

        private ConvertResult ProcessTypeResolveData(TypeResolveData resolveData, ImportData importData)
        {
            importData.Append(resolveData.ImportData);
            return new ConvertResult($"{resolveData.ModuleName}.{resolveData.TypeName}", importData);
        }

        private readonly SemanticModel _model;
        private readonly AppData _appData;
        private readonly ExternalEntityResolver _externalEntityResolver;
    }
}
