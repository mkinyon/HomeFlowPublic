using HomeFlow.Data;

namespace HomeFlow.Features.Core.SystemSettings;

public record UpdateSystemSettingCommand( SystemSetting SystemSetting ) : IRequest;

public class UpdateSystemSettingCommandHandler : IRequestHandler<UpdateSystemSettingCommand>
{
    private readonly IHomeFlowDbContext _context;

    public UpdateSystemSettingCommandHandler( IHomeFlowDbContext context )
    {
        _context = context;
    }

    public async Task Handle( UpdateSystemSettingCommand request, CancellationToken cancellationToken )
    {
        var setting = await _context.SystemSettings.FirstOrDefaultAsync( x => x.Key == request.SystemSetting.Key, cancellationToken );
        if ( setting != null )
        {
            setting.Value = request.SystemSetting.Value;
        }
        else
        {
            setting = new SystemSettingEntity
            {
                Key = request.SystemSetting.Key,
                Value = request.SystemSetting.Value
            };
            _context.SystemSettings.Add( setting );
        }
        await _context.SaveChangesAsync( cancellationToken );
    }
}
