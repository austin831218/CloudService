using System;
using Microsoft.Extensions.Configuration;

namespace CloudService.Common.Configuration
{
    public class AppSettings
    {
        public string ConnectionString { get; private set; }
        public int InitialThreadCount { get; private set; }
        public int MaxThreadCount { get; private set; }
        public int ThreadSuspend { get; private set; }
        public AppSettings(IConfigurationRoot configration)
        {
            ConnectionString = configration.GetSection("ConnectionString")["DB1"];
            InitialThreadCount = Convert.ToInt32(configration.GetSection("ThreadSettings")["InitialThreadCount"]);
            MaxThreadCount = Convert.ToInt32(configration.GetSection("ThreadSettings")["MaxThreadCount"]);
            ThreadSuspend = Convert.ToInt32(configration.GetSection("ThreadSettings")["ThreadSuspend"]);
        }
    }
}
