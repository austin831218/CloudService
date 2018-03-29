using System;
using System.Collections.Generic;
using System.Text;

namespace CloudService.Service.WorkTask
{
    public class TaskManager
    {
        public IList<IConfigurable> WorkTasks { get; }
        public TaskManager()
        {
            WorkTasks = new List<IConfigurable>();
        }
    }
}
