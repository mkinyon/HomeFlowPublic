using HomeFlow.Features.MealPlanning.GroceryStores;
using HomeFlow.Models;

namespace HomeFlow.Features.MealPlanning.GroceryItems;

public class GroceryItemEntity : Model
{
    public string Name { get; set; } = string.Empty;

    public GroceryItemType Type { get; set; } = GroceryItemType.NonFood;

    public virtual ICollection<GroceryStoreAisleGroceryItemEntity> GroceryStoreAisleGroceryItems { get; set; } = new List<GroceryStoreAisleGroceryItemEntity>();

    public override string ToString()
    {
        return Name;
    }
}
