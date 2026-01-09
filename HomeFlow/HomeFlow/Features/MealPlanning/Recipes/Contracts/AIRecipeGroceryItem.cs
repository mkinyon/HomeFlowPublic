namespace HomeFlow.Features.MealPlanning.Recipes;

public class AIRecipeGroceryItem
{
    public int Quantity { get; set; }

    public MeasurementFraction MeasurementFraction { get; set; } = MeasurementFraction.None;

    public MeasurementType MeasurementType { get; set; } = MeasurementType.None;

    public string AdditionalDetail { get; set; } = string.Empty;

    public int Order { get; set; }

    public AIGroceryItem GroceryItem { get; set; } = new AIGroceryItem();
}
