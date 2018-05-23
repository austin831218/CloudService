using CloudService.Infrastructure;
using CloudService.Job;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using NLog;

namespace TestHost.Jobs
{
    public class TestCronJob1 : IJob
    {
        private readonly ILogger _logger = LogManager.GetCurrentClassLogger();
        public void Execute(IJobContext serviceContext, CancellationToken token)
        {

            int count = 10;
            while (count > 0 && !token.IsCancellationRequested)
            {
                serviceContext.Info("I'm doing cronjob {0}", count);
                Thread.Sleep(TimeSpan.FromSeconds(5));
                count--;
            }
        }

    }
}
