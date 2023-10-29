using Quartz;

namespace GalliumPlus.WebApi.Scheduling
{
    public static class QuartzExtensions
    {
        public static Task TriggerJobWithArgs(this IScheduler @this, JobKey jobKey, object args, CancellationToken ct = default)
        {
            IDictionary<string, object> dataMap = new Dictionary<string, object> { ["$args"] = args };
            return @this.TriggerJob(jobKey, new JobDataMap(dataMap), ct);
        }

        public static T GetArgs<T>(this IJobExecutionContext @this)
        {
            return (T)@this.Trigger.JobDataMap["$args"];
        }
    }
}
