using System;
namespace CloudService.Job
{
    public interface IRepeatJob : IJob
    {
        long Interval { get; }
    }
}
