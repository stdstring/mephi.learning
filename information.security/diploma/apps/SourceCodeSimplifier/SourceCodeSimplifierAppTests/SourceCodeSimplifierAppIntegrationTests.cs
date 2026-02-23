using NUnit.Framework;
using SourceCodeSimplifierApp.Config;
using SourceCodeSimplifierApp.Transformers;
using SourceCodeSimplifierAppTests.TestUtils;

namespace SourceCodeSimplifierAppTests
{
    [TestFixture]
    public class SourceCodeSimplifierAppIntegrationTests
    {
        [TearDown]
        public void Cleanup()
        {
            foreach (String testConfigFile in Directory.GetFiles(".", $"*{ConfigGenerator.ConfigSuffix}"))
                File.Delete(testConfigFile);
            if (Directory.Exists(TargetActualDest))
                Directory.Delete(TargetActualDest, true);
        }

        [SetUp]
        public void Init()
        {
            Cleanup();
            DirectoryUtils.CopyDirectory(TargetActualSource, TargetActualDest, true);
        }

        [Test]
        public void ProcessEmptyArgs()
        {
            ExecutionResult executionResult = ExecutionHelper.Execute("");
            ExecutionChecker.CheckResult(executionResult, 0, SourceCodeSimplifierAppOutputDef.AppDescription, "");
        }

        [Test]
        public void ProcessHelp()
        {
            ExecutionResult executionResult = ExecutionHelper.Execute("--help");
            ExecutionChecker.CheckResult(executionResult, 0, SourceCodeSimplifierAppOutputDef.AppDescription, "");
        }

        [Test]
        public void ProcessVersion()
        {
            ExecutionResult executionResult = ExecutionHelper.Execute("--version");
            ExecutionChecker.CheckResult(executionResult, 0, "0.0.1", "");
        }

        [TestCase("--some-strange-option")]
        [TestCase("--config=\"SomeConfig.xml\" --some-strange-option")]
        public void ProcessUnknownArg(String args)
        {
            ExecutionResult executionResult = ExecutionHelper.Execute(args);
            ExecutionChecker.CheckResult(executionResult, -1, SourceCodeSimplifierAppOutputDef.AppDescription, SourceCodeSimplifierAppOutputDef.BadArgsMessage);
        }

        [Test]
        public void ProcessUnknownConfig()
        {
            ExecutionResult executionResult = ExecutionHelper.Execute("--config=\"SomeConfig.xml\"");
            ExecutionChecker.CheckResult(executionResult, -1, "", SourceCodeSimplifierAppOutputDef.UnknownConfigMessage);
        }

        [Test]
        public void ProcessConfigWithoutValue()
        {
            ExecutionResult executionResult = ExecutionHelper.Execute("--config=");
            ExecutionChecker.CheckResult(executionResult, -1, "", SourceCodeSimplifierAppOutputDef.BadConfigMessage);
        }

        [Test]
        public void ProcessUnknownTarget()
        {
            String configPath = ConfigGenerator.Generate("ProcessUnknownTarget", "./SomeUnknownTarget.csproj");
            ExecutionResult executionResult = ExecutionHelper.Execute($"--config=\"{configPath}\"");
            ExecutionChecker.CheckResult(executionResult, -1, "", SourceCodeSimplifierAppOutputDef.UnknownTargetMessage);
        }

        [Test]
        public void ProcessSingleFile()
        {
            String filename = Path.GetFullPath($"{TargetActualDest}/EmptyData.cs");
            String configPath = ConfigGenerator.Generate("ProcessSingleFile", filename);
            ExecutionResult executionResult = ExecutionHelper.Execute($"--config=\"{configPath}\"");
            ExecutionChecker.CheckResult(executionResult, -1, "", SourceCodeSimplifierAppOutputDef.UnsupportedTargetMessage);
        }

        [Test]
        public void ProcessProjectError()
        {
            String actualProject = Path.GetFullPath($"{TargetActualDest}/SourceCodeSimplifierAppTestsTarget.csproj");
            String configPath = ConfigGenerator.Generate("ProcessProjectError", actualProject, OutputLevel.Error, DefaultTransformers);
            ExecutionResult executionResult = ExecutionHelper.Execute($"--config=\"{configPath}\"");
            ExecutionChecker.CheckResult(executionResult, 0, "", "");
            ExecutionChecker.CheckTransformation(TargetExpected, TargetActualDest);
        }

        [Test]
        public void ProcessProjectWarning()
        {
            String actualProject = Path.GetFullPath($"{TargetActualDest}/SourceCodeSimplifierAppTestsTarget.csproj");
            String configPath = ConfigGenerator.Generate("ProcessProjectWarning", actualProject, OutputLevel.Warning, DefaultTransformers);
            ExecutionResult executionResult = ExecutionHelper.Execute($"--config=\"{configPath}\"");
            ExecutionChecker.CheckResult(executionResult, 0, "", "");
            ExecutionChecker.CheckTransformation(TargetExpected, TargetActualDest);
        }

        [Test]
        public void ProcessProjectInfo()
        {
            String actualProjectDir = Path.GetFullPath(TargetActualDest);
            String actualProject = $"{actualProjectDir}\\SourceCodeSimplifierAppTestsTarget.csproj";
            String configPath = ConfigGenerator.Generate("ProcessProjectInfo", actualProject, OutputLevel.Info, DefaultTransformers);
            ExecutionResult executionResult = ExecutionHelper.Execute($"--config=\"{configPath}\"");
            const String expectedOutputTemplate = "Processing of the project {0} is started\r\n" +
                                                  "dotnet build is succeeded\r\n" +
                                                  SourceCodeSimplifierAppOutputDef.CompilationCheckSuccessOutput +
                                                  "Processing of the file {1}\\EmptyData.cs is started\r\n" +
                                                  "Execution of SourceCodeSimplifierApp.Transformers.EmptyTransformer started\r\n" +
                                                  "Execution of SourceCodeSimplifierApp.Transformers.EmptyTransformer finished\r\n" +
                                                  "Processing of the file {1}\\EmptyData.cs is finished\r\n" +
                                                  "Processing of the project {0} is finished";
            String expectedOutput = string.Format(expectedOutputTemplate, actualProject, actualProjectDir);
            ExecutionChecker.CheckResult(executionResult, 0, expectedOutput, "");
            ExecutionChecker.CheckTransformation(TargetExpected, TargetActualDest);
        }

        [Test]
        public void ProcessProjectErrorWithDefaultTransformersConfig()
        {
            String actualProject = Path.GetFullPath($"{TargetActualDest}/SourceCodeSimplifierAppTestsTarget.csproj");
            String configPath = ConfigGenerator.Generate("ProcessProjectError", actualProject, OutputLevel.Error);
            ExecutionResult executionResult = ExecutionHelper.Execute($"--config=\"{configPath}\"");
            ExecutionChecker.CheckResult(executionResult, 0, "", "");
            ExecutionChecker.CheckTransformation(TargetActualSource, TargetActualDest);
        }

        [Test]
        public void ProcessProjectWarningWithDefaultTransformersConfig()
        {
            String actualProject = Path.GetFullPath($"{TargetActualDest}/SourceCodeSimplifierAppTestsTarget.csproj");
            String configPath = ConfigGenerator.Generate("ProcessProjectWarning", actualProject, OutputLevel.Warning);
            ExecutionResult executionResult = ExecutionHelper.Execute($"--config=\"{configPath}\"");
            ExecutionChecker.CheckResult(executionResult, 0, "", "");
            ExecutionChecker.CheckTransformation(TargetActualSource, TargetActualDest);
        }

        [Test]
        public void ProcessProjectInfoWithDefaultTransformersConfig()
        {
            String actualProjectDir = Path.GetFullPath(TargetActualDest);
            String actualProject = $"{actualProjectDir}\\SourceCodeSimplifierAppTestsTarget.csproj";
            String configPath = ConfigGenerator.Generate("ProcessProjectInfo", actualProject, OutputLevel.Info);
            ExecutionResult executionResult = ExecutionHelper.Execute($"--config=\"{configPath}\"");
            const String expectedOutputTemplate = "Processing of the project {0} is started\r\n" +
                                                  "dotnet build is succeeded\r\n" +
                                                  SourceCodeSimplifierAppOutputDef.CompilationCheckSuccessOutput +
                                                  "Processing of the file {1}\\EmptyData.cs is started\r\n" +
                                                  "Processing of the file {1}\\EmptyData.cs is finished\r\n" +
                                                  "Processing of the project {0} is finished";
            String expectedOutput = string.Format(expectedOutputTemplate, actualProject, actualProjectDir);
            ExecutionChecker.CheckResult(executionResult, 0, expectedOutput, "");
            ExecutionChecker.CheckTransformation(TargetActualSource, TargetActualDest);
        }

        private static readonly IDictionary<String, TransformerState> DefaultTransformers = new Dictionary<String, TransformerState>
        {
            {EmptyTransformer.Name, TransformerState.On}
        };

        private const String TargetExpected = "../../../../SourceCodeSimplifierAppTestsExpected";
        private const String TargetActualSource = "../../../../SourceCodeSimplifierAppTestsTarget";
        private const String TargetActualDest = "./SourceCodeSimplifierAppTestsActual";
    }
}
