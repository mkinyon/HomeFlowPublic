using HomeFlow.Data;

namespace HomeFlow.Features.MealPlanning.Recipes;

public record GetRecipeRecommendationsQuery( int Count ) : IRequest<List<Recipe>>;

public class GetRecipeRecommendationsQueryHandler : IRequestHandler<GetRecipeRecommendationsQuery, List<Recipe>>
{
    private readonly IHomeFlowDbContext _context;
    private readonly IMapper _mapper;

    public GetRecipeRecommendationsQueryHandler( IHomeFlowDbContext context, IMapper mapper )
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<List<Recipe>> Handle( GetRecipeRecommendationsQuery request, CancellationToken cancellationToken )
    {
        var cutoffDate = DateTime.UtcNow.AddDays( -30 );

        // Get recipes that haven't been used in the meal planner in the last 30 days
        var recentRecipeIds = await _context.MealPlannerItems
            .Where( mpi => mpi.Created >= cutoffDate )
            .Select( mpi => mpi.RecipeId )
            .Distinct()
            .ToListAsync( cancellationToken );

        var recipes = await _context.Recipes
            .Where( r => !recentRecipeIds.Contains( r.Id ) )
            .ProjectTo<Recipe>( _mapper.ConfigurationProvider )
            .Take( request.Count )
            .ToListAsync( cancellationToken );

        // Get tags for the recipes
        if ( recipes.Any() )
        {
            var tags = await _context.Tags.Where( t => t.EntityType == "Recipe" ).ToListAsync( cancellationToken );

            foreach ( var recipe in recipes )
            {
                recipe.Tags = tags.Where( t => t.EntityId == recipe.Id ).Select( t => t.Name ).ToList();
            }
        }

        return recipes;
    }
}
