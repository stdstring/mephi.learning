using Microsoft.CodeAnalysis;

namespace SimplePythonPorter.Checker
{
    internal static class CompilationChecker
    {
        public static bool CheckCompilationErrors(string filename, Compilation compilation)
        {
            IList<Diagnostic> diagnostics = compilation.GetDiagnostics();
            Diagnostic[] diagnosticErrors = diagnostics.Where(d => d.Severity == DiagnosticSeverity.Error).ToArray();
            Diagnostic[] diagnosticWarnings = diagnostics.Where(d => d.Severity == DiagnosticSeverity.Warning).ToArray();
            bool hasErrors = false;
            Console.WriteLine($"Found {diagnosticErrors.Length} errors in the compilation");
            foreach (Diagnostic diagnostic in diagnosticErrors)
            {
                Console.WriteLine($"Found following error in the compilation of the {filename} entity: {diagnostic.GetMessage()}");
                hasErrors = true;
            }
            Console.WriteLine($"Found {diagnosticWarnings.Length} warnings in the compilation");
            foreach (Diagnostic diagnostic in diagnosticWarnings)
                Console.WriteLine($"Found following warning in the compilation: {diagnostic.GetMessage()}");
            return !hasErrors;
        }
    }
}
