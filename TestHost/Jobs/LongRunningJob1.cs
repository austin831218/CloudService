using CloudService.Job;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace TestHost.Jobs
{
    public class LongRunningJob1 : IJob
    {
        public void Execute(IJobContext serviceContext, CancellationToken token)
        {

            int count = 100;
            while (!token.IsCancellationRequested)
            {
                serviceContext.Info("I'm doing long running job {0}", count);
                Thread.Sleep(TimeSpan.FromSeconds(1));
                count++;
            }
        }
    }
}
