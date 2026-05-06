using Microsoft.CodeAnalysis.CSharp.Syntax;
using SimplePythonPorter.DestStorage;
using SimplePythonPorter.Expressions;

namespace SimplePythonPorter.ExternalEntities
{
    internal record TypeResolveData(String TypeName, String ModuleName, ImportData ImportData)
    {
        public String GetTypeFullName()
        {
            return String.IsNullOrEmpty(ModuleName) ? TypeName : $"{ModuleName}.{TypeName}";
        }
    }

    internal record MemberData(ExpressionSyntax Target, SimpleNameSyntax Name, IReadOnlyList<ArgumentSyntax> Arguments);

    internal record MemberRepresentation(String Target, ConvertedArguments Arguments);

    internal record MemberResolveData(String Member, ImportData ImportData)
    {
        public MemberResolveData(String member) : this(member, new ImportData())
        {
        }
    }
}
