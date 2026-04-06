using Microsoft.CodeAnalysis;
using SimplePythonPorter.Common;
using SimplePythonPorter.DestStorage;
using SimplePythonPorter.Ignored;
using SimplePythonPorter.Utils;

namespace SimplePythonPorter.Converter
{
    internal class FileConverter
    {
        public FileConverter(AppData appData)
        {
            _appData = appData;
            _ignoredEntitiesManager = new IgnoredEntitiesManager();
        }

        public void Convert(String relativeFilePath, SyntaxTree tree, SemanticModel model)
        {
            if (_ignoredEntitiesManager.IsIgnoredFile(relativeFilePath))
                return;
            // Restriction for experiment
            if (!_appData.Results.IsEmpty())
                throw new InvalidOperationException("Unsupported MultiFile project");
            String destRelativePath = PathTransformer.TransformPath(relativeFilePath, _appData.NameTransformer);
            FileStorage currentFile = new FileStorage();
            FileConverterVisitor converter = new FileConverterVisitor(model, currentFile, _appData);
            SyntaxNode root = tree.GetRoot();
            converter.Visit(root);
            if (currentFile.IsEmpty())
                return;
            using (StringWriter writer = new StringWriter())
            {
                currentFile.Save(writer);
                _appData.Results.Add(new TransformResult(destRelativePath, writer.ToString()));
            }
        }

        private readonly AppData _appData;
        private readonly IgnoredEntitiesManager _ignoredEntitiesManager;
    }
}
