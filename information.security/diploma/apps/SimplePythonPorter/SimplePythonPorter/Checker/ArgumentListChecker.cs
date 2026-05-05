using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace SimplePythonPorter.Checker
{
    internal static class ArgumentListChecker
    {
        public static CheckResult CheckForMethod(this IReadOnlyList<ArgumentSyntax> arguments)
        {
            foreach (ArgumentSyntax argument in arguments)
            {
                if (!argument.RefKindKeyword.IsKind(SyntaxKind.None))
                    return new CheckResult.Error("Unsupported argument kind: ref");
                if (!argument.RefOrOutKeyword.IsKind(SyntaxKind.None))
                    return new CheckResult.Error("Unsupported argument kind: out");
            }
            return new CheckResult.Ok();
        }
    }
}