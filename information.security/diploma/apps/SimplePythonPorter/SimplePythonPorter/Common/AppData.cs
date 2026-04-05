namespace SimplePythonPorter.Common
{
    internal record TransformResult(String RelativePath, String Content);

    internal record AppData(IList<TransformResult> Results);
}
