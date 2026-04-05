using SimplePythonPorter.Utils;

namespace SimplePythonPorter.DestStorage
{
    internal class ImportStorage
    {
        public void Save(TextWriter writer)
        {
            foreach (KeyValuePair<String, String> entry in _modulesImport)
            {
                String module = entry.Key;
                String alias = entry.Value;
                String import = String.IsNullOrEmpty(alias) ? $"import {module}" : $"import {module} as {alias}";
                writer.WriteLine(import);
            }
            foreach (KeyValuePair<String, ISet<String>> entry in _entityImport)
            {
                String module = entry.Key;
                String entities = String.Join(", ", entry.Value);
                writer.WriteLine($"from {module} import {entities}");
            }
            writer.WriteLine();
            writer.WriteLine();
        }

        public void AddImport(String module, String alias = "")
        {
            if (String.IsNullOrEmpty(module))
                return;
            _modulesImport.TryAdd(module, alias);
        }

        public void AddEntity(String module, String entity)
        {
            if (!_entityImport.ContainsKey(module))
                _entityImport.Add(module, new SortedSet<String>());
            _entityImport[module].Add(entity);
        }

        public void Append(ImportData data)
        {
            foreach (KeyValuePair<String, String> entry in data.ModulesImport)
                AddImport(entry.Key, entry.Value);
            foreach (KeyValuePair<String, ISet<String>> entry in data.EntityImport)
            {
                if (!_entityImport.ContainsKey(entry.Key))
                    _entityImport.Add(entry.Key, new SortedSet<String>());
                _entityImport[entry.Key].AddRange(entry.Value);
            }
        }

        private readonly IDictionary<String, String> _modulesImport = new SortedDictionary<String, String>();
        private readonly IDictionary<String, ISet<String>> _entityImport = new SortedDictionary<String, ISet<String>>();
    }

    internal class ImportData
    {
        public IDictionary<String, String> ModulesImport = new Dictionary<String, String>();

        public IDictionary<String, ISet<String>> EntityImport = new Dictionary<String, ISet<String>>();

        public ImportData AddImport(String module, String alias = "")
        {
            if (!String.IsNullOrEmpty(module))
                ModulesImport.TryAdd(module, alias);
            return this;
        }

        public ImportData AddEntity(String module, String entity)
        {
            if (!EntityImport.ContainsKey(module))
                EntityImport.Add(module, new SortedSet<String>());
            EntityImport[module].Add(entity);
            return this;
        }

        public ImportData Append(ImportData other)
        {
            foreach (KeyValuePair<String, String> entry in other.ModulesImport)
                AddImport(entry.Key, entry.Value);
            foreach (KeyValuePair<String, ISet<String>> entry in other.EntityImport)
            {
                if (!EntityImport.ContainsKey(entry.Key))
                    EntityImport.Add(entry.Key, new SortedSet<String>());
                EntityImport[entry.Key].AddRange(entry.Value);
            }
            return this;
        }
    }
}