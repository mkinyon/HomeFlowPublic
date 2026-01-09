using HomeFlow.Data;
using HomeFlow.DataManagement;

namespace HomeFlow.Features.Core.DataManagement;

public record ExportDatabaseCommand() : IRequest<MemoryStream>;

public class ExportDatabaseCommandHandler : IRequestHandler<ExportDatabaseCommand, MemoryStream>
{
    private readonly IHomeFlowDbContext _context;
    private readonly IMapper _mapper;

    public ExportDatabaseCommandHandler( IHomeFlowDbContext context, IMapper mapper )
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<MemoryStream> Handle( ExportDatabaseCommand request, CancellationToken cancellationToken )
    {
        return await DatabaseHelper.ExportDatabaseAsync( _context, _mapper, cancellationToken );
    }
}
