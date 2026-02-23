using SourceCodeSimplifierApp.Config;
using SourceCodeSimplifierApp.Output;

namespace SourceCodeSimplifierApp.Transformers
{
    internal static class TransformersFactory
    {
        public static ITransformer[] Create(IOutput output, TransformerEntry[] config)
        {
            IDictionary<String, TransformerState> transformersMap = config.ToDictionary(entry => entry.Name!, entry => entry.State);
            TransformerState GetTransformerState(String name) => transformersMap.TryGetValue(name, out var state) ? state : TransformerState.Off;
            return new ITransformer[]
            {
                new EmptyTransformer(output, GetTransformerState(EmptyTransformer.Name)),
                // node based transformers
                new NameOfExprTransformer(output, GetTransformerState(NameOfExprTransformer.Name)),
                new StringInterpolationExprTransformer(output, GetTransformerState(StringInterpolationExprTransformer.Name)),
                // other transformers
                new ObjectInitializerExprTransformer(output, GetTransformerState(ObjectInitializerExprTransformer.Name)),
                new OutInlineVariableTransformer(output, GetTransformerState(OutInlineVariableTransformer.Name)),
                new NullConditionalOperatorTransformer(output, GetTransformerState(NullConditionalOperatorTransformer.Name))
            };
        }
    }
}
