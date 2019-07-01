namespace Awesome.Net.Workflows.Expressions.Syntaxs
{
    [SyntaxName("JavaScript")]
    public class JavaScriptExpression<T> : WorkflowExpression<T>
    {
        public JavaScriptExpression()
        {
        }

        public JavaScriptExpression(string expression) : base(expression)
        {
        }
    }

    [SyntaxName("JavaScript")]
    public class JavaScriptExpression : JavaScriptExpression<string>
    {
    }
}