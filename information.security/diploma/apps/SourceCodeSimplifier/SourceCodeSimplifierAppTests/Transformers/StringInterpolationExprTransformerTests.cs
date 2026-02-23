using NUnit.Framework;
using SourceCodeSimplifierApp.Config;
using SourceCodeSimplifierApp.Output;
using SourceCodeSimplifierApp.Transformers;
using SourceCodeSimplifierAppTests.TestUtils;

namespace SourceCodeSimplifierAppTests.Transformers
{
    [TestFixture]
    public class StringInterpolationExprTransformerTests
    {
        [TestCase(OutputLevel.Error)]
        [TestCase(OutputLevel.Warning)]
        [TestCase(OutputLevel.Info)]
        public void ProcessWithStringInterpolationExpr(OutputLevel outputLevel)
        {
            const String source = "namespace SomeNamespace\r\n" +
                                  "{\r\n" +
                                  "    public class SomeClass\r\n" +
                                  "    {\r\n" +
                                  "        public void SomeMethod()\r\n" +
                                  "        {\r\n" +
                                  "            string name = \"Cacodemon\";\r\n" +
                                  "            System.DateTime date = System.DateTime.Now;\r\n" +
                                  "            string result = $\"Name = {name,20}, day = {date.DayOfWeek,-15}, date = {date,15:HH:mm}\";\r\n" +
                                  "        }\r\n" +
                                  "    }\r\n" +
                                  "}";
            const String expectedResult = "namespace SomeNamespace\r\n" +
                                          "{\r\n" +
                                          "    public class SomeClass\r\n" +
                                          "    {\r\n" +
                                          "        public void SomeMethod()\r\n" +
                                          "        {\r\n" +
                                          "            string name = \"Cacodemon\";\r\n" +
                                          "            System.DateTime date = System.DateTime.Now;\r\n" +
                                          "            string result = string.Format(\"Name = {0,20}, day = {1,-15}, date = {2,15:HH:mm}\", name, date.DayOfWeek, date);\r\n" +
                                          "        }\r\n" +
                                          "    }\r\n" +
                                          "}";
            String expectedOutput = outputLevel == OutputLevel.Info ? ExpectedOutputForInfoLevel : "";
            TransformerHelper transformerHelper = new TransformerHelper(source, "StringInterpolationExpr", outputLevel);
            transformerHelper.Process(_transformerOnFactory, expectedOutput, expectedResult);
            transformerHelper.Process(_transformerOffFactory, "", source);
        }

        [TestCase(OutputLevel.Error)]
        [TestCase(OutputLevel.Warning)]
        [TestCase(OutputLevel.Info)]
        public void ProcessWithVerbatimStringInterpolationExpr(OutputLevel outputLevel)
        {
            const String source = "namespace SomeNamespace\r\n" +
                                  "{\r\n" +
                                  "    public class SomeClass\r\n" +
                                  "    {\r\n" +
                                  "        public void SomeMethod()\r\n" +
                                  "        {\r\n" +
                                  "            string name = \"Cacodemon\";\r\n" +
                                  "            System.DateTime date = System.DateTime.Now;\r\n" +
                                  "            string result = @$\"Name = {name,20}, day = {date.DayOfWeek,-15}, date = {date,15:HH:mm}\";\r\n" +
                                  "        }\r\n" +
                                  "    }\r\n" +
                                  "}";
            const String expectedResult = "namespace SomeNamespace\r\n" +
                                          "{\r\n" +
                                          "    public class SomeClass\r\n" +
                                          "    {\r\n" +
                                          "        public void SomeMethod()\r\n" +
                                          "        {\r\n" +
                                          "            string name = \"Cacodemon\";\r\n" +
                                          "            System.DateTime date = System.DateTime.Now;\r\n" +
                                          "            string result = string.Format(@\"Name = {0,20}, day = {1,-15}, date = {2,15:HH:mm}\", name, date.DayOfWeek, date);\r\n" +
                                          "        }\r\n" +
                                          "    }\r\n" +
                                          "}";
            String expectedOutput = outputLevel == OutputLevel.Info ? ExpectedOutputForInfoLevel : "";
            TransformerHelper transformerHelper = new TransformerHelper(source, "StringInterpolationExpr", outputLevel);
            transformerHelper.Process(_transformerOnFactory, expectedOutput, expectedResult);
            transformerHelper.Process(_transformerOffFactory, "", source);
        }

        [TestCase(OutputLevel.Error)]
        [TestCase(OutputLevel.Warning)]
        [TestCase(OutputLevel.Info)]
        public void ProcessWithStringInterpolationExprWithCodeFormatting(OutputLevel outputLevel)
        {
            const String source = "namespace SomeNamespace\r\n" +
                                  "{\r\n" +
                                  "    public class SomeClass\r\n" +
                                  "    {\r\n" +
                                  "        public void SomeHandler(string one, string two)\r\n" +
                                  "        {\r\n" +
                                  "        }\r\n" +
                                  "        public void SomeMethod()\r\n" +
                                  "        {\r\n" +
                                  "            string s1 = \"IDDQD\";\r\n" +
                                  "            int i1 = 333;\r\n" +
                                  "            SomeHandler(\r\n" +
                                  "                       $\"iddqd is {s1}\",\r\n" +
                                  "                       $\"idkfa is {2 * i1}\"\r\n" +
                                  "                       );\r\n" +
                                  "        }\r\n" +
                                  "    }\r\n" +
                                  "}";
            const String expectedResult = "namespace SomeNamespace\r\n" +
                                          "{\r\n" +
                                          "    public class SomeClass\r\n" +
                                          "    {\r\n" +
                                          "        public void SomeHandler(string one, string two)\r\n" +
                                          "        {\r\n" +
                                          "        }\r\n" +
                                          "        public void SomeMethod()\r\n" +
                                          "        {\r\n" +
                                          "            string s1 = \"IDDQD\";\r\n" +
                                          "            int i1 = 333;\r\n" +
                                          "            SomeHandler(\r\n" +
                                          "                       string.Format(\"iddqd is {0}\", s1),\r\n" +
                                          "                       string.Format(\"idkfa is {0}\", 2 * i1)\r\n" +
                                          "                       );\r\n" +
                                          "        }\r\n" +
                                          "    }\r\n" +
                                          "}";
            String expectedOutput = outputLevel == OutputLevel.Info ? ExpectedOutputForInfoLevel : "";
            TransformerHelper transformerHelper = new TransformerHelper(source, "StringInterpolationExpr", outputLevel);
            transformerHelper.Process(_transformerOnFactory, expectedOutput, expectedResult);
            transformerHelper.Process(_transformerOffFactory, "", source);
        }

        [TestCase(OutputLevel.Error)]
        [TestCase(OutputLevel.Warning)]
        [TestCase(OutputLevel.Info)]
        public void ProcessWithStringFormatCall(OutputLevel outputLevel)
        {
            const String source = "namespace SomeNamespace\r\n" +
                                  "{\r\n" +
                                  "    public class SomeClass\r\n" +
                                  "    {\r\n" +
                                  "        public void SomeMethod()\r\n" +
                                  "        {\r\n" +
                                  "            string name = \"Cacodemon\";\r\n" +
                                  "            System.DateTime date = System.DateTime.Now;\r\n" +
                                  "            string result = string.Format(\"Name = {0,20}, day = {1,-15}, date = {2,15:HH:mm}\", name, date.DayOfWeek, date);\r\n" +
                                  "        }\r\n" +
                                  "    }\r\n" +
                                  "}";
            String expectedOutput = outputLevel == OutputLevel.Info ? ExpectedOutputForInfoLevel : "";
            TransformerHelper transformerHelper = new TransformerHelper(source, "StringInterpolationExpr", outputLevel);
            transformerHelper.Process(_transformerOnFactory, expectedOutput, source);
            transformerHelper.Process(_transformerOffFactory, "", source);
        }

        private readonly Func<IOutput, ITransformer> _transformerOnFactory = output => new StringInterpolationExprTransformer(output, TransformerState.On);
        private readonly Func<IOutput, ITransformer> _transformerOffFactory = output => new StringInterpolationExprTransformer(output, TransformerState.Off);

        private const String ExpectedOutputForInfoLevel = $"Execution of {StringInterpolationExprTransformer.Name} started\r\n" +
                                                          $"Execution of {StringInterpolationExprTransformer.Name} finished\r\n";
    }
}
