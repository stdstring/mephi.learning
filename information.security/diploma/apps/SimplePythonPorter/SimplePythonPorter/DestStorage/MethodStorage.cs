using SimplePythonPorter.Utils;

namespace SimplePythonPorter.DestStorage
{
    internal class MethodStorage
    {
        public MethodStorage(String methodName,
                             String[] parameters,
                             Int32 indentation,
                             Int32 indentationDelta,
                             ImportStorage importStorage)
        {
            _methodName = methodName;
            _parameters = parameters;
            _globalIndentation = indentation;
            _indentationDelta = indentationDelta;
            ImportStorage = importStorage;
        }

        public void AppendHeaderData(String[] dataPortion)
        {
            _headerData.AddRange(dataPortion);
        }

        public void AppendFooterData(String[] dataPortion)
        {
            _footerData.AddRange(dataPortion);
        }

        public void SetTrailingData(String? data)
        {
            if (data != null)
                _trailingData = data;
        }

        public void Save(TextWriter writer)
        {
            String baseIndentation = IndentationUtils.Create(_globalIndentation);
            String bodyIndentation = IndentationUtils.Create(_globalIndentation + _indentationDelta);
            if (_errorReason == null)
                SaveBorderData(writer, baseIndentation, _headerData);
            foreach (String decorator in _decorators)
                writer.WriteLine($"{baseIndentation}{decorator}");
            String parametersList = String.Join(", ", _parameters);
            writer.WriteLine($"{baseIndentation}def {_methodName}({parametersList}):");
            if (_errorReason != null)
            {
                String errorReason = StringUtils.Escape(_errorReason);
                ImportStorage.AddImport("system");
                writer.WriteLine($"{bodyIndentation}raise system.NotImplementedError(\"{errorReason}\")");
            }
            else
            {
                foreach (String bodyLine in _body)
                    WriteLine(writer, bodyIndentation, bodyLine);
                if (_body.IsEmpty())
                    writer.WriteLine($"{bodyIndentation}pass");
            }
            if (_errorReason == null)
            {
                if (!String.IsNullOrEmpty(_trailingData))
                    writer.WriteLine($"{baseIndentation}{_trailingData}");
                SaveBorderData(writer, baseIndentation, _footerData);
            }
        }

        public void AddDecorator(String decorator)
        {
            _decorators.Add(decorator);
        }

        public void AddBodyLine(String bodyLine)
        {
            if (_errorReason != null)
                return;
            String localIndentation = IndentationUtils.Create(_localIndentation);
            _body.Add($"{localIndentation}{bodyLine}");
        }

        public void AddBodyLines(String[] bodyLines)
        {
            if (_errorReason != null)
                return;
            String localIndentation = IndentationUtils.Create(_localIndentation);
            foreach (String bodyLine in bodyLines)
                _body.Add($"{localIndentation}{bodyLine}");
        }

        public void SetLineTrailingData(String? data)
        {
            if (data == null)
                return;
            if (_body.IsEmpty())
                return;
            _body[^1] = $"{_body[^1]} {data}";
        }

        public void SetError(String errorReason)
        {
            _errorReason = errorReason;
        }

        public void IncreaseLocalIndentation()
        {
            _localIndentation += _indentationDelta;
        }

        public void DecreaseLocalIndentation()
        {
            _localIndentation -= _indentationDelta;
        }

        public Boolean HasError => _errorReason != null;

        public ImportStorage ImportStorage { get; }

        private void SaveBorderData(TextWriter writer, String indentation, IList<String> data)
        {
            if (data.IsEmpty())
                return;
            foreach (String line in data)
                WriteLine(writer, indentation, line);
        }

        private void WriteLine(TextWriter writer, String indentation, String line)
        {
            if (String.IsNullOrEmpty(line))
                writer.WriteLine();
            else
                writer.WriteLine($"{indentation}{line}");
        }

        private readonly String _methodName;
        private readonly String[] _parameters;
        private readonly Int32 _globalIndentation;
        private Int32 _localIndentation;
        private readonly Int32 _indentationDelta;
        private readonly IList<String> _headerData = new List<String>();
        private readonly IList<String> _footerData = new List<String>();
        private String _trailingData = String.Empty;
        private readonly IList<String> _decorators = new List<String>();
        private readonly IList<String> _body = new List<String>();
        private String? _errorReason;
    }
}
