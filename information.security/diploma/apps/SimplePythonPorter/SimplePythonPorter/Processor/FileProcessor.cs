using Microsoft.CodeAnalysis;
using SimplePythonPorter.Common;
using SimplePythonPorter.Converter;
using SimplePythonPorter.Utils;

namespace SimplePythonPorter.Processor
{
    internal class FileProcessor
    {
        public FileProcessor(AppData appData)
        {
            _appData = appData;
        }

        public void Process(String relativeFilename, Document file, Compilation compilation)
        {
            ProcessImpl(relativeFilename, file, compilation);
        }

        private void ProcessImpl(String relativeFilename, Document file, Compilation compilation)
        {
            SyntaxTree tree = file.GetSyntaxTreeAsync().Result.Must("Bad file: without syntax tree");
            SemanticModel model = compilation.GetSemanticModel(tree);
            FileConverter converter = new FileConverter(_appData);
            converter.Convert(relativeFilename, tree, model);
        }

        private readonly AppData _appData;
    }
}
