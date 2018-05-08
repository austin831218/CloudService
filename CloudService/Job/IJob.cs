using System;
namespace CloudService.Job
{
    public interface IJob
    {
        Guid Identifier { get; }
		void Stop();
        void Execute();
    }
}
