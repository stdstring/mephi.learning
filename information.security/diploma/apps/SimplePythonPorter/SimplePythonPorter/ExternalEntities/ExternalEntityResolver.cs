using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SimplePythonPorter.Common;
using SimplePythonPorter.Converter;
using SimplePythonPorter.DestStorage;
using SimplePythonPorter.Expressions;

namespace SimplePythonPorter.ExternalEntities
{
    internal class ExternalEntityResolver
    {
        public ExternalEntityResolver(SemanticModel model, AppData appData, ExpressionConverterSettings settings)
        {
            _model = model;
            _appData = appData;
            _settings = settings;
        }

        public TypeResolveData ResolveType(ITypeSymbol typeSymbol)
        {
            if (typeSymbol.IsBoolean())
                return new TypeResolveData("bool", "", new ImportData());
            if (typeSymbol.IsIntegerNumber())
                return new TypeResolveData("int", "", new ImportData());
            if (typeSymbol.IsFloatNumber())
                return new TypeResolveData("str", "", new ImportData());
            if (typeSymbol.IsString())
                return new TypeResolveData("str", "", new ImportData());
            if (typeSymbol.IsArray())
                return new TypeResolveData("list", "", new ImportData());
            String sourceTypeName = typeSymbol.Name;
            String sourceNamespace = typeSymbol.ContainingNamespace.ToString()!;
            String destTypeName = _appData.NameTransformer.TransformTypeName(sourceTypeName);
            String destModuleName = _appData.NameTransformer.TransformNamespaceName(sourceNamespace);
            ImportData importData = new ImportData();
            importData.AddImport(destModuleName);
            return new TypeResolveData(destTypeName, destModuleName, importData);
        }

        public MemberResolveData ResolveCtor(TypeSyntax type, IReadOnlyList<ArgumentSyntax> argumentsData, ConvertedArguments argumentsRepresentation)
        {
            SymbolInfo symbolInfo = _model.GetSymbolInfo(type);
            switch (symbolInfo.Symbol)
            {
                case null:
                    throw new UnsupportedSyntaxException($"Bad ctor info: {type.Kind()}");
                case INamedTypeSymbol typeSymbol:
                    return ResolveCtor(typeSymbol, argumentsRepresentation);
                default:
                    throw new UnsupportedSyntaxException($"Unexpected ctor info: {type.Kind()}");
            }
        }

        public MemberResolveData ResolveMember(MemberData data, MemberRepresentation representation)
        {
            SimpleNameSyntax memberName = data.Name;
            SymbolInfo memberInfo = _model.GetSymbolInfo(memberName);
            ITypeSymbol typeSymbol = GetContainedType(memberInfo.Symbol);
            String typeFullName = typeSymbol.GetTypeFullName();
            MemberModifier modifier = memberInfo.Symbol!.DeclaredAccessibility.ToMemberModifier();
            if (typeSymbol.IsBoolean())
                throw new UnsupportedSyntaxException("Unsupported member for bool type");
            if (typeSymbol.IsIntegerNumber())
                throw new UnsupportedSyntaxException("Unsupported member for int type");
            if (typeSymbol.IsFloatNumber())
                throw new UnsupportedSyntaxException("Unsupported member for float type");
            if (typeSymbol.IsString())
                return ResolveMemberForString(memberInfo.Symbol, data, representation);
            if (typeSymbol.IsArray())
                throw new UnsupportedSyntaxException("Unsupported member for list type");
            switch (memberInfo.Symbol)
            {
                case IPropertySymbol{IsIndexer: true}:
                    throw new UnsupportedSyntaxException("Unsupported indexer property");
                case IPropertySymbol property:
                    String propertyName = _appData.NameTransformer.TransformPropertyName(typeFullName, property.Name, modifier);
                    String propertyAccess = $"{representation.Target}.{propertyName}";
                    return new MemberResolveData(propertyAccess);
                case IMethodSymbol method:
                    String methodName = _appData.NameTransformer.TransformPropertyName(typeFullName, method.Name, modifier);
                    String methodCall = $"{representation.Target}.{methodName}({String.Join(", ", representation.Arguments.Values)})";
                    return new MemberResolveData(methodCall);
                case IFieldSymbol field:
                    String fieldName = _appData.NameTransformer.TransformFieldName(typeFullName, field.Name, modifier);
                    String fieldAccess = $"{representation.Target}.{fieldName}";
                    return new MemberResolveData(fieldAccess);
                default:
                    throw new UnsupportedSyntaxException($"Unsupported member kind: {memberInfo.Symbol!.Kind}");
            }
        }

        private MemberResolveData ResolveCtor(INamedTypeSymbol typeSymbol, ConvertedArguments argumentsRepresentation)
        {
            if (typeSymbol.IsBoolean())
                throw new UnsupportedSyntaxException("Unsupported ctor for bool type");
            if (typeSymbol.IsIntegerNumber())
                throw new UnsupportedSyntaxException("Unsupported ctor for int type");
            if (typeSymbol.IsFloatNumber())
                throw new UnsupportedSyntaxException("Unsupported ctor for float type");
            if (typeSymbol.IsString())
                throw new UnsupportedSyntaxException("Unsupported ctor for str type");
            if (typeSymbol.IsArray())
                throw new UnsupportedSyntaxException("Unsupported ctor for list type");
            String sourceTypeName = typeSymbol.Name;
            String sourceNamespace = typeSymbol.ContainingNamespace.ToString()!;
            String destTypeName = _appData.NameTransformer.TransformTypeName(sourceTypeName);
            String destModuleName = _appData.NameTransformer.TransformNamespaceName(sourceNamespace);
            if (_settings.CurrentType.ModuleName.Equals(destModuleName))
                return new MemberResolveData($"{destTypeName}({String.Join(", ", argumentsRepresentation.Values)})");
            ImportData importData = new ImportData();
            importData.AddImport(destModuleName);
            String ctorCall = $"{destModuleName}.{destTypeName}({String.Join(", ", argumentsRepresentation.Values)})";
            return new MemberResolveData(ctorCall, importData);
        }

        private ITypeSymbol GetContainedType(ISymbol? symbol)
        {
            return symbol switch
            {
                null => throw new UnsupportedSyntaxException("Unrecognizable symbol"),
                IPropertySymbol property => property.ContainingType,
                IMethodSymbol method => method.ContainingType,
                IFieldSymbol field => field.ContainingType,
                _ => throw new UnsupportedSyntaxException($"Unsupported symbol for member reference: {symbol.Kind}")
            };
        }

        private MemberResolveData ResolveMemberForString(ISymbol symbol, MemberData data, MemberRepresentation representation)
        {
            switch (symbol)
            {
                case IPropertySymbol {Name: "Length"}:
                    return new MemberResolveData($"len({representation.Target})");
                default:
                    throw new UnsupportedSyntaxException($"Unsupported member for string type: {symbol.Name}");
            }
        }

        private readonly SemanticModel _model;
        private readonly AppData _appData;
        private readonly ExpressionConverterSettings _settings;
    }
}
