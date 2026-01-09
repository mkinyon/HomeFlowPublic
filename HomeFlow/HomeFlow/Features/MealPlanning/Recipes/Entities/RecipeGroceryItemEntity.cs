
using HomeFlow.Features.MealPlanning.GroceryItems;
using HomeFlow.Interfaces;
using HomeFlow.Models;

namespace HomeFlow.Features.MealPlanning.Recipes;

public class RecipeGroceryItemEntity : Model, IOrderable
{
    public int Quantity { get; set; }

    public MeasurementFraction MeasurementFraction { get; set; }

    public MeasurementType MeasurementType { get; set; }

    public string AdditionalDetail { get; set; } = string.Empty;

    public Guid RecipeId { get; set; }

    public Guid GroceryItemId { get; set; }

    public int Order { get; set; }

    public virtual RecipeEntity Recipe { get; set; } = default!;

    public virtual GroceryItemEntity GroceryItem { get; set; } = default!;
}
