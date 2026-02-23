using NUnit.Framework;
using SourceCodeSimplifierApp.Config;
using SourceCodeSimplifierApp.Output;
using SourceCodeSimplifierApp.Transformers;
using SourceCodeSimplifierAppTests.TestUtils;

namespace SourceCodeSimplifierAppTests.Transformers
{
    [TestFixture]
    internal class ComplexTransformationCasesTests
    {
        public ComplexTransformationCasesTests()
        {
            TransformerEntry[] transformerOnEntries = new[]
            {
                new TransformerEntry {Name = NameOfExprTransformer.Name, State = TransformerState.On},
                new TransformerEntry {Name = StringInterpolationExprTransformer.Name, State = TransformerState.On},
                new TransformerEntry {Name = ObjectInitializerExprTransformer.Name, State = TransformerState.On},
                new TransformerEntry {Name = OutInlineVariableTransformer.Name, State = TransformerState.On}
            };
            TransformerEntry[] transformerOffEntries = new[]
            {
                new TransformerEntry {Name = NameOfExprTransformer.Name, State = TransformerState.Off},
                new TransformerEntry {Name = StringInterpolationExprTransformer.Name, State = TransformerState.Off},
                new TransformerEntry {Name = ObjectInitializerExprTransformer.Name, State = TransformerState.Off},
                new TransformerEntry {Name = OutInlineVariableTransformer.Name, State = TransformerState.Off}
            };
            _transformersOnFactory = output => TransformersFactory.Create(output, transformerOnEntries);
            _transformersOffFactory = output => TransformersFactory.Create(output, transformerOffEntries);
        }

        [TestCase(OutputLevel.Error)]
        [TestCase(OutputLevel.Warning)]
        [TestCase(OutputLevel.Info)]
        public void ProcessOutInlineVariablesInObjectInitializerExpr(OutputLevel outputLevel)
        {
            const String commonDefinitions = "    public class SomeInnerData\r\n" +
                                             "    {\r\n" +
                                             "        public SomeInnerData(int innerData1, string innerData2, out string innerValue)\r\n" +
                                             "        {\r\n" +
                                             "            InnerData1 = innerData1;\r\n" +
                                             "            InnerData2 = innerData2;\r\n" +
                                             "            innerValue = \"==\" + innerData2 + \"==\";\r\n" +
                                             "        }\r\n" +
                                             "        public int InnerData1;\r\n" +
                                             "        public string InnerData2;\r\n" +
                                             "    }\r\n" +
                                             "    public class SomeOuterData\r\n" +
                                             "    {\r\n" +
                                             "        public SomeOuterData(int outerData1, string outerData2, out string outerValue)\r\n" +
                                             "        {\r\n" +
                                             "            OuterData1 = outerData1;\r\n" +
                                             "            OuterData2 = outerData2;\r\n" +
                                             "            InnerData = new SomeInnerData(19, \"!!!\", out outerValue);\r\n" +
                                             "        }\r\n" +
                                             "        public int OuterData1;\r\n" +
                                             "        public string OuterData2;\r\n" +
                                             "        public SomeInnerData InnerData;\r\n" +
                                             "    }\r\n";
            const String source = "namespace SomeNamespace\r\n" +
                                  "{\r\n" +
                                  commonDefinitions +
                                  "    public class SomeClass\r\n" +
                                  "    {\r\n" +
                                  "        public int CreateNumber(int inValue, out int outValue)\r\n" +
                                  "        {\r\n" +
                                  "            outValue = inValue + 1;\r\n" +
                                  "            return 2 * inValue;\r\n" +
                                  "        }\r\n" +
                                  "        public string CreateString(string inValue, out string outValue)\r\n" +
                                  "        {\r\n" +
                                  "            outValue = \"--\" + inValue + \"--\";\r\n" +
                                  "            return \"==\" + inValue + \"==\";\r\n" +
                                  "        }\r\n" +
                                  "        public void SomeMethod()\r\n" +
                                  "        {\r\n" +
                                  "            SomeOuterData outerData = new SomeOuterData(666, \"IDDQD\", out string outerValue1)\r\n" +
                                  "            {\r\n" +
                                  "                OuterData1 = CreateNumber(13, out int outerValue2),\r\n" +
                                  "                OuterData2 = CreateString(\"IDKFA\", out string outerValue3),\r\n" +
                                  "                InnerData = new SomeInnerData(999, \"IDCLIP\", out string innerValue1)\r\n" +
                                  "                {\r\n" +
                                  "                    InnerData1 = CreateNumber(19, out int innerValue2),\r\n" +
                                  "                    InnerData2 = CreateString(\"IMPULSE 666\", out string innerValue3)\r\n" +
                                  "                }\r\n" +
                                  "            };\r\n" +
                                  "        }\r\n" +
                                  "    }\r\n" +
                                  "}\r\n";
            const String expectedResult = "namespace SomeNamespace\r\n" +
                                          "{\r\n" +
                                          commonDefinitions +
                                          "    public class SomeClass\r\n" +
                                          "    {\r\n" +
                                          "        public int CreateNumber(int inValue, out int outValue)\r\n" +
                                          "        {\r\n" +
                                          "            outValue = inValue + 1;\r\n" +
                                          "            return 2 * inValue;\r\n" +
                                          "        }\r\n" +
                                          "        public string CreateString(string inValue, out string outValue)\r\n" +
                                          "        {\r\n" +
                                          "            outValue = \"--\" + inValue + \"--\";\r\n" +
                                          "            return \"==\" + inValue + \"==\";\r\n" +
                                          "        }\r\n" +
                                          "        public void SomeMethod()\r\n" +
                                          "        {\r\n" +
                                          "            string outerValue1;\r\n" +
                                          "            SomeOuterData outerData = new SomeOuterData(666, \"IDDQD\", out outerValue1);\r\n" +
                                          "            int outerValue2;\r\n" +
                                          "            outerData.OuterData1 = CreateNumber(13, out outerValue2);\r\n" +
                                          "            string outerValue3;\r\n" +
                                          "            outerData.OuterData2 = CreateString(\"IDKFA\", out outerValue3);\r\n" +
                                          "            string innerValue1;\r\n" +
                                          "            outerData.InnerData = new SomeInnerData(999, \"IDCLIP\", out innerValue1);\r\n" +
                                          "            int innerValue2;\r\n" +
                                          "            outerData.InnerData.InnerData1 = CreateNumber(19, out innerValue2);\r\n" +
                                          "            string innerValue3;\r\n" +
                                          "            outerData.InnerData.InnerData2 = CreateString(\"IMPULSE 666\", out innerValue3);\r\n" +
                                          "        }\r\n" +
                                          "    }\r\n" +
                                          "}\r\n";
            String expectedOutput = outputLevel == OutputLevel.Info ? ExpectedOutputForInfoLevel : "";
            TransformerHelper transformerHelper = new TransformerHelper(source, "ComplexTransformationCases", outputLevel);
            transformerHelper.Process(_transformersOnFactory, expectedOutput, expectedResult);
            transformerHelper.Process(_transformersOffFactory, "", source);
        }

        [TestCase(OutputLevel.Error)]
        [TestCase(OutputLevel.Warning)]
        [TestCase(OutputLevel.Info)]
        public void ProcessNameOfExprInStringInterpolationExpr(OutputLevel outputLevel)
        {
            const String source = "namespace SomeNamespace\r\n" +
                                  "{\r\n" +
                                  "    public class SomeClass\r\n" +
                                  "    {\r\n" +
                                  "        public void SomeMethod()\r\n" +
                                  "        {\r\n" +
                                  "            string value = $\"i = {nameof(SomeClass)}, j = {nameof(SomeMethod)}, k = {nameof(SomeNamespace)}\";\r\n" +
                                  "        }\r\n" +
                                  "    }\r\n" +
                                  "}\r\n";
            const String expectedResult = "namespace SomeNamespace\r\n" +
                                          "{\r\n" +
                                          "    public class SomeClass\r\n" +
                                          "    {\r\n" +
                                          "        public void SomeMethod()\r\n" +
                                          "        {\r\n" +
                                          "            string value = string.Format(\"i = {0}, j = {1}, k = {2}\", \"SomeClass\", \"SomeMethod\", \"SomeNamespace\");\r\n" +
                                          "        }\r\n" +
                                          "    }\r\n" +
                                          "}\r\n";
            String expectedOutput = outputLevel == OutputLevel.Info ? ExpectedOutputForInfoLevel : "";
            TransformerHelper transformerHelper = new TransformerHelper(source, "ComplexTransformationCases", outputLevel);
            transformerHelper.Process(_transformersOnFactory, expectedOutput, expectedResult);
            transformerHelper.Process(_transformersOffFactory, "", source);
        }

        private readonly Func<IOutput, ITransformer[]> _transformersOnFactory;
        private readonly Func<IOutput, ITransformer[]> _transformersOffFactory;

        private const String ExpectedOutputForInfoLevel = $"Execution of {NameOfExprTransformer.Name} started\r\n" +
                                                          $"Execution of {NameOfExprTransformer.Name} finished\r\n" +
                                                          $"Execution of {StringInterpolationExprTransformer.Name} started\r\n" +
                                                          $"Execution of {StringInterpolationExprTransformer.Name} finished\r\n" +
                                                          $"Execution of {ObjectInitializerExprTransformer.Name} started\r\n" +
                                                          $"Execution of {ObjectInitializerExprTransformer.Name} finished\r\n" +
                                                          $"Execution of {OutInlineVariableTransformer.Name} started\r\n" +
                                                          $"Execution of {OutInlineVariableTransformer.Name} finished\r\n";
    }
}
