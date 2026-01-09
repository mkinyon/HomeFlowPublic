
using HomeFlow.Features.MealPlanning.GroceryItems;
using HomeFlow.Interfaces;

namespace HomeFlow.Features.MealPlanning.GroceryStores;

public class GroceryStoreAisle : IOrderable
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public int Order { get; set; }

    public Guid GroceryStoreId { get; set; }

    public List<GroceryItem> GroceryItems { get; set; } = new List<GroceryItem>();

    public override string ToString() => Name;
}
