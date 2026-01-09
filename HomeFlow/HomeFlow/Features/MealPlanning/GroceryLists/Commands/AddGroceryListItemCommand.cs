using HomeFlow.Data;

namespace HomeFlow.Features.MealPlanning.GroceryLists;

public record AddGroceryListItemCommand( Guid Id, GroceryListItem GroceryListItemRequest ) : IRequest<Guid>;

public class CreateGroceryItemCommandHandler : IRequestHandler<AddGroceryListItemCommand, Guid>
{
    private readonly IHomeFlowDbContext _context;

    public CreateGroceryItemCommandHandler( IHomeFlowDbContext context )
    {
        _context = context;
    }

    public async Task<Guid> Handle( AddGroceryListItemCommand request, CancellationToken cancellationToken )
    {
        var groceryListEntity = await _context.GroceryLists.FirstOrDefaultAsync( gl => gl.Id == request.Id );

        Guard.Against.NotFound( request.Id, groceryListEntity );

        var entity = new GroceryListItemEntity
        {
            Text = request.GroceryListItemRequest.Text,
            GroceryListId = groceryListEntity.Id,
            SourceRecipeId = request.GroceryListItemRequest.SourceRecipe?.Id,
            RecipeGroceryItemId = request.GroceryListItemRequest.RecipeGroceryItem?.Id,
            GroceryItemId = request.GroceryListItemRequest.GroceryItem?.Id,
            Quantity = request.GroceryListItemRequest.Quantity,
            AdditionalInfo = request.GroceryListItemRequest.AdditionalInfo
        };

        _context.GroceryListItems.Add( entity );
        await _context.SaveChangesAsync( cancellationToken );

        return entity.Id;
    }
}
