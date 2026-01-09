

using HomeFlow.Features.Core.SystemSettings;

namespace HomeFlow.Features.Core.DataManagement;

public interface IDataManagementService
{
    // Define methods for data management here
    Task ImportDataAsync( ImportRequest Request );
    Task<string> OptimizeDataAsync();
    Task SeedDatabaseAsync();
    Task<MemoryStream> ExportDataAsync();
}

public class DataManagementService : IDataManagementService
{
    private readonly IMediator _mediator;
    public DataManagementService( IMediator mediator )
    {
        _mediator = mediator;
    }

    public Task ImportDataAsync( ImportRequest Request ) =>
        _mediator.Send( new ImportDatabaseCommand( Request ) );

    public Task<string> OptimizeDataAsync() =>
        _mediator.Send( new OptimizeDatabaseCommand() );

    public Task SeedDatabaseAsync() =>
        _mediator.Send( new SeedDatabaseCommand() );

    public Task<MemoryStream> ExportDataAsync() =>
        _mediator.Send( new ExportDatabaseCommand() );
}
