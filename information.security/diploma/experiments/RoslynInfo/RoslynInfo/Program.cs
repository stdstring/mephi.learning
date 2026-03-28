using Microsoft.CodeAnalysis.CSharp;

namespace RoslynInfo
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Type[] knownNodeTypes = typeof(CSharpSyntaxNode).Assembly.GetTypes()
                .Where(t => !t.IsAbstract && typeof(CSharpSyntaxNode).IsAssignableFrom(t))
                .OrderBy(t => t.Name)
                .ToArray();
            SyntaxKind[] syntaxKindValues = Enum.GetValues(typeof(SyntaxKind))
                .Cast<SyntaxKind>()
                .ToArray();
            Console.WriteLine($"Node types count = {knownNodeTypes.Length}");
            Console.WriteLine($"SyntaxKind values count = {syntaxKindValues.Length}");
        }
    }
}
