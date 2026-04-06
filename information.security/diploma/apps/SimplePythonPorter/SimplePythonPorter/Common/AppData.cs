using SimplePythonPorter.Converter;

namespace SimplePythonPorter.Common
{
    internal record TransformResult(String RelativePath, String Content);

    internal record AppData(NameTransformer NameTransformer, IList<TransformResult> Results);
}
