namespace SimplePythonPorter.Ignored
{
    internal class IgnoredEntitiesManager
    {
        public Boolean IsIgnoredFile(String relativePath)
        {
            String[] ignoredFilesSuffixes = new[]{".AssemblyInfo.cs", ".AssemblyAttributes.cs", ".GlobalUsings.g.cs"};
            return ignoredFilesSuffixes.Any(relativePath.EndsWith);
        }
    }
}
