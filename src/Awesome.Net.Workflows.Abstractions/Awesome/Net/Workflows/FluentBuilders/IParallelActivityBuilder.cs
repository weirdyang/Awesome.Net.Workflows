using System;

namespace Awesome.Net.Workflows.FluentBuilders
{
    public interface IParallelActivityBuilder
    {
        IActivityBuilder ActivityBuilder { get; }
        IParallelActivityBuilder Do(string branch, Action<IActivityBuilder> branchBuilder);
        IActivityBuilder Join(string id, bool waitAll = true);
    }
}
