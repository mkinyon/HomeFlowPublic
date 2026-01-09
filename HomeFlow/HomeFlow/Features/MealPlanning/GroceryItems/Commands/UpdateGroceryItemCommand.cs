using HomeFlow.Data;


namespace HomeFlow.Features.MealPlanning.GroceryItems;

public record UpdateGroceryItemCommand( GroceryItem GroceryItem ) : IRequest;

public class UpdateGroceryItemCommandHandler : IRequestHandler<UpdateGroceryItemCommand>
{
    private readonly IHomeFlowDbContext _context;

    public UpdateGroceryItemCommandHandler( IHomeFlowDbContext context )
    {
        _context = context;
    }

    public async Task Handle( UpdateGroceryItemCommand request, CancellationToken cancellationToken )
    {
        var entity = await _context.GroceryItems.FirstOrDefaultAsync( i => i.Id == request.GroceryItem.Id, cancellationToken );

        Guard.Against.NotFound( request.GroceryItem.Id, entity );

        entity.Name = request.GroceryItem.Name;

        await _context.SaveChangesAsync( cancellationToken );
    }
}
