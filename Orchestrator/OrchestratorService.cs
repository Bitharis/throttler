using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orchestrator
{
    internal class OrchestratorService
    {
        private object _lock = new object();
        ConcurrentQueue<Guid> queuedTasks = new ConcurrentQueue<Guid>();
        Dictionary<Guid, Result> finishedTasks = new Dictionary<Guid, Result>();
        SemaphoreSlim throttler = new SemaphoreSlim(1,1);
        SomeService service;

        public OrchestratorService()
        {
            service = new SomeService();
            ExecuteQueuedTasks();
        }


        public Task<Guid> StartBackup()
        {
            var session = Guid.NewGuid();
            Console.WriteLine($"Created session {session}.");
            queuedTasks.Enqueue(session);
            Console.WriteLine($"Added session {session} to the queue.");
            return Task.FromResult(session);
        }

        public async Task<Result?> GetBackupStatus(Guid session)
        {
            lock (_lock)
            {
                if (finishedTasks.TryGetValue(session, out Result result))
                {
                    Console.WriteLine($"Backup session {session} completed.");
                    finishedTasks.Remove(session);
                return result;
                }
                else
                {
                    var stillWaiting = queuedTasks.Contains<Guid>(session);
                    Console.WriteLine($"Backup session {session} {(stillWaiting ? "waitng for execution" : "was not found")}");
                    return null;
                }
            }
        }

        private async Task ExecuteQueuedTasks()
        {
            while (true)
            {
                Console.WriteLine("Starting a new execution cycle.");
                await throttler.WaitAsync();
                try
                {
                    if (queuedTasks.TryPeek(out var session))
                    {
                        Console.WriteLine($"Starting a long running task for session: {session}");
                        Result result = await service.StartLongRunningTaskAsync(session);
                        Console.WriteLine($"Long running task for session {session} completed.");

                        queuedTasks.TryDequeue(out _);

                        lock (_lock) 
                        {
                            Console.WriteLine($"Adding it to finished tasks");
                            finishedTasks.TryAdd(session, result);
                        }
                    }
                    else
                    {
                        await Task.Delay(2000);
                    }
                }
                catch(Exception ex)
                {
                    Console.WriteLine("Releasng the semaphore..");
                    Console.WriteLine(ex);
                }
                finally
                {
                    throttler.Release();
                }
            }
        }
    }
}
