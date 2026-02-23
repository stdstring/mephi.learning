using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SourceCodeSimplifierApp.Utils;

namespace SourceCodeSimplifierApp.Variables
{
    internal class VariableManager
    {
        public VariableManager()
        {
            _variableGeneratorMap = new Dictionary<MemberDeclarationSyntax, VariableGenerator>();
        }

        public String GenerateVariableName(SyntaxNode baseNode, String prefix)
        {
            MemberDeclarationSyntax containedMember = baseNode.GetParentMember();
            if (!_variableGeneratorMap.ContainsKey(containedMember))
            {
                VariablesCollector collector = new VariablesCollector();
                ISet<String> memberVariables = collector.CollectExistingVariables(containedMember);
                VariableGenerator generator = new VariableGenerator(memberVariables);
                _variableGeneratorMap[containedMember] = generator;
            }
            return _variableGeneratorMap[containedMember].Generate(prefix);
        }

        private readonly IDictionary<MemberDeclarationSyntax, VariableGenerator> _variableGeneratorMap;
    }
}
