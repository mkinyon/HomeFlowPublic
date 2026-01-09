


namespace HomeFlow.Features.Core.Tags;

public interface ITagService
{
    Task UpdateAsync( string entityTypeName, Guid entityId, List<string> newTags );
}

public class TagService : ITagService
{
    private readonly ISender _sender;

    public TagService( ISender sender )
    {
        _sender = sender;
    }

    public async Task UpdateAsync( string entityTypeName, Guid entityId, List<string> newTags ) =>
        await _sender.Send( new UpdateTagsCommand( entityTypeName, entityId, newTags ) );
}