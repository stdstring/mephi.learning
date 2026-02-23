using NUnit.Framework;
using SourceCodeSimplifierApp.Config;
using SourceCodeSimplifierApp.Output;
using SourceCodeSimplifierApp.Transformers;
using SourceCodeSimplifierAppTests.TestUtils;

namespace SourceCodeSimplifierAppTests.Transformers
{
    [TestFixture]
    public class OutInlineVariableTransformerTests
    {
        [TestCase(OutputLevel.Error)]
        [TestCase(OutputLevel.Warning)]
        [TestCase(OutputLevel.Info)]
        public void ProcessWithOutInlineVariable(OutputLevel outputLevel)
        {
            const String source = "namespace SomeNamespace\r\n" +
                                  "{\r\n" +
                                  "    public class SomeClass\r\n" +
                                  "    {\r\n" +
                                  "        public void OtherMethod(int p1, out int p2, out string p3, string p4)\r\n" +
                                  "        {\r\n" +
                                  "            p2 = p1 + 666;\r\n" +
                                  "            p3 = \"IDDQD\" + p4;\r\n" +
                                  "        }\r\n" +
                                  "        public void SomeMethod()\r\n" +
                                  "        {\r\n" +
                                  "            string p4 = \"IDKFA\";\r\n" +
                                  "            OtherMethod(13, out int p2, out string p3, p4);\r\n" +
                                  "        }\r\n" +
                                  "    }\r\n" +
                                  "}";
            const String expectedResult = "namespace SomeNamespace\r\n" +
                                          "{\r\n" +
                                          "    public class SomeClass\r\n" +
                                          "    {\r\n" +
                                          "        public void OtherMethod(int p1, out int p2, out string p3, string p4)\r\n" +
                                          "        {\r\n" +
                                          "            p2 = p1 + 666;\r\n" +
                                          "            p3 = \"IDDQD\" + p4;\r\n" +
                                          "        }\r\n" +
                                          "        public void SomeMethod()\r\n" +
                                          "        {\r\n" +
                                          "            string p4 = \"IDKFA\";\r\n" +
                                          "            int p2;\r\n" +
                                          "            string p3;\r\n" +
                                          "            OtherMethod(13, out p2, out p3, p4);\r\n" +
                                          "        }\r\n" +
                                          "    }\r\n" +
                                          "}";
            String expectedOutput = outputLevel == OutputLevel.Info ? ExpectedOutputForInfoLevel : "";
            TransformerHelper transformerHelper = new TransformerHelper(source, "OutInlineVariable", outputLevel);
            transformerHelper.Process(_transformerOnFactory, expectedOutput, expectedResult);
            transformerHelper.Process(_transformerOffFactory, "", source);
        }

        [TestCase(OutputLevel.Error)]
        [TestCase(OutputLevel.Warning)]
        [TestCase(OutputLevel.Info)]
        public void ProcessWithNestedOutInlineVariable(OutputLevel outputLevel)
        {
            const String source = "namespace SomeNamespace\r\n" +
                                  "{\r\n" +
                                  "    public class SomeClass\r\n" +
                                  "    {\r\n" +
                                  "        public void OtherMethod(int p1, out int p2, out string p3, string p4)\r\n" +
                                  "        {\r\n" +
                                  "            p2 = p1 + 666;\r\n" +
                                  "            p3 = \"IDDQD\" + p4;\r\n" +
                                  "        }\r\n" +
                                  "        public string AnotherMethod(int p1, out int p2, string p3)\r\n" +
                                  "        {\r\n" +
                                  "            p2 = p1 * 13 + 767;\r\n" +
                                  "            return \"==\" + p3 + \"==\";\r\n" +
                                  "        }\r\n" +
                                  "        public void SomeMethod()\r\n" +
                                  "        {\r\n" +
                                  "            string p4 = \"IDKFA\";\r\n" +
                                  "            OtherMethod(13, out int otherP2, out string otherP3, AnotherMethod(999, out int anotherP2, p4));\r\n" +
                                  "        }\r\n" +
                                  "    }\r\n" +
                                  "}";
            const String expectedResult = "namespace SomeNamespace\r\n" +
                                          "{\r\n" +
                                          "    public class SomeClass\r\n" +
                                          "    {\r\n" +
                                          "        public void OtherMethod(int p1, out int p2, out string p3, string p4)\r\n" +
                                          "        {\r\n" +
                                          "            p2 = p1 + 666;\r\n" +
                                          "            p3 = \"IDDQD\" + p4;\r\n" +
                                          "        }\r\n" +
                                          "        public string AnotherMethod(int p1, out int p2, string p3)\r\n" +
                                          "        {\r\n" +
                                          "            p2 = p1 * 13 + 767;\r\n" +
                                          "            return \"==\" + p3 + \"==\";\r\n" +
                                          "        }\r\n" +
                                          "        public void SomeMethod()\r\n" +
                                          "        {\r\n" +
                                          "            string p4 = \"IDKFA\";\r\n" +
                                          "            int otherP2;\r\n" +
                                          "            string otherP3;\r\n" +
                                          "            int anotherP2;\r\n" +
                                          "            OtherMethod(13, out otherP2, out otherP3, AnotherMethod(999, out anotherP2, p4));\r\n" +
                                          "        }\r\n" +
                                          "    }\r\n" +
                                          "}";
            String expectedOutput = outputLevel == OutputLevel.Info ? ExpectedOutputForInfoLevel : "";
            TransformerHelper transformerHelper = new TransformerHelper(source, "OutInlineVariable", outputLevel);
            transformerHelper.Process(_transformerOnFactory, expectedOutput, expectedResult);
            transformerHelper.Process(_transformerOffFactory, "", source);
        }

        [TestCase(OutputLevel.Error)]
        [TestCase(OutputLevel.Warning)]
        [TestCase(OutputLevel.Info)]
        public void ProcessWithoutOutInlineVariable(OutputLevel outputLevel)
        {
            const String source = "namespace SomeNamespace\r\n" +
                                  "{\r\n" +
                                  "    public class SomeClass\r\n" +
                                  "    {\r\n" +
                                  "        public void OtherMethod(int p1, out int p2, out string p3, string p4)\r\n" +
                                  "        {\r\n" +
                                  "            p2 = p1 + 666;\r\n" +
                                  "            p3 = \"IDDQD\" + p4;\r\n" +
                                  "        }\r\n" +
                                  "        public void SomeMethod()\r\n" +
                                  "        {\r\n" +
                                  "            int p2;\r\n" +
                                  "            string p3;\r\n" +
                                  "            string p4 = \"IDKFA\";\r\n" +
                                  "            OtherMethod(13, out p2, out p3, p4);\r\n" +
                                  "        }\r\n" +
                                  "    }\r\n" +
                                  "}";
            const String expectedResult = "namespace SomeNamespace\r\n" +
                                          "{\r\n" +
                                          "    public class SomeClass\r\n" +
                                          "    {\r\n" +
                                          "        public void OtherMethod(int p1, out int p2, out string p3, string p4)\r\n" +
                                          "        {\r\n" +
                                          "            p2 = p1 + 666;\r\n" +
                                          "            p3 = \"IDDQD\" + p4;\r\n" +
                                          "        }\r\n" +
                                          "        public void SomeMethod()\r\n" +
                                          "        {\r\n" +
                                          "            int p2;\r\n" +
                                          "            string p3;\r\n" +
                                          "            string p4 = \"IDKFA\";\r\n" +
                                          "            OtherMethod(13, out p2, out p3, p4);\r\n" +
                                          "        }\r\n" +
                                          "    }\r\n" +
                                          "}";
            String expectedOutput = outputLevel == OutputLevel.Info ? ExpectedOutputForInfoLevel : "";
            TransformerHelper transformerHelper = new TransformerHelper(source, "OutInlineVariable", outputLevel);
            transformerHelper.Process(_transformerOnFactory, expectedOutput, expectedResult);
            transformerHelper.Process(_transformerOffFactory, "", source);
        }

        private readonly Func<IOutput, ITransformer> _transformerOnFactory = output => new OutInlineVariableTransformer(output, TransformerState.On);
        private readonly Func<IOutput, ITransformer> _transformerOffFactory = output => new OutInlineVariableTransformer(output, TransformerState.Off);

        private const String ExpectedOutputForInfoLevel = $"Execution of {OutInlineVariableTransformer.Name} started\r\n" +
                                                          $"Execution of {OutInlineVariableTransformer.Name} finished\r\n";
    }
}
