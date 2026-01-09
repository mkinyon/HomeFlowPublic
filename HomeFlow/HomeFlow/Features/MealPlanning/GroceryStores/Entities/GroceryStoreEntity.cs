using HomeFlow.Models;

namespace HomeFlow.Features.MealPlanning.GroceryStores;

public class GroceryStoreEntity : Model
{
    public string Name { get; set; } = string.Empty;

    public string Location { get; set; } = string.Empty;

    public virtual ICollection<GroceryStoreAisleEntity> GroceryStoreAisles { get; set; } = new List<GroceryStoreAisleEntity>();
}
