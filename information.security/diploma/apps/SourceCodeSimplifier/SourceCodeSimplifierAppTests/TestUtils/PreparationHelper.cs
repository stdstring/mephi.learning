using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using NUnit.Framework;

namespace SourceCodeSimplifierAppTests.TestUtils
{
    internal static class PreparationHelper
    {
        public static void CheckCompilationErrors(Compilation compilation)
        {
            Console.WriteLine("Checking compilation for errors, warnings and infos:");
            IList<Diagnostic> diagnostics = compilation.GetDiagnostics();
            /*IList<Diagnostic> declarationDiagnostics = compilation.GetDeclarationDiagnostics();
            IList<Diagnostic> methodDiagnostics = compilation.GetMethodBodyDiagnostics();
            IList<Diagnostic> parseDiagnostics = compilation.GetParseDiagnostics();*/
            Boolean hasErrors = false;
            foreach (Diagnostic diagnostic in diagnostics)
            {
                Console.WriteLine($"Diagnostic message: severity = {diagnostic.Severity}, message = \"{diagnostic.GetMessage()}\"");
                if (diagnostic.Severity == DiagnosticSeverity.Error)
                    hasErrors = true;
            }
            Assert.That(hasErrors, Is.False);
            if (diagnostics.Count == 0)
                Console.WriteLine("No any errors, warnings and infos");
            Console.WriteLine();
        }

        public static Document Prepare(String source, String namePrefix)
        {
            AdhocWorkspace workspace = new AdhocWorkspace();
            Project project = workspace.AddProject($"{namePrefix}Project", "C#")
                .WithAssemblyName($"{namePrefix}Assembly")
                .WithMetadataReferences(new[] {MetadataReference.CreateFromFile(typeof(String).Assembly.Location)})
                .WithCompilationOptions(new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
            Document document = project.AddDocument($"{namePrefix}Document", source);
            project = document.Project;
            Compilation? compilation = project.GetCompilationAsync().Result;
            Assert.That(compilation, Is.Not.Null);
            CheckCompilationErrors(compilation!);
            return document;
        }

        public static void CheckCompilationErrors(Document sourceDocument, String namePrefix)
        {
            SourceText sourceText = sourceDocument.GetTextAsync().Result;
            AdhocWorkspace workspace = new AdhocWorkspace();
            Project project = workspace.AddProject($"{namePrefix}Project", "C#")
                .WithAssemblyName($"{namePrefix}Assembly")
                .WithMetadataReferences(new[] { MetadataReference.CreateFromFile(typeof(String).Assembly.Location) })
                .WithCompilationOptions(new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
            Document document = project.AddDocument("{namePrefix}Document", sourceText);
            project = document.Project;
            Compilation? compilation = project.GetCompilationAsync().Result;
            Assert.That(compilation, Is.Not.Null);
            CheckCompilationErrors(compilation!);
        }
    }
}