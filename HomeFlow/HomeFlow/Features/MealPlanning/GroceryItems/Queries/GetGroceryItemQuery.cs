using HomeFlow.Data;

namespace HomeFlow.Features.MealPlanning.GroceryItems;

public record GetGroceryItemQuery : IRequest<List<GroceryItem>>;

public class GetGroceryItemQueryHandler : IRequestHandler<GetGroceryItemQuery, List<GroceryItem>>
{
    private readonly IHomeFlowDbContext _context;
    private readonly IMapper _mapper;

    public GetGroceryItemQueryHandler( IHomeFlowDbContext context, IMapper mapper )
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<List<GroceryItem>> Handle( GetGroceryItemQuery request, CancellationToken cancellationToken )
    {
        var groceryItems = await _context.GroceryItems
            .ProjectTo<GroceryItem>( _mapper.ConfigurationProvider )
            .OrderBy( i => i.Name )
            .ToListAsync( cancellationToken );

        var tags = await _context.Tags.Where( t => t.EntityType == "GroceryItem" ).ToListAsync();

        foreach ( var groceryItem in groceryItems )
        {
            groceryItem.Tags = tags.Where( t => t.EntityId == groceryItem.Id ).Select( t => t.Name ).ToList();
        }

        return groceryItems;
    }
}
