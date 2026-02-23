using SourceCodeSimplifierApp.Transformers;

namespace SourceCodeSimplifierApp.Processors
{
    public interface ISourceProcessor
    {
        void Process(IList<ITransformer> transformers);
    }
}
