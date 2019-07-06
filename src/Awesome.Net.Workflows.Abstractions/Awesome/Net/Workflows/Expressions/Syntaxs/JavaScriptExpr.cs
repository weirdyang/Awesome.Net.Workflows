namespace Awesome.Net.Workflows.Expressions.Syntaxs
{
    [SyntaxName("JavaScript")]
    public class JavaScriptExpr<TReturn> : WorkflowExpression<TReturn>
    {
        public JavaScriptExpr(string expression) : base(expression, "JavaScript")
        {
        }
    }

    [SyntaxName("JavaScript")]
    public class JavaScriptExpr : JavaScriptExpr<string>
    {
        public JavaScriptExpr(string expression) : base(expression)
        {
        }
    }
}
