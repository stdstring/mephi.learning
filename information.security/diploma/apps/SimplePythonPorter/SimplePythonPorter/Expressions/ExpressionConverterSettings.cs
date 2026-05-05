namespace SimplePythonPorter.Expressions
{
    internal struct ExpressionConverterSettings
    {
        public ExpressionConverterSettings()
        {
        }

        public ExpressionConverterSettings(ExpressionConverterSettings other)
        {
            AllowIncrementDecrement = other.AllowIncrementDecrement;
            QuoteMark = other.QuoteMark;
        }

        public Boolean AllowIncrementDecrement { get; set; }

        public Char QuoteMark { get; set; } = '"';

        public readonly ExpressionConverterSettings CreateChild()
        {
            return new ExpressionConverterSettings(this)
            {
                AllowIncrementDecrement = false,
            };
        }
    }
}