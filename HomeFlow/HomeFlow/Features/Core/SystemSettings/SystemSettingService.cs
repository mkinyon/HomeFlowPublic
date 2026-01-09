

namespace HomeFlow.Features.Core.SystemSettings;

public interface ISystemSettingService
{
    Task<string> GetSystemSetting( string key );
    Task UpdateSystemSettingAsync( SystemSetting Request );
}

public class SystemSettingService : ISystemSettingService
{
    private readonly ISender _sender;

    public SystemSettingService( ISender sender )
    {
        _sender = sender;
    }

    public Task<string> GetSystemSetting( string key ) =>
        _sender.Send( new GetSystemSettingValue( key ) );

    public Task UpdateSystemSettingAsync( SystemSetting request ) =>
        _sender.Send( new UpdateSystemSettingCommand( request ) );

}
