using CloudService.Infrastructure;
using System;
using System.Threading;

namespace CloudService.Job
{
    public interface IJob
    {
        void Execute(IJobContext serviceContext, CancellationToken token);
    }
}
