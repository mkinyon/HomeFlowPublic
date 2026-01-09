using HomeFlow.Data;
using HomeFlow.DataManagement;
using HomeFlow.Features.Core.SystemSettings;

namespace HomeFlow.Features.Core.DataManagement;

public record ImportDatabaseCommand( ImportRequest Request ) : IRequest;

public class ImportDatabaseCommandHandler : IRequestHandler<ImportDatabaseCommand>
{
    private readonly IHomeFlowDbContext _context;
    private readonly IMapper _mapper;
    private readonly ISender _sender;

    public ImportDatabaseCommandHandler( IHomeFlowDbContext context, IMapper mapper, ISender sender )
    {
        _context = context;
        _mapper = mapper;
        _sender = sender;
    }

    public async Task Handle( ImportDatabaseCommand request, CancellationToken cancellationToken )
    {
        // unzip and import the data
        if ( request.Request.Data == null || !request.Request.Data.Any() )
        {
            throw new ArgumentException( "Import data cannot be null or empty." );
        }

        using var stream = new MemoryStream();
        stream.Write( request.Request.Data, 0, request.Request.Data.Length );

        await DatabaseHelper.ImportDatabaseAsync( stream, _context, _sender, _mapper, cancellationToken );
    }
}