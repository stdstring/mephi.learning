using NUnit.Framework;
using SourceCodeSimplifierApp.Config;
using SourceCodeSimplifierApp.Output;
using SourceCodeSimplifierApp.Transformers;
using SourceCodeSimplifierAppTests.TestUtils;

namespace SourceCodeSimplifierAppTests.Transformers
{
    [TestFixture]
    public class ObjectInitializerExprTransformerTests
    {
        [TestCase(OutputLevel.Error)]
        [TestCase(OutputLevel.Warning)]
        [TestCase(OutputLevel.Info)]
        public void ProcessWithObjectInitializerExpressions(OutputLevel outputLevel)
        {
            const String commonDefinitions = "    public class SomeOuterData\r\n" +
                                             "    {\r\n" +
                                             "        public SomeOuterData(int outerField1, string outerField2, bool outerField3)\r\n" +
                                             "        {\r\n" +
                                             "            OuterField1 = outerField1;\r\n" +
                                             "            OuterField2 = outerField2;\r\n" +
                                             "            OuterField3 = outerField3;\r\n" +
                                             "            InnerData = new SomeInnerData(777, \"IDKFA\");\r\n" +
                                             "        }\r\n" +
                                             "        public int OuterField1;\r\n" +
                                             "        public string OuterField2;\r\n" +
                                             "        public bool OuterField3;\r\n" +
                                             "        public SomeInnerData InnerData;\r\n" +
                                             "    }\r\n" +
                                             "    public class SomeInnerData\r\n" +
                                             "    {\r\n" +
                                             "        public SomeInnerData(int innerField1, string innerField2)\r\n" +
                                             "        {\r\n" +
                                             "            InnerField1 = innerField1;\r\n" +
                                             "            InnerField2 = innerField2;\r\n" +
                                             "        }\r\n" +
                                             "        public int InnerField1;\r\n" +
                                             "        public string InnerField2;\r\n" +
                                             "    }\r\n";
            const String source = "namespace SomeNamespace\r\n" +
                                  "{\r\n" +
                                  commonDefinitions +
                                  "    public class SomeClass\r\n" +
                                  "    {\r\n" +
                                  "        public void SomeMethod(int i)\r\n" +
                                  "        {\r\n" +
                                  "            SomeOuterData data = new SomeOuterData(1, \"-\", false)\r\n" +
                                  "            {\r\n" +
                                  "                OuterField1 = 666,\r\n" +
                                  "                OuterField2 = \"IDDQD\",\r\n" +
                                  "                OuterField3 = true,\r\n" +
                                  "                InnerData = new SomeInnerData(13, \"---\")\r\n" +
                                  "                {\r\n" +
                                  "                    InnerField1 = 888,\r\n" +
                                  "                    InnerField2 = \"IDCLIP\"\r\n" +
                                  "                }\r\n" +
                                  "            };\r\n" +
                                  "            if (i > 0)\r\n" +
                                  "            {\r\n" +
                                  "                data.InnerData = new SomeInnerData(73, \"+-+\")\r\n" +
                                  "                {\r\n" +
                                  "                    InnerField1 = 22227,\r\n" +
                                  "                    InnerField2 = \"DNMD\"\r\n" +
                                  "                };\r\n" +
                                  "            }\r\n" +
                                  "            else\r\n" +
                                  "                data.InnerData = new SomeInnerData(23, \"++\")\r\n" +
                                  "                {\r\n" +
                                  "                    InnerField1 = 11117,\r\n" +
                                  "                    InnerField2 = \"DNMD\"\r\n" +
                                  "                };\r\n" +
                                  "        }\r\n" +
                                  "    }\r\n" +
                                  "}";
            const String expectedResult = "namespace SomeNamespace\r\n" +
                                          "{\r\n" +
                                          commonDefinitions +
                                          "    public class SomeClass\r\n" +
                                          "    {\r\n" +
                                          "        public void SomeMethod(int i)\r\n" +
                                          "        {\r\n" +
                                          "            SomeOuterData data = new SomeOuterData(1, \"-\", false);\r\n" +
                                          "            data.OuterField1 = 666;\r\n" +
                                          "            data.OuterField2 = \"IDDQD\";\r\n" +
                                          "            data.OuterField3 = true;\r\n" +
                                          "            data.InnerData = new SomeInnerData(13, \"---\");\r\n" +
                                          "            data.InnerData.InnerField1 = 888;\r\n" +
                                          "            data.InnerData.InnerField2 = \"IDCLIP\";\r\n" +
                                          "            if (i > 0)\r\n" +
                                          "            {\r\n" +
                                          "                data.InnerData = new SomeInnerData(73, \"+-+\");\r\n" +
                                          "                data.InnerData.InnerField1 = 22227;\r\n" +
                                          "                data.InnerData.InnerField2 = \"DNMD\";\r\n" +
                                          "            }\r\n" +
                                          "            else\r\n" +
                                          "            {\r\n" +
                                          "                data.InnerData = new SomeInnerData(23, \"++\");\r\n" +
                                          "                data.InnerData.InnerField1 = 11117;\r\n" +
                                          "                data.InnerData.InnerField2 = \"DNMD\";\r\n" +
                                          "            }\r\n" +
                                          "        }\r\n" +
                                          "    }\r\n" +
                                          "}";
            String expectedOutput = outputLevel == OutputLevel.Info ? ExpectedOutputForInfoLevel : "";
            TransformerHelper transformerHelper = new TransformerHelper(source, "ObjectInitializerExpression", outputLevel);
            transformerHelper.Process(_transformerOnFactory, expectedOutput, expectedResult);
            transformerHelper.Process(_transformerOffFactory, "", source);
        }

        [TestCase(OutputLevel.Error)]
        [TestCase(OutputLevel.Warning)]
        [TestCase(OutputLevel.Info)]
        public void ProcessWithoutObjectInitializerExpressions(OutputLevel outputLevel)
        {
            const String commonDefinitions = "    public class SomeOuterData\r\n" +
                                             "    {\r\n" +
                                             "        public SomeOuterData(int outerField1, string outerField2, bool outerField3)\r\n" +
                                             "        {\r\n" +
                                             "            OuterField1 = outerField1;\r\n" +
                                             "            OuterField2 = outerField2;\r\n" +
                                             "            OuterField3 = outerField3;\r\n" +
                                             "            InnerData = new SomeInnerData(777, \"IDKFA\");\r\n" +
                                             "        }\r\n" +
                                             "        public int OuterField1;\r\n" +
                                             "        public string OuterField2;\r\n" +
                                             "        public bool OuterField3;\r\n" +
                                             "        public SomeInnerData InnerData;\r\n" +
                                             "    }\r\n" +
                                             "    public class SomeInnerData\r\n" +
                                             "    {\r\n" +
                                             "        public SomeInnerData(int innerField1, string innerField2)\r\n" +
                                             "        {\r\n" +
                                             "            InnerField1 = innerField1;\r\n" +
                                             "            InnerField2 = innerField2;\r\n" +
                                             "        }\r\n" +
                                             "        public int InnerField1;\r\n" +
                                             "        public string InnerField2;\r\n" +
                                             "    }";
            const String source = "namespace SomeNamespace\r\n" +
                                  "{\r\n" +
                                  commonDefinitions +
                                  "    public class SomeClass\r\n" +
                                  "    {\r\n" +
                                  "        public void SomeMethod(int i)\r\n" +
                                  "        {\r\n" +
                                  "            SomeOuterData data = new SomeOuterData(1, \"-\", false);\r\n" +
                                  "            data.OuterField1 = 666;\r\n" +
                                  "            data.OuterField2 = \"IDDQD\";\r\n" +
                                  "            data.OuterField3 = true;\r\n" +
                                  "            data.InnerData = new SomeInnerData(13, \"---\");\r\n" +
                                  "            data.InnerData.InnerField1 = 888;\r\n" +
                                  "            data.InnerData.InnerField2 = \"IDCLIP\";\r\n" +
                                  "            if (i > 0)\r\n" +
                                  "            {\r\n" +
                                  "                data.InnerData = new SomeInnerData(73, \"+-+\");\r\n" +
                                  "                data.InnerData.InnerField1 = 22227;\r\n" +
                                  "                data.InnerData.InnerField2 = \"DNMD\";\r\n" +
                                  "            }\r\n" +
                                  "            else\r\n" +
                                  "            {\r\n" +
                                  "                data.InnerData = new SomeInnerData(23, \"++\");\r\n" +
                                  "                data.InnerData.InnerField1 = 11117;\r\n" +
                                  "                data.InnerData.InnerField2 = \"DNMD\";\r\n" +
                                  "            }\r\n" +
                                  "        }\r\n" +
                                  "    }\r\n" +
                                  "}";
            String expectedOutput = outputLevel == OutputLevel.Info ? ExpectedOutputForInfoLevel : "";
            TransformerHelper transformerHelper = new TransformerHelper(source, "ObjectInitializerExpression", outputLevel);
            transformerHelper.Process(_transformerOnFactory, expectedOutput, source);
            transformerHelper.Process(_transformerOffFactory, "", source);
        }

        [TestCase(OutputLevel.Error)]
        [TestCase(OutputLevel.Warning)]
        [TestCase(OutputLevel.Info)]
        public void ProcessWithObjectInitializerExpressionsWithComments(OutputLevel outputLevel)
        {
            const String commonDefinitions = "    public class SomeInnerData\r\n" +
                                             "    {\r\n" +
                                             "        public SomeInnerData(int innerField1, string innerField2)\r\n" +
                                             "        {\r\n" +
                                             "            InnerField1 = innerField1;\r\n" +
                                             "            InnerField2 = innerField2;\r\n" +
                                             "        }\r\n" +
                                             "        public int InnerField1;\r\n" +
                                             "        public string InnerField2;\r\n" +
                                             "    }\r\n" +
                                             "    public class SomeOuterData\r\n" +
                                             "    {\r\n" +
                                             "        public SomeOuterData(int outerField1, string outerField2)\r\n" +
                                             "        {\r\n" +
                                             "            OuterField1 = outerField1;\r\n" +
                                             "            OuterField2 = outerField2;\r\n" +
                                             "            InnerData = new SomeInnerData(13, \"===\");\r\n" +
                                             "        }\r\n" +
                                             "        public int OuterField1;\r\n" +
                                             "        public string OuterField2;\r\n" +
                                             "        public SomeInnerData InnerData;\r\n" +
                                             "    }\r\n";
            const String source = "namespace SomeNamespace\r\n" +
                                  "{\r\n" +
                                  commonDefinitions +
                                  "    public class SomeClass\r\n" +
                                  "    {\r\n" +
                                  "        public void SomeMethod(int i)\r\n" +
                                  "        {\r\n" +
                                  "            // Comment 1: bla bla bla 1\r\n" +
                                  "            // Comment 2: bla bla bla 2\r\n" +
                                  "            SomeOuterData data = new SomeOuterData(66, \"++\") // SomeOuterData ctor Footer comment 1\r\n" +
                                  "            {\r\n" +
                                  "                // OuterField1 comment 1: bla bla bla 1\r\n" +
                                  "                // OuterField1 comment 2: bla bla bla 2\r\n" +
                                  "                OuterField1 = 666, // OuterField1 Footer comment 1\r\n" +
                                  "                // OuterField2 comment 1: bla bla bla 1\r\n" +
                                  "                // OuterField2 comment 2: bla bla bla 2\r\n" +
                                  "                OuterField2 = \"IDDQD\", // OuterField2 Footer comment 1\r\n" +
                                  "                // InnerData comment 1: bla bla bla 1\r\n" +
                                  "                // InnerData comment 2: bla bla bla 2\r\n" +
                                  "                InnerData = new SomeInnerData(99, \"++--++\") // SomeInnerData ctor Footer comment 1\r\n" +
                                  "                {\r\n" +
                                  "                    // InnerField1 comment 1: bla bla bla 1\r\n" +
                                  "                    // InnerField1 comment 2: bla bla bla 2\r\n" +
                                  "                    InnerField1 = 999, // InnerField1 Footer comment 1\r\n" +
                                  "                    // InnerField2 comment 1: bla bla bla 1\r\n" +
                                  "                    // InnerField2 comment 2: bla bla bla 2\r\n" +
                                  "                    InnerField2 = \"IDKFA\" // InnerField2 Footer comment 1\r\n" +
                                  "                } // SomeInnerData Footer comment 1\r\n" +
                                  "            }; // SomeOuterData Footer comment 1\r\n" +
                                  "            if (i > 0)\r\n" +
                                  "            {\r\n" +
                                  "                // Comment 11: bla bla bla 11\r\n" +
                                  "                // Comment 12: bla bla bla 12\r\n" +
                                  "                data = new SomeOuterData(13, \"!!!\") // SomeOuterData ctor Footer comment 11\r\n" +
                                  "                {\r\n" +
                                  "                    // OuterField1 comment 11: bla bla bla 11\r\n" +
                                  "                    // OuterField1 comment 12: bla bla bla 12\r\n" +
                                  "                    OuterField1 = 777, // OuterField1 Footer comment 11\r\n" +
                                  "                    // OuterField2 comment 11: bla bla bla 11\r\n" +
                                  "                    // OuterField2 comment 12: bla bla bla 12\r\n" +
                                  "                    OuterField2 = \"IMPULSE 666\", // OuterField2 Footer comment 11\r\n" +
                                  "                    // InnerData comment 11: bla bla bla 11\r\n" +
                                  "                    // InnerData comment 12: bla bla bla 12\r\n" +
                                  "                    InnerData = new SomeInnerData(19, \"=+=\") // SomeInnerData ctor Footer comment 11\r\n" +
                                  "                    {\r\n" +
                                  "                        // InnerField1 comment 11: bla bla bla 11\r\n" +
                                  "                        // InnerField1 comment 12: bla bla bla 12\r\n" +
                                  "                        InnerField1 = 878, // InnerField1 Footer comment 11\r\n" +
                                  "                        // InnerField2 comment 1: bla bla bla 11\r\n" +
                                  "                        // InnerField2 comment 2: bla bla bla 12\r\n" +
                                  "                        InnerField2 = \"DNMD\" // InnerField2 Footer comment 11\r\n" +
                                  "                    } // SomeInnerData Footer comment 11\r\n" +
                                  "                }; // SomeOuterData Footer comment 11\r\n" +
                                  "            }\r\n" +
                                  "            else\r\n" +
                                  "                // Comment 21: bla bla bla 21\r\n" +
                                  "                // Comment 22: bla bla bla 22\r\n" +
                                  "                data = new SomeOuterData(17, \"(+=+)\") // SomeOuterData ctor Footer comment 21\r\n" +
                                  "                {\r\n" +
                                  "                    // OuterField1 comment 21: bla bla bla 21\r\n" +
                                  "                    // OuterField1 comment 22: bla bla bla 22\r\n" +
                                  "                    OuterField1 = 565, // OuterField1 Footer comment 21\r\n" +
                                  "                    // OuterField2 comment 21: bla bla bla 21\r\n" +
                                  "                    // OuterField2 comment 22: bla bla bla 22\r\n" +
                                  "                    OuterField2 = \"IDCLIP\", // OuterField2 Footer comment 21\r\n" +
                                  "                    // InnerData comment 21: bla bla bla 21\r\n" +
                                  "                    // InnerData comment 22: bla bla bla 22\r\n" +
                                  "                    InnerData = new SomeInnerData(37, \"DNMD\") // SomeInnerData ctor Footer comment 21\r\n" +
                                  "                    {\r\n" +
                                  "                        // InnerField1 comment 21: bla bla bla 21\r\n" +
                                  "                        // InnerField1 comment 22: bla bla bla 22\r\n" +
                                  "                        InnerField1 = 444, // InnerField1 Footer comment 21\r\n" +
                                  "                        // InnerField2 comment 1: bla bla bla 21\r\n" +
                                  "                        // InnerField2 comment 2: bla bla bla 22\r\n" +
                                  "                        InnerField2 = \"IDCLIP,IDCLIP\" // InnerField2 Footer comment 21\r\n" +
                                  "                    } // SomeInnerData Footer comment 21\r\n" +
                                  "                }; // SomeOuterData Footer comment 21\r\n" +
                                  "        }\r\n" +
                                  "    }\r\n" +
                                  "}";
            const String expectedResult = "namespace SomeNamespace\r\n" +
                                          "{\r\n" +
                                          commonDefinitions +
                                          "    public class SomeClass\r\n" +
                                          "    {\r\n" +
                                          "        public void SomeMethod(int i)\r\n" +
                                          "        {\r\n" +
                                          "            // Comment 1: bla bla bla 1\r\n" +
                                          "            // Comment 2: bla bla bla 2\r\n" +
                                          "            SomeOuterData data = new SomeOuterData(66, \"++\"); // SomeOuterData Footer comment 1\r\n" +
                                          "            // OuterField1 comment 1: bla bla bla 1\r\n" +
                                          "            // OuterField1 comment 2: bla bla bla 2\r\n" +
                                          "            data.OuterField1 = 666; // OuterField1 Footer comment 1\r\n" +
                                          "            // OuterField2 comment 1: bla bla bla 1\r\n" +
                                          "            // OuterField2 comment 2: bla bla bla 2\r\n" +
                                          "            data.OuterField2 = \"IDDQD\"; // OuterField2 Footer comment 1\r\n" +
                                          "            // InnerData comment 1: bla bla bla 1\r\n" +
                                          "            // InnerData comment 2: bla bla bla 2\r\n" +
                                          "            data.InnerData = new SomeInnerData(99, \"++--++\"); // SomeInnerData ctor Footer comment 1\r\n" +
                                          "            // InnerField1 comment 1: bla bla bla 1\r\n" +
                                          "            // InnerField1 comment 2: bla bla bla 2\r\n" +
                                          "            data.InnerData.InnerField1 = 999; // InnerField1 Footer comment 1\r\n" +
                                          "            // InnerField2 comment 1: bla bla bla 1\r\n" +
                                          "            // InnerField2 comment 2: bla bla bla 2\r\n" +
                                          "            data.InnerData.InnerField2 = \"IDKFA\"; // InnerField2 Footer comment 1\r\n" +
                                          "            if (i > 0)\r\n" +
                                          "            {\r\n" +
                                          "                // Comment 11: bla bla bla 11\r\n" +
                                          "                // Comment 12: bla bla bla 12\r\n" +
                                          "                data = new SomeOuterData(13, \"!!!\"); // SomeOuterData Footer comment 11\r\n" +
                                          "                // OuterField1 comment 11: bla bla bla 11\r\n" +
                                          "                // OuterField1 comment 12: bla bla bla 12\r\n" +
                                          "                data.OuterField1 = 777; // OuterField1 Footer comment 11\r\n" +
                                          "                // OuterField2 comment 11: bla bla bla 11\r\n" +
                                          "                // OuterField2 comment 12: bla bla bla 12\r\n" +
                                          "                data.OuterField2 = \"IMPULSE 666\"; // OuterField2 Footer comment 11\r\n" +
                                          "                // InnerData comment 11: bla bla bla 11\r\n" +
                                          "                // InnerData comment 12: bla bla bla 12\r\n" +
                                          "                data.InnerData = new SomeInnerData(19, \"=+=\"); // SomeInnerData ctor Footer comment 11\r\n" +
                                          "                // InnerField1 comment 11: bla bla bla 11\r\n" +
                                          "                // InnerField1 comment 12: bla bla bla 12\r\n" +
                                          "                data.InnerData.InnerField1 = 878; // InnerField1 Footer comment 11\r\n" +
                                          "                // InnerField2 comment 1: bla bla bla 11\r\n" +
                                          "                // InnerField2 comment 2: bla bla bla 12\r\n" +
                                          "                data.InnerData.InnerField2 = \"DNMD\"; // InnerField2 Footer comment 11\r\n" +
                                          "            }\r\n" +
                                          "            else\r\n" +
                                          "            {\r\n" +
                                          "                // Comment 21: bla bla bla 21\r\n" +
                                          "                // Comment 22: bla bla bla 22\r\n" +
                                          "                data = new SomeOuterData(17, \"(+=+)\"); // SomeOuterData Footer comment 21\r\n" +
                                          "                // OuterField1 comment 21: bla bla bla 21\r\n" +
                                          "                // OuterField1 comment 22: bla bla bla 22\r\n" +
                                          "                data.OuterField1 = 565; // OuterField1 Footer comment 21\r\n" +
                                          "                // OuterField2 comment 21: bla bla bla 21\r\n" +
                                          "                // OuterField2 comment 22: bla bla bla 22\r\n" +
                                          "                data.OuterField2 = \"IDCLIP\"; // OuterField2 Footer comment 21\r\n" +
                                          "                // InnerData comment 21: bla bla bla 21\r\n" +
                                          "                // InnerData comment 22: bla bla bla 22\r\n" +
                                          "                data.InnerData = new SomeInnerData(37, \"DNMD\"); // SomeInnerData ctor Footer comment 21\r\n" +
                                          "                // InnerField1 comment 21: bla bla bla 21\r\n" +
                                          "                // InnerField1 comment 22: bla bla bla 22\r\n" +
                                          "                data.InnerData.InnerField1 = 444; // InnerField1 Footer comment 21\r\n" +
                                          "                // InnerField2 comment 1: bla bla bla 21\r\n" +
                                          "                // InnerField2 comment 2: bla bla bla 22\r\n" +
                                          "                data.InnerData.InnerField2 = \"IDCLIP,IDCLIP\"; // InnerField2 Footer comment 21\r\n" +
                                          "            }\r\n" +
                                          "        }\r\n" +
                                          "    }\r\n" +
                                          "}";
            const String expectedWarning = "ObjectInitializerExpressionDocument(31): [WARNING]: Unprocessed (lost) trailing comment: \"// SomeOuterData ctor Footer comment 1\"\r\n" +
                                           "ObjectInitializerExpressionDocument(41): [WARNING]: Unprocessed (lost) trailing comment: \"// SomeInnerData ctor Footer comment 1\"\r\n" +
                                           "ObjectInitializerExpressionDocument(55): [WARNING]: Unprocessed (lost) trailing comment: \"// SomeOuterData ctor Footer comment 11\"\r\n" +
                                           "ObjectInitializerExpressionDocument(65): [WARNING]: Unprocessed (lost) trailing comment: \"// SomeInnerData ctor Footer comment 11\"\r\n" +
                                           "ObjectInitializerExpressionDocument(79): [WARNING]: Unprocessed (lost) trailing comment: \"// SomeOuterData ctor Footer comment 21\"\r\n" +
                                           "ObjectInitializerExpressionDocument(89): [WARNING]: Unprocessed (lost) trailing comment: \"// SomeInnerData ctor Footer comment 21\"\r\n";
            String expectedOutput = outputLevel switch
            {
                OutputLevel.Error => "",
                OutputLevel.Warning => expectedWarning,
                OutputLevel.Info => String.Format(ExpectedOutputForInfoLevelTemplate, expectedWarning),
                _ => throw new InvalidOperationException($"Unexpected OutputLevel: {outputLevel}")
            };
            TransformerHelper transformerHelper = new TransformerHelper(source, "ObjectInitializerExpression", outputLevel);
            transformerHelper.Process(_transformerOnFactory, expectedOutput, expectedResult);
            transformerHelper.Process(_transformerOffFactory, "", source);
        }

        [TestCase(OutputLevel.Error)]
        [TestCase(OutputLevel.Warning)]
        [TestCase(OutputLevel.Info)]
        public void ProcessWithObjectInitializerExpressionsInOneLine(OutputLevel outputLevel)
        {
            const String commonDefinitions = "    public class SomeInnerData\r\n" +
                                             "    {\r\n" +
                                             "        public SomeInnerData(int innerField1, string innerField2)\r\n" +
                                             "        {\r\n" +
                                             "            InnerField1 = innerField1;\r\n" +
                                             "            InnerField2 = innerField2;\r\n" +
                                             "        }\r\n" +
                                             "        public int InnerField1;\r\n" +
                                             "        public string InnerField2;\r\n" +
                                             "    }\r\n" +
                                             "    public class SomeOuterData\r\n" +
                                             "    {\r\n" +
                                             "        public SomeOuterData(int outerField1, string outerField2)\r\n" +
                                             "        {\r\n" +
                                             "            OuterField1 = outerField1;\r\n" +
                                             "            OuterField2 = outerField2;\r\n" +
                                             "            InnerData = new SomeInnerData(13, \"===\");\r\n" +
                                             "        }\r\n" +
                                             "        public int OuterField1;\r\n" +
                                             "        public string OuterField2;\r\n" +
                                             "        public SomeInnerData InnerData;\r\n" +
                                             "    }\r\n";
            const String source = "namespace SomeNamespace\r\n" +
                                  "{\r\n" +
                                  commonDefinitions +
                                  "    public class SomeClass\r\n" +
                                  "    {\r\n" +
                                  "        public void SomeMethod()\r\n" +
                                  "        {\r\n" +
                                  "            SomeOuterData data = new SomeOuterData(1, \"=\") {OuterField1 = 666, OuterField2 = \"IDDQD\", InnerData = new SomeInnerData(2, \"+\"){InnerField1 = 999, InnerField2 = \"IDKFA\"}};\r\n" +
                                  "        }\r\n" +
                                  "    }\r\n" +
                                  "}";
            const String expectedResult = "namespace SomeNamespace\r\n" +
                                          "{\r\n" +
                                          commonDefinitions +
                                          "    public class SomeClass\r\n" +
                                          "    {\r\n" +
                                          "        public void SomeMethod()\r\n" +
                                          "        {\r\n" +
                                          "            SomeOuterData data = new SomeOuterData(1, \"=\");\r\n" +
                                          "            data.OuterField1 = 666;\r\n" +
                                          "            data.OuterField2 = \"IDDQD\";\r\n" +
                                          "            data.InnerData = new SomeInnerData(2, \"+\");\r\n" +
                                          "            data.InnerData.InnerField1 = 999;\r\n" +
                                          "            data.InnerData.InnerField2 = \"IDKFA\";\r\n" +
                                          "        }\r\n" +
                                          "    }\r\n" +
                                          "}";
            String expectedOutput = outputLevel == OutputLevel.Info ? ExpectedOutputForInfoLevel : "";
            TransformerHelper transformerHelper = new TransformerHelper(source, "ObjectInitializerExpression", outputLevel);
            transformerHelper.Process(_transformerOnFactory, expectedOutput, expectedResult);
            transformerHelper.Process(_transformerOffFactory, "", source);
        }

        [TestCase(OutputLevel.Error)]
        [TestCase(OutputLevel.Warning)]
        [TestCase(OutputLevel.Info)]
        public void ProcessWithObjectInitializerExpressionsWithCollectionInitializers(OutputLevel outputLevel)
        {
            const String commonDefinitions = "    public class SomeData\r\n" +
                                             "    {\r\n" +
                                             "        public SomeData(int field1)\r\n" +
                                             "        {\r\n" +
                                             "            Field1 = field1;\r\n" +
                                             "            Field2 = new int[0];\r\n" +
                                             "            Field3 = new System.Collections.Generic.List<int>();\r\n" +
                                             "            Field4 = new System.Collections.Generic.Dictionary<int, string>();\r\n" +
                                             "        }\r\n" +
                                             "        public int Field1;\r\n" +
                                             "        public int[] Field2;\r\n" +
                                             "        public System.Collections.Generic.IList<int> Field3;\r\n" +
                                             "        public System.Collections.Generic.IDictionary<int, string> Field4;\r\n" +
                                             "    }\r\n";
            const String source = "namespace SomeNamespace\r\n" +
                                  "{\r\n" +
                                  commonDefinitions +
                                  "    public class SomeClass\r\n" +
                                  "    {\r\n" +
                                  "        public void SomeMethod()\r\n" +
                                  "        {\r\n" +
                                  "            SomeData data = new SomeData(666)\r\n" +
                                  "            {\r\n" +
                                  "                Field1 = 999,\r\n" +
                                  "                Field2 = new int[]{1, 2, 3, 4, 5},\r\n" +
                                  "                Field3 = new System.Collections.Generic.List<int>(){1, 2, 3, 4, 5},\r\n" +
                                  "                Field4 = new System.Collections.Generic.Dictionary<int, string>(){{1, \"IDDQD\"}, {2, \"IDKFA\"}, {3, \"IDCLIP\"}}\r\n" +
                                  "            };\r\n" +
                                  "        }\r\n" +
                                  "    }\r\n" +
                                  "}";
            const String expectedResult = "namespace SomeNamespace\r\n" +
                                          "{\r\n" +
                                          commonDefinitions +
                                          "    public class SomeClass\r\n" +
                                          "    {\r\n" +
                                          "        public void SomeMethod()\r\n" +
                                          "        {\r\n" +
                                          "            SomeData data = new SomeData(666);\r\n" +
                                          "            data.Field1 = 999;\r\n" +
                                          "            data.Field2 = new int[]{1, 2, 3, 4, 5};\r\n" +
                                          "            data.Field3 = new System.Collections.Generic.List<int>(){1, 2, 3, 4, 5};\r\n" +
                                          "            data.Field4 = new System.Collections.Generic.Dictionary<int, string>(){{1, \"IDDQD\"}, {2, \"IDKFA\"}, {3, \"IDCLIP\"}};\r\n" +
                                          "        }\r\n" +
                                          "    }\r\n" +
                                          "}";
            String expectedOutput = outputLevel == OutputLevel.Info ? ExpectedOutputForInfoLevel : "";
            TransformerHelper transformerHelper = new TransformerHelper(source, "ObjectInitializerExpression", outputLevel);
            transformerHelper.Process(_transformerOnFactory, expectedOutput, expectedResult);
            transformerHelper.Process(_transformerOffFactory, "", source);
        }

        [TestCase(OutputLevel.Error)]
        [TestCase(OutputLevel.Warning)]
        [TestCase(OutputLevel.Info)]
        public void ProcessWithPropertyInitializerExpressions(OutputLevel outputLevel)
        {
            const String commonDefinitions = "    public class SomeInnerData\r\n" +
                                             "    {\r\n" +
                                             "        public SomeInnerData(int innerField1, string innerField2)\r\n" +
                                             "        {\r\n" +
                                             "            InnerField1 = innerField1;\r\n" +
                                             "            InnerField2 = innerField2;\r\n" +
                                             "        }\r\n" +
                                             "        public int InnerField1;\r\n" +
                                             "        public string InnerField2;\r\n" +
                                             "    }\r\n" +
                                             "    public class SomeOuterData\r\n" +
                                             "    {\r\n" +
                                             "        public SomeOuterData(int outerField1)\r\n" +
                                             "        {\r\n" +
                                             "            OuterField1 = outerField1;\r\n" +
                                             "            InnerData = new SomeInnerData(19, \"IDKFA\");\r\n" +
                                             "         }\r\n" +
                                             "        public int OuterField1;\r\n" +
                                             "        public SomeInnerData InnerData { get; }\r\n" +
                                             "    }\r\n";
            const String source = "namespace SomeNamespace\r\n" +
                                  "{\r\n" +
                                  commonDefinitions +
                                  "    public class SomeClass\r\n" +
                                  "    {\r\n" +
                                  "        public void SomeMethod()\r\n" +
                                  "        {\r\n" +
                                  "            SomeOuterData outerData = new SomeOuterData(13)\r\n" +
                                  "            {\r\n" +
                                  "                OuterField1 = 777,\r\n" +
                                  "                InnerData =\r\n" +
                                  "                {\r\n" +
                                  "                    InnerField1 = 666,\r\n" +
                                  "                    InnerField2 = \"IDDQD\"\r\n" +
                                  "                }\r\n" +
                                  "            };\r\n" +
                                  "        }\r\n" +
                                  "    }\r\n" +
                                  "}";
            const String expectedResult = "namespace SomeNamespace\r\n" +
                                          "{\r\n" +
                                          commonDefinitions +
                                          "    public class SomeClass\r\n" +
                                          "    {\r\n" +
                                          "        public void SomeMethod()\r\n" +
                                          "        {\r\n" +
                                          "            SomeOuterData outerData = new SomeOuterData(13);\r\n" +
                                          "            outerData.OuterField1 = 777;\r\n" +
                                          "            outerData.InnerData.InnerField1 = 666;\r\n" +
                                          "            outerData.InnerData.InnerField2 = \"IDDQD\";\r\n" +
                                          "        }\r\n" +
                                          "    }\r\n" +
                                          "}";
            String expectedOutput = outputLevel == OutputLevel.Info ? ExpectedOutputForInfoLevel : "";
            TransformerHelper transformerHelper = new TransformerHelper(source, "ObjectInitializerExpression", outputLevel);
            transformerHelper.Process(_transformerOnFactory, expectedOutput, expectedResult);
            transformerHelper.Process(_transformerOffFactory, "", source);
        }

        [TestCase(OutputLevel.Error)]
        [TestCase(OutputLevel.Warning)]
        [TestCase(OutputLevel.Info)]
        public void ProcessSimpleObjectInitializerExpressionAsMethodArg(OutputLevel outputLevel)
        {
            const String commonDefinitions = "    public class SomeData()\r\n" +
                                             "    {\r\n" +
                                             "        public int Field1;\r\n" +
                                             "        public string Field2 = \"\";\r\n" +
                                             "    }\r\n";
            const String source = "namespace SomeNamespace\r\n" +
                                  "{\r\n" +
                                  commonDefinitions +
                                  "    public class SomeClass\r\n" +
                                  "    {\r\n" +
                                  "        public void SomeMethod()\r\n" +
                                  "        {\r\n" +
                                  "            OtherMethod(new SomeData(){Field1 = 666, Field2 = \"IDDQD\"});\r\n" +
                                  "        }\r\n" +
                                  "        public void OtherMethod(SomeData data)\r\n" +
                                  "        {\r\n" +
                                  "        }\r\n" +
                                  "    }\r\n" +
                                  "}";
            const String expectedResult = "namespace SomeNamespace\r\n" +
                                          "{\r\n" +
                                          commonDefinitions +
                                          "    public class SomeClass\r\n" +
                                          "    {\r\n" +
                                          "        public void SomeMethod()\r\n" +
                                          "        {\r\n" +
                                          "            SomeData data = new SomeData();\r\n" +
                                          "            data.Field1 = 666;\r\n" +
                                          "            data.Field2 = \"IDDQD\";\r\n" +
                                          "            OtherMethod(data);\r\n" +
                                          "        }\r\n" +
                                          "        public void OtherMethod(SomeData data)\r\n" +
                                          "        {\r\n" +
                                          "        }\r\n" +
                                          "    }\r\n" +
                                          "}";
            String expectedOutput = outputLevel == OutputLevel.Info ? ExpectedOutputForInfoLevel : "";
            TransformerHelper transformerHelper = new TransformerHelper(source, "ObjectInitializerExpression", outputLevel);
            transformerHelper.Process(_transformerOnFactory, expectedOutput, expectedResult);
            transformerHelper.Process(_transformerOffFactory, "", source);
        }

        [TestCase(OutputLevel.Error)]
        [TestCase(OutputLevel.Warning)]
        [TestCase(OutputLevel.Info)]
        public void ProcessSimpleObjectInitializerExpressionAsMethodArgs(OutputLevel outputLevel)
        {
            const String commonDefinitions = "    public class SomeData()\r\n" +
                                             "    {\r\n" +
                                             "        public int Field1;\r\n" +
                                             "        public string Field2 = \"\";\r\n" +
                                             "    }\r\n";
            const String source = "namespace SomeNamespace\r\n" +
                                  "{\r\n" +
                                  commonDefinitions +
                                  "    public class SomeClass\r\n" +
                                  "    {\r\n" +
                                  "        public void SomeMethod()\r\n" +
                                  "        {\r\n" +
                                  "            OtherMethod(new SomeData(){Field1 = 666, Field2 = \"IDDQD\"}, new SomeData(){Field1 = 777, Field2 = \"IDKFA\"});\r\n" +
                                  "        }\r\n" +
                                  "        public void OtherMethod(SomeData data1, SomeData data2)\r\n" +
                                  "        {\r\n" +
                                  "        }\r\n" +
                                  "    }\r\n" +
                                  "}";
            const String expectedResult = "namespace SomeNamespace\r\n" +
                                          "{\r\n" +
                                          commonDefinitions +
                                          "    public class SomeClass\r\n" +
                                          "    {\r\n" +
                                          "        public void SomeMethod()\r\n" +
                                          "        {\r\n" +
                                          "            SomeData data1 = new SomeData();\r\n" +
                                          "            data1.Field1 = 666;\r\n" +
                                          "            data1.Field2 = \"IDDQD\";\r\n" +
                                          "            SomeData data2 = new SomeData();\r\n" +
                                          "            data2.Field1 = 777;\r\n" +
                                          "            data2.Field2 = \"IDKFA\";\r\n" +
                                          "            OtherMethod(data1, data2);\r\n" +
                                          "        }\r\n" +
                                          "        public void OtherMethod(SomeData data1, SomeData data2)\r\n" +
                                          "        {\r\n" +
                                          "        }\r\n" +
                                          "    }\r\n" +
                                          "}";
            String expectedOutput = outputLevel == OutputLevel.Info ? ExpectedOutputForInfoLevel : "";
            TransformerHelper transformerHelper = new TransformerHelper(source, "ObjectInitializerExpression", outputLevel);
            transformerHelper.Process(_transformerOnFactory, expectedOutput, expectedResult);
            transformerHelper.Process(_transformerOffFactory, "", source);
        }

        [TestCase(OutputLevel.Error)]
        [TestCase(OutputLevel.Warning)]
        [TestCase(OutputLevel.Info)]
        public void ProcessSimpleObjectInitializerExpressionInReturn(OutputLevel outputLevel)
        {
            const String commonDefinitions = "    public class SomeData()\r\n" +
                                             "    {\r\n" +
                                             "        public int Field1;\r\n" +
                                             "        public string Field2 = \"\";\r\n" +
                                             "    }\r\n";
            const String source = "namespace SomeNamespace\r\n" +
                                  "{\r\n" +
                                  commonDefinitions +
                                  "    public class SomeClass\r\n" +
                                  "    {\r\n" +
                                  "        public SomeData SomeMethod()\r\n" +
                                  "        {\r\n" +
                                  "            return new SomeData(){Field1 = 666, Field2 = \"IDDQD\"};\r\n" +
                                  "        }\r\n" +
                                  "    }\r\n" +
                                  "}";
            const String expectedResult = "namespace SomeNamespace\r\n" +
                                          "{\r\n" +
                                          commonDefinitions +
                                          "    public class SomeClass\r\n" +
                                          "    {\r\n" +
                                          "        public SomeData SomeMethod()\r\n" +
                                          "        {\r\n" +
                                          "            SomeData returnValue = new SomeData();\r\n" +
                                          "            returnValue.Field1 = 666;\r\n" +
                                          "            returnValue.Field2 = \"IDDQD\";\r\n" +
                                          "            return returnValue;\r\n" +
                                          "        }\r\n" +
                                          "    }\r\n" +
                                          "}";
            String expectedOutput = outputLevel == OutputLevel.Info ? ExpectedOutputForInfoLevel : "";
            TransformerHelper transformerHelper = new TransformerHelper(source, "ObjectInitializerExpression", outputLevel);
            transformerHelper.Process(_transformerOnFactory, expectedOutput, expectedResult);
            transformerHelper.Process(_transformerOffFactory, "", source);
        }

        [TestCase(OutputLevel.Error)]
        [TestCase(OutputLevel.Warning)]
        [TestCase(OutputLevel.Info)]
        public void ProcessComplexObjectInitializerExpressionAsMethodArg(OutputLevel outputLevel)
        {
            const String commonDefinitions = "    public class SomeInnerData()\r\n" +
                                             "    {\r\n" +
                                             "        public int InnerField1;\r\n" +
                                             "        public string InnerField2 = \"\";\r\n" +
                                             "    }\r\n" +
                                             "    public class SomeOuterData()\r\n" +
                                             "    {\r\n" +
                                             "        public int OuterField1;\r\n" +
                                             "        public string OuterField2 = \"\";\r\n" +
                                             "        public SomeInnerData? InnerData = null;\r\n" +
                                             "    }\r\n";
            const String source = "namespace SomeNamespace\r\n" +
                                  "{\r\n" +
                                  commonDefinitions +
                                  "    public class SomeClass\r\n" +
                                  "    {\r\n" +
                                  "        public void SomeMethod()\r\n" +
                                  "        {\r\n" +
                                  "            OtherMethod(new SomeOuterData(){OuterField1 = 666, OuterField2 = \"IDDQD\", InnerData = new SomeInnerData(){InnerField1 = 777, InnerField2 = \"IDKFA\"}});\r\n" +
                                  "        }\r\n" +
                                  "        public void OtherMethod(SomeOuterData someData)\r\n" +
                                  "        {\r\n" +
                                  "        }\r\n" +
                                  "    }\r\n" +
                                  "}";
            const String expectedResult = "namespace SomeNamespace\r\n" +
                                          "{\r\n" +
                                          commonDefinitions +
                                          "    public class SomeClass\r\n" +
                                          "    {\r\n" +
                                          "        public void SomeMethod()\r\n" +
                                          "        {\r\n" +
                                          "            SomeOuterData someData = new SomeOuterData();\r\n" +
                                          "            someData.OuterField1 = 666;\r\n" +
                                          "            someData.OuterField2 = \"IDDQD\";\r\n" +
                                          "            someData.InnerData = new SomeInnerData();\r\n" +
                                          "            someData.InnerData.InnerField1 = 777;\r\n" +
                                          "            someData.InnerData.InnerField2 = \"IDKFA\";\r\n" +
                                          "            OtherMethod(someData);\r\n" +
                                          "        }\r\n" +
                                          "        public void OtherMethod(SomeOuterData someData)\r\n" +
                                          "        {\r\n" +
                                          "        }\r\n" +
                                          "    }\r\n" +
                                          "}";
            String expectedOutput = outputLevel == OutputLevel.Info ? ExpectedOutputForInfoLevel : "";
            TransformerHelper transformerHelper = new TransformerHelper(source, "ObjectInitializerExpression", outputLevel);
            transformerHelper.Process(_transformerOnFactory, expectedOutput, expectedResult);
            transformerHelper.Process(_transformerOffFactory, "", source);
        }

        [TestCase(OutputLevel.Error)]
        [TestCase(OutputLevel.Warning)]
        [TestCase(OutputLevel.Info)]
        public void ProcessSimpleObjectInitializerExpressionInArrayInitializer(OutputLevel outputLevel)
        {
            const String commonDefinitions = "    public class SomeData()\r\n" +
                                             "    {\r\n" +
                                             "        public int Field1;\r\n" +
                                             "        public string Field2 = \"\";\r\n" +
                                             "    }\r\n";
            const String source = "namespace SomeNamespace\r\n" +
                                  "{\r\n" +
                                  commonDefinitions +
                                  "    public class SomeClass\r\n" +
                                  "    {\r\n" +
                                  "        public void SomeMethod()\r\n" +
                                  "        {\r\n" +
                                  "            SomeData[] data = new []{new SomeData(){Field1 = 666, Field2 = \"IDDQD\"}, new SomeData(){Field1 = 777, Field2 = \"IDKFA\"}};\r\n" +
                                  "        }\r\n" +
                                  "    }\r\n" +
                                  "}";
            const String expectedResult = "namespace SomeNamespace\r\n" +
                                          "{\r\n" +
                                          commonDefinitions +
                                          "    public class SomeClass\r\n" +
                                          "    {\r\n" +
                                          "        public void SomeMethod()\r\n" +
                                          "        {\r\n" +
                                          "            SomeData initValue = new SomeData();\r\n" +
                                          "            initValue.Field1 = 666;\r\n" +
                                          "            initValue.Field2 = \"IDDQD\";\r\n" +
                                          "            SomeData initValue2 = new SomeData();\r\n" +
                                          "            initValue2.Field1 = 777;\r\n" +
                                          "            initValue2.Field2 = \"IDKFA\";\r\n" +
                                          "            SomeData[] data = new []{initValue, initValue2};\r\n" +
                                          "        }\r\n" +
                                          "    }\r\n" +
                                          "}";
            String expectedOutput = outputLevel == OutputLevel.Info ? ExpectedOutputForInfoLevel : "";
            TransformerHelper transformerHelper = new TransformerHelper(source, "ObjectInitializerExpression", outputLevel);
            transformerHelper.Process(_transformerOnFactory, expectedOutput, expectedResult);
            transformerHelper.Process(_transformerOffFactory, "", source);
        }

        [TestCase(OutputLevel.Error)]
        [TestCase(OutputLevel.Warning)]
        [TestCase(OutputLevel.Info)]
        public void ProcessSimpleObjectInitializerExpressionInListInitializer(OutputLevel outputLevel)
        {
            const String commonDefinitions = "    public class SomeData()\r\n" +
                                             "    {\r\n" +
                                             "        public int Field1;\r\n" +
                                             "        public string Field2 = \"\";\r\n" +
                                             "    }\r\n";
            const String source = "namespace SomeNamespace\r\n" +
                                  "{\r\n" +
                                  commonDefinitions +
                                  "    public class SomeClass\r\n" +
                                  "    {\r\n" +
                                  "        public void SomeMethod()\r\n" +
                                  "        {\r\n" +
                                  "            System.Collections.Generic.List<SomeData> data = new System.Collections.Generic.List<SomeData>{new SomeData(){Field1 = 666, Field2 = \"IDDQD\"}, new SomeData(){Field1 = 777, Field2 = \"IDKFA\"}};\r\n" +
                                  "        }\r\n" +
                                  "    }\r\n" +
                                  "}";
            const String expectedResult = "namespace SomeNamespace\r\n" +
                                          "{\r\n" +
                                          commonDefinitions +
                                          "    public class SomeClass\r\n" +
                                          "    {\r\n" +
                                          "        public void SomeMethod()\r\n" +
                                          "        {\r\n" +
                                          "            SomeData initValue = new SomeData();\r\n" +
                                          "            initValue.Field1 = 666;\r\n" +
                                          "            initValue.Field2 = \"IDDQD\";\r\n" +
                                          "            SomeData initValue2 = new SomeData();\r\n" +
                                          "            initValue2.Field1 = 777;\r\n" +
                                          "            initValue2.Field2 = \"IDKFA\";\r\n" +
                                          "            System.Collections.Generic.List<SomeData> data = new System.Collections.Generic.List<SomeData>{initValue, initValue2};\r\n" +
                                          "        }\r\n" +
                                          "    }\r\n" +
                                          "}";
            String expectedOutput = outputLevel == OutputLevel.Info ? ExpectedOutputForInfoLevel : "";
            TransformerHelper transformerHelper = new TransformerHelper(source, "ObjectInitializerExpression", outputLevel);
            transformerHelper.Process(_transformerOnFactory, expectedOutput, expectedResult);
            transformerHelper.Process(_transformerOffFactory, "", source);
        }

        [TestCase(OutputLevel.Error)]
        [TestCase(OutputLevel.Warning)]
        [TestCase(OutputLevel.Info)]
        public void ProcessSimpleObjectInitializerExpressionInDictionaryInitializer(OutputLevel outputLevel)
        {
            const String commonDefinitions = "    public class SomeData()\r\n" +
                                             "    {\r\n" +
                                             "        public int Field1;\r\n" +
                                             "        public string Field2 = \"\";\r\n" +
                                             "    }\r\n";
            const String source = "namespace SomeNamespace\r\n" +
                                  "{\r\n" +
                                  commonDefinitions +
                                  "    public class SomeClass\r\n" +
                                  "    {\r\n" +
                                  "        public void SomeMethod()\r\n" +
                                  "        {\r\n" +
                                  "            System.Collections.Generic.Dictionary<int, SomeData> data = new System.Collections.Generic.Dictionary<int, SomeData>{{1, new SomeData(){Field1 = 666, Field2 = \"IDDQD\"}}, {2, new SomeData(){Field1 = 777, Field2 = \"IDKFA\"}}};\r\n" +
                                  "        }\r\n" +
                                  "    }\r\n" +
                                  "}";
            const String expectedResult = "namespace SomeNamespace\r\n" +
                                          "{\r\n" +
                                          commonDefinitions +
                                          "    public class SomeClass\r\n" +
                                          "    {\r\n" +
                                          "        public void SomeMethod()\r\n" +
                                          "        {\r\n" +
                                          "            SomeData initValue = new SomeData();\r\n" +
                                          "            initValue.Field1 = 666;\r\n" +
                                          "            initValue.Field2 = \"IDDQD\";\r\n" +
                                          "            SomeData initValue2 = new SomeData();\r\n" +
                                          "            initValue2.Field1 = 777;\r\n" +
                                          "            initValue2.Field2 = \"IDKFA\";\r\n" +
                                          "            System.Collections.Generic.Dictionary<int, SomeData> data = new System.Collections.Generic.Dictionary<int, SomeData>{{1, initValue}, {2, initValue2}};\r\n" +
                                          "        }\r\n" +
                                          "    }\r\n" +
                                          "}";
            String expectedOutput = outputLevel == OutputLevel.Info ? ExpectedOutputForInfoLevel : "";
            TransformerHelper transformerHelper = new TransformerHelper(source, "ObjectInitializerExpression", outputLevel);
            transformerHelper.Process(_transformerOnFactory, expectedOutput, expectedResult);
            transformerHelper.Process(_transformerOffFactory, "", source);
        }

        [TestCase(OutputLevel.Error)]
        [TestCase(OutputLevel.Warning)]
        [TestCase(OutputLevel.Info)]
        public void ProcessObjectInitializerExprInCallInObjectInitializerExpr(OutputLevel outputLevel)
        {
            const String commonDefinitions = "    public class SomeInnerData\r\n" +
                                             "    {\r\n" +
                                             "        public int InnerField1;\r\n" +
                                             "        public string InnerField2 = \"\";\r\n" +
                                             "    }\r\n" +
                                             "    public class SomeOuterData\r\n" +
                                             "    {\r\n" +
                                             "        public SomeOuterData(SomeInnerData data)\r\n" +
                                             "        {\r\n" +
                                             "            InnerData = data;\r\n" +
                                             "        }\r\n" +
                                             "        public int OuterField1;\r\n" +
                                             "        public string OuterField2 = \"\";\r\n" +
                                             "        public SomeInnerData InnerData;\r\n" +
                                             "    }\r\n";
            const String source = "namespace SomeNamespace\r\n" +
                                  "{\r\n" +
                                  commonDefinitions +
                                  "    public class SomeClass\r\n" +
                                  "    {\r\n" +
                                  "        public void SomeMethod()\r\n" +
                                  "        {\r\n" +
                                  "            int data = 666;\r\n" +
                                  "            data += 3;\r\n" +
                                  "            string data3 = \"IDDQD\";\r\n" +
                                  "            data3 += \"==\";\r\n" +
                                  "            bool someData = true;\r\n" +
                                  "            someData |= (data > 1);\r\n" +
                                  "            OtherMethod(new SomeOuterData(new SomeInnerData(){InnerField1 = 777, InnerField2 = \"IDKFA\"}){OuterField1 = 666, OuterField2 = \"IDDQD\"});\r\n" +
                                  "        }\r\n" +
                                  "        public void OtherMethod(SomeOuterData someData)\r\n" +
                                  "        {\r\n" +
                                  "        }\r\n" +
                                  "    }\r\n" +
                                  "}";
            const String expectedResult = "namespace SomeNamespace\r\n" +
                                          "{\r\n" +
                                          commonDefinitions +
                                          "    public class SomeClass\r\n" +
                                          "    {\r\n" +
                                          "        public void SomeMethod()\r\n" +
                                          "        {\r\n" +
                                          "            int data = 666;\r\n" +
                                          "            data += 3;\r\n" +
                                          "            string data3 = \"IDDQD\";\r\n" +
                                          "            data3 += \"==\";\r\n" +
                                          "            bool someData = true;\r\n" +
                                          "            someData |= (data > 1);\r\n" +
                                          "            SomeInnerData data2 = new SomeInnerData();\r\n" +
                                          "            data2.InnerField1 = 777;\r\n" +
                                          "            data2.InnerField2 = \"IDKFA\";\r\n" +
                                          "            SomeOuterData someData2 = new SomeOuterData(data2);\r\n" +
                                          "            someData2.OuterField1 = 666;\r\n" +
                                          "            someData2.OuterField2 = \"IDDQD\";\r\n" +
                                          "            OtherMethod(someData2);\r\n" +
                                          "        }\r\n" +
                                          "        public void OtherMethod(SomeOuterData someData)\r\n" +
                                          "        {\r\n" +
                                          "        }\r\n" +
                                          "    }\r\n" +
                                          "}";
            String expectedOutput = outputLevel == OutputLevel.Info ? ExpectedOutputForInfoLevel : "";
            TransformerHelper transformerHelper = new TransformerHelper(source, "ObjectInitializerExpression", outputLevel);
            transformerHelper.Process(_transformerOnFactory, expectedOutput, expectedResult);
            transformerHelper.Process(_transformerOffFactory, "", source);
        }

        private readonly Func<IOutput, ITransformer> _transformerOnFactory = output => new ObjectInitializerExprTransformer(output, TransformerState.On);
        private readonly Func<IOutput, ITransformer> _transformerOffFactory = output => new ObjectInitializerExprTransformer(output, TransformerState.Off);

        private const String ExpectedOutputForInfoLevel = $"Execution of {ObjectInitializerExprTransformer.Name} started\r\n" +
                                                          $"Execution of {ObjectInitializerExprTransformer.Name} finished\r\n";
        private const String ExpectedOutputForInfoLevelTemplate = $"Execution of {ObjectInitializerExprTransformer.Name} started\r\n" +
                                                                  "{0}" +
                                                                  $"Execution of {ObjectInitializerExprTransformer.Name} finished\r\n";
    }
}
