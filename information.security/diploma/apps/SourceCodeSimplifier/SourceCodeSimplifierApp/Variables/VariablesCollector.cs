using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SourceCodeSimplifierApp.Utils;

namespace SourceCodeSimplifierApp.Variables
{
    internal class VariablesCollector
    {
        public ISet<String> CollectExistingVariables(SyntaxNode node)
        {
            MemberDeclarationSyntax containedMember = node.GetParentMember();
            return CollectExistingVariables(containedMember);
        }

        public ISet<String> CollectExistingVariables(MemberDeclarationSyntax containedMember)
        {
            HashSet<String> existingVariables = new HashSet<String>();
            existingVariables.AddRange(GetParameterNames(containedMember));
            IList<String> variableDeclarators = containedMember
                .DescendantNodes()
                .OfType<VariableDeclaratorSyntax>()
                .Select(d => d.Identifier.ToString())
                .ToList();
            existingVariables.AddRange(variableDeclarators);
            IList<String> declarationExpressions = containedMember
                .DescendantNodes()
                .OfType<DeclarationExpressionSyntax>()
                .Select(d => d.Designation)
                .OfType<SingleVariableDesignationSyntax>()
                .Select(d => d.Identifier.ToString())
                .ToList();
            existingVariables.AddRange(declarationExpressions);
            return existingVariables;
        }

        private IList<String> GetParameterNames(MemberDeclarationSyntax containedMember)
        {
            return containedMember switch
            {
                MethodDeclarationSyntax methodDeclaration => methodDeclaration
                    .ParameterList
                    .Parameters
                    .Select(p => p.Identifier.ToString())
                    .ToList(),
                ConstructorDeclarationSyntax ctorDeclaration => ctorDeclaration
                    .ParameterList
                    .Parameters
                    .Select(p => p.Identifier.ToString())
                    .ToList(),
                IndexerDeclarationSyntax indexerDeclaration => indexerDeclaration
                    .ParameterList
                    .Parameters
                    .Select(p => p.Identifier.ToString())
                    .Append("value")
                    .ToList(),
                PropertyDeclarationSyntax => new[]{"value"},
                _ => new List<String>()
            };
        }
    }
}
