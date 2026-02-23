using NUnit.Framework;
using SourceCodeSimplifierApp.Config;
using SourceCodeSimplifierApp.Output;
using SourceCodeSimplifierApp.Transformers;
using SourceCodeSimplifierAppTests.TestUtils;

namespace SourceCodeSimplifierAppTests.Transformers
{
    [TestFixture]
    public class NameOfExprTransformerTests
    {
        [TestCase(OutputLevel.Error)]
        [TestCase(OutputLevel.Warning)]
        [TestCase(OutputLevel.Info)]
        public void ProcessWithNameOfExpr(OutputLevel outputLevel)
        {
            const String source = "namespace SomeNamespace\r\n" +
                                  "{\r\n" +
                                  "    public class SomeClass\r\n" +
                                  "    {\r\n" +
                                  "        public void SomeMethod()\r\n" +
                                  "        {\r\n" +
                                  "            string s1 = nameof(SomeMethod);\r\n" +
                                  "            string s2 = nameof(SomeClass);\r\n" +
                                  "            string s3 = nameof(SomeNamespace);\r\n" +
                                  "        }\r\n" +
                                  "    }\r\n" +
                                  "}";
            const String expectedResult = "namespace SomeNamespace\r\n" +
                                          "{\r\n" +
                                          "    public class SomeClass\r\n" +
                                          "    {\r\n" +
                                          "        public void SomeMethod()\r\n" +
                                          "        {\r\n" +
                                          "            string s1 = \"SomeMethod\";\r\n" +
                                          "            string s2 = \"SomeClass\";\r\n" +
                                          "            string s3 = \"SomeNamespace\";\r\n" +
                                          "        }\r\n" +
                                          "    }\r\n" +
                                          "}";
            String expectedOutput = outputLevel == OutputLevel.Info ? ExpectedOutputForInfoLevel : "";
            TransformerHelper transformerHelper = new TransformerHelper(source, "NameOfExpression", outputLevel);
            transformerHelper.Process(_transformerOnFactory, expectedOutput, expectedResult);
            transformerHelper.Process(_transformerOffFactory, "", source);
        }

        [TestCase(OutputLevel.Error)]
        [TestCase(OutputLevel.Warning)]
        [TestCase(OutputLevel.Info)]
        public void ProcessWithNameOfMethod(OutputLevel outputLevel)
        {
            const String source = "namespace SomeNamespace\r\n" +
                                  "{\r\n" +
                                  "    public class SomeClass\r\n" +
                                  "    {\r\n" +
                                  "        public void SomeMethod()\r\n" +
                                  "        {\r\n" +
                                  "            string s1 = nameof(\"SomeMethod\");\r\n" +
                                  "            string s2 = nameof(\"SomeClass\");\r\n" +
                                  "            string s3 = nameof(\"SomeNamespace\");\r\n" +
                                  "        }\r\n" +
                                  "        public string nameof(string p)\r\n" +
                                  "        {\r\n" +
                                  "            return p;\r\n" +
                                  "        }\r\n" +
                                  "    }\r\n" +
                                  "}";
            const String expectedResult = "namespace SomeNamespace\r\n" +
                                          "{\r\n" +
                                          "    public class SomeClass\r\n" +
                                          "    {\r\n" +
                                          "        public void SomeMethod()\r\n" +
                                          "        {\r\n" +
                                          "            string s1 = nameof(\"SomeMethod\");\r\n" +
                                          "            string s2 = nameof(\"SomeClass\");\r\n" +
                                          "            string s3 = nameof(\"SomeNamespace\");\r\n" +
                                          "        }\r\n" +
                                          "        public string nameof(string p)\r\n" +
                                          "        {\r\n" +
                                          "            return p;\r\n" +
                                          "        }\r\n" +
                                          "    }\r\n" +
                                          "}";
            String expectedOutput = outputLevel == OutputLevel.Info ? ExpectedOutputForInfoLevel : "";
            TransformerHelper transformerHelper = new TransformerHelper(source, "NameOfExpression", outputLevel);
            transformerHelper.Process(_transformerOnFactory, expectedOutput, expectedResult);
            transformerHelper.Process(_transformerOffFactory, "", source);
        }

        [TestCase(OutputLevel.Error)]
        [TestCase(OutputLevel.Warning)]
        [TestCase(OutputLevel.Info)]
        public void ProcessWithoutNameOfExpr(OutputLevel outputLevel)
        {
            const String source = "namespace SomeNamespace\r\n" +
                                  "{\r\n" +
                                  "    public class SomeClass\r\n" +
                                  "    {\r\n" +
                                  "        public void SomeMethod()\r\n" +
                                  "        {\r\n" +
                                  "            string s1 = \"SomeMethod\";\r\n" +
                                  "            string s2 = \"SomeClass\";\r\n" +
                                  "            string s3 = \"SomeNamespace\";\r\n" +
                                  "        }\r\n" +
                                  "    }\r\n" +
                                  "}";
            const String expectedResult = "namespace SomeNamespace\r\n" +
                                          "{\r\n" +
                                          "    public class SomeClass\r\n" +
                                          "    {\r\n" +
                                          "        public void SomeMethod()\r\n" +
                                          "        {\r\n" +
                                          "            string s1 = \"SomeMethod\";\r\n" +
                                          "            string s2 = \"SomeClass\";\r\n" +
                                          "            string s3 = \"SomeNamespace\";\r\n" +
                                          "        }\r\n" +
                                          "    }\r\n" +
                                          "}";
            String expectedOutput = outputLevel == OutputLevel.Info ? ExpectedOutputForInfoLevel : "";
            TransformerHelper transformerHelper = new TransformerHelper(source, "NameOfExpression", outputLevel);
            transformerHelper.Process(_transformerOnFactory, expectedOutput, expectedResult);
            transformerHelper.Process(_transformerOffFactory, "", source);
        }

        private readonly Func<IOutput, ITransformer> _transformerOnFactory = output => new NameOfExprTransformer(output, TransformerState.On);
        private readonly Func<IOutput, ITransformer> _transformerOffFactory = output => new NameOfExprTransformer(output, TransformerState.Off);

        private const String ExpectedOutputForInfoLevel = $"Execution of {NameOfExprTransformer.Name} started\r\n" +
                                                          $"Execution of {NameOfExprTransformer.Name} finished\r\n";
    }
}
