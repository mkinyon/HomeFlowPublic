using Cronos;
using HomeFlow.Data;
using HomeFlow.DataManagement;

public class BackupService : BackgroundService
{
    private readonly IServiceProvider _services;

    public BackupService( IServiceProvider services )
    {
        _services = services;
    }

    protected override async Task ExecuteAsync( CancellationToken stoppingToken )
    {
        var cron = CronExpression.Parse( "0 2 * * *" ); // Every day at 2:00 AM

        while ( !stoppingToken.IsCancellationRequested )
        {
            var nextRun = cron.GetNextOccurrence( DateTimeOffset.Now, TimeZoneInfo.Local );
            var delay = nextRun - DateTimeOffset.Now;

            if ( delay.HasValue )
            {
                await Task.Delay( delay.Value, stoppingToken );
            }

            try
            {
                using var scope = _services.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<IHomeFlowDbContext>();
                var mapper = scope.ServiceProvider.GetRequiredService<IMapper>();

                await ExportDatabaseAsync( context, mapper, stoppingToken );
            }
            catch ( Exception ex )
            {
                Console.WriteLine( $"Backup failed at {DateTime.Now}: {ex.Message}" );
            }
        }
    }

    private async Task ExportDatabaseAsync( IHomeFlowDbContext context, IMapper mapper, CancellationToken token )
    {
        using var stream = await DatabaseHelper.ExportDatabaseAsync( context, mapper, token );

        var zipFileName = $"homeflow-{DateTime.Now:yyyyMMddHHmmss}.zip";

        // Ensure backup directory exists
        var backupDirectory = DatabaseHelper.GetBackupDirectory();
        Directory.CreateDirectory( backupDirectory );

        // write the zip file to the local file system
        var filePath = Path.Combine( backupDirectory, zipFileName );
        using ( var fileStream = new FileStream( filePath, FileMode.Create, FileAccess.Write ) )
        {
            await stream.CopyToAsync( fileStream );
        }
    }
}
