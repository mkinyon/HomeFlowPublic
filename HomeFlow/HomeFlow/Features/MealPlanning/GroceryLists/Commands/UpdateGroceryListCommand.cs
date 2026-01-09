using HomeFlow.Data;

namespace HomeFlow.Features.MealPlanning.GroceryLists;

public record UpdateGroceryListCommand( GroceryList GroceryList ) : IRequest;

public class UpdateGroceryListCommandHandler : IRequestHandler<UpdateGroceryListCommand>
{
    private readonly IHomeFlowDbContext _context;

    public UpdateGroceryListCommandHandler( IHomeFlowDbContext context )
    {
        _context = context;
    }

    public async Task Handle( UpdateGroceryListCommand request, CancellationToken cancellationToken )
    {
        var entity = await _context.GroceryLists.FirstOrDefaultAsync( i => i.Id == request.GroceryList.Id, cancellationToken );

        Guard.Against.NotFound( request.GroceryList.Id, entity );

        entity.Name = request.GroceryList.Name;
        entity.IsPrinted = request.GroceryList.IsPrinted;

        await _context.SaveChangesAsync( cancellationToken );
    }
}
