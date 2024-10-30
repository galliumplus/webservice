using GalliumPlus.Core.Data;
using GalliumPlus.Core.Logs.Builders;

namespace GalliumPlus.WebService.Services;

[ScopedService]
public class AuditService(ILogsDao logsDao)
{
    public void AddEntry(Action<AuditLogEntryBuilder> entryConfiguration)
    {
        var builder = new AuditLogEntryBuilder();
        entryConfiguration.Invoke(builder);
        logsDao.AddEntry(builder.Build());
    }
}