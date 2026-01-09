using HomeFlow.Data;

namespace HomeFlow.Features.MealPlanning.GroceryLists;

public record GetByGroceryItemQuery( Guid GroceryItemId ) : IRequest<List<GroceryList>>;

public class GetByGroceryItemQueryHandler : IRequestHandler<GetByGroceryItemQuery, List<GroceryList>>
{
    private readonly IHomeFlowDbContext _context;
    private readonly IMapper _mapper;

    public GetByGroceryItemQueryHandler( IHomeFlowDbContext context, IMapper mapper )
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<List<GroceryList>> Handle( GetByGroceryItemQuery request, CancellationToken cancellationToken )
    {
        var groceryLists = await _context.GroceryLists
            .Include( gl => gl.Items )
            .Where( gl => gl.Items.Any( gli => gli.GroceryItemId == request.GroceryItemId ) )
            .ProjectTo<GroceryList>( _mapper.ConfigurationProvider )
            .ToListAsync( cancellationToken );

        return groceryLists;
    }
}