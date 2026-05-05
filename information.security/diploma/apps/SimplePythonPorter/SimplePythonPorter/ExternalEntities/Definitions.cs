using Microsoft.CodeAnalysis.CSharp.Syntax;
using SimplePythonPorter.DestStorage;
using SimplePythonPorter.Expressions;

namespace SimplePythonPorter.ExternalEntities
{
    internal record TypeResolveData(String TypeName, String ModuleName, ImportData ImportData);

    internal record MemberData(ExpressionSyntax Target, SimpleNameSyntax Name, IReadOnlyList<ArgumentSyntax> Arguments);

    internal record MemberRepresentation(String Target, ConvertedArguments Arguments);

    internal record MemberResolveData(String Member, ImportData ImportData)
    {
        public MemberResolveData(String member) : this(member, new ImportData())
        {
        }
    }

    /*internal interface IExternalEntityResolver
    {
        MemberResolveData ResolveCtor(ITypeSymbol sourceType, IReadOnlyList<ArgumentSyntax> argumentsData, ConvertedArguments argumentsRepresentation);
        MemberResolveData ResolveMember(MemberData data, ITypeSymbol sourceType, MemberRepresentation representation);
    }*/
}
