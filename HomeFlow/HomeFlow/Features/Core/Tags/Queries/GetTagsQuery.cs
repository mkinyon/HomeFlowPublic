using HomeFlow.Data;

namespace HomeFlow.Features.Core.Tags;

public record GetTagsQuery( string EntityType, Guid EntityId ) : IRequest<List<string>>;

public class GetTagsQueryHandler : IRequestHandler<GetTagsQuery, List<string>>
{
    private readonly IHomeFlowDbContext _context;
    private readonly IMapper _mapper;

    public GetTagsQueryHandler( IHomeFlowDbContext context, IMapper mapper )
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<List<string>> Handle( GetTagsQuery request, CancellationToken cancellationToken )
    {

        var tags = await _context.Tags
            .Where( t => t.EntityType == request.EntityType && t.EntityId == request.EntityId )
            .Select( t => t.Name )
            .ToListAsync();

        return tags;
    }
}
