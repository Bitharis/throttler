using System.ComponentModel.DataAnnotations;

namespace Orchestrator
{
    internal class Program
    {
        static OrchestratorService orchestrator = new OrchestratorService();

        static void Main(string[] args)
        {
            var tasks = new List<Task>();
            for (int i = 0; i < 3; i++)
            {
                tasks.Add(Task.Run(TestDriveAsync));
            }

            Task.WhenAll(tasks).Wait();
        }

        static async Task TestDriveAsync()
        {
            var session = await orchestrator.StartBackup();

            bool gotResult = false;

            while(gotResult == false)
            {
                var result = await orchestrator.GetBackupStatus(session);
                if (result == null)
                {
                    await Task.Delay(2000);
                    continue;
                }
                else
                {
                    gotResult = true;
                    Console.WriteLine($"Backup completed. Session: {session}\n\n");
                }
            }
        }
    }
}