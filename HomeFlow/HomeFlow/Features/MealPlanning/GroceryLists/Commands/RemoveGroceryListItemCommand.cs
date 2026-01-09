using HomeFlow.Data;

namespace HomeFlow.Features.MealPlanning.GroceryLists;

public record RemoveGroceryListItemCommand( Guid Id ) : IRequest;

public class DeleteGroceryListItemCommandHandler : IRequestHandler<RemoveGroceryListItemCommand>
{
    private readonly IHomeFlowDbContext _context;

    public DeleteGroceryListItemCommandHandler( IHomeFlowDbContext context )
    {
        _context = context;
    }

    public async Task Handle( RemoveGroceryListItemCommand request, CancellationToken cancellationToken )
    {
        var groceryListItemEntity = await _context.GroceryListItems.FirstOrDefaultAsync( gl => gl.Id == request.Id );

        Guard.Against.NotFound( request.Id, groceryListItemEntity );

        _context.GroceryListItems.Remove( groceryListItemEntity );

        await _context.SaveChangesAsync( cancellationToken );
    }
}
