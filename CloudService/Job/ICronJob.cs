using System;
namespace CloudService.Job
{
    public interface ICronJob : IJob
    {
        string CronExpression { get; set; }
    }
}
