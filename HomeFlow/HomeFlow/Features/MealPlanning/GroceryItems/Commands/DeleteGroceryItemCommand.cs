using HomeFlow.Data;

namespace HomeFlow.Features.MealPlanning.GroceryItems;

public record DeleteGroceryItemCommand( Guid Id ) : IRequest;

public class DeleteGroceryItemCommandHandler : IRequestHandler<DeleteGroceryItemCommand>
{
    private readonly IHomeFlowDbContext _context;

    public DeleteGroceryItemCommandHandler( IHomeFlowDbContext context )
    {
        _context = context;
    }

    public async Task Handle( DeleteGroceryItemCommand request, CancellationToken cancellationToken )
    {
        var entity = await _context.GroceryItems.FirstOrDefaultAsync( i => i.Id == request.Id, cancellationToken );

        Guard.Against.NotFound( request.Id, entity );

        _context.GroceryItems.Remove( entity );
        await _context.SaveChangesAsync( cancellationToken );
    }
}
