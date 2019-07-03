namespace Awesome.Net.Workflows.Expressions
{
    public interface IWorkflowExpression
    {
        string Syntax { get; }

        string Expression { get; }
    }

    public interface IWorkflowExpression<T> : IWorkflowExpression
    {
    }
}
