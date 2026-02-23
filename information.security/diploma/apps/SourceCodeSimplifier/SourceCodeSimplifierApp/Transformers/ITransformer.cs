using Microsoft.CodeAnalysis;

namespace SourceCodeSimplifierApp.Transformers
{
    public interface ITransformer
    {
        Document Transform(Document source);
    }
}
