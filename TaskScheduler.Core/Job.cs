using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskScheduler.Core
{
    internal class Job<T>
    {
        public Job(Guid jobId, Func<Task<T>> taskDelegate)
        {
            ID = jobId;
            TaskDelegate = taskDelegate;
        }

        public Guid ID { get; private set; }

        public Func<Task<T>> TaskDelegate { get; private set; }
    }
}
