using HomeFlow.Data;

namespace HomeFlow.Features.MealPlanning.GroceryItems;

public record CreateGroceryItemCommand( Features.MealPlanning.GroceryItems.GroceryItem GroceryItemRequest ) : IRequest<Guid>;

public class CreateGroceryItemCommandHandler : IRequestHandler<CreateGroceryItemCommand, Guid>
{
    private readonly IHomeFlowDbContext _context;

    public CreateGroceryItemCommandHandler( IHomeFlowDbContext context )
    {
        _context = context;
    }

    public async Task<Guid> Handle( CreateGroceryItemCommand request, CancellationToken cancellationToken )
    {
        var entity = new GroceryItemEntity
        {
            Name = request.GroceryItemRequest.Name,
        };

        _context.GroceryItems.Add( entity );

        await _context.SaveChangesAsync( cancellationToken );

        return entity.Id;
    }
}
