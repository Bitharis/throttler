using ConcurrentObservableQueue;
using System.Collections.Concurrent;

namespace TaskScheduler.Core
{
    public class TaskScheduler<T>
    {
        private object locket = new object();
        private SemaphoreSlim semaphore;

        private IConcurrentObservableQueue<Job<T>> taskSessionIdQueue;
        private ConcurrentDictionary<Guid, T> completedTasks;

        public void Init(int initialCount, int maxCount)
        {
            semaphore = new SemaphoreSlim(initialCount, maxCount);
            taskSessionIdQueue = new ConcurrentObservableQueue<Job<T>>();
            taskSessionIdQueue.OnCollectionChanged += OnCollectionChangedHandler;
            completedTasks = new ConcurrentDictionary<Guid, T>();
        }

        public Task<Guid> ScheduleNewTaskAsync(Func<Task<T>> taskDelegate)
        {
            var taskSessionId = Guid.NewGuid();
            Console.WriteLine($"Add new task to the queue with ID:{taskSessionId}");
            taskSessionIdQueue.Enqueue(new Job<T>(taskSessionId, taskDelegate));
            return Task.FromResult(taskSessionId);
        }

        public async Task<T?> GetTaskStatus(Guid taskSessionId)
        {
            lock (completedTasks)
            {
                if (completedTasks.TryGetValue(taskSessionId, out T? result))
                {
                    Console.WriteLine($"Backup session {taskSessionId} completed.");
                    completedTasks.TryRemove(taskSessionId, out _);
                    return result;
                }
                else
                {
                    Console.WriteLine($"Task with session id: {taskSessionId} is waitng for execution");
                    return default(T);
                }
            }
        }

        private async Task StartExecuteQueuedTasksLoopAsync()
        {
            while (taskSessionIdQueue.IsEmpty == false)
            {
                Console.WriteLine("Starting a new execution cycle.");
                await semaphore.WaitAsync();
                try
                {
                    if (taskSessionIdQueue.TryPeek(out var job))
                    {
                        Console.WriteLine($"Starting a long running task for session: {job.ID}");
                        T? result = await job.TaskDelegate.Invoke();

                        Console.WriteLine($"Long running task for session {job.ID} completed.");
                        completedTasks.TryAdd(job.ID, result);
                        taskSessionIdQueue.TryDequeue(out _);
                    }
                    else
                    {
                        await Task.Delay(1000);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
                finally
                {
                    semaphore.Release();
                }
            }
        }

        private void OnCollectionChangedHandler(object? sender, CollectionChangedEventArgs e)
        {
            if(e.ChangeType == CollectionChangeType.FirstItemEnqueued)
            {
                StartExecuteQueuedTasksLoopAsync();
            }
        }
    }
}
