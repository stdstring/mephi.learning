using Microsoft.CodeAnalysis.CSharp.Syntax;
using SimplePythonPorter.DestStorage;
using SimplePythonPorter.Utils;
using System.Globalization;

namespace SimplePythonPorter.Expressions
{
    internal class LiteralExpressionConverter
    {
        public LiteralExpressionConverter(ExpressionConverterSettings settings)
        {
            _settings = settings;
        }

        public ConvertResult Convert(LiteralExpressionSyntax expression)
        {
            LiteralConverter literalConverter = new LiteralConverter();
            String text = literalConverter.Convert(expression, _settings);
            return new ConvertResult(text, new ImportData());
        }

        private readonly ExpressionConverterSettings _settings;
    }

    internal class LiteralConverter
    {
        public String Convert(LiteralExpressionSyntax expression, ExpressionConverterSettings settings)
        {
            Char quoteMark = settings.QuoteMark;
            switch (expression.Token.Value)
            {
                case null:
                    return expression.Token.Text;
                case Double value:
                    return value.ToString(CultureInfo.InvariantCulture);
                case Single value:
                    return value.ToString(CultureInfo.InvariantCulture);
                case Boolean value:
                    return value ? "True" : "False";
                case Char:
                    return $"{quoteMark}{StringUtils.ConvertEscapeSequences(expression.Token.Text.Trim('\''))}{quoteMark}";
                case String when expression.Token.Text.StartsWith('@'):
                    String quotes = new String(quoteMark, 3);
                    return $"{quotes}{StringUtils.PrepareVerbatimString(StringUtils.UnquoteString(expression.Token.Text))}{quotes}";
                case String:
                    return $"{quoteMark}{StringUtils.ConvertEscapeSequences(StringUtils.UnquoteString(expression.Token.Text))}{quoteMark}";
                default:
                    return expression.Token.Value.ToString() ?? expression.Token.Text;
            }
        }
    }
}
