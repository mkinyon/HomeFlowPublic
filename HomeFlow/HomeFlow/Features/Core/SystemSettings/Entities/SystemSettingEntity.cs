using HomeFlow.Models;

namespace HomeFlow.Features.Core.SystemSettings;

public class SystemSettingEntity : Model
{
    public string Key { get; set; } = string.Empty;

    public string Value { get; set; } = string.Empty;
}
