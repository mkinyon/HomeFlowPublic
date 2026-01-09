using HomeFlow.Data;

namespace HomeFlow.Features.MealPlanning.GroceryStores;

public record DeleteGroceryStoreCommand( Guid Id ) : IRequest;

public class DeleteGroceryStoreCommandHandler : IRequestHandler<DeleteGroceryStoreCommand>
{
    private readonly IHomeFlowDbContext _context;

    public DeleteGroceryStoreCommandHandler( IHomeFlowDbContext context )
    {
        _context = context;
    }

    public async Task Handle( DeleteGroceryStoreCommand request, CancellationToken cancellationToken )
    {
        var entity = await _context.GroceryStores.FirstOrDefaultAsync( i => i.Id == request.Id, cancellationToken );

        Guard.Against.NotFound( request.Id, entity );

        _context.GroceryStores.Remove( entity );
        await _context.SaveChangesAsync( cancellationToken );
    }
}
