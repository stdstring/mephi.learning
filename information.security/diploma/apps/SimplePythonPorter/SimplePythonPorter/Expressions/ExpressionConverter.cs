using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SimplePythonPorter.Common;
using SimplePythonPorter.DestStorage;
using System.Text;

namespace SimplePythonPorter.Expressions
{
    internal record ConvertResult(String Result, ImportData ImportData, IList<String> AfterResults)
    {
        public ConvertResult(String Result, ImportData ImportData) : this(Result, ImportData, Array.Empty<String>())
        {
        }
    }

    internal class ExpressionConverter
    {
        public ExpressionConverter(SemanticModel model, AppData appData)
        {
            _model = model;
            _appData = appData;
        }

        public ConvertResult Convert(ExpressionSyntax expression)
        {
            ExpressionConverterVisitor visitor = new ExpressionConverterVisitor(_model, _appData);
            visitor.VisitExpression(expression);
            return new ConvertResult(visitor.Buffer.ToString(), visitor.ImportData, visitor.AfterResults);
        }

        private readonly SemanticModel _model;
        private readonly AppData _appData;
    }

    internal class ExpressionConverterVisitor : CSharpSyntaxWalker
    {
        public ExpressionConverterVisitor(SemanticModel model, AppData appData)
        {
            _model = model;
            _appData = appData;
        }

        public StringBuilder Buffer { get; } = new StringBuilder();

        public ImportData ImportData { get; } = new ImportData();

        public IList<String> AfterResults { get; } = new List<String>();

        public void VisitExpression(ExpressionSyntax expression)
        {
            /*switch (expression)
            {
                default:
                    throw new UnsupportedSyntaxException($"Unsupported expression: {expression.Kind()}");
            }*/
            Buffer.Append("<expression>");
        }

        private readonly SemanticModel _model;
        private readonly AppData _appData;
    }
}
