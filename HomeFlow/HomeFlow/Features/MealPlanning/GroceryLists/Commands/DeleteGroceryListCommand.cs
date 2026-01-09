using HomeFlow.Data;

namespace HomeFlow.Features.MealPlanning.GroceryLists;

public record DeleteGroceryListCommand( Guid Id ) : IRequest;

public class DeleteGroceryListCommandHandler : IRequestHandler<DeleteGroceryListCommand>
{
    private readonly IHomeFlowDbContext _context;

    public DeleteGroceryListCommandHandler( IHomeFlowDbContext context )
    {
        _context = context;
    }

    public async Task Handle( DeleteGroceryListCommand request, CancellationToken cancellationToken )
    {
        var entity = await _context.GroceryLists.FirstOrDefaultAsync( i => i.Id == request.Id, cancellationToken );

        Guard.Against.NotFound( request.Id, entity );

        _context.GroceryLists.Remove( entity );
        await _context.SaveChangesAsync( cancellationToken );
    }
}
