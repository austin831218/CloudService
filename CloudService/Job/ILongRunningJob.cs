using System;
namespace CloudService.Job
{
    public interface ILongRunningJob : IJob
    {
        void Stop();
    }
}
