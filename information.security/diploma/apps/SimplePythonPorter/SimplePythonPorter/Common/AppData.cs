using SimplePythonPorter.Converter;

namespace SimplePythonPorter.Common
{
    public record TransformResult(String RelativePath, String Content);

    public record AppData(NameTransformer NameTransformer, IList<TransformResult> Results);
}
