using HomeFlow.Data;

namespace HomeFlow.Features.MealPlanning.Recipes;

public record GetByGroceryItemQuery( Guid GroceryItemId ) : IRequest<List<Recipe>>;

public class GetByGroceryItemQueryHandler : IRequestHandler<GetByGroceryItemQuery, List<Recipe>>
{
    private readonly IHomeFlowDbContext _context;
    private readonly IMapper _mapper;

    public GetByGroceryItemQueryHandler( IHomeFlowDbContext context, IMapper mapper )
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<List<Recipe>> Handle( GetByGroceryItemQuery request, CancellationToken cancellationToken )
    {
        var recipes = await _context.Recipes
            .Where( r => r.RecipeGroceryItems.Any( ri => ri.GroceryItemId == request.GroceryItemId ) )
            .ProjectTo<Recipe>( _mapper.ConfigurationProvider )
            .ToListAsync( cancellationToken );

        return recipes;
    }
}
