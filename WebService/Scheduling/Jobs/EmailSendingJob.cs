using GalliumPlus.Core.Email;
using Quartz;

namespace GalliumPlus.WebService.Scheduling.Jobs
{
    [JobConfiguration("Send", Group = "Email", StoreDurably = true)]
    public class EmailSendingJob : IJob
    {
        public record class Args(string recipient, string subject, string template, object view);

        public static JobKey JobKey => new("Send", "Email");

        private IEmailTemplateLoader loader;
        private IEmailSender sender;
        private ILogger logger;

        public EmailSendingJob(IEmailTemplateLoader loader, IEmailSender sender, ILoggerFactory loggerFactory)
        {
            this.loader = loader;
            this.sender = sender;
            this.logger = loggerFactory.CreateLogger<EmailSendingJob>();
        }

        public async Task Execute(IJobExecutionContext context)
        {
            Args args = context.GetArgs<Args>();

            EmailTemplate template = this.loader.GetTemplate(args.template);

            this.logger.LogInformation("");

            await this.sender.SendAsync(args.recipient, args.subject, template.Render(args.view));
        }
    }
}
