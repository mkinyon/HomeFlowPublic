using HomeFlow.Data;
using HomeFlow.Features.MealPlanning.GroceryItems;

namespace HomeFlow.Features.MealPlanning.Recipes;

public record CreateRecipeCommand( Recipe RecipeRequest ) : IRequest<Guid>;

public class CreateRecipeCommandHandler : IRequestHandler<CreateRecipeCommand, Guid>
{
    private readonly IHomeFlowDbContext _context;

    public CreateRecipeCommandHandler( IHomeFlowDbContext context )
    {
        _context = context;
    }

    public async Task<Guid> Handle( CreateRecipeCommand request, CancellationToken cancellationToken )
    {
        var entity = new RecipeEntity
        {
            Name = request.RecipeRequest.Name,
            Description = request.RecipeRequest.Description,
            Author = request.RecipeRequest.Author,
            RecipeType = request.RecipeRequest.RecipeType,
            Servings = request.RecipeRequest.Servings,
            PrepTimeInMinutes = request.RecipeRequest.PrepTimeInMinutes,
            CookTimeInMinutes = request.RecipeRequest.CookTimeInMinutes,
            TotalTimeInMinutes = request.RecipeRequest.TotalTimeInMinutes
        };

        if ( request.RecipeRequest.Image != null )
        {
            var imageEntity = _context.ImageFiles.FirstOrDefault( i => i.Id == request.RecipeRequest.Image.Id );
            entity.Image = imageEntity;
        }

        foreach ( var item in request.RecipeRequest.RecipeSteps )
        {
            entity.RecipeSteps.Add( new RecipeStepEntity
            {
                Text = item.Text,
                Order = item.Order
            } );
        }

        foreach ( var groceryItemRequest in request.RecipeRequest.RecipeGroceryItems )
        {
            var recipeGroceryItem = new RecipeGroceryItemEntity
            {
                Quantity = groceryItemRequest.Quantity,
                MeasurementType = groceryItemRequest.MeasurementType,
                AdditionalDetail = groceryItemRequest.AdditionalDetail,
                Order = groceryItemRequest.Order
            };

            var groceryItem = _context.GroceryItems.FirstOrDefault( gi => gi.Name == groceryItemRequest.GroceryItem.Name );
            if ( groceryItem == null )
            {
                groceryItem = new();
                groceryItem.Id = new Guid();
                groceryItem.Type = GroceryItemType.Food;
                groceryItem.Name = groceryItemRequest.GroceryItem.Name;

                _context.GroceryItems.Add( groceryItem );
            }

            recipeGroceryItem.GroceryItem = groceryItem;

            entity.RecipeGroceryItems.Add( recipeGroceryItem );
        }

        _context.Recipes.Add( entity );
        await _context.SaveChangesAsync( cancellationToken );

        return entity.Id;
    }
}
