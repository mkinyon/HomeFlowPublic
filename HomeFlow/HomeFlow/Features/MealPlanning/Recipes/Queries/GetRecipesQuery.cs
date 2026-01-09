using HomeFlow.Data;


namespace HomeFlow.Features.MealPlanning.Recipes;

public record GetRecipesQuery : IRequest<List<Recipe>>;

public class GetRecipesQueryHanderer : IRequestHandler<GetRecipesQuery, List<Recipe>>
{
    private readonly IHomeFlowDbContext _context;
    private readonly IMapper _mapper;

    public GetRecipesQueryHanderer( IHomeFlowDbContext context, IMapper mapper )
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<List<Recipe>> Handle( GetRecipesQuery request, CancellationToken cancellationToken )
    {
        var recipes = await _context.Recipes
            .Include( r => r.Image )
            .Include( r => r.RecipeSteps.OrderBy( rs => rs.Order ) )
            .Include( r => r.RecipeGroceryItems )
            .ThenInclude( r => r.GroceryItem )
            .ProjectTo<Recipe>( _mapper.ConfigurationProvider )
            .OrderBy( r => r.Name )
            .ToListAsync();

        var tags = await _context.Tags.Where( t => t.EntityType == "Recipe" ).ToListAsync();

        foreach ( var recipe in recipes )
        {
            recipe.Tags = tags.Where( t => t.EntityId == recipe.Id ).Select( t => t.Name ).ToList();
        }

        return recipes;
    }
}
