using Microsoft.CodeAnalysis;
using SimplePythonPorter.Common;
using SimplePythonPorter.DestStorage;

namespace SimplePythonPorter.ExternalEntities
{
    internal class ExternalEntityResolver
    {
        public ExternalEntityResolver(SemanticModel model, AppData appData)
        {
            _model = model;
            _appData = appData;
        }

        public TypeResolveData ResolveType(ITypeSymbol typeSymbol)
        {
            return new TypeResolveData("<XXX>", "<YYY>", new ImportData());
        }

        private readonly SemanticModel _model;
        private readonly AppData _appData;
    }
}
