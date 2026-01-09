using HomeFlow.Data;

namespace HomeFlow.Features.MealPlanning.GroceryItems;

public record GetGroceryItemById( Guid Id ) : IRequest<GroceryItem?>;

public class GetGroceryItemByIdHandler : IRequestHandler<GetGroceryItemById, GroceryItem?>
{
    private readonly IHomeFlowDbContext _context;

    public GetGroceryItemByIdHandler( IHomeFlowDbContext context )
    {
        _context = context;
    }

    public async Task<GroceryItem?> Handle( GetGroceryItemById request, CancellationToken cancellationToken )
    {
        var entity = await _context.GroceryItems.FirstOrDefaultAsync( i => i.Id == request.Id, cancellationToken );

        Guard.Against.NotFound( request.Id, entity );

        GroceryItem groceryItem = new GroceryItem
        {
            Id = entity.Id,
            Name = entity.Name
        };

        groceryItem.Tags = await _context.Tags.Where( t => t.EntityId == groceryItem.Id ).Select( t => t.Name ).ToListAsync();

        return groceryItem;
    }
}
