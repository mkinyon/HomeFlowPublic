using FluentAssertions;
using HomeFlow.Data;
using HomeFlow.Features.MealPlanning.GroceryItems;
using HomeFlow.Features.MealPlanning.Recipes;
using Microsoft.EntityFrameworkCore;

namespace HomeFlow.Tests.Integration.Features.MealPlanning.Commands;

public class UpdateRecipeCommandHandlerIntegrationTests
{
    private DbContextOptions<HomeFlowDbContext> CreateInMemoryOptions() =>
        new DbContextOptionsBuilder<HomeFlowDbContext>()
            .UseInMemoryDatabase( Guid.NewGuid().ToString() )
            .Options;

    [Fact]
    public async Task Handle_ShouldUpdateRecipeAndPersistChanges_WhenUsingInMemoryDb()
    {
        // Arrange
        var recipeId = Guid.NewGuid();
        var groceryItemName = "Dill Pickles";

        var options = CreateInMemoryOptions();

        using ( var context = new HomeFlowDbContext( options ) )
        {
            var existingRecipe = new RecipeEntity
            {
                Id = recipeId,
                Name = "Old Recipe",
                Description = "Old Desc",
                RecipeSteps = new List<RecipeStepEntity>(),
                RecipeGroceryItems = new List<RecipeGroceryItemEntity>()
            };
            context.Recipes.Add( existingRecipe );
            await context.SaveChangesAsync();
        }

        using ( var context = new HomeFlowDbContext( options ) )
        {
            var handler = new UpdateRecipeCommandHandler( context );

            var request = new UpdateRecipeCommand( new Recipe
            {
                Id = recipeId,
                Name = "New Recipe",
                Description = "Updated Description",
                Author = "Chef Test",
                RecipeType = RecipeType.MainDish,
                TotalTimeInMinutes = 20,
                CookTimeInMinutes = 5,
                PrepTimeInMinutes = 15,
                Servings = 2,
                RecipeSteps = new List<RecipeStep>
                {
                    new() { Id = Guid.Empty, Text = "Mix all ingredients", Order = 0 }
                },
                RecipeGroceryItems = new List<RecipeGroceryItem>
                {
                    new()
                    {
                        GroceryItem = new GroceryItem { Name = groceryItemName },
                        Quantity = 1,
                        MeasurementType = MeasurementType.Cups,
                        MeasurementFraction = MeasurementFraction.None
                    }
                }
            } );

            // Act
            await handler.Handle( request, CancellationToken.None );
        }

        // Assert
        using ( var context = new HomeFlowDbContext( options ) )
        {
            var updated = await context.Recipes
                .Include( r => r.RecipeSteps )
                .Include( r => r.RecipeGroceryItems )
                .ThenInclude( g => g.GroceryItem )
                .FirstOrDefaultAsync( r => r.Id == recipeId );

            updated.Should().NotBeNull();
            updated.Name.Should().Be( "New Recipe" );
            updated.Description.Should().Be( "Updated Description" );
            updated.RecipeSteps.Should().ContainSingle( s => s.Text == "Mix all ingredients" );
            updated.RecipeGroceryItems.Should().ContainSingle( g => g.GroceryItem.Name == groceryItemName );
        }
    }
}
