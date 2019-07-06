namespace Awesome.Net.Workflows.Expressions
{
    public class WorkflowExpression<TReturn>
    {
        public string Syntax { get; }

        public string Expression { get; }

        public override string ToString() => Expression;

        public WorkflowExpression(string expression, string syntax)
        {
            Expression = expression;
            Syntax = syntax;
        }
    }
}
