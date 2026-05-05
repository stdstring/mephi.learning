using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SimplePythonPorter.Common;
using SimplePythonPorter.Converter;
using SimplePythonPorter.DestStorage;
using SimplePythonPorter.ExternalEntities;

namespace SimplePythonPorter.Expressions
{
    internal class MemberAccessExpressionConverter
    {
        public MemberAccessExpressionConverter(SemanticModel model, AppData appData, ExpressionConverterSettings settings)
        {
            _model = model;
            _appData = appData;
            _settings = settings;
            _expressionConverter = new ExpressionConverter(model, appData, settings);
            _externalEntityResolver = new ExternalEntityResolver(model, appData, settings);
        }

        public ConvertResult Convert(MemberAccessExpressionSyntax expression)
        {
            return Convert(expression, null);
        }

        public ConvertResult Convert(MemberAccessExpressionSyntax expression, ArgumentListSyntax? argumentList)
        {
            ImportData importData = new ImportData();
            ArgumentListConverter argumentListConverter = new ArgumentListConverter(_model, _appData, _settings.CreateChild());
            IReadOnlyList<ArgumentSyntax> arguments = argumentList.GetArguments();
            ConvertArgumentsResult convertedArguments = argumentListConverter.Convert(expression, arguments);
            importData.Append(convertedArguments.ImportData);
            ExpressionSyntax target = expression.Expression;
            SymbolInfo targetInfo = _model.GetSymbolInfo(target);
            switch (targetInfo.Symbol)
            {
                // TODO (std_string) : for some types of expression we receive null, but this is not error of recognition. Think about this
                case INamespaceSymbol:
                    return _expressionConverter.Convert(expression.Name);
                case ITypeSymbol type:
                {
                    TypeResolveData resolveData = _externalEntityResolver.ResolveType(type);
                    importData.Append(resolveData.ImportData);
                    String typeName = resolveData.GetTypeFullName();
                    return ConvertImpl(expression, target, arguments, importData: importData, typeName, convertedArguments.Result);
                }
                default:
                {
                    ConvertResult targetDest = _expressionConverter.Convert(target);
                    importData.Append(targetDest.ImportData);
                    return ConvertImpl(expression, target, arguments, importData: importData, targetDest.Result, convertedArguments.Result);
                }
            }
        }

        private ConvertResult ConvertImpl(MemberAccessExpressionSyntax expression,
                                          ExpressionSyntax target,
                                          IReadOnlyList<ArgumentSyntax> arguments,
                                          ImportData importData,
                                          String targetRepresentation,
                                          ConvertedArguments convertedArguments)
        {
            SimpleNameSyntax name = expression.Name;
            MemberData memberData = new MemberData(target, name, arguments);
            MemberRepresentation memberRepresentation = new MemberRepresentation(targetRepresentation, convertedArguments);
            MemberResolveData resolveData = _externalEntityResolver.ResolveMember(memberData, memberRepresentation);
            importData.Append(resolveData.ImportData);
            return new ConvertResult(resolveData.Member, importData);
        }

        private readonly SemanticModel _model;
        private readonly AppData _appData;
        private readonly ExpressionConverterSettings _settings;
        private readonly ExpressionConverter _expressionConverter;
        private readonly ExternalEntityResolver _externalEntityResolver;
    }
}
