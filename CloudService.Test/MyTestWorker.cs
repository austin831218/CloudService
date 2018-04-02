using System;
using System.Collections.Generic;
using CloudService.Common;
using CloudService.Service.WorkTask;
using Microsoft.Extensions.Logging;

namespace CloudService.Test
{
    public class MyTestWorker : WorkTaskBase<MyTestWorker>
    {
        private ILogger<MyTestWorker> _log;
        public MyTestWorker()
        {
            _log = DependencyResolver.Resolve<ILogger<MyTestWorker>>();
        }
        protected override void Execute(MyTestWorker task)
        {
            throw new NotImplementedException();
        }

        protected override void OnException(MyTestWorker task, Exception ex)
        {
            base.OnException(task, ex);
        }

        protected override IEnumerable<MyTestWorker> PrepareTask()
        {
            throw new NotImplementedException();
        }
    }
}
