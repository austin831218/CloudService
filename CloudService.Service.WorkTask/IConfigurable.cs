using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CloudService.Service.WorkTask
{
    public interface IConfigurable
    {
        IList<Worker> Workers { get; }
        void IncreaseWorkerCount(int count);
        Task Stop();
        Task Stop(Action onStop);

    }
}
