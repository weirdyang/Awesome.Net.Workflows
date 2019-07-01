namespace Awesome.Net.Workflows.Expressions.Syntaxs
{
    [SyntaxName("Liquid")]
    public class LiquidExpression : WorkflowExpression<string>
    {
        public LiquidExpression()
        {
        }

        public LiquidExpression(string expression) : base(expression)
        {
        }
    }
}