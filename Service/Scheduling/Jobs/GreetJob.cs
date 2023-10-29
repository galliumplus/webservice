using Quartz;

namespace GalliumPlus.WebApi.Scheduling.Jobs
{
    [JobConfiguration("Greet", StoreDurably = true)]
    public class GreetJob : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            string? name = context.Trigger.JobDataMap.GetString("name");

            await Task.Delay(TimeSpan.FromSeconds(10));

            Console.WriteLine($"Hello, {name ?? "World"}!");
        }
    }
}
