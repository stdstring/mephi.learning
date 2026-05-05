using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SimplePythonPorter.Common;
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
            return new TypeResolveData("<XXX>", "<YYY>", new ImportData());
        }

        public MemberResolveData ResolveCtor(TypeSyntax type, IReadOnlyList<ArgumentSyntax> argumentsData, ConvertedArguments argumentsRepresentation)
        {
            return new MemberResolveData("<ZZZ>", new ImportData());
        }

        public MemberResolveData ResolveMember(MemberData data, MemberRepresentation representation)
        {
            return new MemberResolveData("<ZZZ-666>", new ImportData());
        }

        private readonly SemanticModel _model;
        private readonly AppData _appData;
        private readonly ExpressionConverterSettings _settings;
    }
}
