using NUnit.Framework;
using SourceCodeSimplifierApp.Config;
using SourceCodeSimplifierApp.Output;
using SourceCodeSimplifierApp.Transformers;
using SourceCodeSimplifierAppTests.TestUtils;

namespace SourceCodeSimplifierAppTests.Transformers
{
    [TestFixture]
    public class ForLoopTransformerTests
    {
        [TestCase(OutputLevel.Error)]
        [TestCase(OutputLevel.Warning)]
        [TestCase(OutputLevel.Info)]
        public void ProcessForLoopWithIncrement(OutputLevel outputLevel)
        {
            const String source = "namespace SomeNamespace\r\n" +
                                  "{\r\n" +
                                  "    public class SomeClass\r\n" +
                                  "    {\r\n" +
                                  "        public void SomeProcess(int index)\r\n" +
                                  "        {\r\n" +
                                  "        }\r\n" +
                                  "        public void SomeMethod()\r\n" +
                                  "        {\r\n" +
                                  "            for (int index = 0; index < 100; ++index)\r\n" +
                                  "            {\r\n" +
                                  "                SomeProcess(index);\r\n" +
                                  "            }\r\n" +
                                  "        }\r\n" +
                                  "    }\r\n" +
                                  "}";
            const String expectedResult = "namespace SomeNamespace\r\n" +
                                          "{\r\n" +
                                          "    public class SomeClass\r\n" +
                                          "    {\r\n" +
                                          "        public void SomeProcess(int index)\r\n" +
                                          "        {\r\n" +
                                          "        }\r\n" +
                                          "        public void SomeMethod()\r\n" +
                                          "        {\r\n" +
                                          "            int index = 0;\r\n" +
                                          "            while (index < 100)\r\n" +
                                          "            {\r\n" +
                                          "                SomeProcess(index);\r\n" +
                                          "                index += 1;\r\n" +
                                          "            }\r\n" +
                                          "        }\r\n" +
                                          "    }\r\n" +
                                          "}";
            String expectedOutput = outputLevel == OutputLevel.Info ? ExpectedOutputForInfoLevel : "";
            TransformerHelper transformerHelper = new TransformerHelper(source, "ForLoop", outputLevel);
            transformerHelper.Process(_transformerOnFactory, expectedOutput, expectedResult);
            transformerHelper.Process(_transformerOffFactory, "", source);
        }

        [TestCase(OutputLevel.Error)]
        [TestCase(OutputLevel.Warning)]
        [TestCase(OutputLevel.Info)]
        public void ProcessForLoopWithDecrement(OutputLevel outputLevel)
        {
            const String source = "namespace SomeNamespace\r\n" +
                                  "{\r\n" +
                                  "    public class SomeClass\r\n" +
                                  "    {\r\n" +
                                  "        public void SomeProcess(int index)\r\n" +
                                  "        {\r\n" +
                                  "        }\r\n" +
                                  "        public void SomeMethod()\r\n" +
                                  "        {\r\n" +
                                  "            for (int index = 100; index > 0; --index)\r\n" +
                                  "            {\r\n" +
                                  "                SomeProcess(index);\r\n" +
                                  "            }\r\n" +
                                  "        }\r\n" +
                                  "    }\r\n" +
                                  "}";
            const String expectedResult = "namespace SomeNamespace\r\n" +
                                          "{\r\n" +
                                          "    public class SomeClass\r\n" +
                                          "    {\r\n" +
                                          "        public void SomeProcess(int index)\r\n" +
                                          "        {\r\n" +
                                          "        }\r\n" +
                                          "        public void SomeMethod()\r\n" +
                                          "        {\r\n" +
                                          "            int index = 100;\r\n" +
                                          "            while (index > 0)\r\n" +
                                          "            {\r\n" +
                                          "                SomeProcess(index);\r\n" +
                                          "                index -= 1;\r\n" +
                                          "            }\r\n" +
                                          "        }\r\n" +
                                          "    }\r\n" +
                                          "}";
            String expectedOutput = outputLevel == OutputLevel.Info ? ExpectedOutputForInfoLevel : "";
            TransformerHelper transformerHelper = new TransformerHelper(source, "ForLoop", outputLevel);
            transformerHelper.Process(_transformerOnFactory, expectedOutput, expectedResult);
            transformerHelper.Process(_transformerOffFactory, "", source);
        }

        [TestCase(OutputLevel.Error)]
        [TestCase(OutputLevel.Warning)]
        [TestCase(OutputLevel.Info)]
        public void ProcessForLoopWithAddAssignment(OutputLevel outputLevel)
        {
            const String source = "namespace SomeNamespace\r\n" +
                                  "{\r\n" +
                                  "    public class SomeClass\r\n" +
                                  "    {\r\n" +
                                  "        public void SomeProcess(int index)\r\n" +
                                  "        {\r\n" +
                                  "        }\r\n" +
                                  "        public void SomeMethod()\r\n" +
                                  "        {\r\n" +
                                  "            for (int index = 0; index < 100; index += 2)\r\n" +
                                  "            {\r\n" +
                                  "                SomeProcess(index);\r\n" +
                                  "            }\r\n" +
                                  "        }\r\n" +
                                  "    }\r\n" +
                                  "}";
            const String expectedResult = "namespace SomeNamespace\r\n" +
                                          "{\r\n" +
                                          "    public class SomeClass\r\n" +
                                          "    {\r\n" +
                                          "        public void SomeProcess(int index)\r\n" +
                                          "        {\r\n" +
                                          "        }\r\n" +
                                          "        public void SomeMethod()\r\n" +
                                          "        {\r\n" +
                                          "            int index = 0;\r\n" +
                                          "            while (index < 100)\r\n" +
                                          "            {\r\n" +
                                          "                SomeProcess(index);\r\n" +
                                          "                index += 2;\r\n" +
                                          "            }\r\n" +
                                          "        }\r\n" +
                                          "    }\r\n" +
                                          "}";
            String expectedOutput = outputLevel == OutputLevel.Info ? ExpectedOutputForInfoLevel : "";
            TransformerHelper transformerHelper = new TransformerHelper(source, "ForLoop", outputLevel);
            transformerHelper.Process(_transformerOnFactory, expectedOutput, expectedResult);
            transformerHelper.Process(_transformerOffFactory, "", source);
        }

        [TestCase(OutputLevel.Error)]
        [TestCase(OutputLevel.Warning)]
        [TestCase(OutputLevel.Info)]
        public void ProcessForLoopWithNonBlockBody(OutputLevel outputLevel)
        {
            const String source = "namespace SomeNamespace\r\n" +
                                  "{\r\n" +
                                  "    public class SomeClass\r\n" +
                                  "    {\r\n" +
                                  "        public void SomeProcess(int index)\r\n" +
                                  "        {\r\n" +
                                  "        }\r\n" +
                                  "        public void SomeMethod()\r\n" +
                                  "        {\r\n" +
                                  "            for (int index = 0; index < 100; index += 2)\r\n" +
                                  "                SomeProcess(index);\r\n" +
                                  "        }\r\n" +
                                  "    }\r\n" +
                                  "}";
            const String expectedResult = "namespace SomeNamespace\r\n" +
                                          "{\r\n" +
                                          "    public class SomeClass\r\n" +
                                          "    {\r\n" +
                                          "        public void SomeProcess(int index)\r\n" +
                                          "        {\r\n" +
                                          "        }\r\n" +
                                          "        public void SomeMethod()\r\n" +
                                          "        {\r\n" +
                                          "            int index = 0;\r\n" +
                                          "            while (index < 100)\r\n" +
                                          "            {\r\n" +
                                          "                SomeProcess(index);\r\n" +
                                          "                index += 2;\r\n" +
                                          "            }\r\n" +
                                          "        }\r\n" +
                                          "    }\r\n" +
                                          "}";
            String expectedOutput = outputLevel == OutputLevel.Info ? ExpectedOutputForInfoLevel : "";
            TransformerHelper transformerHelper = new TransformerHelper(source, "ForLoop", outputLevel);
            transformerHelper.Process(_transformerOnFactory, expectedOutput, expectedResult);
            transformerHelper.Process(_transformerOffFactory, "", source);
        }

        [TestCase(OutputLevel.Error)]
        [TestCase(OutputLevel.Warning)]
        [TestCase(OutputLevel.Info)]
        public void ProcessForLoopWithNonBlockParent(OutputLevel outputLevel)
        {
            const String source = "namespace SomeNamespace\r\n" +
                                  "{\r\n" +
                                  "    public class SomeClass\r\n" +
                                  "    {\r\n" +
                                  "        public void SomeProcess(int index)\r\n" +
                                  "        {\r\n" +
                                  "        }\r\n" +
                                  "        public void SomeMethod(bool value)\r\n" +
                                  "        {\r\n" +
                                  "            if (value)\r\n" +
                                  "                for (int index = 0; index < 100; ++index)\r\n" +
                                  "                {\r\n" +
                                  "                    SomeProcess(index);\r\n" +
                                  "                }\r\n" +
                                  "        }\r\n" +
                                  "    }\r\n" +
                                  "}";
            const String expectedResult = "namespace SomeNamespace\r\n" +
                                          "{\r\n" +
                                          "    public class SomeClass\r\n" +
                                          "    {\r\n" +
                                          "        public void SomeProcess(int index)\r\n" +
                                          "        {\r\n" +
                                          "        }\r\n" +
                                          "        public void SomeMethod(bool value)\r\n" +
                                          "        {\r\n" +
                                          "            if (value)\r\n" +
                                          "            {\r\n" +
                                          "                int index = 0;\r\n" +
                                          "                while (index < 100)\r\n" +
                                          "                {\r\n" +
                                          "                    SomeProcess(index);\r\n" +
                                          "                    index += 1;\r\n" +
                                          "                }\r\n" +
                                          "            }\r\n" +
                                          "        }\r\n" +
                                          "    }\r\n" +
                                          "}";
            String expectedOutput = outputLevel == OutputLevel.Info ? ExpectedOutputForInfoLevel : "";
            TransformerHelper transformerHelper = new TransformerHelper(source, "ForLoop", outputLevel);
            transformerHelper.Process(_transformerOnFactory, expectedOutput, expectedResult);
            transformerHelper.Process(_transformerOffFactory, "", source);
        }

        [TestCase(OutputLevel.Error)]
        [TestCase(OutputLevel.Warning)]
        [TestCase(OutputLevel.Info)]
        public void ProcessForLoopWithoutDeclarationSection(OutputLevel outputLevel)
        {
            const String source = "namespace SomeNamespace\r\n" +
                                  "{\r\n" +
                                  "    public class SomeClass\r\n" +
                                  "    {\r\n" +
                                  "        public void SomeProcess(int index)\r\n" +
                                  "        {\r\n" +
                                  "        }\r\n" +
                                  "        public void SomeMethod()\r\n" +
                                  "        {\r\n" +
                                  "            int index = 0;\r\n" +
                                  "            for (; index < 100; ++index)\r\n" +
                                  "            {\r\n" +
                                  "                SomeProcess(index);\r\n" +
                                  "            }\r\n" +
                                  "        }\r\n" +
                                  "    }\r\n" +
                                  "}";
            const String expectedResult = "namespace SomeNamespace\r\n" +
                                          "{\r\n" +
                                          "    public class SomeClass\r\n" +
                                          "    {\r\n" +
                                          "        public void SomeProcess(int index)\r\n" +
                                          "        {\r\n" +
                                          "        }\r\n" +
                                          "        public void SomeMethod()\r\n" +
                                          "        {\r\n" +
                                          "            int index = 0;\r\n" +
                                          "            while (index < 100)\r\n" +
                                          "            {\r\n" +
                                          "                SomeProcess(index);\r\n" +
                                          "                index += 1;\r\n" +
                                          "            }\r\n" +
                                          "        }\r\n" +
                                          "    }\r\n" +
                                          "}";
            String expectedOutput = outputLevel == OutputLevel.Info ? ExpectedOutputForInfoLevel : "";
            TransformerHelper transformerHelper = new TransformerHelper(source, "ForLoop", outputLevel);
            transformerHelper.Process(_transformerOnFactory, expectedOutput, expectedResult);
            transformerHelper.Process(_transformerOffFactory, "", source);
        }

        [TestCase(OutputLevel.Error)]
        [TestCase(OutputLevel.Warning)]
        [TestCase(OutputLevel.Info)]
        public void ProcessForLoopWithoutConditionSection(OutputLevel outputLevel)
        {
            const String source = "namespace SomeNamespace\r\n" +
                                  "{\r\n" +
                                  "    public class SomeClass\r\n" +
                                  "    {\r\n" +
                                  "        public void SomeProcess(int index)\r\n" +
                                  "        {\r\n" +
                                  "        }\r\n" +
                                  "        public void SomeMethod()\r\n" +
                                  "        {\r\n" +
                                  "            for (int index = 0; ; ++index)\r\n" +
                                  "            {\r\n" +
                                  "                if (index >= 100)\r\n" +
                                  "                    break;\r\n" +
                                  "                SomeProcess(index);\r\n" +
                                  "            }\r\n" +
                                  "        }\r\n" +
                                  "    }\r\n" +
                                  "}";
            const String expectedResult = "namespace SomeNamespace\r\n" +
                                          "{\r\n" +
                                          "    public class SomeClass\r\n" +
                                          "    {\r\n" +
                                          "        public void SomeProcess(int index)\r\n" +
                                          "        {\r\n" +
                                          "        }\r\n" +
                                          "        public void SomeMethod()\r\n" +
                                          "        {\r\n" +
                                          "            int index = 0;\r\n" +
                                          "            while (true)\r\n" +
                                          "            {\r\n" +
                                          "                if (index >= 100)\r\n" +
                                          "                    break;\r\n" +
                                          "                SomeProcess(index);\r\n" +
                                          "                index += 1;\r\n" +
                                          "            }\r\n" +
                                          "        }\r\n" +
                                          "    }\r\n" +
                                          "}";
            String expectedOutput = outputLevel == OutputLevel.Info ? ExpectedOutputForInfoLevel : "";
            TransformerHelper transformerHelper = new TransformerHelper(source, "ForLoop", outputLevel);
            transformerHelper.Process(_transformerOnFactory, expectedOutput, expectedResult);
            transformerHelper.Process(_transformerOffFactory, "", source);
        }

        [TestCase(OutputLevel.Error)]
        [TestCase(OutputLevel.Warning)]
        [TestCase(OutputLevel.Info)]
        public void ProcessForLoopWithoutIncrementSection(OutputLevel outputLevel)
        {
            const String source = "namespace SomeNamespace\r\n" +
                                  "{\r\n" +
                                  "    public class SomeClass\r\n" +
                                  "    {\r\n" +
                                  "        public void SomeProcess(int index)\r\n" +
                                  "        {\r\n" +
                                  "        }\r\n" +
                                  "        public void SomeMethod()\r\n" +
                                  "        {\r\n" +
                                  "            for (int index = 0; index < 100; )\r\n" +
                                  "            {\r\n" +
                                  "                SomeProcess(index);\r\n" +
                                  "                index += 2;\r\n" +
                                  "            }\r\n" +
                                  "        }\r\n" +
                                  "    }\r\n" +
                                  "}";
            const String expectedResult = "namespace SomeNamespace\r\n" +
                                          "{\r\n" +
                                          "    public class SomeClass\r\n" +
                                          "    {\r\n" +
                                          "        public void SomeProcess(int index)\r\n" +
                                          "        {\r\n" +
                                          "        }\r\n" +
                                          "        public void SomeMethod()\r\n" +
                                          "        {\r\n" +
                                          "            int index = 0;\r\n" +
                                          "            while (index < 100)\r\n" +
                                          "            {\r\n" +
                                          "                SomeProcess(index);\r\n" +
                                          "                index += 2;\r\n" +
                                          "            }\r\n" +
                                          "        }\r\n" +
                                          "    }\r\n" +
                                          "}";
            String expectedOutput = outputLevel == OutputLevel.Info ? ExpectedOutputForInfoLevel : "";
            TransformerHelper transformerHelper = new TransformerHelper(source, "ForLoop", outputLevel);
            transformerHelper.Process(_transformerOnFactory, expectedOutput, expectedResult);
            transformerHelper.Process(_transformerOffFactory, "", source);
        }

        [TestCase(OutputLevel.Error)]
        [TestCase(OutputLevel.Warning)]
        [TestCase(OutputLevel.Info)]
        public void ProcessSeveralForLoops(OutputLevel outputLevel)
        {
            const String source = "namespace SomeNamespace\r\n" +
                                  "{\r\n" +
                                  "    public class SomeClass\r\n" +
                                  "    {\r\n" +
                                  "        public void SomeProcess(int index)\r\n" +
                                  "        {\r\n" +
                                  "        }\r\n" +
                                  "        public void SomeMethod()\r\n" +
                                  "        {\r\n" +
                                  "            for (int index1 = 0; index1 < 100; ++index1)\r\n" +
                                  "            {\r\n" +
                                  "                SomeProcess(index1);\r\n" +
                                  "            }\r\n" +
                                  "            for (int index2 = 0; index2 < 150; ++index2)\r\n" +
                                  "            {\r\n" +
                                  "                SomeProcess(index2);\r\n" +
                                  "            }\r\n" +
                                  "        }\r\n" +
                                  "    }\r\n" +
                                  "}";
            const String expectedResult = "namespace SomeNamespace\r\n" +
                                          "{\r\n" +
                                          "    public class SomeClass\r\n" +
                                          "    {\r\n" +
                                          "        public void SomeProcess(int index)\r\n" +
                                          "        {\r\n" +
                                          "        }\r\n" +
                                          "        public void SomeMethod()\r\n" +
                                          "        {\r\n" +
                                          "            int index1 = 0;\r\n" +
                                          "            while (index1 < 100)\r\n" +
                                          "            {\r\n" +
                                          "                SomeProcess(index1);\r\n" +
                                          "                index1 += 1;\r\n" +
                                          "            }\r\n" +
                                          "            int index2 = 0;\r\n" +
                                          "            while (index2 < 150)\r\n" +
                                          "            {\r\n" +
                                          "                SomeProcess(index2);\r\n" +
                                          "                index2 += 1;\r\n" +
                                          "            }\r\n" +
                                          "        }\r\n" +
                                          "    }\r\n" +
                                          "}";
            String expectedOutput = outputLevel == OutputLevel.Info ? ExpectedOutputForInfoLevel : "";
            TransformerHelper transformerHelper = new TransformerHelper(source, "ForLoop", outputLevel);
            transformerHelper.Process(_transformerOnFactory, expectedOutput, expectedResult);
            transformerHelper.Process(_transformerOffFactory, "", source);
        }

        [TestCase(OutputLevel.Error)]
        [TestCase(OutputLevel.Warning)]
        [TestCase(OutputLevel.Info)]
        public void ProcessNestedForLoops(OutputLevel outputLevel)
        {
            const String source = "namespace SomeNamespace\r\n" +
                                  "{\r\n" +
                                  "    public class SomeClass\r\n" +
                                  "    {\r\n" +
                                  "        public void SomeProcess(int index)\r\n" +
                                  "        {\r\n" +
                                  "        }\r\n" +
                                  "        public void SomeMethod()\r\n" +
                                  "        {\r\n" +
                                  "            for (int index1 = 0; index1 < 100; ++index1)\r\n" +
                                  "            {\r\n" +
                                  "                SomeProcess(index1);\r\n" +
                                  "                for (int index2 = 0; index2 < 150; ++index2)\r\n" +
                                  "                {\r\n" +
                                  "                    SomeProcess(index1 + index2);\r\n" +
                                  "                }\r\n" +
                                  "            }\r\n" +
                                  "        }\r\n" +
                                  "    }\r\n" +
                                  "}";
            const String expectedResult = "namespace SomeNamespace\r\n" +
                                          "{\r\n" +
                                          "    public class SomeClass\r\n" +
                                          "    {\r\n" +
                                          "        public void SomeProcess(int index)\r\n" +
                                          "        {\r\n" +
                                          "        }\r\n" +
                                          "        public void SomeMethod()\r\n" +
                                          "        {\r\n" +
                                          "            int index1 = 0;\r\n" +
                                          "            while (index1 < 100)\r\n" +
                                          "            {\r\n" +
                                          "                SomeProcess(index1);\r\n" +
                                          "                int index2 = 0;\r\n" +
                                          "                while (index2 < 150)\r\n" +
                                          "                {\r\n" +
                                          "                    SomeProcess(index1 + index2);\r\n" +
                                          "                    index2 += 1;\r\n" +
                                          "                }\r\n" +
                                          "                index1 += 1;\r\n" +
                                          "            }\r\n" +
                                          "        }\r\n" +
                                          "    }\r\n" +
                                          "}";
            String expectedOutput = outputLevel == OutputLevel.Info ? ExpectedOutputForInfoLevel : "";
            TransformerHelper transformerHelper = new TransformerHelper(source, "ForLoop", outputLevel);
            transformerHelper.Process(_transformerOnFactory, expectedOutput, expectedResult);
            transformerHelper.Process(_transformerOffFactory, "", source);
        }

        [TestCase(OutputLevel.Error)]
        [TestCase(OutputLevel.Warning)]
        [TestCase(OutputLevel.Info)]
        public void ProcessNestedForLoopsWithNonBlockBody(OutputLevel outputLevel)
        {
            const String source = "namespace SomeNamespace\r\n" +
                                  "{\r\n" +
                                  "    public class SomeClass\r\n" +
                                  "    {\r\n" +
                                  "        public void SomeProcess(int index)\r\n" +
                                  "        {\r\n" +
                                  "        }\r\n" +
                                  "        public void SomeMethod(bool value)\r\n" +
                                  "        {\r\n" +
                                  "            if (value)\r\n" +
                                  "                for (int index1 = 0; index1 < 100; ++index1)\r\n" +
                                  "                    for (int index2 = 0; index2 < 150; ++index2)\r\n" +
                                  "                        SomeProcess(index1 + index2);\r\n" +
                                  "        }\r\n" +
                                  "    }\r\n" +
                                  "}";
            const String expectedResult = "namespace SomeNamespace\r\n" +
                                          "{\r\n" +
                                          "    public class SomeClass\r\n" +
                                          "    {\r\n" +
                                          "        public void SomeProcess(int index)\r\n" +
                                          "        {\r\n" +
                                          "        }\r\n" +
                                          "        public void SomeMethod(bool value)\r\n" +
                                          "        {\r\n" +
                                          "            if (value)\r\n" +
                                          "            {\r\n" +
                                          "                int index1 = 0;\r\n" +
                                          "                while (index1 < 100)\r\n" +
                                          "                {\r\n" +
                                          "                    int index2 = 0;\r\n" +
                                          "                    while (index2 < 150)\r\n" +
                                          "                    {\r\n" +
                                          "                        SomeProcess(index1 + index2);\r\n" +
                                          "                        index2 += 1;\r\n" +
                                          "                    }\r\n" +
                                          "                    index1 += 1;\r\n" +
                                          "                }\r\n" +
                                          "            }\r\n" +
                                          "        }\r\n" +
                                          "    }\r\n" +
                                          "}";
            String expectedOutput = outputLevel == OutputLevel.Info ? ExpectedOutputForInfoLevel : "";
            TransformerHelper transformerHelper = new TransformerHelper(source, "ForLoop", outputLevel);
            transformerHelper.Process(_transformerOnFactory, expectedOutput, expectedResult);
            transformerHelper.Process(_transformerOffFactory, "", source);
        }

        private readonly Func<IOutput, ITransformer> _transformerOnFactory = output => new ForLoopTransformer(output, TransformerState.On);
        private readonly Func<IOutput, ITransformer> _transformerOffFactory = output => new ForLoopTransformer(output, TransformerState.Off);

        private const String ExpectedOutputForInfoLevel = $"Execution of {ForLoopTransformer.Name} started\r\n" +
                                                          $"Execution of {ForLoopTransformer.Name} finished\r\n";
    }
}
