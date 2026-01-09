using HomeFlow.Features.MealPlanning.GroceryItems;
using HomeFlow.Features.MealPlanning.Recipes;
using HomeFlow.Models;

namespace HomeFlow.Features.MealPlanning.GroceryLists;

public class GroceryListItemEntity : Model
{
    public Guid GroceryListId { get; set; }

    public string Text { get; set; } = string.Empty;

    public Guid? SourceRecipeId { get; set; }

    public Guid? RecipeGroceryItemId { get; set; }

    public Guid? GroceryItemId { get; set; }

    public int Quantity { get; set; }

    public string AdditionalInfo { get; set; } = string.Empty;

    public virtual GroceryListEntity GroceryList { get; set; } = default!;

    public virtual RecipeEntity? SourceRecipe { get; set; }

    public virtual RecipeGroceryItemEntity? RecipeGroceryItem { get; set; }

    public virtual GroceryItemEntity? GroceryItem { get; set; }
}
