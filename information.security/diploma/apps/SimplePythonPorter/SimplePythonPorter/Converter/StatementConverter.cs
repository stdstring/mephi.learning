using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SimplePythonPorter.Common;
using SimplePythonPorter.DestStorage;
using SimplePythonPorter.Expressions;
using SimplePythonPorter.Utils;

namespace SimplePythonPorter.Converter
{
    internal class StatementConverterVisitor : CSharpSyntaxWalker
    {
        public StatementConverterVisitor(SemanticModel model, MethodStorage currentMethod, AppData appData)
        {
            _model = model;
            _currentMethod = currentMethod;
            _appData = appData;
        }

        public override void VisitBlock(BlockSyntax node)
        {
            VisitStatements(node.Statements, false);
        }

        public override void VisitLocalDeclarationStatement(LocalDeclarationStatementSyntax node)
        {
            foreach (VariableDeclaratorSyntax variable in node.Declaration.Variables)
                ProcessVariableDeclaration(variable);
        }

        public override void VisitExpressionStatement(ExpressionStatementSyntax node)
        {
            String expression = ConvertExpression(node.Expression);
            _currentMethod.AddBodyLine(expression);
        }

        public override void VisitForEachStatement(ForEachStatementSyntax node)
        {
            TypeInfo loopVariableInfo = _model.GetTypeInfo(node.Type);
            if (loopVariableInfo.Type == null)
                throw new UnsupportedSyntaxException("Unrecognizable type of foreach loop variable");
            TypeInfo loopSourceInfo = _model.GetTypeInfo(node.Expression);
            if (loopSourceInfo.Type == null)
                throw new UnsupportedSyntaxException("Unrecognizable type of foreach loop source");
            String enumerationVariable = _appData.NameTransformer.TransformLocalVariableName(node.Identifier.Text);
            String forEachExpression = ConvertExpression(node.Expression);
            _currentMethod.AddBodyLine($"for {enumerationVariable} in {forEachExpression}:");
            VisitStatement(node.Statement, true);
        }

        public override void VisitIfStatement(IfStatementSyntax node)
        {
            VisitIfStatementImpl(node, "if");
        }

        public override void VisitWhileStatement(WhileStatementSyntax node)
        {
            String condition = ConvertExpression(node.Condition);
            _currentMethod.AddBodyLine($"while {condition}:");
            VisitStatement(node.Statement, true);
        }

        public override void VisitDoStatement(DoStatementSyntax node)
        {
            _currentMethod.AddBodyLine("while True:");
            VisitStatement(node.Statement, true);
            _currentMethod.IncreaseLocalIndentation();
            String condition = ConvertExpression(node.Condition);
            _currentMethod.AddBodyLine($"if {condition}:");
            _currentMethod.AddBodyLine("break");
            _currentMethod.DecreaseLocalIndentation();
        }

        public override void VisitBreakStatement(BreakStatementSyntax node)
        {
            _currentMethod.AddBodyLine("break");
        }

        public override void VisitContinueStatement(ContinueStatementSyntax node)
        {
            _currentMethod.AddBodyLine("continue");
        }

        public override void VisitReturnStatement(ReturnStatementSyntax node)
        {
            String delimiter = node.Expression == null ? "" : " ";
            String expression = node.Expression == null ? "" : ConvertExpression(node.Expression);
            _currentMethod.AddBodyLine($"return{delimiter}{expression}");
        }

        public override void VisitThrowStatement(ThrowStatementSyntax node)
        {
            if (node.Expression == null)
                _currentMethod.AddBodyLine("raise");
            else
            {
                String exception = ConvertExpression(node.Expression);
                _currentMethod.AddBodyLine($"raise {exception}");
            }
        }

        private void VisitStatements(IReadOnlyList<StatementSyntax> statements, bool indent)
        {
            foreach (StatementSyntax statement in statements)
                VisitStatement(statement, indent);
        }

        private void VisitStatement(StatementSyntax statement, bool indent)
        {
            if (indent)
                _currentMethod.IncreaseLocalIndentation();
            ISet<SyntaxKind> knownStatements = new HashSet<SyntaxKind>
            {
                SyntaxKind.LocalDeclarationStatement,
                SyntaxKind.ExpressionStatement,
                SyntaxKind.ForEachStatement,
                SyntaxKind.IfStatement,
                //SyntaxKind.SwitchStatement,
                SyntaxKind.WhileStatement,
                SyntaxKind.DoStatement,
                SyntaxKind.BreakStatement,
                SyntaxKind.ContinueStatement,
                SyntaxKind.Block,
                SyntaxKind.ReturnStatement,
                SyntaxKind.ThrowStatement
            };
            if (!knownStatements.Contains(statement.Kind()))
                throw new UnsupportedSyntaxException($"Unsupported statement type: {statement.Kind()}");
            Visit(statement);
            if (indent)
                _currentMethod.DecreaseLocalIndentation();
        }

        private void ProcessVariableDeclaration(VariableDeclaratorSyntax variable)
        {
            String name = _appData.NameTransformer.TransformLocalVariableName(variable.Identifier.Text);
            String initializer = "None";
            //IList<String> afterResults = new List<String>();
            if (variable.Initializer != null)
            {
                ExpressionConverter expressionConverter = new ExpressionConverter(_model, _appData);
                ConvertResult result = expressionConverter.Convert(variable.Initializer.Value);
                _currentMethod.ImportStorage.Append(result.ImportData);
                initializer = result.Result;
                //afterResults = result.AfterResults.Select(entry => $"{name}.{entry}").ToList();
            }
            _currentMethod.AddBodyLine($"{name} = {initializer}");
            //afterResults.Foreach(entry => _currentMethod.AddBodyLine($"{entry}"));
        }

        private void VisitIfStatementImpl(IfStatementSyntax node, String ifOperator)
        {
            String condition = ConvertExpression(node.Condition);
            _currentMethod.AddBodyLine($"{ifOperator} {condition}:");
            VisitStatement(node.Statement, true);
            switch (node.Else)
            {
                case null:
                    break;
                case var _ when node.Else.Statement.Kind() == SyntaxKind.IfStatement:
                    VisitIfStatementImpl(node.Else.Statement.MustCast<StatementSyntax, IfStatementSyntax>(), "elif");
                    break;
                default:
                    _currentMethod.AddBodyLine("else:");
                    VisitStatement(node.Else.Statement, true);
                    break;
            }
        }
        private String ConvertExpression(ExpressionSyntax expression)
        {
            ExpressionConverter expressionConverter = new ExpressionConverter(_model, _appData);
            ConvertResult result = expressionConverter.Convert(expression);
            _currentMethod.ImportStorage.Append(result.ImportData);
            return result.Result;
        }

        private readonly SemanticModel _model;
        private readonly MethodStorage _currentMethod;
        private readonly AppData _appData;
    }
}
