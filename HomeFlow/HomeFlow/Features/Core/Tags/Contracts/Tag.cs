namespace HomeFlow.Features.Core.Tags;

public class Tag
{
    public Guid Id { get; set; } = Guid.Empty;

    public string Name { get; set; } = string.Empty;

    public string EntityType { get; set; } = string.Empty;

    public Guid EntityId { get; set; } = Guid.Empty;
}
