using System.ComponentModel.DataAnnotations;
using TaskScheduler;
using TaskScheduler.Core;

namespace Orchestrator
{
    internal class Program
    {
        static TaskScheduler<bool> scheduler = new TaskScheduler<bool>();

        static void Main(string[] args)
        {
            scheduler.Init(1,1);

            var tasks = new List<Task>();
            for (int i = 0; i < 3; i++)
            {
                tasks.Add(Task.Run(TestDriveAsync));
            }

            Task.WhenAll(tasks).Wait();
        }

        static async Task TestDriveAsync()
        {
            var service = new SomeService();
            var session = await scheduler.ScheduleNewTaskAsync(service.StartLongRunningTaskAsync);

            bool gotResult = false;

            while(gotResult == false)
            {
                var result = scheduler.GetTaskStatus(session);
                if (result.IsCompleted)
                {
                    await Task.Delay(2000);
                    continue;
                }
                else
                {
                    gotResult = true;
                    //Console.WriteLine($"Backup completed. Session: {session}\n\n");
                }
            }
        }
    }
}