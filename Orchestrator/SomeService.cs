using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskScheduler
{
    internal class SomeService
    {
        public async Task<bool> StartLongRunningTaskAsync()
        {
            await Task.Delay(3000);
            return true;
        }
    }
}
