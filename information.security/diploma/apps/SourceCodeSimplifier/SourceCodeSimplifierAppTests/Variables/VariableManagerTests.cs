using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NUnit.Framework;
using SourceCodeSimplifierApp.Variables;
using SourceCodeSimplifierAppTests.TestUtils;

namespace SourceCodeSimplifierAppTests.Variables
{
    [TestFixture]
    public class VariableManagerTests
    {
        [Test]
        public void GenerateVariableInMethod()
        {
            const String source = "namespace SomeNamespace\r\n" +
                                  "{\r\n" +
                                  CommonDefs +
                                  "    public class SomeClass\r\n" +
                                  "    {\r\n" +
                                  "        public void SomeMethod(int param1, string param2)\r\n" +
                                  "        {\r\n" +
                                  "            int var1 = 666;\r\n" +
                                  "            var1 += 1;\r\n" +
                                  "            string var2 = \"\";\r\n" +
                                  "            var2 += \"IDDQD\";\r\n" +
                                  "            OtherMethod(out int outVar1, out string outVar2);\r\n" +
                                  "            SomeData var3 = new SomeData(out int dataVar1, out string dataVar2);\r\n" +
                                  "            AnotherMethod(var3);\r\n" +
                                  "            AnotherMethod(new SomeData(out int dataVar3, out string dataVar4));\r\n" +
                                  "            {\r\n" +
                                  "                int innerVar1 = 666;\r\n" +
                                  "                innerVar1 += 1;\r\n" +
                                  "            }\r\n" +
                                  "            {\r\n" +
                                  "                int innerVar1 = 666;\r\n" +
                                  "                innerVar1 += 1;\r\n" +
                                  "            }\r\n" +
                                  "        }\r\n" +
                                  "        public void SomeMethod2(int data1, out string data2)\r\n" +
                                  "        {\r\n" +
                                  "            data2 = \"IDDQD\";\r\n" +
                                  "            int superVar1 = 666;\r\n" +
                                  "            superVar1 += 999;\r\n" +
                                  "        }\r\n" +
                                  "        public void OtherMethod(out int param1, out string param2)\r\n" +
                                  "        {\r\n" +
                                  "            param1 = 666;\r\n" +
                                  "            param2 = \"IDDQD\";\r\n" +
                                  "        }\r\n" +
                                  "        public void AnotherMethod(SomeData data)\r\n" +
                                  "        {\r\n" +
                                  "        }\r\n" +
                                  "    }\r\n" +
                                  "}";
            VariableManager variableManager = new VariableManager();
            Document sourceDocument = PreparationHelper.Prepare(source, "VariablesCollector");
            SyntaxNode root = sourceDocument.GetSyntaxRootAsync().Result.Must();
            GenerateVariables<MethodDeclarationSyntax>(variableManager,
                root,
                method => method.Identifier.Text == "SomeMethod",
                method => method.Body,
                new Variable("var", "var"),
                new Variable("var", "var4"),
                new Variable("param", "param"),
                new Variable("param", "param3"),
                new Variable("data", "data"),
                new Variable("data", "data2"),
                new Variable("item", "item"),
                new Variable("item", "item2"));
            GenerateVariables<MethodDeclarationSyntax>(variableManager,
                root,
                method => method.Identifier.Text == "SomeMethod2",
                method => method.Body,
                new Variable("var", "var"),
                new Variable("var", "var2"),
                new Variable("param", "param"),
                new Variable("param", "param2"),
                new Variable("data", "data"),
                new Variable("data", "data3"),
                new Variable("item", "item"),
                new Variable("item", "item2"));
        }

        [Test]
        public void GenerateVariableInCtor()
        {
            const String source = "namespace SomeNamespace\r\n" +
                                  "{\r\n" +
                                  CommonDefs +
                                  "    public class SomeClass\r\n" +
                                  "    {\r\n" +
                                  "        public SomeClass(int param1, string param2)\r\n" +
                                  "        {\r\n" +
                                  "            int var1 = 666;\r\n" +
                                  "            var1 += 1;\r\n" +
                                  "            string var2 = \"\";\r\n" +
                                  "            var2 += \"IDDQD\";\r\n" +
                                  "            OtherMethod(out int outVar1, out string outVar2);\r\n" +
                                  "            SomeData var3 = new SomeData(out int dataVar1, out string dataVar2);\r\n" +
                                  "            AnotherMethod(var3);\r\n" +
                                  "            AnotherMethod(new SomeData(out int dataVar3, out string dataVar4));\r\n" +
                                  "            {\r\n" +
                                  "                int innerVar1 = 666;\r\n" +
                                  "                innerVar1 += 1;\r\n" +
                                  "            }\r\n" +
                                  "            {\r\n" +
                                  "                int innerVar1 = 666;\r\n" +
                                  "                innerVar1 += 1;\r\n" +
                                  "            }\r\n" +
                                  "        }\r\n" +
                                  "        public SomeClass(int data)\r\n" +
                                  "        {\r\n" +
                                  "            int superVar1 = 666;\r\n" +
                                  "            superVar1 += 999;\r\n" +
                                  "            string superVar2 = \"IDDQD\";\r\n" +
                                  "            superVar2 += \"==\";\r\n" +
                                  "        }\r\n" +
                                  "        public void OtherMethod(out int param1, out string param2)\r\n" +
                                  "        {\r\n" +
                                  "            param1 = 666;\r\n" +
                                  "            param2 = \"IDDQD\";\r\n" +
                                  "        }\r\n" +
                                  "        public void AnotherMethod(SomeData data)\r\n" +
                                  "        {\r\n" +
                                  "        }\r\n" +
                                  "    }\r\n" +
                                  "}";
            VariableManager variableManager = new VariableManager();
            Document sourceDocument = PreparationHelper.Prepare(source, "VariablesCollector");
            SyntaxNode root = sourceDocument.GetSyntaxRootAsync().Result.Must();
            GenerateVariables<ConstructorDeclarationSyntax>(variableManager,
                root,
                ctor => (ctor.Identifier.Text == "SomeClass") && (ctor.ParameterList.Parameters.Count == 2),
                ctor => ctor.Body,
                new Variable("var", "var"),
                new Variable("var", "var4"),
                new Variable("param", "param"),
                new Variable("param", "param3"),
                new Variable("data", "data"),
                new Variable("data", "data2"),
                new Variable("item", "item"),
                new Variable("item", "item2"));
            GenerateVariables<ConstructorDeclarationSyntax>(variableManager,
                root,
                ctor => (ctor.Identifier.Text == "SomeClass") && (ctor.ParameterList.Parameters.Count == 1),
                ctor => ctor.Body,
                new Variable("var", "var"),
                new Variable("var", "var2"),
                new Variable("param", "param"),
                new Variable("param", "param2"),
                new Variable("data", "data2"),
                new Variable("data", "data3"),
                new Variable("item", "item"),
                new Variable("item", "item2"));
        }

        [Test]
        public void GenerateVariableInPropertyWithGetOnly()
        {
            const String source = "namespace SomeNamespace\r\n" +
                                  "{\r\n" +
                                  CommonDefs +
                                  "    public class SomeClass\r\n" +
                                  "    {\r\n" +
                                  "        public int SomeValue\r\n" +
                                  "        {\r\n" +
                                  "            get\r\n" +
                                  "            {\r\n" +
                                  "                int var1 = 666;\r\n" +
                                  "                var1 += 1;\r\n" +
                                  "                string var2 = \"\";\r\n" +
                                  "                var2 += \"IDDQD\";\r\n" +
                                  "                OtherMethod(out int outVar1, out string outVar2);\r\n" +
                                  "                SomeData var3 = new SomeData(out int dataVar1, out string dataVar2);\r\n" +
                                  "                AnotherMethod(var3);\r\n" +
                                  "                AnotherMethod(new SomeData(out int dataVar3, out string dataVar4));\r\n" +
                                  "                {\r\n" +
                                  "                    int innerVar1 = 666;\r\n" +
                                  "                    innerVar1 += 1;\r\n" +
                                  "                }\r\n" +
                                  "                {\r\n" +
                                  "                    int innerVar1 = 666;\r\n" +
                                  "                    innerVar1 += 1;\r\n" +
                                  "                }\r\n" +
                                  "                return _someValue;\r\n" +
                                  "            }\r\n" +
                                  "        }\r\n" +
                                  "        public int SomeValue2\r\n" +
                                  "        {\r\n" +
                                  "            get\r\n" +
                                  "            {\r\n" +
                                  "                int data = 666;\r\n" +
                                  "                data += 3;\r\n" +
                                  "                int superVar1 = 666;\r\n" +
                                  "                superVar1 += 999;\r\n" +
                                  "                string superVar2 = \"IDDQD\";\r\n" +
                                  "                superVar2 += \"==\";\r\n" +
                                  "                return _someValue;\r\n" +
                                  "            }\r\n" +
                                  "        }\r\n" +
                                  "        public void OtherMethod(out int param1, out string param2)\r\n" +
                                  "        {\r\n" +
                                  "            param1 = 666;\r\n" +
                                  "            param2 = \"IDDQD\";\r\n" +
                                  "        }\r\n" +
                                  "        public void AnotherMethod(SomeData data)\r\n" +
                                  "        {\r\n" +
                                  "        }\r\n" +
                                  "        private int _someValue = 0;\r\n" +
                                  "    }\r\n" +
                                  "}";
            VariableManager variableManager = new VariableManager();
            Document sourceDocument = PreparationHelper.Prepare(source, "VariablesCollector");
            SyntaxNode root = sourceDocument.GetSyntaxRootAsync().Result.Must();
            GenerateVariables<PropertyDeclarationSyntax>(variableManager,
                root,
                prop => prop.Identifier.Text == "SomeValue",
                prop => prop.AccessorList.Must().Accessors.FirstOrDefault(a => a.Keyword.IsKind(SyntaxKind.GetKeyword)).Must().Body,
                new Variable("var", "var"),
                new Variable("var", "var4"),
                new Variable("data", "data"),
                new Variable("data", "data2"),
                new Variable("value", "value2"),
                new Variable("value", "value3"),
                new Variable("item", "item"),
                new Variable("item", "item2"));
            GenerateVariables<PropertyDeclarationSyntax>(variableManager,
                root,
                prop => prop.Identifier.Text == "SomeValue2",
                prop => prop.AccessorList.Must().Accessors.FirstOrDefault(a => a.Keyword.IsKind(SyntaxKind.GetKeyword)).Must().Body,
                new Variable("var", "var"),
                new Variable("var", "var2"),
                new Variable("data", "data2"),
                new Variable("data", "data3"),
                new Variable("value", "value2"),
                new Variable("value", "value3"),
                new Variable("item", "item"),
                new Variable("item", "item2"));
        }

        [Test]
        public void GenerateVariableInPropertyWithSetOnly()
        {
            const String source = "namespace SomeNamespace\r\n" +
                                  "{\r\n" +
                                  CommonDefs +
                                  "    public class SomeClass\r\n" +
                                  "    {\r\n" +
                                  "        public int SomeValue\r\n" +
                                  "        {\r\n" +
                                  "            set\r\n" +
                                  "            {\r\n" +
                                  "                int var1 = 666;\r\n" +
                                  "                var1 += 1;\r\n" +
                                  "                string var2 = \"\";\r\n" +
                                  "                var2 += \"IDDQD\";\r\n" +
                                  "                OtherMethod(out int outVar1, out string outVar2);\r\n" +
                                  "                SomeData var3 = new SomeData(out int dataVar1, out string dataVar2);\r\n" +
                                  "                AnotherMethod(var3);\r\n" +
                                  "                AnotherMethod(new SomeData(out int dataVar3, out string dataVar4));\r\n" +
                                  "                {\r\n" +
                                  "                    int innerVar1 = 666;\r\n" +
                                  "                    innerVar1 += 1;\r\n" +
                                  "                }\r\n" +
                                  "                {\r\n" +
                                  "                    int innerVar1 = 666;\r\n" +
                                  "                    innerVar1 += 1;\r\n" +
                                  "                }\r\n" +
                                  "                _someValue = value;\r\n" +
                                  "            }\r\n" +
                                  "        }\r\n" +
                                  "        public int SomeValue2\r\n" +
                                  "        {\r\n" +
                                  "            set\r\n" +
                                  "            {\r\n" +
                                  "                int data = 666;\r\n" +
                                  "                data += 3;\r\n" +
                                  "                int superVar1 = 666;\r\n" +
                                  "                superVar1 += 999;\r\n" +
                                  "                string superVar2 = \"IDDQD\";\r\n" +
                                  "                superVar2 += \"==\";\r\n" +
                                  "                _someValue = value;\r\n" +
                                  "            }\r\n" +
                                  "        }\r\n" +
                                  "        public void OtherMethod(out int param1, out string param2)\r\n" +
                                  "        {\r\n" +
                                  "            param1 = 666;\r\n" +
                                  "            param2 = \"IDDQD\";\r\n" +
                                  "        }\r\n" +
                                  "        public void AnotherMethod(SomeData data)\r\n" +
                                  "        {\r\n" +
                                  "        }\r\n" +
                                  "        private int _someValue = 0;\r\n" +
                                  "    }\r\n" +
                                  "}";
            VariableManager variableManager = new VariableManager();
            Document sourceDocument = PreparationHelper.Prepare(source, "VariablesCollector");
            SyntaxNode root = sourceDocument.GetSyntaxRootAsync().Result.Must();
            GenerateVariables<PropertyDeclarationSyntax>(variableManager,
                root,
                prop => prop.Identifier.Text == "SomeValue",
                prop => prop.AccessorList.Must().Accessors.FirstOrDefault(a => a.Keyword.IsKind(SyntaxKind.SetKeyword)).Must().Body,
                new Variable("var", "var"),
                new Variable("var", "var4"),
                new Variable("data", "data"),
                new Variable("data", "data2"),
                new Variable("value", "value2"),
                new Variable("value", "value3"),
                new Variable("item", "item"),
                new Variable("item", "item2"));
            GenerateVariables<PropertyDeclarationSyntax>(variableManager,
                root,
                prop => prop.Identifier.Text == "SomeValue2",
                prop => prop.AccessorList.Must().Accessors.FirstOrDefault(a => a.Keyword.IsKind(SyntaxKind.SetKeyword)).Must().Body,
                new Variable("var", "var"),
                new Variable("var", "var2"),
                new Variable("data", "data2"),
                new Variable("data", "data3"),
                new Variable("value", "value2"),
                new Variable("value", "value3"),
                new Variable("item", "item"),
                new Variable("item", "item2"));
        }

        [Test]
        public void GenerateVariableInProperty()
        {
            const String source = "namespace SomeNamespace\r\n" +
                                  "{\r\n" +
                                  CommonDefs +
                                  "    public class SomeClass\r\n" +
                                  "    {\r\n" +
                                  "        public int SomeValue\r\n" +
                                  "        {\r\n" +
                                  "            get\r\n" +
                                  "            {\r\n" +
                                  "                int var1 = 666;\r\n" +
                                  "                var1 += 1;\r\n" +
                                  "                OtherMethod(out int outVar1, out string outVar2);\r\n" +
                                  "                SomeData var3 = new SomeData(out int dataVar1, out string dataVar2);\r\n" +
                                  "                AnotherMethod(var3);\r\n" +
                                  "                {\r\n" +
                                  "                    int innerVar1 = 666;\r\n" +
                                  "                    innerVar1 += 1;\r\n" +
                                  "                }\r\n" +
                                  "                return _someValue;\r\n" +
                                  "            }\r\n" +
                                  "            set\r\n" +
                                  "            {\r\n" +
                                  "                string var2 = \"\";\r\n" +
                                  "                var2 += \"IDDQD\";\r\n" +
                                  "                AnotherMethod(new SomeData(out int dataVar3, out string dataVar4));\r\n" +
                                  "                {\r\n" +
                                  "                    int innerVar1 = 666;\r\n" +
                                  "                    innerVar1 += 1;\r\n" +
                                  "                }\r\n" +
                                  "                _someValue = value;\r\n" +
                                  "            }\r\n" +
                                  "        }\r\n" +
                                  "        public int SomeValue2\r\n" +
                                  "        {\r\n" +
                                  "            get\r\n" +
                                  "            {\r\n" +
                                  "                int data = 666;\r\n" +
                                  "                data += 3;\r\n" +
                                  "                string superVar2 = \"IDDQD\";\r\n" +
                                  "                superVar2 += \"==\";\r\n" +
                                  "                return _someValue;\r\n" +
                                  "            }\r\n" +
                                  "            set\r\n" +
                                  "            {\r\n" +
                                  "                int superVar1 = 666;\r\n" +
                                  "                superVar1 += 999;\r\n" +
                                  "                _someValue = value;\r\n" +
                                  "            }\r\n" +
                                  "        }\r\n" +
                                  "        public void OtherMethod(out int param1, out string param2)\r\n" +
                                  "        {\r\n" +
                                  "            param1 = 666;\r\n" +
                                  "            param2 = \"IDDQD\";\r\n" +
                                  "        }\r\n" +
                                  "        public void AnotherMethod(SomeData data)\r\n" +
                                  "        {\r\n" +
                                  "        }\r\n" +
                                  "        private int _someValue = 0;\r\n" +
                                  "    }\r\n" +
                                  "}";
            VariableManager variableManager = new VariableManager();
            Document sourceDocument = PreparationHelper.Prepare(source, "VariablesCollector");
            SyntaxNode root = sourceDocument.GetSyntaxRootAsync().Result.Must();
            GenerateVariables<PropertyDeclarationSyntax>(variableManager,
                root,
                prop => prop.Identifier.Text == "SomeValue",
                prop => prop.AccessorList.Must().Accessors.FirstOrDefault(a => a.Keyword.IsKind(SyntaxKind.GetKeyword)).Must().Body,
                new Variable("var", "var"),
                new Variable("var", "var4"),
                new Variable("data", "data"),
                new Variable("data", "data2"),
                new Variable("value", "value2"),
                new Variable("value", "value3"),
                new Variable("item", "item"),
                new Variable("item", "item2"));
            GenerateVariables<PropertyDeclarationSyntax>(variableManager,
                root,
                prop => prop.Identifier.Text == "SomeValue2",
                prop => prop.AccessorList.Must().Accessors.FirstOrDefault(a => a.Keyword.IsKind(SyntaxKind.GetKeyword)).Must().Body,
                new Variable("var", "var"),
                new Variable("var", "var2"),
                new Variable("data", "data2"),
                new Variable("data", "data3"),
                new Variable("value", "value2"),
                new Variable("value", "value3"),
                new Variable("item", "item"),
                new Variable("item", "item2"));
        }

        [Test]
        public void GenerateVariableInIndexerWithGetOnly()
        {
            const String source = "namespace SomeNamespace\r\n" +
                                  "{\r\n" +
                                  CommonDefs +
                                  "    public class SomeClass\r\n" +
                                  "    {\r\n" +
                                  "        public int this[int index]\r\n" +
                                  "        {\r\n" +
                                  "            get\r\n" +
                                  "            {\r\n" +
                                  "                int var1 = 666;\r\n" +
                                  "                var1 += 1;\r\n" +
                                  "                string var2 = \"\";\r\n" +
                                  "                var2 += \"IDDQD\";\r\n" +
                                  "                OtherMethod(out int outVar1, out string outVar2);\r\n" +
                                  "                SomeData var3 = new SomeData(out int dataVar1, out string dataVar2);\r\n" +
                                  "                AnotherMethod(var3);\r\n" +
                                  "                AnotherMethod(new SomeData(out int dataVar3, out string dataVar4));\r\n" +
                                  "                {\r\n" +
                                  "                    int innerVar1 = 666;\r\n" +
                                  "                    innerVar1 += 1;\r\n" +
                                  "                }\r\n" +
                                  "                {\r\n" +
                                  "                    int innerVar1 = 666;\r\n" +
                                  "                    innerVar1 += 1;\r\n" +
                                  "                }\r\n" +
                                  "                return _someValue;\r\n" +
                                  "            }\r\n" +
                                  "        }\r\n" +
                                  "        public int this[int index1, int index2]\r\n" +
                                  "        {\r\n" +
                                  "            get\r\n" +
                                  "            {\r\n" +
                                  "                int data = 666;\r\n" +
                                  "                data += 3;\r\n" +
                                  "                int superVar1 = 666;\r\n" +
                                  "                superVar1 += 999;\r\n" +
                                  "                string superVar2 = \"IDDQD\";\r\n" +
                                  "                superVar2 += \"==\";\r\n" +
                                  "                return _someValue;\r\n" +
                                  "            }\r\n" +
                                  "        }\r\n" +
                                  "        public void OtherMethod(out int param1, out string param2)\r\n" +
                                  "        {\r\n" +
                                  "            param1 = 666;\r\n" +
                                  "            param2 = \"IDDQD\";\r\n" +
                                  "        }\r\n" +
                                  "        public void AnotherMethod(SomeData data)\r\n" +
                                  "        {\r\n" +
                                  "        }\r\n" +
                                  "        private int _someValue = 0;\r\n" +
                                  "    }\r\n" +
                                  "}";
            VariableManager variableManager = new VariableManager();
            Document sourceDocument = PreparationHelper.Prepare(source, "VariablesCollector");
            SyntaxNode root = sourceDocument.GetSyntaxRootAsync().Result.Must();
            GenerateVariables<IndexerDeclarationSyntax>(variableManager,
                root,
                idx => idx.ParameterList.Parameters.Count == 1,
                idx => idx.AccessorList.Must().Accessors.FirstOrDefault(a => a.Keyword.IsKind(SyntaxKind.GetKeyword)).Must().Body,
                new Variable("var", "var"),
                new Variable("var", "var4"),
                new Variable("index", "index2"),
                new Variable("index", "index3"),
                new Variable("data", "data"),
                new Variable("data", "data2"),
                new Variable("value", "value2"),
                new Variable("value", "value3"),
                new Variable("item", "item"),
                new Variable("item", "item2"));
            GenerateVariables<IndexerDeclarationSyntax>(variableManager,
                root,
                idx => idx.ParameterList.Parameters.Count == 2,
                idx => idx.AccessorList.Must().Accessors.FirstOrDefault(a => a.Keyword.IsKind(SyntaxKind.GetKeyword)).Must().Body,
                new Variable("var", "var"),
                new Variable("var", "var2"),
                new Variable("index", "index"),
                new Variable("index", "index3"),
                new Variable("data", "data2"),
                new Variable("data", "data3"),
                new Variable("value", "value2"),
                new Variable("value", "value3"),
                new Variable("item", "item"),
                new Variable("item", "item2"));
        }

        [Test]
        public void GenerateVariableInIndexerWithSetOnly()
        {
            const String source = "namespace SomeNamespace\r\n" +
                                  "{\r\n" +
                                  CommonDefs +
                                  "    public class SomeClass\r\n" +
                                  "    {\r\n" +
                                  "        public int this[int index]\r\n" +
                                  "        {\r\n" +
                                  "            set\r\n" +
                                  "            {\r\n" +
                                  "                int var1 = 666;\r\n" +
                                  "                var1 += 1;\r\n" +
                                  "                string var2 = \"\";\r\n" +
                                  "                var2 += \"IDDQD\";\r\n" +
                                  "                OtherMethod(out int outVar1, out string outVar2);\r\n" +
                                  "                SomeData var3 = new SomeData(out int dataVar1, out string dataVar2);\r\n" +
                                  "                AnotherMethod(var3);\r\n" +
                                  "                AnotherMethod(new SomeData(out int dataVar3, out string dataVar4));\r\n" +
                                  "                {\r\n" +
                                  "                    int innerVar1 = 666;\r\n" +
                                  "                    innerVar1 += 1;\r\n" +
                                  "                }\r\n" +
                                  "                {\r\n" +
                                  "                    int innerVar1 = 666;\r\n" +
                                  "                    innerVar1 += 1;\r\n" +
                                  "                }\r\n" +
                                  "                _someValue = value;\r\n" +
                                  "            }\r\n" +
                                  "        }\r\n" +
                                  "        public int this[int index1, int index2]\r\n" +
                                  "        {\r\n" +
                                  "            set\r\n" +
                                  "            {\r\n" +
                                  "                int data = 666;\r\n" +
                                  "                data += 3;\r\n" +
                                  "                int superVar1 = 666;\r\n" +
                                  "                superVar1 += 999;\r\n" +
                                  "                string superVar2 = \"IDDQD\";\r\n" +
                                  "                superVar2 += \"==\";\r\n" +
                                  "                _someValue = value;\r\n" +
                                  "            }\r\n" +
                                  "        }\r\n" +
                                  "        public void OtherMethod(out int param1, out string param2)\r\n" +
                                  "        {\r\n" +
                                  "            param1 = 666;\r\n" +
                                  "            param2 = \"IDDQD\";\r\n" +
                                  "        }\r\n" +
                                  "        public void AnotherMethod(SomeData data)\r\n" +
                                  "        {\r\n" +
                                  "        }\r\n" +
                                  "        private int _someValue = 0;\r\n" +
                                  "    }\r\n" +
                                  "}";
            VariableManager variableManager = new VariableManager();
            Document sourceDocument = PreparationHelper.Prepare(source, "VariablesCollector");
            SyntaxNode root = sourceDocument.GetSyntaxRootAsync().Result.Must();
            GenerateVariables<IndexerDeclarationSyntax>(variableManager,
                root,
                idx => idx.ParameterList.Parameters.Count == 1,
                idx => idx.AccessorList.Must().Accessors.FirstOrDefault(a => a.Keyword.IsKind(SyntaxKind.SetKeyword)).Must().Body,
                new Variable("var", "var"),
                new Variable("var", "var4"),
                new Variable("index", "index2"),
                new Variable("index", "index3"),
                new Variable("data", "data"),
                new Variable("data", "data2"),
                new Variable("value", "value2"),
                new Variable("value", "value3"),
                new Variable("item", "item"),
                new Variable("item", "item2"));
            GenerateVariables<IndexerDeclarationSyntax>(variableManager,
                root,
                idx => idx.ParameterList.Parameters.Count == 2,
                idx => idx.AccessorList.Must().Accessors.FirstOrDefault(a => a.Keyword.IsKind(SyntaxKind.SetKeyword)).Must().Body,
                new Variable("var", "var"),
                new Variable("var", "var2"),
                new Variable("index", "index"),
                new Variable("index", "index3"),
                new Variable("data", "data2"),
                new Variable("data", "data3"),
                new Variable("value", "value2"),
                new Variable("value", "value3"),
                new Variable("item", "item"),
                new Variable("item", "item2"));
        }

        [Test]
        public void GenerateVariableInIndexer()
        {
            const String source = "namespace SomeNamespace\r\n" +
                                  "{\r\n" +
                                  CommonDefs +
                                  "    public class SomeClass\r\n" +
                                  "    {\r\n" +
                                  "        public int this[int index]\r\n" +
                                  "        {\r\n" +
                                  "            get\r\n" +
                                  "            {\r\n" +
                                  "                int var1 = 666;\r\n" +
                                  "                var1 += 1;\r\n" +
                                  "                OtherMethod(out int outVar1, out string outVar2);\r\n" +
                                  "                SomeData var3 = new SomeData(out int dataVar1, out string dataVar2);\r\n" +
                                  "                AnotherMethod(var3);\r\n" +
                                  "                {\r\n" +
                                  "                    int innerVar1 = 666;\r\n" +
                                  "                    innerVar1 += 1;\r\n" +
                                  "                }\r\n" +
                                  "                return _someValue;\r\n" +
                                  "            }\r\n" +
                                  "            set\r\n" +
                                  "            {\r\n" +
                                  "                string var2 = \"\";\r\n" +
                                  "                var2 += \"IDDQD\";\r\n" +
                                  "                AnotherMethod(new SomeData(out int dataVar3, out string dataVar4));\r\n" +
                                  "                {\r\n" +
                                  "                    int innerVar1 = 666;\r\n" +
                                  "                    innerVar1 += 1;\r\n" +
                                  "                }\r\n" +
                                  "                _someValue = value;\r\n" +
                                  "            }\r\n" +
                                  "        }\r\n" +
                                  "        public int this[int index1, int index2]\r\n" +
                                  "        {\r\n" +
                                  "            get\r\n" +
                                  "            {\r\n" +
                                  "                int data = 666;\r\n" +
                                  "                data += 3;\r\n" +
                                  "                string superVar2 = \"IDDQD\";\r\n" +
                                  "                superVar2 += \"==\";\r\n" +
                                  "                return _someValue;\r\n" +
                                  "            }\r\n" +
                                  "            set\r\n" +
                                  "            {\r\n" +
                                  "                int superVar1 = 666;\r\n" +
                                  "                superVar1 += 999;\r\n" +
                                  "            }\r\n" +
                                  "        }\r\n" +
                                  "        public void OtherMethod(out int param1, out string param2)\r\n" +
                                  "        {\r\n" +
                                  "            param1 = 666;\r\n" +
                                  "            param2 = \"IDDQD\";\r\n" +
                                  "        }\r\n" +
                                  "        public void AnotherMethod(SomeData data)\r\n" +
                                  "        {\r\n" +
                                  "        }\r\n" +
                                  "        private int _someValue = 0;\r\n" +
                                  "    }\r\n" +
                                  "}";
            VariableManager variableManager = new VariableManager();
            Document sourceDocument = PreparationHelper.Prepare(source, "VariablesCollector");
            SyntaxNode root = sourceDocument.GetSyntaxRootAsync().Result.Must();
            GenerateVariables<IndexerDeclarationSyntax>(variableManager,
                root,
                idx => idx.ParameterList.Parameters.Count == 1,
                idx => idx.AccessorList.Must().Accessors.FirstOrDefault(a => a.Keyword.IsKind(SyntaxKind.GetKeyword)).Must().Body,
                new Variable("var", "var"),
                new Variable("var", "var4"),
                new Variable("index", "index2"),
                new Variable("index", "index3"),
                new Variable("data", "data"),
                new Variable("data", "data2"),
                new Variable("value", "value2"),
                new Variable("value", "value3"),
                new Variable("item", "item"),
                new Variable("item", "item2"));
            GenerateVariables<IndexerDeclarationSyntax>(variableManager,
                root,
                idx => idx.ParameterList.Parameters.Count == 2,
                idx => idx.AccessorList.Must().Accessors.FirstOrDefault(a => a.Keyword.IsKind(SyntaxKind.GetKeyword)).Must().Body,
                new Variable("var", "var"),
                new Variable("var", "var2"),
                new Variable("index", "index"),
                new Variable("index", "index3"),
                new Variable("data", "data2"),
                new Variable("data", "data3"),
                new Variable("value", "value2"),
                new Variable("value", "value3"),
                new Variable("item", "item"),
                new Variable("item", "item2"));
        }

        private record Variable(String Prefix, String ExpectedName);

        private void GenerateVariables<TMember>(VariableManager variableManager,
                                                SyntaxNode root,
                                                Func<TMember, Boolean> memberSelector,
                                                Func<TMember, BlockSyntax?> bodyAccessor,
                                                params Variable[] generateExpectations) where TMember : MemberDeclarationSyntax
        {
            TMember member = root.DescendantNodes().OfType<TMember>().FirstOrDefault(memberSelector).Must();
            BlockSyntax body = bodyAccessor(member).Must();
            Assert.That(body.Statements.Count, Is.GreaterThan(0));
            StatementSyntax firstStatement = body.Statements[0];
            foreach (Variable generateExpectation in generateExpectations)
            {
                String prefix = generateExpectation.Prefix;
                String expectedName = generateExpectation.ExpectedName;
                Assert.That(variableManager.GenerateVariableName(firstStatement, prefix), Is.EqualTo(expectedName));
            }

        }

        private const String CommonDefs = "    public class SomeData\r\n" +
                                          "    {\r\n" +
                                          "        public SomeData(out int param1, out string param2)\r\n" +
                                          "        {\r\n" +
                                          "            param1 = 666;\r\n" +
                                          "            param2 = \"IDDQD\";\r\n" +
                                          "        }\r\n" +
                                          "    }\r\n";
    }
}
