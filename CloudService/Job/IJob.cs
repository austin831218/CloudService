using System;
namespace CloudService.Job
{
    public interface IJob
    {
        string Name { get; }
        void Execute();
    }
}
