using System;
using System.Collections.Generic;
using System.Text;

namespace CloudService.Infrastructure.Interface
{
    public interface IConfigurable
    {
        void IncreaseWorker(int count);
        void DecreaseWorker(int count);
    }
}
