using SimplePythonPorter.Utils;

namespace SimplePythonPorter.DestStorage
{
    internal class MethodStorage
    {
        public MethodStorage(String methodName, Int32 indentation, ImportStorage importStorage)
        {
            _methodName = methodName;
            _globalIndentation = indentation;
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
            String bodyIndentation = IndentationUtils.Create(_globalIndentation + StorageDef.IndentationDelta);
            if (_errorReason == null)
                SaveBorderData(writer, baseIndentation, _headerData);
            foreach (String decorator in _decorators)
                writer.WriteLine($"{baseIndentation}{decorator}");
            writer.WriteLine($"{baseIndentation}def {_methodName}(self):");
            if (_errorReason != null)
            {
                String errorReason = StringUtils.Escape(_errorReason);
                writer.WriteLine($"{bodyIndentation}raise NotImplementedError(\"{errorReason}\")");
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

        public void IncreaseLocalIndentation(Int32 delta)
        {
            if (delta < 0)
                throw new ArgumentOutOfRangeException(nameof(delta));
            _localIndentation += delta;
        }

        public void DecreaseLocalIndentation(Int32 delta)
        {
            if ((delta < 0) || (delta > _localIndentation))
                throw new ArgumentOutOfRangeException(nameof(delta));
            _localIndentation -= delta;
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
        private readonly Int32 _globalIndentation;
        private Int32 _localIndentation;
        private readonly IList<String> _headerData = new List<String>();
        private readonly IList<String> _footerData = new List<String>();
        private String _trailingData = String.Empty;
        private readonly IList<String> _decorators = new List<String>();
        private readonly IList<String> _body = new List<String>();
        private String? _errorReason;
    }
}
