using HomeFlow.Data;

namespace HomeFlow.Features.MealPlanning.GroceryItems;

public record MergeGroceryItemsCommand( GroceryItem NewGroceryItem, List<GroceryItem> GroceryItemsToMerge ) : IRequest;

public class MergeGroceryItemsCommandHandler : IRequestHandler<MergeGroceryItemsCommand>
{
    private readonly IHomeFlowDbContext _context;

    public MergeGroceryItemsCommandHandler( IHomeFlowDbContext context )
    {
        _context = context;
    }

    public async Task Handle( MergeGroceryItemsCommand request, CancellationToken cancellationToken )
    {
        // Save new grocery item to database
        var newItemEntity = new GroceryItemEntity
        {
            Name = request.NewGroceryItem.Name,
            Type = request.NewGroceryItem.GroceryItemType
        };

        _context.GroceryItems.Add( newItemEntity );
        await _context.SaveChangesAsync( cancellationToken );


        foreach ( var item in request.GroceryItemsToMerge )
        {
            // Update recipes
            var recipeGroceryItems = await _context.RecipeGroceryItems
                .Where( rgi => rgi.GroceryItemId == item.Id )
                .ToListAsync( cancellationToken );

            foreach ( var rpi in recipeGroceryItems )
            {
                rpi.GroceryItemId = newItemEntity.Id;
            }

            // Update grocery lists
            var groceryListItems = await _context.GroceryListItems
                .Where( gli => gli.GroceryItemId == item.Id )
                .ToListAsync( cancellationToken );

            foreach ( var gli in groceryListItems )
            {
                gli.GroceryItemId = newItemEntity.Id;
            }
        }

        // save merges
        await _context.SaveChangesAsync( cancellationToken );

        // Delete old grocery items
        foreach ( var item in request.GroceryItemsToMerge )
        {
            var entity = await _context.GroceryItems.FirstOrDefaultAsync( i => i.Id == item.Id, cancellationToken );
            Guard.Against.NotFound( item.Id, entity );
            _context.GroceryItems.Remove( entity );
        }

        await _context.SaveChangesAsync( cancellationToken );
    }
}
