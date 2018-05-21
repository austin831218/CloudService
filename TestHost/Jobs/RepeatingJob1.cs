using CloudService.Infrastructure;
using CloudService.Job;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace TestHost.Jobs
{
    public class RepeatingJob1 : IJob
    {
        public void Execute(IServiceContext serviceContext, CancellationToken token)
        {

            int count = 10;
            while (count > 0 && !token.IsCancellationRequested)
            {
                serviceContext.Info("I'm doing repeating job {0}", count);
                Thread.Sleep(TimeSpan.FromSeconds(1));
                count--;
            }
        }
    }
}
