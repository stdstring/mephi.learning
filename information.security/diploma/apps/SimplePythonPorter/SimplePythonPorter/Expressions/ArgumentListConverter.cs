using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SimplePythonPorter.Common;
using SimplePythonPorter.Converter;
using SimplePythonPorter.DestStorage;

namespace SimplePythonPorter.Expressions
{
    // TODO (std_string) : think about more smart solution
    internal record ConvertedArguments(String[] Values, String[]? NamedValues)
    {
        public String[] GetArguments(Boolean canUseNamedArguments)
        {
            return canUseNamedArguments && NamedValues != null ? NamedValues : Values;
        }
    }

    internal record ConvertArgumentsResult(ConvertedArguments Result, ImportData ImportData);

    internal class ArgumentListConverter
    {
        public ArgumentListConverter(SemanticModel model, AppData appData, ExpressionConverterSettings settings)
        {
            _model = model;
            _appData = appData;
            _expressionConverter = new ExpressionConverter(model, appData, settings);
        }

        public ConvertArgumentsResult Convert(ExpressionSyntax source, IReadOnlyList<ArgumentSyntax> arguments)
        {
            ImportData importData = new ImportData();
            if (arguments.Count == 0)
                return new ConvertArgumentsResult(new ConvertedArguments(Array.Empty<String>(), null), importData);
            IMethodSymbol methodSymbol = source.GetMethodSymbol(_model);
            IReadOnlyList<IParameterSymbol> parameters = methodSymbol.Parameters;
            Boolean useNamedArguments = (arguments[^1].NameColon is not null) || IsOverloadedMember(methodSymbol);
            Boolean hasParamsArguments = (parameters[^1].IsParams) && (arguments.Count >= parameters.Count);
            Int32 destCount = Math.Min(parameters.Count, arguments.Count);
            String[] destArguments = new String[destCount];
            String[]? destNamedArguments = useNamedArguments ? new String[destCount] : null;
            Int32 usualArgumentCount = arguments.Count < parameters.Count ? arguments.Count : parameters.Count;
            if (hasParamsArguments)
                --usualArgumentCount;
            for (Int32 index = 0; index < usualArgumentCount; ++index)
            {
                (String value, String? namedValue) argument = ProcessUsualArgument(useNamedArguments, arguments[index], parameters[index], importData);
                destArguments[index] = argument.value;
                if (destNamedArguments != null)
                    destNamedArguments[index] = argument.namedValue!;
            }
            if (hasParamsArguments)
            {
                (String value, String? namedValue) argument = ProcessParamsArgument(arguments, useNamedArguments, usualArgumentCount, parameters[^1], importData);
                destArguments[^1] = argument.value;
                if (destNamedArguments != null)
                    destNamedArguments[^1] = argument.namedValue!;
            }
            return new ConvertArgumentsResult(new ConvertedArguments(destArguments, destNamedArguments), importData);
        }

        private Boolean IsOverloadedMember(IMethodSymbol methodSymbol)
        {
            IMethodSymbol[] methods = methodSymbol.ContainingType.GetMembers(methodSymbol.Name).OfType<IMethodSymbol>().ToArray();
            return methods switch
            {
                {Length: 1} => false,
                {Length: 2} when methods.Any(method => method.Parameters.Length == 0) => false,
                _ => true
            };
        }

        private String? GetArgumentName(Boolean useNamedArguments, ArgumentSyntax argument, IParameterSymbol parameter)
        {
            if (!useNamedArguments)
                return null;
            return argument.NameColon switch
            {
                null => _appData.NameTransformer.TransformLocalVariableName(parameter.Name),
                var nameColon => _appData.NameTransformer.TransformLocalVariableName(nameColon.Name.ToString())
            };
        }

        private (String value, String? namedValue) ProcessUsualArgument(Boolean useNamedArguments, ArgumentSyntax argument, IParameterSymbol parameter, ImportData importData)
        {
            String argumentValue = ConvertExpression(argument.Expression, importData);
            return GetArgumentName(useNamedArguments, argument, parameter) switch
            {
                null => (value: argumentValue, namedValue: null),
                var name => (value: argumentValue, namedValue: $"{name}={argumentValue}")
            };
        }

        private String ProcessParamsArgumentValue(IReadOnlyList<ArgumentSyntax> arguments, Int32 startIndex, ImportData importData)
        {
            Int32 paramsSize = arguments.Count - startIndex;
            String[] paramsValues = new String[paramsSize];
            for (Int32 index = 0; index < paramsSize; ++index)
                paramsValues[index] = ConvertExpression(arguments[startIndex + index].Expression, importData);
            switch (paramsSize)
            {
                case 1:
                    TypeInfo lastArgumentInfo = _model.GetTypeInfo(arguments[^1].Expression);
                    switch (lastArgumentInfo.Type)
                    {
                        case null:
                            throw new UnsupportedSyntaxException("Unrecognizable params argument");
                        case IArrayTypeSymbol:
                            return paramsValues[0];
                        default:
                            return $"[{String.Join(", ", paramsValues)}]";
                    }
                default:
                    return $"[{String.Join(", ", paramsValues)}]";
            }
        }

        private (String value, String? namedValue) ProcessParamsArgument(IReadOnlyList<ArgumentSyntax> arguments, Boolean useNamedArguments, Int32 startIndex, IParameterSymbol parameter, ImportData importData)
        {
            String argumentValue = ProcessParamsArgumentValue(arguments, startIndex, importData);
            return GetArgumentName(useNamedArguments, arguments[^1], parameter) switch
            {
                null => (value: argumentValue, namedValue: null),
                var name => (value: argumentValue, namedValue: $"{name}={argumentValue}")
            };
        }

        private String ConvertExpression(ExpressionSyntax expression, ImportData importData)
        {
            ConvertResult result = _expressionConverter.Convert(expression);
            importData.Append(result.ImportData);
            return result.Result;
        }

        private readonly SemanticModel _model;
        private readonly AppData _appData;
        private readonly ExpressionConverter _expressionConverter;
    }
}
