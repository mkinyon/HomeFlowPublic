namespace HomeFlow.Features.MealPlanning.GroceryItems;

public class GroceryItemVM
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public GroceryItemType GroceryItemType { get; set; } = GroceryItemType.NonFood;
    public int RecipeCount { get; set; }
    public int GroceryListCount { get; set; }
    public List<string> Tags { get; set; } = new();
}
