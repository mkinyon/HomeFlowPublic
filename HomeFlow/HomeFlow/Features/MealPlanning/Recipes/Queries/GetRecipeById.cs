using HomeFlow.Data;

namespace HomeFlow.Features.MealPlanning.Recipes;

public record GetRecipeById( Guid Id ) : IRequest<Recipe?>;

public class GetRecipeByIdHandler : IRequestHandler<GetRecipeById, Recipe?>
{
    private readonly IHomeFlowDbContext _context;
    private readonly IMapper _mapper;

    public GetRecipeByIdHandler( IHomeFlowDbContext context, IMapper mapper )
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Recipe?> Handle( GetRecipeById request, CancellationToken cancellationToken )
    {
        var recipe = await _context.Recipes
            .Include( r => r.Image )
            .Include( r => r.RecipeSteps )
            .Include( r => r.RecipeGroceryItems )
            .ThenInclude( ri => ri.GroceryItem )
            .ProjectTo<Recipe>( _mapper.ConfigurationProvider )
            .FirstOrDefaultAsync( r => r.Id == request.Id, cancellationToken );

        if ( recipe != null )
        {
            recipe.RecipeSteps = recipe.RecipeSteps.OrderBy( rs => rs.Order ).ToList();
            recipe.RecipeGroceryItems = recipe.RecipeGroceryItems.OrderBy( rgi => rgi.Order ).ToList();

            recipe.Tags = await _context.Tags.Where( t => t.EntityId == recipe.Id ).Select( t => t.Name ).ToListAsync();
        }

        return recipe;
    }
}
