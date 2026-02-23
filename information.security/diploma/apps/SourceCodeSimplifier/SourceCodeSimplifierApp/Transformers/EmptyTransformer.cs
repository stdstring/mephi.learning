using Microsoft.CodeAnalysis;
using SourceCodeSimplifierApp.Config;
using SourceCodeSimplifierApp.Output;

namespace SourceCodeSimplifierApp.Transformers
{
    // This transformer is for creation of tests only
    internal class EmptyTransformer : ITransformer
    {
        public const String Name = "SourceCodeSimplifierApp.Transformers.EmptyTransformer";

        public EmptyTransformer(IOutput output, TransformerState transformerState)
        {
            _output = output;
            _transformerState = transformerState;
        }

        public Document Transform(Document source)
        {
            if (_transformerState == TransformerState.Off)
                return source;
            _output.WriteInfoLine($"Execution of {Name} started");
            Document dest = source;
            _output.WriteInfoLine($"Execution of {Name} finished");
            return dest;
        }

        private readonly IOutput _output;
        private readonly TransformerState _transformerState;
    }
}
