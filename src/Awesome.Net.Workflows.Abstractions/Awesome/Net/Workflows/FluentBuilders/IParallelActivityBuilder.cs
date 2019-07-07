using System;

namespace Awesome.Net.Workflows.FluentBuilders
{
    public interface IParallelActivityBuilder
    {
        IActivityBuilder ActivityBuilder { get; }
        IParallelActivityBuilder Branch(Action<IActivityBuilder> branchBuilder, string branchName = null);
        IActivityBuilder Join(string id, bool waitAll = true);
    }
}
