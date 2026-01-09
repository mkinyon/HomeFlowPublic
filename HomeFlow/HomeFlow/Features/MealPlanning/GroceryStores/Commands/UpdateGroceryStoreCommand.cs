using HomeFlow.Data;


namespace HomeFlow.Features.MealPlanning.GroceryStores;

public record UpdateGroceryStoreCommand( GroceryStore GroceryStore ) : IRequest;

public class UpdateGroceryStoreCommandHandler : IRequestHandler<UpdateGroceryStoreCommand>
{
    private readonly IHomeFlowDbContext _context;

    public UpdateGroceryStoreCommandHandler( IHomeFlowDbContext context )
    {
        _context = context;
    }

    public async Task Handle( UpdateGroceryStoreCommand request, CancellationToken cancellationToken )
    {
        var entity = await _context.GroceryStores
            .Include( gs => gs.GroceryStoreAisles )
            .ThenInclude( gsa => gsa.GroceryStoreAisleGroceryItems )
            .FirstOrDefaultAsync( gs => gs.Id == request.GroceryStore.Id, cancellationToken );

        Guard.Against.NotFound( request.GroceryStore.Id, entity );

        entity.Name = request.GroceryStore.Name;
        entity.Location = request.GroceryStore.Location;

        var requestAisles = request.GroceryStore.GroceryStoreAisles.ToList();

        // Find aisles to remove
        foreach ( var existingAisle in entity.GroceryStoreAisles.ToList() )
        {
            if ( !requestAisles.Any( rs => rs.Id == existingAisle.Id ) )
            {
                _context.GroceryStoreAisles.Remove( existingAisle );
            }
        }

        // Add or update aisles
        int i = 0;
        foreach ( var requestAisle in requestAisles.OrderBy( a => a.Order ) )
        {
            var existingAisle = entity.GroceryStoreAisles
                .FirstOrDefault( rs => rs.Id != Guid.Empty && rs.Id == requestAisle.Id );

            if ( existingAisle == null )
            {
                existingAisle = new GroceryStoreAisleEntity
                {
                    Id = Guid.Empty,
                    Name = requestAisle.Name,
                    Order = i,
                };
                entity.GroceryStoreAisles.Add( existingAisle );
            }
            else
            {
                existingAisle.Name = requestAisle.Name;
                existingAisle.Order = i;
            }

            i++;

            var requestGroceryItems = requestAisle.GroceryItems.ToList();

            // Find grocery items to remove
            foreach ( var existingGroceryItem in existingAisle.GroceryStoreAisleGroceryItems.ToList() )
            {
                if ( !requestGroceryItems.Any( rs => rs.Id == existingGroceryItem.GroceryItemId ) )
                {
                    _context.GroceryStoreAisleGroceryItems.Remove( existingGroceryItem );
                }
            }

            // Add or update grocery items
            foreach ( var requestGroceryItem in requestGroceryItems )
            {
                var existingAisleGroceryItem = existingAisle.GroceryStoreAisleGroceryItems
                    .FirstOrDefault( rs => rs.GroceryItemId != Guid.Empty && rs.GroceryItemId == requestGroceryItem.Id );

                if ( existingAisleGroceryItem == null )
                {
                    existingAisleGroceryItem = new GroceryStoreAisleGroceryItemEntity
                    {
                        GroceryItemId = requestGroceryItem.Id,
                        GroceryStoreAisleId = existingAisle.Id,
                    };
                    existingAisle.GroceryStoreAisleGroceryItems.Add( existingAisleGroceryItem );
                }
                else
                {
                    existingAisleGroceryItem.GroceryItemId = requestGroceryItem.Id;
                }
            }
        }

        await _context.SaveChangesAsync( cancellationToken );
    }
}
