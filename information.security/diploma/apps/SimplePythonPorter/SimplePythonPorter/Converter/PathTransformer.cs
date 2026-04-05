namespace SimplePythonPorter.Converter
{
    internal static class PathTransformer
    {
        public static String TransformPath(String sourcePath, NameTransformer nameTransformer)
        {
            String[] parts = sourcePath.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            String[] result = new String[parts.Length];
            for (Int32 index = 0; index < parts.Length; ++index)
                result[index] = index < parts.Length - 1
                    ? nameTransformer.TransformFileObjectName(parts[index])
                    : $"{nameTransformer.TransformFileObjectName(Path.GetFileNameWithoutExtension(parts[index]))}.py";
            return String.Join(Path.DirectorySeparatorChar, result);
        }
    }
}