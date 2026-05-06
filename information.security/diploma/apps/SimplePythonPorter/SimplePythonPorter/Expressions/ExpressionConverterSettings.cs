using SimplePythonPorter.Converter;

namespace SimplePythonPorter.Expressions
{
    internal class ExpressionConverterSettings
    {
        public ExpressionConverterSettings(CurrentProcessingType currentType)
        {
            CurrentType = currentType;
        }

        public ExpressionConverterSettings(ExpressionConverterSettings other)
        {
            CurrentType = other.CurrentType;
            AllowIncrementDecrement = other.AllowIncrementDecrement;
            QuoteMark = other.QuoteMark;
        }

        public CurrentProcessingType CurrentType { get; }

        public Boolean AllowIncrementDecrement { get; set; }

        public Char QuoteMark { get; set; } = '"';

        public ExpressionConverterSettings CreateChild()
        {
            return new ExpressionConverterSettings(this)
            {
                AllowIncrementDecrement = false,
            };
        }
    }
}