using System;
using System.Collections.Generic;
using System.Text;

namespace CloudService.Host
{
    public interface IHostConifuration
    {
        int Capacity { get; set; }
        int MaxCapacity { get; set; }
    }
    internal class HostConfiguration : IHostConifuration
    {
        public HostConfiguration()
        {
            this.Capacity = 10;
            this.MaxCapacity = 20;
        }
        public int Capacity { get; set; }
        public int MaxCapacity { get; set; }
    }
}
