using System;

namespace Awesome.Net.Workflows.FluentBuilders
{
    public interface IWorkflow
    {
        Guid Id { get; }

        string WorkflowTypeId { get; }

        /// <summary>
        /// The name of this workflow.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Whether this workflow definition is enabled or not.
        /// </summary>
        bool IsEnabled { get; }

        /// <summary>
        /// Controls whether this workflow can spawn one or multiple instances.
        /// </summary>
        bool IsSingleton { get; }

        /// <summary>
        /// Controls whether workflow instances will be deleted upon completion.
        /// </summary>
        bool DeleteFinishedWorkflows { get; }

        void Build(IWorkflowBuilder builder);
    }
}
