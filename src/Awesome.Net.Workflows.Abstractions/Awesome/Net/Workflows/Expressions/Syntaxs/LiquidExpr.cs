namespace Awesome.Net.Workflows.Expressions.Syntaxs
{
    [SyntaxName("Liquid")]
    public class LiquidExpr : WorkflowExpression<string>
    {
        public LiquidExpr(string expression) : base(expression, "Liquid")
        {
        }
    }
}
