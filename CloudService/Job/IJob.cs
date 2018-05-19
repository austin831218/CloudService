using CloudService.Infrastructure;
using System;
using System.Threading;

namespace CloudService.Job
{
    public interface IJob
    {
        void Stop(IServiceContext serviceContext, CancellationToken token);
        void Execute(IServiceContext serviceContext, CancellationToken token);
    }
}
