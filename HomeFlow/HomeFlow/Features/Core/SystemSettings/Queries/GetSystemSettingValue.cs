using HomeFlow.Data;

namespace HomeFlow.Features.Core.SystemSettings;

public record GetSystemSettingValue( string key ) : IRequest<string>;

public class GetSystemSettingValueHandler : IRequestHandler<GetSystemSettingValue, string>
{
    private readonly IHomeFlowDbContext _context;

    public GetSystemSettingValueHandler( IHomeFlowDbContext context )
    {
        _context = context;
    }

    public async Task<string> Handle( GetSystemSettingValue request, CancellationToken cancellationToken )
    {
        var entity = await _context.SystemSettings.FirstOrDefaultAsync( x => x.Key == request.key, cancellationToken );

        return entity?.Value ?? string.Empty;
    }
}
