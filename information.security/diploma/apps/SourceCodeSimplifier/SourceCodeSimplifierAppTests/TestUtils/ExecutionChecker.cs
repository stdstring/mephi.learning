using NUnit.Framework;

namespace SourceCodeSimplifierAppTests.TestUtils
{
    internal static class ExecutionChecker
    {
        public static void CheckResult(ExecutionResult result, Int32 exitCode, String outputData, String errorData)
        {
            Int32 actualExitCode = result.ExitCode;
            String actualOutputData = result.OutputData;
            String actualErrorData = result.ErrorData;
            if (exitCode != actualExitCode)
            {
                Console.WriteLine($"Expected exit code is {exitCode}, but actual exit code is {actualExitCode}");
                Console.WriteLine($"Actual output: {actualOutputData}");
                Console.WriteLine($"Actual error: {actualErrorData}");
            }
            Assert.That(actualExitCode, Is.EqualTo(exitCode));
            Assert.That(actualOutputData, Is.EqualTo(outputData));
            Assert.That(actualErrorData, Is.EqualTo(errorData));
        }

        public static void CheckTransformation(String expectedRoot, String actualRoot)
        {
            String[] expectedFiles = Directory.GetFiles(expectedRoot, "*.cs");
            String[] actualFiles = Directory.GetFiles(actualRoot, "*.cs");
            Assert.That(actualFiles.Length, Is.EqualTo(expectedFiles.Length));
            for (Int32 index = 0; index < expectedFiles.Length; ++index)
            {
                String expectedFilename = Path.GetFileName(expectedFiles[index]);
                String actualFilename = Path.GetFileName(actualFiles[index]);
                Assert.That(actualFilename, Is.EqualTo(expectedFilename));
                String expectedContent = File.ReadAllText(expectedFiles[index]);
                String actualContent = File.ReadAllText(actualFiles[index]);
                Assert.That(actualContent, Is.EqualTo(expectedContent));
            }
        }
    }
}
