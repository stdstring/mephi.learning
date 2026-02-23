using Microsoft.CodeAnalysis;
using SourceCodeSimplifierApp.Output;

namespace SourceCodeSimplifierApp.Utils
{
    internal static class CompilationChecker
    {
        public static Boolean CheckCompilationErrors(String filename, Compilation compilation, IOutput output)
        {
            output.WriteInfoLine("Checking compilation for errors and warnings:");
            IList<Diagnostic> diagnostics = compilation.GetDiagnostics();
            Diagnostic[] diagnosticErrors = diagnostics.Where(d => d.Severity == DiagnosticSeverity.Error).ToArray();
            Diagnostic[] diagnosticWarnings = diagnostics.Where(d => d.Severity == DiagnosticSeverity.Warning).ToArray();
            Boolean hasErrors = false;
            output.WriteInfoLine($"Found {diagnosticErrors.Length} errors in the compilation");
            foreach (Diagnostic diagnostic in diagnosticErrors)
            {
                output.WriteErrorLine($"Found following error in the compilation of the {filename} entity: {diagnostic.GetMessage()}");
                hasErrors = true;
            }
            output.WriteInfoLine($"Found {diagnosticWarnings.Length} warnings in the compilation");
            foreach (Diagnostic diagnostic in diagnosticWarnings)
                output.WriteWarningLine($"Found following warning in the compilation: {diagnostic.GetMessage()}");
            return !hasErrors;
        }
    }
}