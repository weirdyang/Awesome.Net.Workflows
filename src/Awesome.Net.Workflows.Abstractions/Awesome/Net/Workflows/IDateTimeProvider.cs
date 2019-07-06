using System;

namespace Awesome.Net.Workflows
{
    public interface IDateTimeProvider
    {
        DateTime Now { get; }
    }
}
