namespace SourceCodeSimplifierApp.Variables
{
    internal class VariableGenerator
    {
        public VariableGenerator(ISet<String> knownVariables)
        {
            _knownVariables = new HashSet<String>(knownVariables);
            _prefixMap = new Dictionary<String, Int32>();
        }

        public String Generate(String prefix)
        {
            Int32 suffix = 0;
            if (_prefixMap.ContainsKey(prefix))
                suffix = _prefixMap[prefix];
            else
                _prefixMap.Add(prefix, 0);
            return Generate(prefix, suffix);
        }

        private String Generate(String prefix, Int32 suffix)
        {
            for (;;++suffix)
            {
                String variable = suffix == 0 ? prefix : $"{prefix}{suffix + 1}";
                if (!_knownVariables.Contains(variable))
                {
                    _knownVariables.Add(variable);
                    _prefixMap[prefix] = suffix;
                    return variable;
                }
            }
        }

        private readonly ISet<String> _knownVariables;
        private readonly IDictionary<String, Int32> _prefixMap;
    }
}
