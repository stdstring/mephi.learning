using SimplePythonPorter.Utils;

namespace SimplePythonPorter.DestStorage
{
    internal class FileStorage
    {
        public FileStorage()
        {
            _indentation = 0;
            ImportStorage = new ImportStorage();
        }

        public void AppendHeaderData(String[] dataPortion)
        {
            _headerData.AddRange(dataPortion);
        }

        public void Save(TextWriter writer)
        {
                writer.WriteLine("# -*- coding: utf-8 -*-");
                writer.WriteLine();
                SaveBorderData(writer, _headerData);
                ImportStorage.Save(writer);
                for (Int32 index = 0; index < _classes.Count; ++index)
                {
                    if (index > 0)
                        writer.WriteLine();
                    _classes[index].Save(writer);
                }
        }

        public ClassStorage CreateClassStorage(String className)
        {
            Int32 indentation = _indentation + (_indentation > 0 ? StorageDef.IndentationDelta : 0);
            ClassStorage classStorage = new ClassStorage(className, indentation, ImportStorage);
            _classes.Add(classStorage);
            return classStorage;
        }

        public Boolean IsEmpty() => _classes.IsEmpty();

        public ImportStorage ImportStorage { get; }

        private void SaveBorderData(TextWriter writer, IList<String> data)
        {
            if (data.IsEmpty())
                return;
            foreach (String line in data)
                writer.WriteLine(line);
            writer.WriteLine();
        }

        private readonly Int32 _indentation;
        private readonly IList<String> _headerData = new List<String>();
        private readonly IList<ClassStorage> _classes = new List<ClassStorage>();
    }
}
