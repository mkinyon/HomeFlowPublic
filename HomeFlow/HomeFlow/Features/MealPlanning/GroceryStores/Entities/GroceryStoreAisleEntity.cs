using HomeFlow.Interfaces;
using HomeFlow.Models;

namespace HomeFlow.Features.MealPlanning.GroceryStores;

public class GroceryStoreAisleEntity : Model, IOrderable
{
    public string Name { get; set; } = string.Empty;

    public int Order { get; set; }

    public Guid GroceryStoreId { get; set; }

    public virtual GroceryStoreEntity GroceryStore { get; set; } = default!;

    public virtual ICollection<GroceryStoreAisleGroceryItemEntity> GroceryStoreAisleGroceryItems { get; set; } = new List<GroceryStoreAisleGroceryItemEntity>();
}
