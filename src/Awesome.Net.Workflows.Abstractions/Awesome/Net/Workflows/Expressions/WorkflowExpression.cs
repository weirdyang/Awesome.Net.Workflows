using Awesome.Net.Workflows.Expressions.Syntaxs;

namespace Awesome.Net.Workflows.Expressions
{
    public abstract class WorkflowExpression
    {
        protected WorkflowExpression()
        {
        }

        protected WorkflowExpression(string expression)
        {
            Expression = expression;
        }

        public string Syntax => SyntaxNameAttribute.GetSyntax(GetType());

        public string Expression { get; set; }

        public override string ToString() => Expression;
    }

    public abstract class WorkflowExpression<T> : WorkflowExpression, IWorkflowExpression<T>
    {
        protected WorkflowExpression()
        {
        }

        protected WorkflowExpression(string expression) : base(expression)
        {
        }
    }
}
