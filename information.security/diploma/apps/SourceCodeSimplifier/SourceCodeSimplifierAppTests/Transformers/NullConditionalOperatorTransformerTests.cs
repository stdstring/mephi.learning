using NUnit.Framework;
using SourceCodeSimplifierApp.Config;
using SourceCodeSimplifierApp.Output;
using SourceCodeSimplifierApp.Transformers;
using SourceCodeSimplifierAppTests.TestUtils;

namespace SourceCodeSimplifierAppTests.Transformers
{
    [TestFixture]
    public class NullConditionalOperatorTransformerTests
    {
        [TestCase(OutputLevel.Error)]
        [TestCase(OutputLevel.Warning)]
        [TestCase(OutputLevel.Info)]
        public void ProcessLocalVariableDeclaration(OutputLevel outputLevel)
        {
            const String source = "namespace SomeNamespace\r\n" +
                                  "{\r\n" +
                                  CommonDefinitions +
                                  "    public class SomeClass\r\n" +
                                  "    {\r\n" +
                                  "        public void SomeMethod()\r\n" +
                                  "        {\r\n" +
                                  "            AnotherData anotherData = new AnotherData();\r\n" +
                                  "            string value = anotherData.CreateOtherData()?.CreateSomeData()?.CreateStr();\r\n" +
                                  "            string value2 = anotherData.CreateSeveralOtherData()[2]?.CreateSomeData()?.CreateStr();\r\n" +
                                  "            string value3 = anotherData.Process().CreateOtherData()?.CreateSomeData()?.CreateStr();\r\n" +
                                  "            string value4 = anotherData.CreateOtherData()?.Process().CreateSomeData()?.CreateStr();\r\n" +
                                  "            string value5 = anotherData.Process().CreateOtherData()?.Process().CreateSomeData()?.CreateStr();\r\n" +
                                  "        }\r\n" +
                                  "    }\r\n" +
                                  "}";
            const String expectedResult = "namespace SomeNamespace\r\n" +
                                          "{\r\n" +
                                          CommonDefinitions +
                                          "    public class SomeClass\r\n" +
                                          "    {\r\n" +
                                          "        public void SomeMethod()\r\n" +
                                          "        {\r\n" +
                                          "            AnotherData anotherData = new AnotherData();\r\n" +
                                          "            string value = default(string);\r\n" +
                                          "            SomeNamespace.OtherData condExpression = anotherData.CreateOtherData();\r\n" +
                                          "            if (condExpression != null)\r\n" +
                                          "            {\r\n" +
                                          "                SomeNamespace.SomeData condExpression2 = condExpression.CreateSomeData();\r\n" +
                                          "                if (condExpression2 != null)\r\n" +
                                          "                {\r\n" +
                                          "                    value = condExpression2.CreateStr();\r\n" +
                                          "                }\r\n" +
                                          "            }\r\n" +
                                          "            string value2 = default(string);\r\n" +
                                          "            SomeNamespace.OtherData condExpression3 = anotherData.CreateSeveralOtherData()[2];\r\n" +
                                          "            if (condExpression3 != null)\r\n" +
                                          "            {\r\n" +
                                          "                SomeNamespace.SomeData condExpression4 = condExpression3.CreateSomeData();\r\n" +
                                          "                if (condExpression4 != null)\r\n" +
                                          "                {\r\n" +
                                          "                    value2 = condExpression4.CreateStr();\r\n" +
                                          "                }\r\n" +
                                          "            }\r\n" +
                                          "            string value3 = default(string);\r\n" +
                                          "            SomeNamespace.OtherData condExpression5 = anotherData.Process().CreateOtherData();\r\n" +
                                          "            if (condExpression5 != null)\r\n" +
                                          "            {\r\n" +
                                          "                SomeNamespace.SomeData condExpression6 = condExpression5.CreateSomeData();\r\n" +
                                          "                if (condExpression6 != null)\r\n" +
                                          "                {\r\n" +
                                          "                    value3 = condExpression6.CreateStr();\r\n" +
                                          "                }\r\n" +
                                          "            }\r\n" +
                                          "            string value4 = default(string);\r\n" +
                                          "            SomeNamespace.OtherData condExpression7 = anotherData.CreateOtherData();\r\n" +
                                          "            if (condExpression7 != null)\r\n" +
                                          "            {\r\n" +
                                          "                SomeNamespace.SomeData condExpression8 = condExpression7.Process().CreateSomeData();\r\n" +
                                          "                if (condExpression8 != null)\r\n" +
                                          "                {\r\n" +
                                          "                    value4 = condExpression8.CreateStr();\r\n" +
                                          "                }\r\n" +
                                          "            }\r\n" +
                                          "            string value5 = default(string);\r\n" +
                                          "            SomeNamespace.OtherData condExpression9 = anotherData.Process().CreateOtherData();\r\n" +
                                          "            if (condExpression9 != null)\r\n" +
                                          "            {\r\n" +
                                          "                SomeNamespace.SomeData condExpression10 = condExpression9.Process().CreateSomeData();\r\n" +
                                          "                if (condExpression10 != null)\r\n" +
                                          "                {\r\n" +
                                          "                    value5 = condExpression10.CreateStr();\r\n" +
                                          "                }\r\n" +
                                          "            }\r\n" +
                                          "        }\r\n" +
                                          "    }\r\n" +
                                          "}";
            String expectedOutput = "";
            if (outputLevel == OutputLevel.Info)
                expectedOutput = String.Format(ExpectedOutputForInfoLevelTemplate, CreateOnePassOutputs(5));
            TransformerHelper transformerHelper = new TransformerHelper(source, "NullConditionalOperator", outputLevel);
            transformerHelper.Process(_transformerOnFactory, expectedOutput, expectedResult);
            transformerHelper.Process(_transformerOffFactory, "", source);
        }

        [TestCase(OutputLevel.Error)]
        [TestCase(OutputLevel.Warning)]
        [TestCase(OutputLevel.Info)]
        public void ProcessVariableAssignment(OutputLevel outputLevel)
        {
            const String source = "namespace SomeNamespace\r\n" +
                                  "{\r\n" +
                                  CommonDefinitions +
                                  "    public class SomeClass\r\n" +
                                  "    {\r\n" +
                                  "        public void SomeMethod()\r\n" +
                                  "        {\r\n" +
                                  "            AnotherData anotherData = new AnotherData();\r\n" +
                                  "            string value = \"\";\r\n" +
                                  "            string value2 = \"\";\r\n" +
                                  "            string value3 = \"\";\r\n" +
                                  "            string value4 = \"\";\r\n" +
                                  "            string value5 = \"\";\r\n" +
                                  "            value = anotherData.CreateOtherData()?.CreateSomeData()?.CreateStr();\r\n" +
                                  "            value2 = anotherData.CreateSeveralOtherData()[2]?.CreateSomeData()?.CreateStr();\r\n" +
                                  "            value3 = anotherData.Process().CreateOtherData()?.CreateSomeData()?.CreateStr();\r\n" +
                                  "            value4 = anotherData.CreateOtherData()?.Process().CreateSomeData()?.CreateStr();\r\n" +
                                  "            value5 = anotherData.Process().CreateOtherData()?.Process().CreateSomeData()?.CreateStr();\r\n" +
                                  "        }\r\n" +
                                  "    }\r\n" +
                                  "}";
            const String expectedResult = "namespace SomeNamespace\r\n" +
                                          "{\r\n" +
                                          CommonDefinitions +
                                          "    public class SomeClass\r\n" +
                                          "    {\r\n" +
                                          "        public void SomeMethod()\r\n" +
                                          "        {\r\n" +
                                          "            AnotherData anotherData = new AnotherData();\r\n" +
                                          "            string value = \"\";\r\n" +
                                          "            string value2 = \"\";\r\n" +
                                          "            string value3 = \"\";\r\n" +
                                          "            string value4 = \"\";\r\n" +
                                          "            string value5 = \"\";\r\n" +
                                          "            SomeNamespace.OtherData condExpression = anotherData.CreateOtherData();\r\n" +
                                          "            if (condExpression != null)\r\n" +
                                          "            {\r\n" +
                                          "                SomeNamespace.SomeData condExpression2 = condExpression.CreateSomeData();\r\n" +
                                          "                if (condExpression2 != null)\r\n" +
                                          "                {\r\n" +
                                          "                    value = condExpression2.CreateStr();\r\n" +
                                          "                }\r\n" +
                                          "            }\r\n" +
                                          "            SomeNamespace.OtherData condExpression3 = anotherData.CreateSeveralOtherData()[2];\r\n" +
                                          "            if (condExpression3 != null)\r\n" +
                                          "            {\r\n" +
                                          "                SomeNamespace.SomeData condExpression4 = condExpression3.CreateSomeData();\r\n" +
                                          "                if (condExpression4 != null)\r\n" +
                                          "                {\r\n" +
                                          "                    value2 = condExpression4.CreateStr();\r\n" +
                                          "                }\r\n" +
                                          "            }\r\n" +
                                          "            SomeNamespace.OtherData condExpression5 = anotherData.Process().CreateOtherData();\r\n" +
                                          "            if (condExpression5 != null)\r\n" +
                                          "            {\r\n" +
                                          "                SomeNamespace.SomeData condExpression6 = condExpression5.CreateSomeData();\r\n" +
                                          "                if (condExpression6 != null)\r\n" +
                                          "                {\r\n" +
                                          "                    value3 = condExpression6.CreateStr();\r\n" +
                                          "                }\r\n" +
                                          "            }\r\n" +
                                          "            SomeNamespace.OtherData condExpression7 = anotherData.CreateOtherData();\r\n" +
                                          "            if (condExpression7 != null)\r\n" +
                                          "            {\r\n" +
                                          "                SomeNamespace.SomeData condExpression8 = condExpression7.Process().CreateSomeData();\r\n" +
                                          "                if (condExpression8 != null)\r\n" +
                                          "                {\r\n" +
                                          "                    value4 = condExpression8.CreateStr();\r\n" +
                                          "                }\r\n" +
                                          "            }\r\n" +
                                          "            SomeNamespace.OtherData condExpression9 = anotherData.Process().CreateOtherData();\r\n" +
                                          "            if (condExpression9 != null)\r\n" +
                                          "            {\r\n" +
                                          "                SomeNamespace.SomeData condExpression10 = condExpression9.Process().CreateSomeData();\r\n" +
                                          "                if (condExpression10 != null)\r\n" +
                                          "                {\r\n" +
                                          "                    value5 = condExpression10.CreateStr();\r\n" +
                                          "                }\r\n" +
                                          "            }\r\n" +
                                          "        }\r\n" +
                                          "    }\r\n" +
                                          "}";
            String expectedOutput = "";
            if (outputLevel == OutputLevel.Info)
                expectedOutput = String.Format(ExpectedOutputForInfoLevelTemplate, CreateOnePassOutputs(5));
            TransformerHelper transformerHelper = new TransformerHelper(source, "NullConditionalOperator", outputLevel);
            transformerHelper.Process(_transformerOnFactory, expectedOutput, expectedResult);
            transformerHelper.Process(_transformerOffFactory, "", source);
        }

        [TestCase(OutputLevel.Error)]
        [TestCase(OutputLevel.Warning)]
        [TestCase(OutputLevel.Info)]
        public void ProcessMethodCall(OutputLevel outputLevel)
        {
            const String source = "namespace SomeNamespace\r\n" +
                                  "{\r\n" +
                                  CommonDefinitions +
                                  "    public class SomeClass\r\n" +
                                  "    {\r\n" +
                                  "        public void SomeMethod()\r\n" +
                                  "        {\r\n" +
                                  "            AnotherData anotherData = new AnotherData();\r\n" +
                                  "            anotherData.CreateOtherData()?.CreateSomeData()?.DoIt();\r\n" +
                                  "            anotherData.CreateSeveralOtherData()[3]?.CreateSomeData()?.DoIt();\r\n" +
                                  "        }\r\n" +
                                  "    }\r\n" +
                                  "}";
            const String expectedResult = "namespace SomeNamespace\r\n" +
                                          "{\r\n" +
                                          CommonDefinitions +
                                          "    public class SomeClass\r\n" +
                                          "    {\r\n" +
                                          "        public void SomeMethod()\r\n" +
                                          "        {\r\n" +
                                          "            AnotherData anotherData = new AnotherData();\r\n" +
                                          "            SomeNamespace.OtherData condExpression = anotherData.CreateOtherData();\r\n" +
                                          "            if (condExpression != null)\r\n" +
                                          "            {\r\n" +
                                          "                SomeNamespace.SomeData condExpression2 = condExpression.CreateSomeData();\r\n" +
                                          "                if (condExpression2 != null)\r\n" +
                                          "                {\r\n" +
                                          "                    condExpression2.DoIt();\r\n" +
                                          "                }\r\n" +
                                          "            }\r\n" +
                                          "            SomeNamespace.OtherData condExpression3 = anotherData.CreateSeveralOtherData()[3];\r\n" +
                                          "            if (condExpression3 != null)\r\n" +
                                          "            {\r\n" +
                                          "                SomeNamespace.SomeData condExpression4 = condExpression3.CreateSomeData();\r\n" +
                                          "                if (condExpression4 != null)\r\n" +
                                          "                {\r\n" +
                                          "                    condExpression4.DoIt();\r\n" +
                                          "                }\r\n" +
                                          "            }\r\n" +
                                          "        }\r\n" +
                                          "    }\r\n" +
                                          "}";
            String expectedOutput = "";
            if (outputLevel == OutputLevel.Info)
                expectedOutput = String.Format(ExpectedOutputForInfoLevelTemplate, CreateOnePassOutputs(2));
            TransformerHelper transformerHelper = new TransformerHelper(source, "NullConditionalOperator", outputLevel);
            transformerHelper.Process(_transformerOnFactory, expectedOutput, expectedResult);
            transformerHelper.Process(_transformerOffFactory, "", source);
        }

        [TestCase(OutputLevel.Error)]
        [TestCase(OutputLevel.Warning)]
        [TestCase(OutputLevel.Info)]
        public void ProcessLocalVariableDeclarationWithNullCoalescingExpr(OutputLevel outputLevel)
        {
            const String source = "namespace SomeNamespace\r\n" +
                                  "{\r\n" +
                                  CommonDefinitions +
                                  "    public class SomeClass\r\n" +
                                  "    {\r\n" +
                                  "        public void SomeMethod()\r\n" +
                                  "        {\r\n" +
                                  "            AnotherData anotherData = new AnotherData();\r\n" +
                                  "            string value = anotherData.CreateOtherData()?.CreateSomeData()?.CreateStr() ?? \"\";\r\n" +
                                  "            string value2 = anotherData.CreateOtherData()?.CreateSomeData()?.CreateStr() ?? CreateDefaultStr() ?? \"\";\r\n" +
                                  "        }\r\n" +
                                  "        public string CreateDefaultStr()\r\n" +
                                  "        {\r\n" +
                                  "            return \"\";\r\n" +
                                  "        }\r\n" +
                                  "    }\r\n" +
                                  "}";
            const String expectedResult = "namespace SomeNamespace\r\n" +
                                          "{\r\n" +
                                          CommonDefinitions +
                                          "    public class SomeClass\r\n" +
                                          "    {\r\n" +
                                          "        public void SomeMethod()\r\n" +
                                          "        {\r\n" +
                                          "            AnotherData anotherData = new AnotherData();\r\n" +
                                          "            string value = default(string);\r\n" +
                                          "            SomeNamespace.OtherData condExpression = anotherData.CreateOtherData();\r\n" +
                                          "            if (condExpression != null)\r\n" +
                                          "            {\r\n" +
                                          "                SomeNamespace.SomeData condExpression2 = condExpression.CreateSomeData();\r\n" +
                                          "                if (condExpression2 != null)\r\n" +
                                          "                {\r\n" +
                                          "                    value = condExpression2.CreateStr();\r\n" +
                                          "                }\r\n" +
                                          "            }\r\n" +
                                          "            if (value == null)\r\n" +
                                          "            {\r\n" +
                                          "                value = \"\";\r\n" +
                                          "            }\r\n" +
                                          "            string value2 = default(string);\r\n" +
                                          "            SomeNamespace.OtherData condExpression3 = anotherData.CreateOtherData();\r\n" +
                                          "            if (condExpression3 != null)\r\n" +
                                          "            {\r\n" +
                                          "                SomeNamespace.SomeData condExpression4 = condExpression3.CreateSomeData();\r\n" +
                                          "                if (condExpression4 != null)\r\n" +
                                          "                {\r\n" +
                                          "                    value2 = condExpression4.CreateStr();\r\n" +
                                          "                }\r\n" +
                                          "            }\r\n" +
                                          "            if (value2 == null)\r\n" +
                                          "            {\r\n" +
                                          "                value2 = CreateDefaultStr() ?? \"\";\r\n" +
                                          "            }\r\n" +
                                          "        }\r\n" +
                                          "        public string CreateDefaultStr()\r\n" +
                                          "        {\r\n" +
                                          "            return \"\";\r\n" +
                                          "        }\r\n" +
                                          "    }\r\n" +
                                          "}";
            String expectedOutput = "";
            if (outputLevel == OutputLevel.Info)
                expectedOutput = String.Format(ExpectedOutputForInfoLevelTemplate, CreateOnePassOutputs(2));
            TransformerHelper transformerHelper = new TransformerHelper(source, "NullConditionalOperator", outputLevel);
            transformerHelper.Process(_transformerOnFactory, expectedOutput, expectedResult);
            transformerHelper.Process(_transformerOffFactory, "", source);
        }

        [TestCase(OutputLevel.Error)]
        [TestCase(OutputLevel.Warning)]
        [TestCase(OutputLevel.Info)]
        public void ProcessVariableAssignmentWithNullCoalescingExpr(OutputLevel outputLevel)
        {
            const String source = "namespace SomeNamespace\r\n" +
                                  "{\r\n" +
                                  CommonDefinitions +
                                  "    public class SomeClass\r\n" +
                                  "    {\r\n" +
                                  "        public void SomeMethod()\r\n" +
                                  "        {\r\n" +
                                  "            AnotherData anotherData = new AnotherData();\r\n" +
                                  "            string value = \"\";\r\n" +
                                  "            string value2 = \"\";\r\n" +
                                  "            value = anotherData.CreateOtherData()?.CreateSomeData()?.CreateStr() ?? \"\";\r\n" +
                                  "            value2 = anotherData.CreateOtherData()?.CreateSomeData()?.CreateStr() ?? CreateDefaultStr() ?? \"\";\r\n" +
                                  "        }\r\n" +
                                  "        public string CreateDefaultStr()\r\n" +
                                  "        {\r\n" +
                                  "            return \"\";\r\n" +
                                  "        }\r\n" +
                                  "    }\r\n" +
                                  "}";
            const String expectedResult = "namespace SomeNamespace\r\n" +
                                          "{\r\n" +
                                          CommonDefinitions +
                                          "    public class SomeClass\r\n" +
                                          "    {\r\n" +
                                          "        public void SomeMethod()\r\n" +
                                          "        {\r\n" +
                                          "            AnotherData anotherData = new AnotherData();\r\n" +
                                          "            string value = \"\";\r\n" +
                                          "            string value2 = \"\";\r\n" +
                                          "            SomeNamespace.OtherData condExpression = anotherData.CreateOtherData();\r\n" +
                                          "            if (condExpression != null)\r\n" +
                                          "            {\r\n" +
                                          "                SomeNamespace.SomeData condExpression2 = condExpression.CreateSomeData();\r\n" +
                                          "                if (condExpression2 != null)\r\n" +
                                          "                {\r\n" +
                                          "                    value = condExpression2.CreateStr();\r\n" +
                                          "                }\r\n" +
                                          "            }\r\n" +
                                          "            if (value == null)\r\n" +
                                          "            {\r\n" +
                                          "                value = \"\";\r\n" +
                                          "            }\r\n" +
                                          "            SomeNamespace.OtherData condExpression3 = anotherData.CreateOtherData();\r\n" +
                                          "            if (condExpression3 != null)\r\n" +
                                          "            {\r\n" +
                                          "                SomeNamespace.SomeData condExpression4 = condExpression3.CreateSomeData();\r\n" +
                                          "                if (condExpression4 != null)\r\n" +
                                          "                {\r\n" +
                                          "                    value2 = condExpression4.CreateStr();\r\n" +
                                          "                }\r\n" +
                                          "            }\r\n" +
                                          "            if (value2 == null)\r\n" +
                                          "            {\r\n" +
                                          "                value2 = CreateDefaultStr() ?? \"\";\r\n" +
                                          "            }\r\n" +
                                          "        }\r\n" +
                                          "        public string CreateDefaultStr()\r\n" +
                                          "        {\r\n" +
                                          "            return \"\";\r\n" +
                                          "        }\r\n" +
                                          "    }\r\n" +
                                          "}";
            String expectedOutput = "";
            if (outputLevel == OutputLevel.Info)
                expectedOutput = String.Format(ExpectedOutputForInfoLevelTemplate, CreateOnePassOutputs(2));
            TransformerHelper transformerHelper = new TransformerHelper(source, "NullConditionalOperator", outputLevel);
            transformerHelper.Process(_transformerOnFactory, expectedOutput, expectedResult);
            transformerHelper.Process(_transformerOffFactory, "", source);
        }

        [TestCase(OutputLevel.Error)]
        [TestCase(OutputLevel.Warning)]
        [TestCase(OutputLevel.Info)]
        public void ProcessLocalVariableDeclarationWithNullCoalescingInnerExpr(OutputLevel outputLevel)
        {
            const String source = "namespace SomeNamespace\r\n" +
                                  "{\r\n" +
                                  CommonDefinitions +
                                  "    public class SomeClass\r\n" +
                                  "    {\r\n" +
                                  "        public void SomeMethod()\r\n" +
                                  "        {\r\n" +
                                  "            AnotherData anotherData = new AnotherData();\r\n" +
                                  "            string value = (anotherData.CreateOtherData() ?? CreateDefaultOtherData() ?? CreateDefaultOtherData2()).Process()?.CreateSomeData()?.CreateStr();\r\n" +
                                  "            string value2 = (anotherData.Process()?.CreateOtherData() ?? CreateDefaultOtherData() ?? CreateDefaultOtherData2()).Process()?.CreateSomeData()?.CreateStr();\r\n" +
                                  "        }\r\n" +
                                  "        public string CreateDefaultStr()\r\n" +
                                  "        {\r\n" +
                                  "            return \"\";\r\n" +
                                  "        }\r\n" +
                                  "        public OtherData CreateDefaultOtherData()\r\n" +
                                  "        {\r\n" +
                                  "            return new OtherData();\r\n" +
                                  "        }\r\n" +
                                  "        public OtherData CreateDefaultOtherData2()\r\n" +
                                  "        {\r\n" +
                                  "            return new OtherData();\r\n" +
                                  "        }\r\n" +
                                  "    }\r\n" +
                                  "}";
            const String expectedResult = "namespace SomeNamespace\r\n" +
                                          "{\r\n" +
                                          CommonDefinitions +
                                          "    public class SomeClass\r\n" +
                                          "    {\r\n" +
                                          "        public void SomeMethod()\r\n" +
                                          "        {\r\n" +
                                          "            AnotherData anotherData = new AnotherData();\r\n" +
                                          "            string value = default(string);\r\n" +
                                          "            SomeNamespace.OtherData condExpression = (anotherData.CreateOtherData() ?? CreateDefaultOtherData() ?? CreateDefaultOtherData2()).Process();\r\n" +
                                          "            if (condExpression != null)\r\n" +
                                          "            {\r\n" +
                                          "                SomeNamespace.SomeData condExpression2 = condExpression.CreateSomeData();\r\n" +
                                          "                if (condExpression2 != null)\r\n" +
                                          "                {\r\n" +
                                          "                    value = condExpression2.CreateStr();\r\n" +
                                          "                }\r\n" +
                                          "            }\r\n" +
                                          "            SomeNamespace.OtherData expression = default(SomeNamespace.OtherData);\r\n" +
                                          "            SomeNamespace.AnotherData condExpression3 = anotherData.Process();\r\n" +
                                          "            if (condExpression3 != null)\r\n" +
                                          "            {\r\n" +
                                          "                expression = condExpression3.CreateOtherData();\r\n" +
                                          "            }\r\n" +
                                          "            if (expression == null)\r\n" +
                                          "            {\r\n" +
                                          "                expression = CreateDefaultOtherData() ?? CreateDefaultOtherData2();\r\n" +
                                          "            }\r\n" +
                                          "            string value2 = default(string);\r\n" +
                                          "            SomeNamespace.OtherData condExpression4 = expression.Process();\r\n" +
                                          "            if (condExpression4 != null)\r\n" +
                                          "            {\r\n" +
                                          "                SomeNamespace.SomeData condExpression5 = condExpression4.CreateSomeData();\r\n" +
                                          "                if (condExpression5 != null)\r\n" +
                                          "                {\r\n" +
                                          "                    value2 = condExpression5.CreateStr();\r\n" +
                                          "                }\r\n" +
                                          "            }\r\n" +
                                          "        }\r\n" +
                                          "        public string CreateDefaultStr()\r\n" +
                                          "        {\r\n" +
                                          "            return \"\";\r\n" +
                                          "        }\r\n" +
                                          "        public OtherData CreateDefaultOtherData()\r\n" +
                                          "        {\r\n" +
                                          "            return new OtherData();\r\n" +
                                          "        }\r\n" +
                                          "        public OtherData CreateDefaultOtherData2()\r\n" +
                                          "        {\r\n" +
                                          "            return new OtherData();\r\n" +
                                          "        }\r\n" +
                                          "    }\r\n" +
                                          "}";
            String expectedOutput = "";
            if (outputLevel == OutputLevel.Info)
            {
                String iterations = String.Join("", new []{ CreateNPassOutput(1), CreateNPassOutput(2)});
                expectedOutput = String.Format(ExpectedOutputForInfoLevelTemplate, iterations);
            }
            TransformerHelper transformerHelper = new TransformerHelper(source, "NullConditionalOperator", outputLevel);
            transformerHelper.Process(_transformerOnFactory, expectedOutput, expectedResult);
            transformerHelper.Process(_transformerOffFactory, "", source);
        }

        [TestCase(OutputLevel.Error)]
        [TestCase(OutputLevel.Warning)]
        [TestCase(OutputLevel.Info)]
        public void ProcessVariableAssignmentWithNullCoalescingInnerExpr(OutputLevel outputLevel)
        {
            const String source = "namespace SomeNamespace\r\n" +
                                  "{\r\n" +
                                  CommonDefinitions +
                                  "    public class SomeClass\r\n" +
                                  "    {\r\n" +
                                  "        public void SomeMethod()\r\n" +
                                  "        {\r\n" +
                                  "            AnotherData anotherData = new AnotherData();\r\n" +
                                  "            string value = \"\";\r\n" +
                                  "            string value2 = \"\";\r\n" +
                                  "            value = (anotherData.CreateOtherData() ?? CreateDefaultOtherData() ?? CreateDefaultOtherData2())?.CreateSomeData()?.CreateStr();\r\n" +
                                  "            value2 = (anotherData.Process()?.CreateOtherData() ?? CreateDefaultOtherData() ?? CreateDefaultOtherData2())?.CreateSomeData()?.CreateStr();\r\n" +
                                  "        }\r\n" +
                                  "        public string CreateDefaultStr()\r\n" +
                                  "        {\r\n" +
                                  "            return \"\";\r\n" +
                                  "        }\r\n" +
                                  "        public OtherData CreateDefaultOtherData()\r\n" +
                                  "        {\r\n" +
                                  "            return new OtherData();\r\n" +
                                  "        }\r\n" +
                                  "        public OtherData CreateDefaultOtherData2()\r\n" +
                                  "        {\r\n" +
                                  "            return new OtherData();\r\n" +
                                  "        }\r\n" +
                                  "    }\r\n" +
                                  "}";
            const String expectedResult = "namespace SomeNamespace\r\n" +
                                          "{\r\n" +
                                          CommonDefinitions +
                                          "    public class SomeClass\r\n" +
                                          "    {\r\n" +
                                          "        public void SomeMethod()\r\n" +
                                          "        {\r\n" +
                                          "            AnotherData anotherData = new AnotherData();\r\n" +
                                          "            string value = \"\";\r\n" +
                                          "            string value2 = \"\";\r\n" +
                                          "            SomeNamespace.OtherData condExpression = (anotherData.CreateOtherData() ?? CreateDefaultOtherData() ?? CreateDefaultOtherData2());\r\n" +
                                          "            if (condExpression != null)\r\n" +
                                          "            {\r\n" +
                                          "                SomeNamespace.SomeData condExpression2 = condExpression.CreateSomeData();\r\n" +
                                          "                if (condExpression2 != null)\r\n" +
                                          "                {\r\n" +
                                          "                    value = condExpression2.CreateStr();\r\n" +
                                          "                }\r\n" +
                                          "            }\r\n" +
                                          "            SomeNamespace.OtherData expression = default(SomeNamespace.OtherData);\r\n" +
                                          "            SomeNamespace.AnotherData condExpression3 = anotherData.Process();\r\n" +
                                          "            if (condExpression3 != null)\r\n" +
                                          "            {\r\n" +
                                          "                expression = condExpression3.CreateOtherData();\r\n" +
                                          "            }\r\n" +
                                          "            if (expression == null)\r\n" +
                                          "            {\r\n" +
                                          "                expression = CreateDefaultOtherData() ?? CreateDefaultOtherData2();\r\n" +
                                          "            }\r\n" +
                                          "            SomeNamespace.OtherData condExpression4 = expression;\r\n" +
                                          "            if (condExpression4 != null)\r\n" +
                                          "            {\r\n" +
                                          "                SomeNamespace.SomeData condExpression5 = condExpression4.CreateSomeData();\r\n" +
                                          "                if (condExpression5 != null)\r\n" +
                                          "                {\r\n" +
                                          "                    value2 = condExpression5.CreateStr();\r\n" +
                                          "                }\r\n" +
                                          "            }\r\n" +
                                          "        }\r\n" +
                                          "        public string CreateDefaultStr()\r\n" +
                                          "        {\r\n" +
                                          "            return \"\";\r\n" +
                                          "        }\r\n" +
                                          "        public OtherData CreateDefaultOtherData()\r\n" +
                                          "        {\r\n" +
                                          "            return new OtherData();\r\n" +
                                          "        }\r\n" +
                                          "        public OtherData CreateDefaultOtherData2()\r\n" +
                                          "        {\r\n" +
                                          "            return new OtherData();\r\n" +
                                          "        }\r\n" +
                                          "    }\r\n" +
                                          "}";
            String expectedOutput = "";
            if (outputLevel == OutputLevel.Info)
            {
                String iterations = String.Join("", new[] {CreateNPassOutput(1), CreateNPassOutput(2)});
                expectedOutput = String.Format(ExpectedOutputForInfoLevelTemplate, iterations);
            }
            TransformerHelper transformerHelper = new TransformerHelper(source, "NullConditionalOperator", outputLevel);
            transformerHelper.Process(_transformerOnFactory, expectedOutput, expectedResult);
            transformerHelper.Process(_transformerOffFactory, "", source);
        }

        [TestCase(OutputLevel.Error)]
        [TestCase(OutputLevel.Warning)]
        [TestCase(OutputLevel.Info)]
        public void ProcessMethodCallWithNullCoalescingInnerExpr(OutputLevel outputLevel)
        {
            const String source = "namespace SomeNamespace\r\n" +
                                  "{\r\n" +
                                  CommonDefinitions +
                                  "    public class SomeClass\r\n" +
                                  "    {\r\n" +
                                  "        public void SomeMethod()\r\n" +
                                  "        {\r\n" +
                                  "            AnotherData anotherData = new AnotherData();\r\n" +
                                  "            (anotherData.CreateOtherData() ?? CreateDefaultOtherData() ?? CreateDefaultOtherData2())?.CreateSomeData()?.DoIt();\r\n" +
                                  "            (anotherData.Process()?.CreateOtherData() ?? CreateDefaultOtherData() ?? CreateDefaultOtherData2())?.CreateSomeData()?.DoIt();\r\n" +
                                  "        }\r\n" +
                                  "        public string CreateDefaultStr()\r\n" +
                                  "        {\r\n" +
                                  "            return \"\";\r\n" +
                                  "        }\r\n" +
                                  "        public OtherData CreateDefaultOtherData()\r\n" +
                                  "        {\r\n" +
                                  "            return new OtherData();\r\n" +
                                  "        }\r\n" +
                                  "        public OtherData CreateDefaultOtherData2()\r\n" +
                                  "        {\r\n" +
                                  "            return new OtherData();\r\n" +
                                  "        }\r\n" +
                                  "    }\r\n" +
                                  "}";
            const String expectedResult = "namespace SomeNamespace\r\n" +
                                          "{\r\n" +
                                          CommonDefinitions +
                                          "    public class SomeClass\r\n" +
                                          "    {\r\n" +
                                          "        public void SomeMethod()\r\n" +
                                          "        {\r\n" +
                                          "            AnotherData anotherData = new AnotherData();\r\n" +
                                          "            SomeNamespace.OtherData condExpression = (anotherData.CreateOtherData() ?? CreateDefaultOtherData() ?? CreateDefaultOtherData2());\r\n" +
                                          "            if (condExpression != null)\r\n" +
                                          "            {\r\n" +
                                          "                SomeNamespace.SomeData condExpression2 = condExpression.CreateSomeData();\r\n" +
                                          "                if (condExpression2 != null)\r\n" +
                                          "                {\r\n" +
                                          "                    condExpression2.DoIt();\r\n" +
                                          "                }\r\n" +
                                          "            }\r\n" +
                                          "            SomeNamespace.OtherData expression = default(SomeNamespace.OtherData);\r\n" +
                                          "            SomeNamespace.AnotherData condExpression3 = anotherData.Process();\r\n" +
                                          "            if (condExpression3 != null)\r\n" +
                                          "            {\r\n" +
                                          "                expression = condExpression3.CreateOtherData();\r\n" +
                                          "            }\r\n" +
                                          "            if (expression == null)\r\n" +
                                          "            {\r\n" +
                                          "                expression = CreateDefaultOtherData() ?? CreateDefaultOtherData2();\r\n" +
                                          "            }\r\n" +
                                          "            SomeNamespace.OtherData condExpression4 = expression;\r\n" +
                                          "            if (condExpression4 != null)\r\n" +
                                          "            {\r\n" +
                                          "                SomeNamespace.SomeData condExpression5 = condExpression4.CreateSomeData();\r\n" +
                                          "                if (condExpression5 != null)\r\n" +
                                          "                {\r\n" +
                                          "                    condExpression5.DoIt();\r\n" +
                                          "                }\r\n" +
                                          "            }\r\n" +
                                          "        }\r\n" +
                                          "        public string CreateDefaultStr()\r\n" +
                                          "        {\r\n" +
                                          "            return \"\";\r\n" +
                                          "        }\r\n" +
                                          "        public OtherData CreateDefaultOtherData()\r\n" +
                                          "        {\r\n" +
                                          "            return new OtherData();\r\n" +
                                          "        }\r\n" +
                                          "        public OtherData CreateDefaultOtherData2()\r\n" +
                                          "        {\r\n" +
                                          "            return new OtherData();\r\n" +
                                          "        }\r\n" +
                                          "    }\r\n" +
                                          "}";
            String expectedOutput = "";
            if (outputLevel == OutputLevel.Info)
            {
                String iterations = String.Join("", new[] {CreateNPassOutput(1), CreateNPassOutput(2)});
                expectedOutput = String.Format(ExpectedOutputForInfoLevelTemplate, iterations);
            }
            TransformerHelper transformerHelper = new TransformerHelper(source, "NullConditionalOperator", outputLevel);
            transformerHelper.Process(_transformerOnFactory, expectedOutput, expectedResult);
            transformerHelper.Process(_transformerOffFactory, "", source);
        }

        [TestCase(OutputLevel.Error)]
        [TestCase(OutputLevel.Warning)]
        [TestCase(OutputLevel.Info)]
        public void ProcessMethodArguments(OutputLevel outputLevel)
        {
            const String source = "namespace SomeNamespace\r\n" +
                                  "{\r\n" +
                                  CommonDefinitions +
                                  "    public class SomeClass\r\n" +
                                  "    {\r\n" +
                                  "        public void SomeMethod()\r\n" +
                                  "        {\r\n" +
                                  "            AnotherData anotherData = new AnotherData();\r\n" +
                                  "            OtherMethod(anotherData.CreateOtherData()?.CreateSomeData()?.CreateStr());\r\n" +
                                  "            OtherMethod(anotherData.CreateSeveralOtherData()[2]?.CreateSomeData()?.CreateStr());\r\n" +
                                  "        }\r\n" +
                                  "        public void OtherMethod(string str)\r\n" +
                                  "        {\r\n" +
                                  "        }\r\n" +
                                  "    }\r\n" +
                                  "}";
            const String expectedResult = "namespace SomeNamespace\r\n" +
                                          "{\r\n" +
                                          CommonDefinitions +
                                          "    public class SomeClass\r\n" +
                                          "    {\r\n" +
                                          "        public void SomeMethod()\r\n" +
                                          "        {\r\n" +
                                          "            AnotherData anotherData = new AnotherData();\r\n" +
                                          "            string str = default(string);\r\n" +
                                          "            SomeNamespace.OtherData condExpression = anotherData.CreateOtherData();\r\n" +
                                          "            if (condExpression != null)\r\n" +
                                          "            {\r\n" +
                                          "                SomeNamespace.SomeData condExpression2 = condExpression.CreateSomeData();\r\n" +
                                          "                if (condExpression2 != null)\r\n" +
                                          "                {\r\n" +
                                          "                    str = condExpression2.CreateStr();\r\n" +
                                          "                }\r\n" +
                                          "            }\r\n" +
                                          "            OtherMethod(str);\r\n" +
                                          "            string str2 = default(string);\r\n" +
                                          "            SomeNamespace.OtherData condExpression3 = anotherData.CreateSeveralOtherData()[2];\r\n" +
                                          "            if (condExpression3 != null)\r\n" +
                                          "            {\r\n" +
                                          "                SomeNamespace.SomeData condExpression4 = condExpression3.CreateSomeData();\r\n" +
                                          "                if (condExpression4 != null)\r\n" +
                                          "                {\r\n" +
                                          "                    str2 = condExpression4.CreateStr();\r\n" +
                                          "                }\r\n" +
                                          "            }\r\n" +
                                          "            OtherMethod(str2);\r\n" +
                                          "        }\r\n" +
                                          "        public void OtherMethod(string str)\r\n" +
                                          "        {\r\n" +
                                          "        }\r\n" +
                                          "    }\r\n" +
                                          "}";
            String expectedOutput = "";
            if (outputLevel == OutputLevel.Info)
                expectedOutput = String.Format(ExpectedOutputForInfoLevelTemplate, CreateOnePassOutputs(2));
            TransformerHelper transformerHelper = new TransformerHelper(source, "NullConditionalOperator", outputLevel);
            transformerHelper.Process(_transformerOnFactory, expectedOutput, expectedResult);
            transformerHelper.Process(_transformerOffFactory, "", source);
        }

        [TestCase(OutputLevel.Error)]
        [TestCase(OutputLevel.Warning)]
        [TestCase(OutputLevel.Info)]
        public void ProcessMethodArgumentsWithNullCoalescingExpr(OutputLevel outputLevel)
        {
            const String source = "namespace SomeNamespace\r\n" +
                                  "{\r\n" +
                                  CommonDefinitions +
                                  "    public class SomeClass\r\n" +
                                  "    {\r\n" +
                                  "        public void SomeMethod()\r\n" +
                                  "        {\r\n" +
                                  "            AnotherData anotherData = new AnotherData();\r\n" +
                                  "            OtherMethod(anotherData.CreateOtherData()?.CreateSomeData()?.CreateStr() ?? \"\");\r\n" +
                                  "            OtherMethod(anotherData.CreateOtherData()?.CreateSomeData()?.CreateStr() ?? CreateDefaultStr() ?? \"\");\r\n" +
                                  "        }\r\n" +
                                  "        public string CreateDefaultStr()\r\n" +
                                  "        {\r\n" +
                                  "            return \"\";\r\n" +
                                  "        }\r\n" +
                                  "        public void OtherMethod(string str)\r\n" +
                                  "        {\r\n" +
                                  "        }\r\n" +
                                  "    }\r\n" +
                                  "}";
            const String expectedResult = "namespace SomeNamespace\r\n" +
                                          "{\r\n" +
                                          CommonDefinitions +
                                          "    public class SomeClass\r\n" +
                                          "    {\r\n" +
                                          "        public void SomeMethod()\r\n" +
                                          "        {\r\n" +
                                          "            AnotherData anotherData = new AnotherData();\r\n" +
                                          "            string str = default(string);\r\n" +
                                          "            SomeNamespace.OtherData condExpression = anotherData.CreateOtherData();\r\n" +
                                          "            if (condExpression != null)\r\n" +
                                          "            {\r\n" +
                                          "                SomeNamespace.SomeData condExpression2 = condExpression.CreateSomeData();\r\n" +
                                          "                if (condExpression2 != null)\r\n" +
                                          "                {\r\n" +
                                          "                    str = condExpression2.CreateStr();\r\n" +
                                          "                }\r\n" +
                                          "            }\r\n" +
                                          "            if (str == null)\r\n" +
                                          "            {\r\n" +
                                          "                str = \"\";\r\n" +
                                          "            }\r\n" +
                                          "            OtherMethod(str);\r\n" +
                                          "            string str2 = default(string);\r\n" +
                                          "            SomeNamespace.OtherData condExpression3 = anotherData.CreateOtherData();\r\n" +
                                          "            if (condExpression3 != null)\r\n" +
                                          "            {\r\n" +
                                          "                SomeNamespace.SomeData condExpression4 = condExpression3.CreateSomeData();\r\n" +
                                          "                if (condExpression4 != null)\r\n" +
                                          "                {\r\n" +
                                          "                    str2 = condExpression4.CreateStr();\r\n" +
                                          "                }\r\n" +
                                          "            }\r\n" +
                                          "            if (str2 == null)\r\n" +
                                          "            {\r\n" +
                                          "                str2 = CreateDefaultStr() ?? \"\";\r\n" +
                                          "            }\r\n" +
                                          "            OtherMethod(str2);\r\n" +
                                          "        }\r\n" +
                                          "        public string CreateDefaultStr()\r\n" +
                                          "        {\r\n" +
                                          "            return \"\";\r\n" +
                                          "        }\r\n" +
                                          "        public void OtherMethod(string str)\r\n" +
                                          "        {\r\n" +
                                          "        }\r\n" +
                                          "    }\r\n" +
                                          "}";
            String expectedOutput = "";
            if (outputLevel == OutputLevel.Info)
                expectedOutput = String.Format(ExpectedOutputForInfoLevelTemplate, CreateOnePassOutputs(2));
            TransformerHelper transformerHelper = new TransformerHelper(source, "NullConditionalOperator", outputLevel);
            transformerHelper.Process(_transformerOnFactory, expectedOutput, expectedResult);
            transformerHelper.Process(_transformerOffFactory, "", source);
        }

        [TestCase(OutputLevel.Error)]
        [TestCase(OutputLevel.Warning)]
        [TestCase(OutputLevel.Info)]
        public void ProcessMethodArgumentsWithNullCoalescingInnerExpr(OutputLevel outputLevel)
        {
            const String source = "namespace SomeNamespace\r\n" +
                                  "{\r\n" +
                                  CommonDefinitions +
                                  "    public class SomeClass\r\n" +
                                  "    {\r\n" +
                                  "        public void SomeMethod()\r\n" +
                                  "        {\r\n" +
                                  "            AnotherData anotherData = new AnotherData();\r\n" +
                                  "            OtherMethod((anotherData.CreateOtherData() ?? CreateDefaultOtherData() ?? CreateDefaultOtherData2())?.CreateSomeData()?.CreateStr());\r\n" +
                                  "            OtherMethod((anotherData.Process()?.CreateOtherData() ?? CreateDefaultOtherData() ?? CreateDefaultOtherData2())?.CreateSomeData()?.CreateStr());\r\n" +
                                  "            OtherMethod((anotherData.CreateOtherData() ?? CreateDefaultOtherData() ?? CreateDefaultOtherData2())?.CreateSomeData()?.CreateStr() ?? \"\");\r\n" +
                                  "            OtherMethod((anotherData.Process()?.CreateOtherData() ?? CreateDefaultOtherData() ?? CreateDefaultOtherData2())?.CreateSomeData()?.CreateStr() ?? \"\");\r\n" +
                                  "        }\r\n" +
                                  "        public string CreateDefaultStr()\r\n" +
                                  "        {\r\n" +
                                  "            return \"\";\r\n" +
                                  "        }\r\n" +
                                  "        public OtherData CreateDefaultOtherData()\r\n" +
                                  "        {\r\n" +
                                  "            return new OtherData();\r\n" +
                                  "        }\r\n" +
                                  "        public OtherData CreateDefaultOtherData2()\r\n" +
                                  "        {\r\n" +
                                  "            return new OtherData();\r\n" +
                                  "        }\r\n" +
                                  "        public void OtherMethod(string str)\r\n" +
                                  "        {\r\n" +
                                  "        }\r\n" +
                                  "    }\r\n" +
                                  "}";
            const String expectedResult = "namespace SomeNamespace\r\n" +
                                          "{\r\n" +
                                          CommonDefinitions +
                                          "    public class SomeClass\r\n" +
                                          "    {\r\n" +
                                          "        public void SomeMethod()\r\n" +
                                          "        {\r\n" +
                                          "            AnotherData anotherData = new AnotherData();\r\n" +
                                          "            string str = default(string);\r\n" +
                                          "            SomeNamespace.OtherData condExpression = (anotherData.CreateOtherData() ?? CreateDefaultOtherData() ?? CreateDefaultOtherData2());\r\n" +
                                          "            if (condExpression != null)\r\n" +
                                          "            {\r\n" +
                                          "                SomeNamespace.SomeData condExpression2 = condExpression.CreateSomeData();\r\n" +
                                          "                if (condExpression2 != null)\r\n" +
                                          "                {\r\n" +
                                          "                    str = condExpression2.CreateStr();\r\n" +
                                          "                }\r\n" +
                                          "            }\r\n" +
                                          "            OtherMethod(str);\r\n" +
                                          "            SomeNamespace.OtherData expression = default(SomeNamespace.OtherData);\r\n" +
                                          "            SomeNamespace.AnotherData condExpression3 = anotherData.Process();\r\n" +
                                          "            if (condExpression3 != null)\r\n" +
                                          "            {\r\n" +
                                          "                expression = condExpression3.CreateOtherData();\r\n" +
                                          "            }\r\n" +
                                          "            if (expression == null)\r\n" +
                                          "            {\r\n" +
                                          "                expression = CreateDefaultOtherData() ?? CreateDefaultOtherData2();\r\n" +
                                          "            }\r\n" +
                                          "            string str2 = default(string);\r\n" +
                                          "            SomeNamespace.OtherData condExpression4 = expression;\r\n" +
                                          "            if (condExpression4 != null)\r\n" +
                                          "            {\r\n" +
                                          "                SomeNamespace.SomeData condExpression5 = condExpression4.CreateSomeData();\r\n" +
                                          "                if (condExpression5 != null)\r\n" +
                                          "                {\r\n" +
                                          "                    str2 = condExpression5.CreateStr();\r\n" +
                                          "                }\r\n" +
                                          "            }\r\n" +
                                          "            OtherMethod(str2);\r\n" +
                                          "            string str3 = default(string);\r\n" +
                                          "            SomeNamespace.OtherData condExpression6 = (anotherData.CreateOtherData() ?? CreateDefaultOtherData() ?? CreateDefaultOtherData2());\r\n" +
                                          "            if (condExpression6 != null)\r\n" +
                                          "            {\r\n" +
                                          "                SomeNamespace.SomeData condExpression7 = condExpression6.CreateSomeData();\r\n" +
                                          "                if (condExpression7 != null)\r\n" +
                                          "                {\r\n" +
                                          "                    str3 = condExpression7.CreateStr();\r\n" +
                                          "                }\r\n" +
                                          "            }\r\n" +
                                          "            if (str3 == null)\r\n" +
                                          "            {\r\n" +
                                          "                str3 = \"\";\r\n" +
                                          "            }\r\n" +
                                          "            OtherMethod(str3);\r\n" +
                                          "            SomeNamespace.OtherData expression2 = default(SomeNamespace.OtherData);\r\n" +
                                          "            SomeNamespace.AnotherData condExpression8 = anotherData.Process();\r\n" +
                                          "            if (condExpression8 != null)\r\n" +
                                          "            {\r\n" +
                                          "                expression2 = condExpression8.CreateOtherData();\r\n" +
                                          "            }\r\n" +
                                          "            if (expression2 == null)\r\n" +
                                          "            {\r\n" +
                                          "                expression2 = CreateDefaultOtherData() ?? CreateDefaultOtherData2();\r\n" +
                                          "            }\r\n" +
                                          "            string str4 = default(string);\r\n" +
                                          "            SomeNamespace.OtherData condExpression9 = expression2;\r\n" +
                                          "            if (condExpression9 != null)\r\n" +
                                          "            {\r\n" +
                                          "                SomeNamespace.SomeData condExpression10 = condExpression9.CreateSomeData();\r\n" +
                                          "                if (condExpression10 != null)\r\n" +
                                          "                {\r\n" +
                                          "                    str4 = condExpression10.CreateStr();\r\n" +
                                          "                }\r\n" +
                                          "            }\r\n" +
                                          "            if (str4 == null)\r\n" +
                                          "            {\r\n" +
                                          "                str4 = \"\";\r\n" +
                                          "            }\r\n" +
                                          "            OtherMethod(str4);\r\n" +
                                          "        }\r\n" +
                                          "        public string CreateDefaultStr()\r\n" +
                                          "        {\r\n" +
                                          "            return \"\";\r\n" +
                                          "        }\r\n" +
                                          "        public OtherData CreateDefaultOtherData()\r\n" +
                                          "        {\r\n" +
                                          "            return new OtherData();\r\n" +
                                          "        }\r\n" +
                                          "        public OtherData CreateDefaultOtherData2()\r\n" +
                                          "        {\r\n" +
                                          "            return new OtherData();\r\n" +
                                          "        }\r\n" +
                                          "        public void OtherMethod(string str)\r\n" +
                                          "        {\r\n" +
                                          "        }\r\n" +
                                          "    }\r\n" +
                                          "}";
            String expectedOutput = "";
            if (outputLevel == OutputLevel.Info)
            {
                String iterations = String.Join("", new[] {CreateNPassOutput(1), CreateNPassOutput(2), CreateNPassOutput(1), CreateNPassOutput(2)});
                expectedOutput = String.Format(ExpectedOutputForInfoLevelTemplate, iterations);
            }
            TransformerHelper transformerHelper = new TransformerHelper(source, "NullConditionalOperator", outputLevel);
            transformerHelper.Process(_transformerOnFactory, expectedOutput, expectedResult);
            transformerHelper.Process(_transformerOffFactory, "", source);
        }

        [TestCase(OutputLevel.Error)]
        [TestCase(OutputLevel.Warning)]
        [TestCase(OutputLevel.Info)]
        public void ProcessReturn(OutputLevel outputLevel)
        {
            const String source = "namespace SomeNamespace\r\n" +
                                  "{\r\n" +
                                  CommonDefinitions +
                                  "    public class SomeClass\r\n" +
                                  "    {\r\n" +
                                  "        public string AnotherMethod(AnotherData anotherData)\r\n" +
                                  "        {\r\n" +
                                  "            return anotherData.CreateOtherData()?.CreateSomeData()?.CreateStr();\r\n" +
                                  "        }\r\n" +
                                  "        public string AnotherMethod2(AnotherData anotherData)\r\n" +
                                  "        {\r\n" +
                                  "            return anotherData.CreateSeveralOtherData()[2]?.CreateSomeData()?.CreateStr();\r\n" +
                                  "        }\r\n" +
                                  "    }\r\n" +
                                  "}";
            const String expectedResult = "namespace SomeNamespace\r\n" +
                                          "{\r\n" +
                                          CommonDefinitions +
                                          "    public class SomeClass\r\n" +
                                          "    {\r\n" +
                                          "        public string AnotherMethod(AnotherData anotherData)\r\n" +
                                          "        {\r\n" +
                                          "            string returnExpression = default(string);\r\n" +
                                          "            SomeNamespace.OtherData condExpression = anotherData.CreateOtherData();\r\n" +
                                          "            if (condExpression != null)\r\n" +
                                          "            {\r\n" +
                                          "                SomeNamespace.SomeData condExpression2 = condExpression.CreateSomeData();\r\n" +
                                          "                if (condExpression2 != null)\r\n" +
                                          "                {\r\n" +
                                          "                    returnExpression = condExpression2.CreateStr();\r\n" +
                                          "                }\r\n" +
                                          "            }\r\n" +
                                          "            return returnExpression;\r\n" +
                                          "        }\r\n" +
                                          "        public string AnotherMethod2(AnotherData anotherData)\r\n" +
                                          "        {\r\n" +
                                          "            string returnExpression = default(string);\r\n" +
                                          "            SomeNamespace.OtherData condExpression = anotherData.CreateSeveralOtherData()[2];\r\n" +
                                          "            if (condExpression != null)\r\n" +
                                          "            {\r\n" +
                                          "                SomeNamespace.SomeData condExpression2 = condExpression.CreateSomeData();\r\n" +
                                          "                if (condExpression2 != null)\r\n" +
                                          "                {\r\n" +
                                          "                    returnExpression = condExpression2.CreateStr();\r\n" +
                                          "                }\r\n" +
                                          "            }\r\n" +
                                          "            return returnExpression;\r\n" +
                                          "        }\r\n" +
                                          "    }\r\n" +
                                          "}";
            String expectedOutput = "";
            if (outputLevel == OutputLevel.Info)
                expectedOutput = String.Format(ExpectedOutputForInfoLevelTemplate, CreateOnePassOutputs(2));
            TransformerHelper transformerHelper = new TransformerHelper(source, "NullConditionalOperator", outputLevel);
            transformerHelper.Process(_transformerOnFactory, expectedOutput, expectedResult);
            transformerHelper.Process(_transformerOffFactory, "", source);
        }

        [TestCase(OutputLevel.Error)]
        [TestCase(OutputLevel.Warning)]
        [TestCase(OutputLevel.Info)]
        public void ProcessReturnWithNullCoalescingExpr(OutputLevel outputLevel)
        {
            const String source = "namespace SomeNamespace\r\n" +
                                  "{\r\n" +
                                  CommonDefinitions +
                                  "    public class SomeClass\r\n" +
                                  "    {\r\n" +
                                  "        public string AnotherMethod(AnotherData anotherData)\r\n" +
                                  "        {\r\n" +
                                  "            return anotherData.CreateOtherData()?.CreateSomeData()?.CreateStr() ?? \"\";\r\n" +
                                  "        }\r\n" +
                                  "        public string AnotherMethod2(AnotherData anotherData)\r\n" +
                                  "        {\r\n" +
                                  "            return anotherData.CreateOtherData()?.CreateSomeData()?.CreateStr() ?? CreateDefaultStr() ?? \"\";\r\n" +
                                  "        }\r\n" +
                                  "        public string CreateDefaultStr()\r\n" +
                                  "        {\r\n" +
                                  "            return \"\";\r\n" +
                                  "        }\r\n" +
                                  "    }\r\n" +
                                  "}";
            const String expectedResult = "namespace SomeNamespace\r\n" +
                                          "{\r\n" +
                                          CommonDefinitions +
                                          "    public class SomeClass\r\n" +
                                          "    {\r\n" +
                                          "        public string AnotherMethod(AnotherData anotherData)\r\n" +
                                          "        {\r\n" +
                                          "            string returnExpression = default(string);\r\n" +
                                          "            SomeNamespace.OtherData condExpression = anotherData.CreateOtherData();\r\n" +
                                          "            if (condExpression != null)\r\n" +
                                          "            {\r\n" +
                                          "                SomeNamespace.SomeData condExpression2 = condExpression.CreateSomeData();\r\n" +
                                          "                if (condExpression2 != null)\r\n" +
                                          "                {\r\n" +
                                          "                    returnExpression = condExpression2.CreateStr();\r\n" +
                                          "                }\r\n" +
                                          "            }\r\n" +
                                          "            if (returnExpression == null)\r\n" +
                                          "            {\r\n" +
                                          "                returnExpression = \"\";\r\n" +
                                          "            }\r\n" +
                                          "            return returnExpression;\r\n" +
                                          "        }\r\n" +
                                          "        public string AnotherMethod2(AnotherData anotherData)\r\n" +
                                          "        {\r\n" +
                                          "            string returnExpression = default(string);\r\n" +
                                          "            SomeNamespace.OtherData condExpression = anotherData.CreateOtherData();\r\n" +
                                          "            if (condExpression != null)\r\n" +
                                          "            {\r\n" +
                                          "                SomeNamespace.SomeData condExpression2 = condExpression.CreateSomeData();\r\n" +
                                          "                if (condExpression2 != null)\r\n" +
                                          "                {\r\n" +
                                          "                    returnExpression = condExpression2.CreateStr();\r\n" +
                                          "                }\r\n" +
                                          "            }\r\n" +
                                          "            if (returnExpression == null)\r\n" +
                                          "            {\r\n" +
                                          "                returnExpression = CreateDefaultStr() ?? \"\";\r\n" +
                                          "            }\r\n" +
                                          "            return returnExpression;\r\n" +
                                          "        }\r\n" +
                                          "        public string CreateDefaultStr()\r\n" +
                                          "        {\r\n" +
                                          "            return \"\";\r\n" +
                                          "        }\r\n" +
                                          "    }\r\n" +
                                          "}";
            String expectedOutput = "";
            if (outputLevel == OutputLevel.Info)
                expectedOutput = String.Format(ExpectedOutputForInfoLevelTemplate, CreateOnePassOutputs(2));
            TransformerHelper transformerHelper = new TransformerHelper(source, "NullConditionalOperator", outputLevel);
            transformerHelper.Process(_transformerOnFactory, expectedOutput, expectedResult);
            transformerHelper.Process(_transformerOffFactory, "", source);
        }

        [TestCase(OutputLevel.Error)]
        [TestCase(OutputLevel.Warning)]
        [TestCase(OutputLevel.Info)]
        public void ProcessReturnWithNullCoalescingInnerExpr(OutputLevel outputLevel)
        {
            const String source = "namespace SomeNamespace\r\n" +
                                  "{\r\n" +
                                  CommonDefinitions +
                                  "    public class SomeClass\r\n" +
                                  "    {\r\n" +
                                  "        public string AnotherMethod(AnotherData anotherData)\r\n" +
                                  "        {\r\n" +
                                  "            return (anotherData.CreateOtherData() ?? CreateDefaultOtherData() ?? CreateDefaultOtherData2())?.CreateSomeData()?.CreateStr();\r\n" +
                                  "        }\r\n" +
                                  "        public string AnotherMethod2(AnotherData anotherData)\r\n" +
                                  "        {\r\n" +
                                  "            return (anotherData.Process()?.CreateOtherData() ?? CreateDefaultOtherData() ?? CreateDefaultOtherData2())?.CreateSomeData()?.CreateStr() ?? \"\";\r\n" +
                                  "        }\r\n" +
                                  "        public string CreateDefaultStr()\r\n" +
                                  "        {\r\n" +
                                  "            return \"\";\r\n" +
                                  "        }\r\n" +
                                  "        public OtherData CreateDefaultOtherData()\r\n" +
                                  "        {\r\n" +
                                  "            return new OtherData();\r\n" +
                                  "        }\r\n" +
                                  "        public OtherData CreateDefaultOtherData2()\r\n" +
                                  "        {\r\n" +
                                  "            return new OtherData();\r\n" +
                                  "        }\r\n" +
                                  "    }\r\n" +
                                  "}";
            const String expectedResult = "namespace SomeNamespace\r\n" +
                                          "{\r\n" +
                                          CommonDefinitions +
                                          "    public class SomeClass\r\n" +
                                          "    {\r\n" +
                                          "        public string AnotherMethod(AnotherData anotherData)\r\n" +
                                          "        {\r\n" +
                                          "            string returnExpression = default(string);\r\n" +
                                          "            SomeNamespace.OtherData condExpression = (anotherData.CreateOtherData() ?? CreateDefaultOtherData() ?? CreateDefaultOtherData2());\r\n" +
                                          "            if (condExpression != null)\r\n" +
                                          "            {\r\n" +
                                          "                SomeNamespace.SomeData condExpression2 = condExpression.CreateSomeData();\r\n" +
                                          "                if (condExpression2 != null)\r\n" +
                                          "                {\r\n" +
                                          "                    returnExpression = condExpression2.CreateStr();\r\n" +
                                          "                }\r\n" +
                                          "            }\r\n" +
                                          "            return returnExpression;\r\n" +
                                          "        }\r\n" +
                                          "        public string AnotherMethod2(AnotherData anotherData)\r\n" +
                                          "        {\r\n" +
                                          "            SomeNamespace.OtherData expression = default(SomeNamespace.OtherData);\r\n" +
                                          "            SomeNamespace.AnotherData condExpression = anotherData.Process();\r\n" +
                                          "            if (condExpression != null)\r\n" +
                                          "            {\r\n" +
                                          "                expression = condExpression.CreateOtherData();\r\n" +
                                          "            }\r\n" +
                                          "            if (expression == null)\r\n" +
                                          "            {\r\n" +
                                          "                expression = CreateDefaultOtherData() ?? CreateDefaultOtherData2();\r\n" +
                                          "            }\r\n" +
                                          "            string returnExpression = default(string);\r\n" +
                                          "            SomeNamespace.OtherData condExpression2 = expression;\r\n" +
                                          "            if (condExpression2 != null)\r\n" +
                                          "            {\r\n" +
                                          "                SomeNamespace.SomeData condExpression3 = condExpression2.CreateSomeData();\r\n" +
                                          "                if (condExpression3 != null)\r\n" +
                                          "                {\r\n" +
                                          "                    returnExpression = condExpression3.CreateStr();\r\n" +
                                          "                }\r\n" +
                                          "            }\r\n" +
                                          "            if (returnExpression == null)\r\n" +
                                          "            {\r\n" +
                                          "                returnExpression = \"\";\r\n" +
                                          "            }\r\n" +
                                          "            return returnExpression;\r\n" +
                                          "        }\r\n" +
                                          "        public string CreateDefaultStr()\r\n" +
                                          "        {\r\n" +
                                          "            return \"\";\r\n" +
                                          "        }\r\n" +
                                          "        public OtherData CreateDefaultOtherData()\r\n" +
                                          "        {\r\n" +
                                          "            return new OtherData();\r\n" +
                                          "        }\r\n" +
                                          "        public OtherData CreateDefaultOtherData2()\r\n" +
                                          "        {\r\n" +
                                          "            return new OtherData();\r\n" +
                                          "        }\r\n" +
                                          "    }\r\n" +
                                          "}";
            String expectedOutput = "";
            if (outputLevel == OutputLevel.Info)
            {
                String iterations = String.Join("", new[] {CreateNPassOutput(1), CreateNPassOutput(2)});
                expectedOutput = String.Format(ExpectedOutputForInfoLevelTemplate, iterations);
            }
            TransformerHelper transformerHelper = new TransformerHelper(source, "NullConditionalOperator", outputLevel);
            transformerHelper.Process(_transformerOnFactory, expectedOutput, expectedResult);
            transformerHelper.Process(_transformerOffFactory, "", source);
        }

        [TestCase(OutputLevel.Error)]
        [TestCase(OutputLevel.Warning)]
        [TestCase(OutputLevel.Info)]
        public void ProcessComplexCaseOfInnerExpr(OutputLevel outputLevel)
        {
            const String source = "namespace SomeNamespace\r\n" +
                                  "{\r\n" +
                                  CommonDefinitions +
                                  "    public class SomeClass\r\n" +
                                  "    {\r\n" +
                                  "        public void SomeMethod()\r\n" +
                                  "        {\r\n" +
                                  "            AnotherData anotherData = new AnotherData();\r\n" +
                                  "            string value = ((anotherData.Process()?.CreateOtherData() ?? CreateDefaultOtherData())?.Process() ?? CreateDefaultOtherData())?.CreateSomeData()?.CreateStr() ?? \"\";\r\n" +
                                  "        }\r\n" +
                                  "        public OtherData CreateDefaultOtherData()\r\n" +
                                  "        {\r\n" +
                                  "            return new OtherData();\r\n" +
                                  "        }\r\n" +
                                  "        public string AnotherMethod(AnotherData anotherData)\r\n" +
                                  "        {\r\n" +
                                  "            return ((anotherData.Process()?.CreateOtherData() ?? CreateDefaultOtherData())?.Process() ?? CreateDefaultOtherData())?.CreateSomeData()?.CreateStr() ?? \"\";\r\n" +
                                  "        }\r\n" +
                                  "    }\r\n" +
                                  "}";
            const String expectedResult = "namespace SomeNamespace\r\n" +
                                          "{\r\n" +
                                          CommonDefinitions +
                                          "    public class SomeClass\r\n" +
                                          "    {\r\n" +
                                          "        public void SomeMethod()\r\n" +
                                          "        {\r\n" +
                                          "            AnotherData anotherData = new AnotherData();\r\n" +
                                          "            SomeNamespace.OtherData expression = default(SomeNamespace.OtherData);\r\n" +
                                          "            SomeNamespace.AnotherData condExpression = anotherData.Process();\r\n" +
                                          "            if (condExpression != null)\r\n" +
                                          "            {\r\n" +
                                          "                expression = condExpression.CreateOtherData();\r\n" +
                                          "            }\r\n" +
                                          "            if (expression == null)\r\n" +
                                          "            {\r\n" +
                                          "                expression = CreateDefaultOtherData();\r\n" +
                                          "            }\r\n" +
                                          "            SomeNamespace.OtherData expression2 = default(SomeNamespace.OtherData);\r\n" +
                                          "            SomeNamespace.OtherData condExpression2 = expression;\r\n" +
                                          "            if (condExpression2 != null)\r\n" +
                                          "            {\r\n" +
                                          "                expression2 = condExpression2.Process();\r\n" +
                                          "            }\r\n" +
                                          "            if (expression2 == null)\r\n" +
                                          "            {\r\n" +
                                          "                expression2 = CreateDefaultOtherData();\r\n" +
                                          "            }\r\n" +
                                          "            string value = default(string);\r\n" +
                                          "            SomeNamespace.OtherData condExpression3 = expression2;\r\n" +
                                          "            if (condExpression3 != null)\r\n" +
                                          "            {\r\n" +
                                          "                SomeNamespace.SomeData condExpression4 = condExpression3.CreateSomeData();\r\n" +
                                          "                if (condExpression4 != null)\r\n" +
                                          "                {\r\n" +
                                          "                    value = condExpression4.CreateStr();\r\n" +
                                          "                }\r\n" +
                                          "            }\r\n" +
                                          "            if (value == null)\r\n" +
                                          "            {\r\n" +
                                          "                value = \"\";\r\n" +
                                          "            }\r\n" +
                                          "        }\r\n" +
                                          "        public OtherData CreateDefaultOtherData()\r\n" +
                                          "        {\r\n" +
                                          "            return new OtherData();\r\n" +
                                          "        }\r\n" +
                                          "        public string AnotherMethod(AnotherData anotherData)\r\n" +
                                          "        {\r\n" +
                                          "            SomeNamespace.OtherData expression = default(SomeNamespace.OtherData);\r\n" +
                                          "            SomeNamespace.AnotherData condExpression = anotherData.Process();\r\n" +
                                          "            if (condExpression != null)\r\n" +
                                          "            {\r\n" +
                                          "                expression = condExpression.CreateOtherData();\r\n" +
                                          "            }\r\n" +
                                          "            if (expression == null)\r\n" +
                                          "            {\r\n" +
                                          "                expression = CreateDefaultOtherData();\r\n" +
                                          "            }\r\n" +
                                          "            SomeNamespace.OtherData expression2 = default(SomeNamespace.OtherData);\r\n" +
                                          "            SomeNamespace.OtherData condExpression2 = expression;\r\n" +
                                          "            if (condExpression2 != null)\r\n" +
                                          "            {\r\n" +
                                          "                expression2 = condExpression2.Process();\r\n" +
                                          "            }\r\n" +
                                          "            if (expression2 == null)\r\n" +
                                          "            {\r\n" +
                                          "                expression2 = CreateDefaultOtherData();\r\n" +
                                          "            }\r\n" +
                                          "            string returnExpression = default(string);\r\n" +
                                          "            SomeNamespace.OtherData condExpression3 = expression2;\r\n" +
                                          "            if (condExpression3 != null)\r\n" +
                                          "            {\r\n" +
                                          "                SomeNamespace.SomeData condExpression4 = condExpression3.CreateSomeData();\r\n" +
                                          "                if (condExpression4 != null)\r\n" +
                                          "                {\r\n" +
                                          "                    returnExpression = condExpression4.CreateStr();\r\n" +
                                          "                }\r\n" +
                                          "            }\r\n" +
                                          "            if (returnExpression == null)\r\n" +
                                          "            {\r\n" +
                                          "                returnExpression = \"\";\r\n" +
                                          "            }\r\n" +
                                          "            return returnExpression;\r\n" +
                                          "        }\r\n" +
                                          "    }\r\n" +
                                          "}";
            String expectedOutput = "";
            if (outputLevel == OutputLevel.Info)
            {
                String iterations = String.Join("", new[] {CreateNPassOutput(3), CreateNPassOutput(3)});
                expectedOutput = String.Format(ExpectedOutputForInfoLevelTemplate, iterations);
            }
            TransformerHelper transformerHelper = new TransformerHelper(source, "NullConditionalOperator", outputLevel);
            transformerHelper.Process(_transformerOnFactory, expectedOutput, expectedResult);
            transformerHelper.Process(_transformerOffFactory, "", source);
        }

        private readonly Func<IOutput, ITransformer> _transformerOnFactory = output => new NullConditionalOperatorTransformer(output, TransformerState.On);
        private readonly Func<IOutput, ITransformer> _transformerOffFactory = output => new NullConditionalOperatorTransformer(output, TransformerState.Off);

        private const String CommonDefinitions = "    public class SomeData\r\n" +
                                                 "    {\r\n" +
                                                 "        public void DoIt()\r\n" +
                                                 "        {\r\n" +
                                                 "        }\r\n" +
                                                 "        public string CreateStr()\r\n" +
                                                 "        {\r\n" +
                                                 "            return \"\";\r\n" +
                                                 "        }\r\n" +
                                                 "    }\r\n" +
                                                 "    public class OtherData\r\n" +
                                                 "    {\r\n" +
                                                 "        public SomeData CreateSomeData()\r\n" +
                                                 "        {\r\n" +
                                                 "            return new SomeData();\r\n" +
                                                 "        }\r\n" +
                                                 "        public OtherData Process()\r\n" +
                                                 "        {\r\n" +
                                                 "            return this;\r\n" +
                                                 "        }\r\n" +
                                                 "        public void CopyFrom(OtherData data)\r\n" +
                                                 "        {\r\n" +
                                                 "        }\r\n" +
                                                 "    }\r\n" +
                                                 "    public class AnotherData\r\n" +
                                                 "    {\r\n" +
                                                 "        public OtherData CreateOtherData()\r\n" +
                                                 "        {\r\n" +
                                                 "            return new OtherData();\r\n" +
                                                 "        }\r\n" +
                                                 "        public OtherData[] CreateSeveralOtherData()\r\n" +
                                                 "        {\r\n" +
                                                 "            return new OtherData[10];\r\n" +
                                                 "        }\r\n" +
                                                 "        public AnotherData Process()\r\n" +
                                                 "        {\r\n" +
                                                 "            return this;\r\n" +
                                                 "        }\r\n" +
                                                 "    }\r\n";

        private String CreateOnePassOutputs(Int32 count)
        {
            String onePassTransformOutputForInfoLevel = string.Format(IterationTransformOutputForInfoLevel, 1);
            return String.Join("", Enumerable.Range(0, count).Select(_ => onePassTransformOutputForInfoLevel));
        }

        private String CreateNPassOutput(Int32 passCount)
        {
            return String.Join("", Enumerable.Range(1, passCount).Select(iteration => String.Format(IterationTransformOutputForInfoLevel, iteration)));
        }

        private const String ExpectedOutputForInfoLevelTemplate = $"Execution of {NullConditionalOperatorTransformer.Name} started\r\n" +
                                                                  "{0}" +
                                                                  $"Execution of {NullConditionalOperatorTransformer.Name} finished\r\n";

        private const String IterationTransformOutputForInfoLevel = "Transformation iteration number {0} started\r\n" +
                                                                    "Transformation iteration number {0} finished\r\n";
    }
}
