namespace HomeFlow.Features.MealPlanning.GroceryStores;

public class GroceryStore
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Location { get; set; } = string.Empty;

    public List<GroceryStoreAisle> GroceryStoreAisles { get; set; } = new List<GroceryStoreAisle>();

    public override string ToString() => $"{Name} ({Location})";
}
