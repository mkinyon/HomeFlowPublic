using HomeFlow.Data;
using HomeFlow.DataManagement;

namespace HomeFlow.Features.Core.DataManagement;

public record SeedDatabaseCommand() : IRequest;

public class SeedDatabaseCommandHandler : IRequestHandler<SeedDatabaseCommand>
{
    private readonly IHomeFlowDbContext _context;
    private readonly IMapper _mapper;

    public SeedDatabaseCommandHandler( IHomeFlowDbContext context, IMapper mapper )
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task Handle( SeedDatabaseCommand request, CancellationToken cancellationToken )
    {
        await DatabaseHelper.SeedDatabaseAsync( _context, _mapper );
    }
}