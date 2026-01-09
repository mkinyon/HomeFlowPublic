using HomeFlow.Data;
using HomeFlow.DataManagement;

namespace HomeFlow.Features.Core.DataManagement;

public record OptimizeDatabaseCommand() : IRequest<string>;

public class OptimizeDatabaseCommandHandler : IRequestHandler<OptimizeDatabaseCommand, string>
{
    private readonly IHomeFlowDbContext _context;

    public OptimizeDatabaseCommandHandler( IHomeFlowDbContext context )
    {
        _context = context;
    }

    public async Task<string> Handle( OptimizeDatabaseCommand request, CancellationToken cancellationToken )
    {
        return await DatabaseHelper.OptimizeDatabaseAsync( _context );
    }
}