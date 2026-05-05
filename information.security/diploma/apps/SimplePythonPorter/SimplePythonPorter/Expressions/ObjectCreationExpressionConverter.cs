using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SimplePythonPorter.Checker;
using SimplePythonPorter.Common;
using SimplePythonPorter.Converter;
using SimplePythonPorter.DestStorage;
using SimplePythonPorter.ExternalEntities;

namespace SimplePythonPorter.Expressions
{
    internal class ObjectCreationExpressionConverter
    {
        public ObjectCreationExpressionConverter(SemanticModel model, AppData appData, ExpressionConverterSettings settings)
        {
            _model = model;
            _appData = appData;
            _settings = settings;
            _externalEntityResolver = new ExternalEntityResolver(model, appData, settings);
        }

        public ConvertResult Convert(ObjectCreationExpressionSyntax expression)
        {
            ImportData importData = new ImportData();
            IReadOnlyList<ArgumentSyntax> arguments = expression.ArgumentList.GetArguments();
            arguments.CheckForMethod().MustSuccess();
            if (expression.Initializer is {Expressions.Count: > 0})
                throw new UnsupportedSyntaxException("Forbidden object initializer");
            TypeSyntax type = expression.Type;
            ArgumentListConverter argumentListConverter = new ArgumentListConverter(_model, _appData, _settings.CreateChild());
            ConvertArgumentsResult convertedArguments = argumentListConverter.Convert(expression, arguments);
            importData.Append(convertedArguments.ImportData);
            MemberResolveData resolveData = _externalEntityResolver.ResolveCtor(type, arguments, convertedArguments.Result);
            importData.Append(resolveData.ImportData);
            return new ConvertResult(resolveData.Member, importData);
        }

        private readonly SemanticModel _model;
        private readonly AppData _appData;
        private readonly ExpressionConverterSettings _settings;
        private readonly ExternalEntityResolver _externalEntityResolver;
    }
}
