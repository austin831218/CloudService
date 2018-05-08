using System;
namespace CloudService.Job
{
    public interface IJob
    {
        string Name { get; }
        Guid Identifier { get; }
        int RequestConcurrencyThreads { get; }
        void Execute();
    }
}
