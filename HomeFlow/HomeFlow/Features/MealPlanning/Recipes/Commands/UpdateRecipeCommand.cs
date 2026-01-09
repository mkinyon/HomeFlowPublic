using HomeFlow.Data;
using HomeFlow.Features.MealPlanning.GroceryItems;

namespace HomeFlow.Features.MealPlanning.Recipes;

public record UpdateRecipeCommand( Recipe Recipe ) : IRequest;

public class UpdateRecipeCommandHandler : IRequestHandler<UpdateRecipeCommand>
{
    private readonly IHomeFlowDbContext _context;

    public UpdateRecipeCommandHandler( IHomeFlowDbContext context )
    {
        _context = context;
    }

    public async Task Handle( UpdateRecipeCommand request, CancellationToken cancellationToken )
    {
        var entity = await _context.Recipes
        .Include( r => r.RecipeSteps )
        .Include( r => r.RecipeGroceryItems )
        .ThenInclude( ri => ri.GroceryItem )
        .FirstOrDefaultAsync( r => r.Id == request.Recipe.Id, cancellationToken );


        Guard.Against.NotFound( request.Recipe.Id, entity );

        entity.Name = request.Recipe.Name;
        entity.Description = request.Recipe.Description;
        entity.Author = request.Recipe.Author;
        entity.RecipeType = request.Recipe.RecipeType;
        entity.TotalTimeInMinutes = request.Recipe.TotalTimeInMinutes;
        entity.CookTimeInMinutes = request.Recipe.CookTimeInMinutes;
        entity.PrepTimeInMinutes = request.Recipe.PrepTimeInMinutes;
        entity.Servings = request.Recipe.Servings;

        if ( request.Recipe.Image != null )
        {
            var imageEntity = _context.ImageFiles.FirstOrDefault( i => i.Id == request.Recipe.Image.Id );
            entity.Image = imageEntity;
        }
        else
        {
            entity.Image = null;
            entity.ImageId = null;
        }

        var requestSteps = request.Recipe.RecipeSteps.ToList();

        // Find steps to remove
        foreach ( var existingStep in entity.RecipeSteps.ToList() )
        {
            if ( !requestSteps.Any( rs => rs.Id == existingStep.Id ) )
            {
                _context.RecipeSteps.Remove( existingStep );
            }
        }

        // Add or update steps
        foreach ( var recipeStepDto in request.Recipe.RecipeSteps )
        {
            var existingStep = entity.RecipeSteps
                .FirstOrDefault( rs => rs.Id != Guid.Empty && rs.Id == recipeStepDto.Id );

            if ( existingStep == null )
            {
                // If the step is not found, it's new and should be added
                var newStep = new RecipeStepEntity
                {
                    Id = Guid.Empty,
                    Text = recipeStepDto.Text,
                    Order = recipeStepDto.Order,
                    RecipeId = entity.Id
                };
                entity.RecipeSteps.Add( newStep );
            }
            else
            {
                // If found, update existing step's properties
                existingStep.Text = recipeStepDto.Text;
                existingStep.Order = recipeStepDto.Order;
            }
        }


        var requestGroceryItems = request.Recipe.RecipeGroceryItems.ToList();

        // Find grocery items to remove
        foreach ( var existingGroceryItem in entity.RecipeGroceryItems.ToList() )
        {
            if ( !requestGroceryItems.Any( ri => ri.Id == existingGroceryItem.Id ) )
            {
                _context.RecipeGroceryItems.Remove( existingGroceryItem );
            }
        }

        foreach ( var recipeGroceryItemDto in request.Recipe.RecipeGroceryItems )
        {
            // check to see if the grocery item already exists
            var groceryItem = _context.GroceryItems
                .FirstOrDefault( gi => gi.Name == recipeGroceryItemDto.GroceryItem.Name );

            if ( groceryItem == null )
            {
                groceryItem = new GroceryItems.GroceryItemEntity
                {
                    Id = Guid.NewGuid(),
                    Type = GroceryItemType.Food,
                    Name = recipeGroceryItemDto.GroceryItem.Name
                };

                _context.GroceryItems.Add( groceryItem );
            }

            // next, check if this recipe already has a matching RecipeGroceryItem
            var existingRecipeGroceryItem = entity.RecipeGroceryItems
                .FirstOrDefault( ri => ri.Id != Guid.Empty && ri.Id == recipeGroceryItemDto.Id );

            if ( existingRecipeGroceryItem == null )
            {
                var newRecipeGroceryItem = new RecipeGroceryItemEntity
                {
                    Quantity = recipeGroceryItemDto.Quantity,
                    MeasurementFraction = recipeGroceryItemDto.MeasurementFraction,
                    MeasurementType = recipeGroceryItemDto.MeasurementType,
                    RecipeId = entity.Id,
                    GroceryItem = groceryItem,
                    AdditionalDetail = recipeGroceryItemDto.AdditionalDetail,
                    Order = recipeGroceryItemDto.Order
                };

                entity.RecipeGroceryItems.Add( newRecipeGroceryItem );
            }
            else
            {
                existingRecipeGroceryItem.Quantity = recipeGroceryItemDto.Quantity;
                existingRecipeGroceryItem.MeasurementFraction = recipeGroceryItemDto.MeasurementFraction;
                existingRecipeGroceryItem.MeasurementType = recipeGroceryItemDto.MeasurementType;
                existingRecipeGroceryItem.Order = recipeGroceryItemDto.Order;
                existingRecipeGroceryItem.GroceryItem = groceryItem;
                existingRecipeGroceryItem.AdditionalDetail = recipeGroceryItemDto.AdditionalDetail;
            }
        }

        await _context.SaveChangesAsync( cancellationToken );
    }
}
