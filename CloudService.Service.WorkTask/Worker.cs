using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CloudService.Service.WorkTask
{
    public class Worker
    {
        private string _name = "Task";
        public string Name { get => _name; }
        private Task _task;

        public Worker(Task task) 
        {
            _name += "_" + task.Id.ToString();
            _task = task;
        }
        public Worker(string name, Task task) 
        {
            _name = name;
            _task = task;
        }
        public TaskStatus GetTaskStatus()
        {
            return _task.Status;
        }
    }
}
