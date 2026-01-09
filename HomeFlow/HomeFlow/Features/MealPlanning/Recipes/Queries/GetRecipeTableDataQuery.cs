using HomeFlow.Data;
using HomeFlow.Services;


namespace HomeFlow.Features.MealPlanning.Recipes;

public record GetRecipeTableDataQuery( QueryOptions QueryOptions ) : IRequest<TableData<Recipe>>;

public class GetRecipeTableDataQueryHanderer : IRequestHandler<GetRecipeTableDataQuery, TableData<Recipe>>
{
    private readonly IHomeFlowDbContext _context;
    private readonly IMapper _mapper;

    public GetRecipeTableDataQueryHanderer( IHomeFlowDbContext context, IMapper mapper )
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<TableData<Recipe>> Handle( GetRecipeTableDataQuery request, CancellationToken cancellationToken )
    {
        var query = _context.Recipes.AsQueryable();

        // search
        string searchString = (request.QueryOptions.SearchTerm ?? string.Empty).ToLower();
        if ( searchString != string.Empty )
        {
            query = query.Where( r => r.Name.ToLower().Contains( searchString ) ||
                                      r.Description.ToLower().Contains( searchString ) ||
                                      r.Author.ToLower().Contains( searchString ) );
        }

        // sorting
        switch ( request.QueryOptions.SortBy )
        {
            case "name":
                query = request.QueryOptions.SortDescending
                    ? query.OrderByDescending( r => r.Name )
                    : query.OrderBy( r => r.Name );
                break;
            case "type":
                query = request.QueryOptions.SortDescending
                    ? query.OrderByDescending( r => r.RecipeType )
                    : query.OrderBy( r => r.RecipeType );
                break;
            case "description":
                query = request.QueryOptions.SortDescending
                    ? query.OrderByDescending( r => r.Description )
                    : query.OrderBy( r => r.Description );
                break;
            case "author":
                query = request.QueryOptions.SortDescending
                    ? query.OrderByDescending( r => r.Author )
                    : query.OrderBy( r => r.Author );
                break;
            default:
                query = query.OrderBy( i => i.Name );
                break;
        }

        var total = query.Count();

        if ( request.QueryOptions.PageSize > 0 )
        {
            query = query.Skip( request.QueryOptions.Page * request.QueryOptions.PageSize )
                            .Take( request.QueryOptions.PageSize );
        }

        var recipes = await query
                .Include( r => r.Image )
                .Include( r => r.RecipeSteps.OrderBy( rs => rs.Order ) )
                .Include( r => r.RecipeGroceryItems )
                .ThenInclude( r => r.GroceryItem )
                .ProjectTo<Recipe>( _mapper.ConfigurationProvider )
                .ToListAsync( cancellationToken );

        // get tags
        var tags = await _context.Tags.Where( t => t.EntityType == "Recipe" ).ToListAsync();
        foreach ( var recipe in recipes )
        {
            recipe.Tags = tags.Where( t => t.EntityId == recipe.Id ).Select( t => t.Name ).ToList();
        }

        return new TableData<Recipe>
        {
            TotalItems = total,
            Items = recipes
        };
    }
}
