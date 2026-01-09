using HomeFlow.Data;

namespace HomeFlow.Features.MealPlanning.GroceryLists;

public record GetGroceryListQuery( bool IncludePrinted ) : IRequest<List<GroceryList>>;

public class GetGroceryListQueryHanderer : IRequestHandler<GetGroceryListQuery, List<GroceryList>>
{
    private readonly IHomeFlowDbContext _context;
    private readonly IMapper _mapper;

    public GetGroceryListQueryHanderer( IHomeFlowDbContext context, IMapper mapper )
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<List<GroceryList>> Handle( GetGroceryListQuery request, CancellationToken cancellationToken )
    {
        var groceryLists = await _context.GroceryLists
            .Include( r => r.Items )
            .ProjectTo<GroceryList>( _mapper.ConfigurationProvider )
            .Where( r => request.IncludePrinted || !r.IsPrinted )
            .OrderBy( r => r.Name )
            .ToListAsync();

        return groceryLists;
    }
}
