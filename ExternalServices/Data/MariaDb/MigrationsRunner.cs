using FluentMigrator.Runner;
using Microsoft.Extensions.DependencyInjection;

namespace GalliumPlus.WebApi.Data.MariaDb;

public class MigrationsRunner
{
    public static void UpdateDatabase(IServiceProvider serviceProvider)
    {
        var runner = serviceProvider.GetRequiredService<IMigrationRunner>();
        runner.MigrateUp();
    }
}