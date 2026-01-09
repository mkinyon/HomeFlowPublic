namespace HomeFlow.Features.Core.Tags;

public class TagsRequest
{
    public Guid Id { get; set; } = Guid.Empty;

    public string EntityType { get; set; } = string.Empty;

    public Guid EntityId { get; set; } = Guid.Empty;

    public List<string> Tags { get; set; } = new List<string>();
}
