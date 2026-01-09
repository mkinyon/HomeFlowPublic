using HomeFlow.Data;

namespace HomeFlow.Features.MealPlanning.Recipes;

public record SearchRecipesQuery( string SearchTerm, int Count ) : IRequest<List<Recipe>>;

public class SearchRecipesQueryHandler : IRequestHandler<SearchRecipesQuery, List<Recipe>>
{
    private readonly IHomeFlowDbContext _context;
    private readonly IMapper _mapper;

    public SearchRecipesQueryHandler( IHomeFlowDbContext context, IMapper mapper )
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<List<Recipe>> Handle( SearchRecipesQuery request, CancellationToken cancellationToken )
    {
        var recipes = await _context.Recipes
            .Include( r => r.Image )
            .Include( r => r.RecipeGroceryItems )
            .ThenInclude( r => r.GroceryItem )
            .ProjectTo<Recipe>( _mapper.ConfigurationProvider )
            .ToListAsync( cancellationToken: cancellationToken );

        var tags = await _context.Tags.Where( t => t.EntityType == "Recipe" ).ToListAsync();
        foreach ( var recipe in recipes )
        {
            recipe.Tags = tags.Where( t => t.EntityId == recipe.Id ).Select( t => t.Name ).ToList();
        }

        return recipes
            .Where( r => r.Name.Contains( request.SearchTerm, StringComparison.OrdinalIgnoreCase ) ||
                         r.Tags.Contains( request.SearchTerm, StringComparer.OrdinalIgnoreCase ) )
            .OrderBy( r => r.Name )
            .Take( request.Count )
            .ToList();
    }
}
