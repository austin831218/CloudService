using System;
using System.Collections.Generic;
using CloudService.Common;
using CloudService.Service.WorkTask;
using Microsoft.Extensions.Logging;

namespace CloudService.Test
{
    public class MyTestWorker : WorkTaskBase<int>
    {
        private ILogger<MyTestWorker> _log;

        public MyTestWorker() : this("Worker")
        {
        }
        public MyTestWorker(string name) : base(name)
        {
            _log = DependencyResolver.Resolve<ILogger<MyTestWorker>>();
        }
        public MyTestWorker(string name, WorkTaskSettings settings) : base(name, settings)
        {
            _log = DependencyResolver.Resolve<ILogger<MyTestWorker>>();
        }
        protected override void Execute(int task)
        {
            _log.LogInformation($"Executing task {task}");
            System.Threading.Thread.Sleep(1000);
        }

        protected override void OnException(int task, Exception ex)
        {
            base.OnException(task, ex);
        }

        protected override IEnumerable<int> PrepareTask()
        {
            _log.LogInformation($"Prepared 10 tasks");
            var tasks = new List<int>();
            for (var i = 0; i < 1000; i++)
            {
                tasks.Add(i);
            }
            return tasks;
        }
    }
}
