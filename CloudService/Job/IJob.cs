using CloudService.Infrastructure;
using System;
namespace CloudService.Job
{
    public interface IJob
    {       
		void Stop(IServiceContext serviceContext);
        void Execute(IServiceContext serviceContext);
    }
}
