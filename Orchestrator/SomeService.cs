using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orchestrator
{
    internal class SomeService
    {
        public async Task<Result> StartLongRunningTaskAsync(Guid session)
        {
            Console.WriteLine($"Executing task for {session}");
            await Task.Delay(3000);
            return new Result { Succeeded = true };
        }
    }
}
