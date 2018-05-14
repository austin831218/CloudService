using CloudService.Job;
using System;
using System.Collections.Generic;
using System.Text;

namespace CloudService.Infrastructure
{
    public interface IJobQueue
    {
        void Enqueue(string item, int number);
        string Dequeue();
        int Length { get; }
    }
}
