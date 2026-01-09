namespace HomeFlow.Features.MealPlanning.GroceryItems;

public class GroceryItem
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public GroceryItemType GroceryItemType { get; set; } = GroceryItemType.NonFood;

    public List<string> Tags { get; set; } = new List<string>();

    public override string ToString() => Name;
}
