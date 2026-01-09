using HomeFlow.Features.MealPlanning.GroceryItems;

namespace HomeFlow.Features.MealPlanning.GroceryStores;

public class GroceryStoreAisleGroceryItemEntity
{
    public Guid GroceryStoreAisleId { get; set; }

    public Guid GroceryItemId { get; set; }

    public virtual GroceryStoreAisleEntity GroceryStoreAisle { get; set; } = default!;

    public virtual GroceryItemEntity GroceryItem { get; set; } = default!;
}
