using HomeFlow.Models;

namespace HomeFlow.Features.Core.Tags;

public class TagEntity : Model
{
    public string Name { get; set; } = string.Empty;

    public string EntityType { get; set; } = string.Empty;

    public Guid EntityId { get; set; }
}
