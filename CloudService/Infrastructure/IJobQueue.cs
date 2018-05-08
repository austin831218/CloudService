using System;
using System.Collections.Generic;
using System.Text;

namespace CloudService.Infrastructure
{
    public interface IJobQueue
    {
        void Enqueue(string item);
        string Dequeue();
        int Length { get; }
    }
}
