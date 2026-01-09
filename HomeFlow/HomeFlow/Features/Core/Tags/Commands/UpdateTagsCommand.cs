using HomeFlow.Data;

namespace HomeFlow.Features.Core.Tags;

public record UpdateTagsCommand( string EntityTypeName, Guid EntityId, List<string> Tags ) : IRequest;

public class UpdateTagsCommandHandler : IRequestHandler<UpdateTagsCommand>
{
    private readonly IHomeFlowDbContext _context;

    public UpdateTagsCommandHandler( IHomeFlowDbContext context )
    {
        _context = context;
    }

    public async Task Handle( UpdateTagsCommand request, CancellationToken cancellationToken )
    {
        var existingTags = _context.Tags
            .Where( et => et.EntityType == request.EntityTypeName && et.EntityId == request.EntityId )
            .ToList();

        // Remove tags that are no longer in the newTags list
        foreach ( var tag in existingTags.Where( t => !request.Tags.Contains( t.Name ) ).ToList() )
        {
            var tagToRemove = _context.Tags.First( t => t.Id == tag.Id );
            _context.Tags.Remove( tagToRemove );
        }

        // Add new tags that are not in the existing tags list
        foreach ( var newTag in request.Tags.Where( t => !existingTags.Select( et => et.Name ).Contains( t ) ).ToList() )
        {
            _context.Tags.Add( new TagEntity
            {
                EntityType = request.EntityTypeName,
                EntityId = request.EntityId,
                Name = newTag
            } );
        }

        await _context.SaveChangesAsync();
    }
}
