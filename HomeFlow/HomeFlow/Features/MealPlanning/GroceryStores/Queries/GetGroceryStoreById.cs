using HomeFlow.Data;
using HomeFlow.Features.MealPlanning.GroceryItems;

namespace HomeFlow.Features.MealPlanning.GroceryStores;

public record GetGroceryStoreById( Guid Id ) : IRequest<GroceryStore?>;

public class GetGroceryStoreByIdHandler : IRequestHandler<GetGroceryStoreById, GroceryStore?>
{
    private readonly IHomeFlowDbContext _context;

    public GetGroceryStoreByIdHandler( IHomeFlowDbContext context )
    {
        _context = context;
    }

    public async Task<GroceryStore?> Handle( GetGroceryStoreById request, CancellationToken cancellationToken )
    {
        var entity = await _context.GroceryStores
            .Include( gs => gs.GroceryStoreAisles )
            .ThenInclude( a => a.GroceryStoreAisleGroceryItems )
            .ThenInclude( ai => ai.GroceryItem )
            .FirstOrDefaultAsync( gs => gs.Id == request.Id, cancellationToken );

        Guard.Against.NotFound( request.Id, entity );

        GroceryStore GroceryStore = new GroceryStore
        {
            Id = entity.Id,
            Name = entity.Name,
            Location = entity.Location
        };

        foreach ( var item in entity.GroceryStoreAisles.OrderBy( a => a.Order ) )
        {
            GroceryStore.GroceryStoreAisles.Add( new GroceryStoreAisle
            {
                Id = item.Id,
                Name = item.Name,
                Order = item.Order,
                GroceryItems = item.GroceryStoreAisleGroceryItems
                    .Select( i => new GroceryItem
                    {
                        Id = i.GroceryItem.Id,
                        Name = i.GroceryItem.Name,
                    } )
                    .ToList()
            } );
        }

        return GroceryStore;
    }
}
