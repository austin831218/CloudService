using System;
using Microsoft.Extensions.Configuration;

namespace CloudService.Model.Configuration
{
    public class AppSettings
    {
        public string ConnectionString { get; private set; }
        public AppSettings(IConfigurationRoot configration)
        {
            ConnectionString = configration.GetSection("ConnectionString")["DB1"];
        }
    }
}
