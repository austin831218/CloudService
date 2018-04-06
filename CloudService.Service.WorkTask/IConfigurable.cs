using System;
using System.Collections.Concurrent;
using System.Text;
using System.Threading.Tasks;

namespace CloudService.Service.WorkTask
{
    public interface IConfigurable
    {
        string Name { get; }
        ConcurrentDictionary<int, Worker> Workers { get; }
        //ConcurrentBag<T> Errors { get; }
        void IncreaseWorkerCount(int count);
        Task Stop();
        Task Stop(Action onStop);

    }
}
