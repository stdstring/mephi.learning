using SimplePythonPorter.Utils;

namespace SimplePythonPorter.DestStorage
{
    internal class ClassStorage
    {
        public ClassStorage(String className, Int32 indentation, ImportStorage importStorage)
        {
            _className = className;
            _indentation = indentation;
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

        // TODO (std_string) : add support of writing trailing data
        public void Save(TextWriter writer)
        {
            String baseIndentation = IndentationUtils.Create(_indentation);
            String bodyIndentation = IndentationUtils.Create(_indentation + StorageDef.IndentationDelta);
            SaveBorderData(writer, baseIndentation, _headerData);
            foreach (String decorator in _decorators)
                writer.WriteLine($"{baseIndentation}{decorator}");
            String baseClassesPart = _baseClasses.IsEmpty() ? "" : $"({String.Join(",", _baseClasses)})";
            writer.WriteLine($"{baseIndentation}class {_className}{baseClassesPart}:");
            for (Int32 index = 0; index < _methods.Count; ++index)
            {
                if (index > 0)
                    writer.WriteLine();
                _methods[index].Save(writer);
            }
            if (_methods.IsEmpty())
                writer.WriteLine($"{bodyIndentation}pass");
            if (!String.IsNullOrEmpty(_trailingData))
                writer.WriteLine($"{baseIndentation}{_trailingData}");
            SaveBorderData(writer, baseIndentation, _footerData);
        }

        public void AddDecorator(String decorator)
        {
            _decorators.Add(decorator);
        }

        public void AddBaseClass(String baseClass)
        {
            _baseClasses.Add(baseClass);
        }

        public MethodStorage CreateMethodStorage(String methodName)
        {
            Int32 indentation = _indentation + StorageDef.IndentationDelta;
            MethodStorage currentMethod = new MethodStorage(methodName, indentation, ImportStorage);
            _methods.Add(currentMethod);
            return currentMethod;
        }

        public ImportStorage ImportStorage { get; }

        private void SaveBorderData(TextWriter writer, String indentation, IList<String> data)
        {
            if (data.IsEmpty())
                return;
            foreach (String line in data)
                writer.WriteLine($"{indentation}{line}");
        }

        private readonly String _className;
        private readonly Int32 _indentation;
        private readonly IList<String> _headerData = new List<String>();
        private readonly IList<String> _footerData = new List<String>();
        private String _trailingData = String.Empty;
        private readonly IList<String> _decorators = new List<String>();
        private readonly IList<String> _baseClasses = new List<String>();
        private readonly IList<MethodStorage> _methods = new List<MethodStorage>();
    }
}
