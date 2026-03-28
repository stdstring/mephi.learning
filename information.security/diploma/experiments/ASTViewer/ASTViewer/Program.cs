using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ASTViewer
{
    internal class Program
    {
        private static Document Prepare(String source, String namePrefix)
        {
            AdhocWorkspace workspace = new AdhocWorkspace();
            Project project = workspace.AddProject($"{namePrefix}Project", "C#")
                .WithAssemblyName($"{namePrefix}Assembly")
                .WithMetadataReferences(new[] { MetadataReference.CreateFromFile(typeof(String).Assembly.Location) })
                .WithCompilationOptions(new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
            Document document = project.AddDocument($"{namePrefix}Document", source);
            project = document.Project;
            Compilation? compilation = project.GetCompilationAsync().Result;
            if (compilation == null)
                throw new InvalidOperationException("Bad compilation");
            CheckCompilationErrors(compilation);
            return document;
        }

        private static void CheckCompilationErrors(Compilation compilation)
        {
            Console.WriteLine("Checking compilation for errors, warnings and infos:");
            IList<Diagnostic> diagnostics = compilation.GetDiagnostics();
            Boolean hasErrors = false;
            foreach (Diagnostic diagnostic in diagnostics)
            {
                Console.WriteLine($"Diagnostic message: severity = {diagnostic.Severity}, message = \"{diagnostic.GetMessage()}\"");
                if (diagnostic.Severity == DiagnosticSeverity.Error)
                    hasErrors = true;
            }
            if (hasErrors)
                throw new InvalidOperationException("Bad compilation");
            if (diagnostics.Count == 0)
                Console.WriteLine("No any errors, warnings and infos");
            Console.WriteLine();
        }

        private static void ShowMethodBody(Document document, String methodName)
        {
            SyntaxNode root = document.GetSyntaxRootAsync().Result!;
            MethodDeclarationSyntax method = root
                .DescendantNodes()
                .OfType<MethodDeclarationSyntax>()
                .First(m => m.Identifier.Text == methodName);
            BlockSyntax body = method.Body!;
            PrintAst(body);
        }

        private static void PrintAst(SyntaxNodeOrToken node, string indent = "", bool isLast = true)
        {
            Console.Write(indent);
            Console.Write(isLast ? "└─ " : "├─ ");
            if (node.IsNode)
            {
                SyntaxNode? syntaxNode = node.AsNode();
                Console.WriteLine($"{syntaxNode?.GetType().Name} [{syntaxNode?.Kind()}]");
            }
            else
            {
                SyntaxToken token = node.AsToken();
                Console.WriteLine($"Token: '{token.Text}' [{token.Kind()}]");
            }
            String newIndent = indent + (isLast ? "   " : "│  ");
            ChildSyntaxList children = node.ChildNodesAndTokens();
            for (int index = 0; index < children.Count; ++index)
            {
                SyntaxNodeOrToken child = children[index];
                PrintAst(child, newIndent, index == children.Count - 1);
            }
        }

        static void Main(string[] _)
        {
            Console.WriteLine("For loop:");
            Document documentForLoop = Prepare(SourceForLoop, "ForLoop");
            ShowMethodBody(documentForLoop, "SomeMethod");
            Console.WriteLine();
            Console.WriteLine("While loop:");
            Document documentWhileLoop = Prepare(SourceWhileLoop, "WhileLoop");
            ShowMethodBody(documentWhileLoop, "SomeMethod");
        }

        private const String SourceForLoop = "namespace SomeNamespace\r\n" +
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
        private const String SourceWhileLoop = "namespace SomeNamespace\r\n" +
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
                                               "                ++index;\r\n" +
                                               "            }\r\n" +
                                               "        }\r\n" +
                                               "    }\r\n" +
                                               "}";
    }
}
