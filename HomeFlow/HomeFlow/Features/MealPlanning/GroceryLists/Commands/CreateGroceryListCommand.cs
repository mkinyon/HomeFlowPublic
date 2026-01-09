using HomeFlow.Data;

namespace HomeFlow.Features.MealPlanning.GroceryLists;

public record CreateGroceryListCommand( GroceryList GroceryListRequest ) : IRequest<Guid>;

public class CreateGroceryListCommandHandler : IRequestHandler<CreateGroceryListCommand, Guid>
{
    private readonly IHomeFlowDbContext _context;

    public CreateGroceryListCommandHandler( IHomeFlowDbContext context )
    {
        _context = context;
    }

    public async Task<Guid> Handle( CreateGroceryListCommand request, CancellationToken cancellationToken )
    {
        var entity = new GroceryListEntity
        {
            Name = request.GroceryListRequest.Name,
            IsPrinted = request.GroceryListRequest.IsPrinted
        };

        foreach ( var item in request.GroceryListRequest.Items )
        {
            entity.Items.Add( new GroceryListItemEntity
            {
                RecipeGroceryItemId = item.RecipeGroceryItem?.Id,
                Text = item.Text,
                GroceryItemId = item.GroceryItem?.Id ?? null,
            } );
        }

        _context.GroceryLists.Add( entity );

        await _context.SaveChangesAsync( cancellationToken );

        return entity.Id;
    }
}
