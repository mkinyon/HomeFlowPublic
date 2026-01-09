using HomeFlow.Data;

namespace HomeFlow.Features.MealPlanning.GroceryLists;

public record GetGroceryListById( Guid Id ) : IRequest<GroceryList?>;

public class GetGroceryListByIdHandler : IRequestHandler<GetGroceryListById, GroceryList?>
{
    private readonly IHomeFlowDbContext _context;
    private readonly IMapper _mapper;

    public GetGroceryListByIdHandler( IHomeFlowDbContext context, IMapper mapper )
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<GroceryList?> Handle( GetGroceryListById request, CancellationToken cancellationToken )
    {
        var groceryList = await _context.GroceryLists
            .Include( gl => gl.Items )
                .ThenInclude( i => i.SourceRecipe )
            .Include( gl => gl.Items )
                .ThenInclude( i => i.RecipeGroceryItem )
            .Include( gl => gl.Items )
                .ThenInclude( i => i.GroceryItem )
            .ProjectTo<GroceryList>( _mapper.ConfigurationProvider )
            .FirstOrDefaultAsync( i => i.Id == request.Id, cancellationToken );

        Guard.Against.NotFound( request.Id, groceryList );

        return groceryList;
    }
}
