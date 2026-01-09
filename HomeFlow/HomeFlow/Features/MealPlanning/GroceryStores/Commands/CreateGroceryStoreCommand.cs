using HomeFlow.Data;

namespace HomeFlow.Features.MealPlanning.GroceryStores;

public record CreateGroceryStoreCommand( GroceryStore GroceryStoreRequest ) : IRequest<Guid>;

public class CreateGroceryStoreCommandHandler : IRequestHandler<CreateGroceryStoreCommand, Guid>
{
    private readonly IHomeFlowDbContext _context;

    public CreateGroceryStoreCommandHandler( IHomeFlowDbContext context )
    {
        _context = context;
    }

    public async Task<Guid> Handle( CreateGroceryStoreCommand request, CancellationToken cancellationToken )
    {
        var entity = new GroceryStoreEntity
        {
            Name = request.GroceryStoreRequest.Name,
            Location = request.GroceryStoreRequest.Location,
        };

        int i = 0;
        foreach ( var item in request.GroceryStoreRequest.GroceryStoreAisles )
        {
            var aisleEntity = new GroceryStoreAisleEntity
            {
                Name = item.Name,
                Order = i
            };

            i++;

            entity.GroceryStoreAisles.Add( aisleEntity );

            foreach ( var groceryItem in item.GroceryItems )
            {
                var storeAisleItemEntity = new GroceryStoreAisleGroceryItemEntity
                {
                    GroceryItemId = groceryItem.Id,
                    GroceryStoreAisle = aisleEntity
                };
                aisleEntity.GroceryStoreAisleGroceryItems.Add( storeAisleItemEntity );
            }
        }

        _context.GroceryStores.Add( entity );

        await _context.SaveChangesAsync( cancellationToken );

        return entity.Id;
    }
}
