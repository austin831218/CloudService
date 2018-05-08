using CloudService.Job;
using System;
using System.Collections.Generic;
using System.Linq;
using NLog;
using NCrontab;

namespace CloudService.Infrastructure
{
    class JobArranger
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private IEnumerable<IJob> _jobs;
        private IJobQueue _q;
        public JobArranger(IEnumerable<IJob> jobs, IJobQueue q)
        {
            _logger.Debug($"Jobs {string.Join(",", jobs.Select(x => x.Name))} are onboard");
            _jobs = jobs;
            _q = q;
        }


        public void ArrangeJobs()
        {
            var cronJobs = new List<ICronJob>();
            foreach(var j in _jobs)
            {
                if(j is ICronJob)
                {
                    cronJobs.Add(j as ICronJob);
                }
                else
                {
                    _q.Enqueue(j.Name);
                }
            }
            if (cronJobs.Count > 0)
            {
                keepArrangingCronJobs(cronJobs.ToArray());
            }
        }

        private void keepArrangingCronJobs(params ICronJob[] jobs)
        {
            
        }
    }
}
