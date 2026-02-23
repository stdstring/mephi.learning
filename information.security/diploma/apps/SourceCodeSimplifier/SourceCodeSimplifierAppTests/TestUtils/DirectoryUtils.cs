namespace SourceCodeSimplifierAppTests.TestUtils
{
    internal static class DirectoryUtils
    {
        public static void CopyDirectory(String source, String dest, Boolean overwriteContent)
        {
            DirectoryInfo destDirectory = new DirectoryInfo(dest);
            Boolean isDestEmpty = destDirectory.Exists &&
                                  (destDirectory.GetFiles().Length == 0) &&
                                  (destDirectory.GetDirectories().Length == 0);
            if (!isDestEmpty && !overwriteContent)
                throw new InvalidOperationException("Nonempty dest directory");
            if (destDirectory.Exists)
                destDirectory.Delete(true);
            destDirectory.Create();
            Copy(source, destDirectory.FullName);
        }

        private static void Copy(String sourceRoot, String destRoot)
        {
            foreach (String sourceFile in Directory.GetFiles(sourceRoot))
            {
                String fileName = Path.GetFileName(sourceFile);
                String destFile = Path.Combine(destRoot, fileName);
                File.Copy(sourceFile, destFile);
            }
            foreach (String sourceDirectory in Directory.GetDirectories(sourceRoot))
            {
                String directoryName = Path.GetFileName(sourceDirectory);
                String destDirectory = Path.Combine(destRoot, directoryName);
                Directory.CreateDirectory(destDirectory);
                Copy(sourceDirectory, destDirectory);
            }
        }
    }
}
